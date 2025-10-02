using Godot;

public partial class Player : CharacterBody2D
{
	[ExportGroup("Movement")]
	[Export] public float Speed = 80.0f;
	[Export] public float SprintSpeed = 111.0f;
	[Export] public float JumpVelocity = -250.0f;
	[Export] public float Acceleration = 1500.0f;
	[Export] public float Friction = 1200.0f;
	[Export] public float GravityMultiplier = 0.7f;

	[ExportGroup("Stamina")]
	[Export] public float MaxStamina = 100.0f;
	[Export] public float StaminaDrainRate = 20.0f;
	[Export] public float StaminaRegenRate = 18.0f;
	[Export] public float IdleStaminaRegenRate = 60.0f;
	[Export] public float IdleRegenDelay = 0.5f;
	[Export] public float MinStaminaToSprint = 5.0f;
	[Export] public float MaxJumpStaminaCost = 25.0f;
	[Export] public float MinJumpStaminaCost = 10.0f;

	[ExportGroup("Health")]
	[Export] public float MaxHealth = 100.0f;

	[ExportGroup("Jump")]
	[Export] public float CoyoteTime = 0.15f;
	[Export] public float JumpBufferTime = 0.1f;
	[Export] public bool CanDoubleJump = true;

	[ExportGroup("Animation")]
	[Export] public float RunSpeedScaleMin = 1.0f;
	[Export] public float RunSpeedScaleMax = 1.3f;

	[ExportGroup("Respawn")]
	[Export] public Marker2D SpawnPoint;

	private AnimatedSprite2D sprite;
	private float currentStamina;
	private float currentHealth;
	private float coyoteTimer;
	private float jumpBufferTimer;
	private float idleTimer;
	private bool hasDoubleJump;
	private bool canSprint = true;
	private bool sprintInputReleased = true;
	private bool facingRight = true;
	private bool isDead = false;
	private string currentAnimation = "";
	private Vector2 startPosition;
	private float jumpStartVelocity = 0f;

	public override void _Ready()
	{
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		currentStamina = MaxStamina;
		currentHealth = MaxHealth;
		startPosition = GlobalPosition;

		if (SpawnPoint != null)
		{
			GlobalPosition = SpawnPoint.GlobalPosition;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isDead) return;

		float deltaF = (float)delta;
		Vector2 velocity = Velocity;

		HandleGravity(ref velocity, deltaF);
		HandleJump(ref velocity, deltaF);
		HandleMovement(ref velocity, deltaF);
		HandleStamina(deltaF);
		CheckDeath();
		UpdateAnimation(velocity);

		Velocity = velocity;
		MoveAndSlide();
	}

	private void HandleGravity(ref Vector2 velocity, float delta)
	{
		if (!IsOnFloor())
		{
			velocity += GetGravity() * GravityMultiplier * delta;
			coyoteTimer -= delta;
		}
		else
		{
			coyoteTimer = CoyoteTime;
			hasDoubleJump = true;
			jumpStartVelocity = 0f;
		}
	}

	private void HandleJump(ref Vector2 velocity, float delta)
	{
		if (jumpBufferTimer > 0)
		{
			jumpBufferTimer -= delta;
		}

		if (Input.IsActionJustPressed("jump"))
		{
			jumpBufferTimer = JumpBufferTime;
		}

		if (jumpBufferTimer > 0)
		{
			if (coyoteTimer > 0 && currentStamina >= MinJumpStaminaCost)
			{
				velocity.Y = JumpVelocity;
				jumpStartVelocity = JumpVelocity;
				jumpBufferTimer = 0;
				coyoteTimer = 0;
			}
			else if (CanDoubleJump && hasDoubleJump && !IsOnFloor() && currentStamina >= MinJumpStaminaCost)
			{
				velocity.Y = JumpVelocity;
				jumpStartVelocity = JumpVelocity;
				hasDoubleJump = false;
				jumpBufferTimer = 0;
			}
		}

		if (Input.IsActionJustReleased("jump") && velocity.Y < 0 && jumpStartVelocity != 0f)
		{
			float jumpProgress = 1.0f - (velocity.Y / jumpStartVelocity);
			jumpProgress = Mathf.Clamp(jumpProgress, 0.0f, 1.0f);
			
			float staminaCost = Mathf.Lerp(MaxJumpStaminaCost, MinJumpStaminaCost, jumpProgress);
			currentStamina -= staminaCost;
			
			jumpStartVelocity = 0f;
			velocity.Y *= 0.5f;
			
			GD.Print($"Jump released early! Progress: {jumpProgress:F2}, Cost: {staminaCost:F1}");
		}
		else if (velocity.Y >= 0 && jumpStartVelocity != 0f && !IsOnFloor())
		{
			currentStamina -= MinJumpStaminaCost;
			jumpStartVelocity = 0f;
			
			GD.Print($"Full jump completed! Cost: {MinJumpStaminaCost}");
		}
	}

