using Godot;
using System;
using System.Threading.Tasks;
//using System.Timers;
//using Timer = System.Timers.Timer;

public class player : KinematicBody2D
{
	[Export]
	private int playerNumber = 1;
	private string pn = "";
	private AnimatedSprite sprite;
	[Export]
	private bool flip = true;
	[Export]
	private int moveSpeed = 350;
	private Vector2 playerSpeed = new Vector2(0, 0);
	private int lastSpeed;
	[Export]
	private int acceleration = 20;
	[Export]
	private float dashTime = 0.15f;
	[Export]
	private float dashSpeedMultiplier = 2.5f;
	private bool dashing = false;
	private Vector2 direction;
	private Timer dashTimer;
	[Export]
	private float dashCooldown = 3;
	private DateTime lastDash = DateTime.Now;
	private CPUParticles2D dashParticles;
	private Timer particlesTimer;
	[Export]
	private bool touchControls = false;
	[Export]
	private int touchRadius = 30;
	[Export]
	private int dashDistanceThreshold = 50;
	private int touchedByID = -1;
	private bool hovered = false;
	private Vector2 targetPosition;
	private int lastDistanceBetweenDrag = 0;

	public override void _Ready()
	{
		InputPickable = true;
		pn = playerNumber.ToString();
		(sprite = GetChild<AnimatedSprite>(0)).FlipH = flip;
		sprite.Connect("animation_finished", this, nameof(Idle));
		CollisionPolygon2D hitbox = this.GetChild<CollisionPolygon2D>(1);
		if (flip) hitbox.Transform = new Transform2D((float)Math.PI, -1*hitbox.Transform.origin);
		dashParticles = GetChild<CPUParticles2D>(2);
		dashParticles.Transform = new Transform2D(0, new Vector2(dashParticles.Transform.origin.x * ((flip) ? -1 : 1), dashParticles.Transform.origin.y));
		if (flip) { dashParticles.Gravity *= -1; dashParticles.Direction *= -1; }
		(dashTimer = (Timer)GetNode("dashTimer")).Connect("timeout", this, nameof(DashTimeout));
		(particlesTimer = (Timer)GetNode("particlesTimer")).Connect("timeout", this, nameof(ResetDashParticles));
		Connect("mouse_entered", this, nameof(Hovered));
		Connect("mouse_exited", this, nameof(Hovered));
	}

	public override void _PhysicsProcess(float delta)
	{
		if (lastDash.CompareTo(DateTime.Now) > 0) dashParticles.Emitting = false;
		else if (!dashParticles.Emitting) dashParticles.Emitting = true;

		Vector2 speedGoal = touchedByID >= 0 ? TouchToSpeedGoal() : InputsToSpeedGoal();
		speedGoal = ((!dashing) ? speedGoal : direction) * moveSpeed;
		playerSpeed = playerSpeed.LinearInterpolate(speedGoal, acceleration * delta);
		MoveAndSlide(playerSpeed);
		for (int i = 0; i < GetSlideCount(); i++)
		{
			string colliderName = ((Node2D)GetSlideCollision(i).Collider).Name;
			if (colliderName == "ball")
				((ball)GetSlideCollision(i).Collider).collideWith(this, delta, -1 * GetSlideCollision(i).Normal);
		}
	}

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("dash" + pn) && !dashing && lastDash < DateTime.Now) Dash(true);

		if (@event is InputEventScreenTouch screenTouch)
        {
			if (screenTouch.Pressed && screenTouch.Position.DistanceTo(GlobalPosition) < touchRadius)
            {
				//GD.Print(pn + " : " + screenTouch.Index);
				targetPosition = screenTouch.Position;
				touchedByID = screenTouch.Index;
			}
				
			else if (touchedByID == screenTouch.Index)
            {
				//GD.Print("released");
				touchedByID = -1;
            }
		} else if (@event is InputEventScreenDrag screenDrag)
        {
			if (screenDrag.Index == touchedByID)
            {
				
				lastDistanceBetweenDrag = (int)screenDrag.Position.DistanceTo(targetPosition);
				//GD.Print(screenDrag.Position.DistanceTo(targetPosition));
				if (lastDistanceBetweenDrag > dashDistanceThreshold && !dashing && lastDash < DateTime.Now)
                {
					direction = new Vector2(screenDrag.Position.x - targetPosition.x, screenDrag.Position.y - targetPosition.y).Normalized();
					Dash(false);
                }
					
				targetPosition = screenDrag.Position;
			}
				
		}
			
	}

    private Vector2 InputsToSpeedGoal()
	{
		return new Vector2(
			Input.GetActionStrength("moveright" + pn) - Input.GetActionStrength("moveleft" + pn),
			Input.GetActionStrength("movedown" + pn) - Input.GetActionStrength("moveup" + pn)
		).Normalized();
	}

	private Vector2 TouchToSpeedGoal()
	{
		return new Vector2(targetPosition.x - GlobalPosition.x, targetPosition.y - GlobalPosition.y).Normalized();// *(lastDistanceBetweenDrag/dashDistanceThreshold);
	}

	private void Hovered()
    {
		hovered = !hovered;
		GD.Print("Hovered " + hovered.ToString() );
    }

	/*private void Touched()
    {
		if (hovered)
        {
			touchedByID = 1;
		}
			
	}*/

	internal Vector2 getSpeed()
	{
		return playerSpeed;
	}

	internal void Ping()
    {
		sprite.Play("ping");
    }
	public void Idle()
    {
		sprite.Play("idle");
	}

	private void Dash(bool keyboardInitiated)
    {
		dashing = true;
		lastSpeed = moveSpeed;
        moveSpeed = (int)(moveSpeed * dashSpeedMultiplier);
		if (keyboardInitiated) direction = playerSpeed.Normalized();
		BurstDashParticles();
		dashTimer.Start(dashTime);
		particlesTimer.Start(dashTime + 0.5f);
	}
    private void DashTimeout()
	{
        moveSpeed = lastSpeed;
		lastDash = DateTime.Now.AddSeconds(dashCooldown);
		dashing = false;
	}
	private void BurstDashParticles()
    {
		dashParticles.InitialVelocity = 100;
		dashParticles.LinearAccel = 100;
		dashParticles.Amount *= 10;
		dashParticles.ColorRamp.Colors[0] = Colors.DarkRed;
		dashParticles.ColorRamp.Colors[1] = Colors.OrangeRed;
		dashParticles.ScaleAmount = 3;
	}
	private void ResetDashParticles()
	{
		try
        {
			dashParticles.InitialVelocity = 0;
			dashParticles.LinearAccel = 0;
			dashParticles.Amount /= 10;
			dashParticles.ColorRamp.Colors[0] = Colors.DarkRed;
			dashParticles.ColorRamp.Colors[1] = Colors.IndianRed;
			dashParticles.ScaleAmount = 2;
		}
		catch { }
	}
}
