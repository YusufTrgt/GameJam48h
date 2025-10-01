using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// Movement Settings
	[Export] public float Speed = 80.0f;
	[Export] public float SprintSpeed = 140.0f;
	[Export] public float JumpVelocity = -250.0f;
	[Export] public float Acceleration = 1500.0f;
	[Export] public float Friction = 1200.0f;
	[Export] public float GravityMul = 0.7f;
	
	// Health System
	[Export] public float MaxHealth = 100.0f;
	private float currentHealth;
	
	// Stamina System
	[Export] public float MaxStamina = 100.0f;
	[Export] public float StaminaDrainRate = 20.0f;
	[Export] public float StaminaRegenRate = 14.0f;
	[Export] public float MinStaminaToSprint = 5.0f;
	[Export] public float JumpStaminaCost = 25.0f;
	[Export] public float MinJumpStaminaCost = 10.0f;
	private float currentStamina;
	private bool canSprint = true;
	private bool sprintInputReleased = true;
	
	// Coyote Time
	[Export] public float CoyoteTime = 0.15f;
	private float coyoteTimer = 0.0f;
	
	// Jump Buffer
	[Export] public float JumpBufferTime = 0.1f;
	private float jumpBufferTimer = 0.0f;
	
	// Double Jump
	[Export] public bool CanDoubleJump = true;
	private bool hasDoubleJump = false;
	private bool jumpStaminaPaid = false;
	
	// Animations & Visuals
	private AnimatedSprite2D sprite;
	private bool facingRight = true;
	[Export] public float RunAnimThreshold = 12f;      
	[Export] public float RunSpeedScaleMin = 1.0f;     
	[Export] public float RunSpeedScaleMax = 1.3f;     

	private string _currentAnim = "";

	// Air Input Lock
	private float airInputAxis = 0;

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		currentStamina = MaxStamina;
		currentHealth = MaxHealth;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		float deltaF = (float)delta;

		// Gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * GravityMul * deltaF;
			coyoteTimer -= deltaF;
		}
		else
		{
			coyoteTimer = CoyoteTime;
			hasDoubleJump = true;
			jumpStaminaPaid = false;
			airInputAxis = 0;
		}

		// Jump Buffer Timer
		if (jumpBufferTimer > 0)
		{
			jumpBufferTimer -= deltaF;
		}

		// Jump Input Detection
		if (Input.IsActionJustPressed("jump"))
		{
			jumpBufferTimer = JumpBufferTime;
		}

		// Jump Logic (mit Coyote Time & Buffer & Stamina)
		if (jumpBufferTimer > 0)
		{
			if (coyoteTimer > 0 && currentStamina >= MinJumpStaminaCost)
			{
				velocity.Y = JumpVelocity;
				jumpStaminaPaid = false;
				jumpBufferTimer = 0;
				coyoteTimer = 0;
			}
			else if (CanDoubleJump && hasDoubleJump && !IsOnFloor() && currentStamina >= MinJumpStaminaCost)
			{
				velocity.Y = JumpVelocity;
				jumpStaminaPaid = false;
				hasDoubleJump = false;
				jumpBufferTimer = 0;
			}
		}

		// Variable Jump Height mit dynamischer Stamina-Kosten
		if (Input.IsActionJustReleased("jump") && velocity.Y < 0 && !jumpStaminaPaid)
		{
			float jumpProgress = 1.0f - (velocity.Y / JumpVelocity);
			jumpProgress = Mathf.Clamp(jumpProgress, 0.0f, 1.0f);
			
			float staminaCost = Mathf.Lerp(MinJumpStaminaCost, JumpStaminaCost, jumpProgress);
			currentStamina -= staminaCost;
			jumpStaminaPaid = true;
			
			velocity.Y *= 0.5f;
		}
		else if (velocity.Y >= 0 && !jumpStaminaPaid && !IsOnFloor())
		{
			currentStamina -= JumpStaminaCost;
			jumpStaminaPaid = true;
		}

		// Horizontal Movement mit Air-Input-Lock
		float inputAxis = 0;
		if (Input.IsActionPressed("move_right"))
			inputAxis += 1;
		if (Input.IsActionPressed("move_left"))
			inputAxis -= 1;

		if (IsOnFloor())
		{
			// Am Boden - normaler Input zählt
		}
		else
		{
			// In der Luft - letzte Eingabe merken
			if (inputAxis != 0)
				airInputAxis = inputAxis;
			inputAxis = airInputAxis;
		}

		// Sprint mit Shift & Stamina System
		float currentSpeed = Speed;
		if (!Input.IsActionPressed("sprint"))
		{
			sprintInputReleased = true;
		}
		if (Input.IsActionPressed("sprint") && canSprint && sprintInputReleased && inputAxis != 0)
		{
			currentSpeed = SprintSpeed;
			currentStamina -= StaminaDrainRate * deltaF;
			
			if (currentStamina <= 0)
			{
				currentStamina = 0;
				canSprint = false;
				sprintInputReleased = false;
			}
		}
		else
		{
			currentStamina += StaminaRegenRate * deltaF;
			if (currentStamina > MaxStamina)
				currentStamina = MaxStamina;
			
			if (currentStamina >= MinStaminaToSprint)
				canSprint = true;
		}

		// Bewegung
		if (inputAxis != 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, inputAxis * currentSpeed, Acceleration * deltaF);
			
			if (inputAxis > 0 && !facingRight)
			{
				facingRight = true;
				sprite.FlipH = false;
			}
			else if (inputAxis < 0 && facingRight)
			{
				facingRight = false;
				sprite.FlipH = true;
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * deltaF);
		}

		UpdateAnimation(velocity);

		Velocity = velocity;
		MoveAndSlide();
	}
	
	private void PlayAnim(string name, float speedScale = 1f) {
		if (_currentAnim == name && Mathf.IsEqualApprox(sprite.SpeedScale, speedScale)) return;
		sprite.SpeedScale = speedScale;
		sprite.Play(name);
		_currentAnim = name;
	}

	private void UpdateAnimation(Vector2 velocity) {
		// In der Luft: (optional) jump/fall unterscheiden
		if (!IsOnFloor()) {
			if (velocity.Y < -5f)
				PlayAnim("jump");
			else PlayAnim("fall");
			return;
		}

		// Am Boden: run vs idle
		float xSpeed = Mathf.Abs(velocity.X);
		if (xSpeed > RunAnimThreshold){
			// t 0 bei Speed, 1 bei SprintSpeed
			float baseSpeed = Mathf.Max(1f, Speed); // Schutz gegen 0
			float t = Mathf.InverseLerp(baseSpeed, SprintSpeed, xSpeed);
			float animSpeed = Mathf.Lerp(RunSpeedScaleMin, RunSpeedScaleMax, Mathf.Clamp(t, 0f, 1f));

			PlayAnim("run", animSpeed);
		}
		else {
			PlayAnim("idle", 1f);
		}
	}

	
	public float GetStaminaPercent()
	{
		return currentStamina / MaxStamina;
	}
	
	public float GetCurrentStamina()
	{
		return currentStamina;
	}
	
	public float GetHealthPercent()
	{
		return currentHealth / MaxHealth;
	}
	
	public float GetCurrentHealth()
	{
		return currentHealth;
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
	
	private void Die()
	{
		GD.Print("Player ist gestorben!");
		// TODO: Game Over Screen, Respawn, etc.
	}
}