	private void HandleMovement(ref Vector2 velocity, float delta)
	{
		float inputAxis = 0;
		if (Input.IsActionPressed("move_right")) inputAxis += 1;
		if (Input.IsActionPressed("move_left")) inputAxis -= 1;

		float targetSpeed = Speed;
		bool isSprinting = false;

		if (!Input.IsActionPressed("sprint"))
		{
			sprintInputReleased = true;
		}

		if (Input.IsActionPressed("sprint") && canSprint && sprintInputReleased && inputAxis != 0 && IsOnFloor())
		{
			targetSpeed = SprintSpeed;
			isSprinting = true;
		}

		if (inputAxis != 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, inputAxis * targetSpeed, Acceleration * delta);

			if (inputAxis > 0 && !facingRight)
			{
				facingRight = true;
				if (sprite != null) sprite.FlipH = false;
			}
			else if (inputAxis < 0 && facingRight)
			{
				facingRight = false;
				if (sprite != null) sprite.FlipH = true;
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * delta);
		}
	}

	private void HandleStamina(float delta)
	{
		bool isMoving = Mathf.Abs(Velocity.X) > 5.0f;
		bool isSprinting = Input.IsActionPressed("sprint") && canSprint && sprintInputReleased && isMoving && IsOnFloor();

		if (isSprinting)
		{
			currentStamina -= StaminaDrainRate * delta;
			idleTimer = 0;

			if (currentStamina <= 0)
			{
				currentStamina = 0;
				canSprint = false;
				sprintInputReleased = false;
			}
		}
		else
		{
			float regenRate = StaminaRegenRate;

			if (IsOnFloor() && !isMoving)
			{
				idleTimer += delta;
				if (idleTimer >= IdleRegenDelay)
				{
					regenRate = IdleStaminaRegenRate;
				}
			}
			else
			{
				idleTimer = 0;
			}

			currentStamina += regenRate * delta;

			if (currentStamina > MaxStamina)
			{
				currentStamina = MaxStamina;
			}

			if (currentStamina >= MinStaminaToSprint)
			{
				canSprint = true;
			}
		}
	}

	private void CheckDeath()
	{
		if (GlobalPosition.Y > GameConstants.DEATH_Y && !isDead)
		{
			Die();
		}
	}

	private void UpdateAnimation(Vector2 velocity)
	{
		if (sprite == null) return;

		if (!IsOnFloor())
		{
			if (velocity.Y < -5f)
			{
				PlayAnimation("jump");
			}
			else
			{
				PlayAnimation("fall");
			}
			return;
		}

		float xSpeed = Mathf.Abs(velocity.X);
		if (xSpeed > GameConstants.RUN_ANIMATION_THRESHOLD)
		{
			float t = Mathf.InverseLerp(Speed, SprintSpeed, xSpeed);
			float animSpeed = Mathf.Lerp(RunSpeedScaleMin, RunSpeedScaleMax, Mathf.Clamp(t, 0f, 1f));
			PlayAnimation("run", animSpeed);
		}
		else
		{
			PlayAnimation("idle");
		}
	}

	private void PlayAnimation(string animName, float speedScale = 1f)
	{
		if (sprite == null) return;
		if (currentAnimation == animName && Mathf.Abs(sprite.SpeedScale - speedScale) < 0.01f)
		{
			return;
		}

		sprite.SpeedScale = speedScale;
		sprite.Play(animName);
		currentAnimation = animName;
	}

	private void Die()
	{
		isDead = true;
		CallDeferred(nameof(Respawn));
	}

	private void Respawn()
	{
		Velocity = Vector2.Zero;
		Vector2 targetPos = (SpawnPoint != null) ? SpawnPoint.GlobalPosition : startPosition;

		if (targetPos.Y > GameConstants.DEATH_Y - 10f)
		{
			targetPos.Y = GameConstants.DEATH_Y - 10f;
		}

		GlobalPosition = targetPos;
		currentHealth = MaxHealth;
		currentStamina = MaxStamina;
		hasDoubleJump = true;
		jumpStartVelocity = 0f;
		isDead = false;
	}

	public void TakeDamage(float damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Die();
		}
	}

	public void Heal(float amount)
	{
		currentHealth += amount;
		if (currentHealth > MaxHealth)
			currentHealth = MaxHealth;
	}

	public float GetStaminaPercent() => currentStamina / MaxStamina;
	public float GetCurrentStamina() => currentStamina;
	public float GetHealthPercent() => currentHealth / MaxHealth;
	public float GetCurrentHealth() => currentHealth;
}
