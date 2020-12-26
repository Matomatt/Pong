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
	[Export]
	private bool flip = true;
	[Export]
	private int moveSpeed = 300;
	private Vector2 playerSpeed = new Vector2(0, 0);
	private int lastSpeed;
	[Export]
	private int acceleration = 10;
	[Export]
	private float dashTime = 0.2f;
	[Export]
	private float dashSpeedMultiplier = 2.5f;
	private bool dashing = false;
	private Vector2 direction;
	[Export]
	private float dashCooldown = 2;
	private DateTime lastDash = DateTime.Now;
	private CPUParticles2D dashParticles;

	public override void _Ready()
	{
		pn = playerNumber.ToString();
		this.GetChild<Sprite>(0).FlipH = flip;
		dashParticles = this.GetChild<CPUParticles2D>(2);
		dashParticles.Transform = new Godot.Transform2D(0, new Vector2(dashParticles.Transform.origin.x * ((flip) ? -1 : 1), dashParticles.Transform.origin.y));
		if (flip) { dashParticles.Gravity *= -1; dashParticles.Direction *= -1; }
	}
	//CollisionShape2D hitbox = this.GetChild<CollisionShape2D>(1);
	//hitbox.Transform = new Godot.Transform2D(0, new Vector2(5 * ((flip) ? -1 : 1), hitbox.Transform.origin.y));

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionPressed("quit")) GetTree().Quit();
		if (lastDash.CompareTo(DateTime.Now) > 0) dashParticles.Emitting = false;
		else if (!dashParticles.Emitting) dashParticles.Emitting = true;
		if (Input.IsActionPressed("dash" + pn) && !dashing && lastDash < DateTime.Now) Dash();

		Vector2 speedGoal = new Vector2(
			Input.GetActionStrength("moveright" + pn) - Input.GetActionStrength("moveleft" + pn),
			Input.GetActionStrength("movedown" + pn) - Input.GetActionStrength("moveup" + pn)
		).Normalized();
		speedGoal = ((!dashing)? speedGoal : direction) * moveSpeed;
		playerSpeed = playerSpeed.LinearInterpolate(speedGoal, acceleration * delta);
		MoveAndSlide(playerSpeed);
	}

    private void Dash()
    {
		dashing = true;
		lastSpeed = moveSpeed;
        moveSpeed = (int)(moveSpeed * dashSpeedMultiplier);
		direction = playerSpeed.Normalized();
		dashParticles.InitialVelocity = 100;
		dashParticles.LinearAccel = 100;
		dashParticles.Amount *= 10;
		dashParticles.ColorRamp.Colors[0] = Colors.DarkRed;
		dashParticles.ColorRamp.Colors[1] = Colors.OrangeRed;
		dashParticles.ScaleAmount = 3;
		DashTimeout(dashTime);
	}

    private async void DashTimeout(float time)
	{
		GD.Print("Timeout");
		await Task.Delay(TimeSpan.FromMilliseconds(time * 1000f));
        dashParticles.InitialVelocity = 0;
        dashParticles.LinearAccel = 0;
        dashParticles.Amount /= 10;
        dashParticles.ColorRamp.Colors[0] = Colors.DarkRed;
        dashParticles.ColorRamp.Colors[1] = Colors.IndianRed;
        dashParticles.ScaleAmount = 2;
        moveSpeed = lastSpeed;
		lastDash = DateTime.Now.AddSeconds(dashCooldown);
		dashing = false;
	}
}
