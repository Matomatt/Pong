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
	[Export]
	private int acceleration = 20; //Acceleration for the Lerp
	[Export]
	private bool touchControls = false;
	[Export]
	private int touchRadius = 30;
	[Export]
	private int dashDistanceThreshold = 50; //Dash when the last touched position is at this distance from the current touched position
	private int touchedByID = -1;
	private Vector2 targetPosition;
	private int lastDistanceBetweenDrag = 0;
	private float appliedForce = 0;
	private float forceWhittling = 20f;
	private Vector2 forceDirection = new Vector2(0,0);
	// SUPERPOWERS
	[Export]
	private string[] powers = { "dash", "zawarudo" };
	private float[] powerCooldownTimes = { 3f, 4f };
	private Timer[] powerTimers = new Timer[2];
	private Timer[] particlesTimers = new Timer[2];
	private DateTime[] powerCooldowns = { DateTime.Now, DateTime.Now };
	private CPUParticles2D[] powerParticles = new CPUParticles2D[2];
	// Dash
	[Export]
	private float dashTime = 0.15f;
	[Export]
	private float dashSpeedMultiplier = 2.5f;
	private Vector2 direction;
	private int lastSpeed;
	// ZAWARUDO
	const float SLOWDOWNDIVIDER = 4f;
	const float SLOWDOWNDURATION = 1.5f;
	public float speedModifier = 1f;
	// Black Hole
	const float BLACKHOLEDURATION = 1f;

	public override void _Ready()
	{
		InputPickable = true;
		pn = playerNumber.ToString();
		(sprite = GetChild<AnimatedSprite>(0)).FlipH = flip;
		sprite.Connect("animation_finished", this, nameof(Idle));
		CollisionPolygon2D hitbox = this.GetChild<CollisionPolygon2D>(1);
		if (flip) hitbox.Transform = new Transform2D((float)Math.PI, -1*hitbox.Transform.origin);
		// SUPERPOWERS
		globalVariables globalVariables = (globalVariables)GetNode("/root/GlobalVariables");
		for (int i = 0; i<powers.Length; i++)
        {
			powers[i] = globalVariables.selectedPowers[(playerNumber - 1) * 2 + i];
			foreach (Godot.Collections.Dictionary power in globalVariables.powers)
				if (powers[i] == power["id"].ToString())
					powerCooldownTimes[i] = power["cooldown"].ToString().ToFloat();

			Godot.Collections.Array parametersArrays = new Godot.Collections.Array();
			parametersArrays.Add(i);
			(powerTimers[i] = (Timer)GetNode("powerTimer" + (i + 1).ToString())).Connect("timeout", this, nameof(PowerTimerTimeout), parametersArrays);
			(particlesTimers[i] = (Timer)GetNode("particlesTimer" + (i + 1).ToString())).Connect("timeout", this, nameof(ResetPowerParticles), parametersArrays);
		}
		// Particles
		for (int i = 0; i < powerParticles.Length; i++)
		{
			switch (powers[i])
            {
				case "dash":
					powerParticles[i] = (CPUParticles2D)GetNode("dashParticles");
					powerParticles[i].Transform = new Transform2D(0, new Vector2(powerParticles[i].Transform.origin.x * ((flip) ? -1 : 1), powerParticles[i].Transform.origin.y));
					if (flip) { powerParticles[i].Gravity *= -1; powerParticles[i].Direction *= -1; }
					break;
			}
			if (powerParticles[i] != null) powerParticles[i].Visible = true;
		}
	}

    public override void _PhysicsProcess(float delta)
	{
		for (int i = 0; i < powers.Length; i++) {
			if (powerParticles[i] != null) {
				if (powerCooldowns[i].CompareTo(DateTime.Now) > 0) powerParticles[i].Emitting = false;
				else if (!powerParticles[i].Emitting) powerParticles[i].Emitting = true;
			}
		}

		Vector2 speedGoalUnit = touchedByID >= 0 ? TouchToSpeedGoal() : InputsToSpeedGoal();
		Vector2 speedGoal = speedGoalUnit * moveSpeed;

		// SuperPowers
		for (int i = 0; i < powers.Length; i++)
        {
			if (!powerTimers[i].IsStopped())
            {
				switch (powers[i])
				{
					case "dash":
						speedGoal = direction * moveSpeed;
						break;
					case "blackhole":
						player[] rackets = ((mainscene)GetNode("/root/mainscene")).rackets;
						for (int j = 0; j < rackets.Length; j++)
						{
							if (j + 1 != playerNumber)
							{
								rackets[j].MoveAndCollideBis(GlobalPosition, delta);
							}
						}
						ball ball = (ball)GetNode("../ball");
						ball.MoveAndBounce(GlobalPosition, delta);
						break;
				}
			}
		}

		playerSpeed = playerSpeed.LinearInterpolate(speedGoal, acceleration * delta);
		if (appliedForce != 0) appliedForce -= appliedForce * forceWhittling * delta;
		if (appliedForce < 10) appliedForce = 0;

		MoveAndCollideBis(playerSpeed * speedModifier + forceDirection * appliedForce, delta);
	}

	public void MoveAndCollideBis(Vector2 speed, float delta)
    {
		MoveAndSlide(speed);

		for (int i = 0; i < GetSlideCount(); i++)
			if (((Node2D)GetSlideCollision(i).Collider).Name == "ball")
				((ball)GetSlideCollision(i).Collider).collideWith(this, delta, -1 * GetSlideCollision(i).Normal);
	}

	internal void ApplyForce(Vector2 direction, float force = 2000f, float whittling = 10f)
    {
		forceDirection = direction;
		appliedForce = force;
		forceWhittling = whittling;
    }

    public override void _Input(InputEvent @event)
    {
		for (int i = 0; i<powers.Length; i++)
			if (@event.IsActionPressed("power" + pn + (i+1).ToString())) ActivatePower(i, true);

		if (@event is InputEventScreenTouch screenTouch)
        {
			if (screenTouch.Pressed && screenTouch.Position.DistanceTo(GlobalPosition) < touchRadius)
            {
				targetPosition = screenTouch.Position;
				touchedByID = screenTouch.Index;
			}
			else if (touchedByID == screenTouch.Index)
				touchedByID = -1;
			else if (screenTouch.Position.DistanceTo(new Vector2(256, 0)) < 20)
				((pause)GetNode("/root/Pause")).TogglePause(); 
			else if (screenTouch.Position.x < 256 && playerNumber <= 2/2 || screenTouch.Position.x >= 256 && playerNumber > 2 / 2)
				ActivatePower(1, false);
		} 
		else if (@event is InputEventScreenDrag screenDrag)
        {
			if (screenDrag.Index == touchedByID)
            {
				lastDistanceBetweenDrag = (int)screenDrag.Position.DistanceTo(targetPosition);
				if (lastDistanceBetweenDrag > dashDistanceThreshold)
                {
					direction = new Vector2(screenDrag.Position.x - targetPosition.x, screenDrag.Position.y - targetPosition.y).Normalized();
					ActivatePower(0, false);
				}
				targetPosition = screenDrag.Position;
			}
		}
	}
	// Inputs
    private Vector2 InputsToSpeedGoal()
	{
		return new Vector2(
			Input.GetActionStrength("moveright" + pn) - Input.GetActionStrength("moveleft" + pn),
			Input.GetActionStrength("movedown" + pn) - Input.GetActionStrength("moveup" + pn)
		).Normalized();
	}
	private Vector2 TouchToSpeedGoal()
	{
		return new Vector2(targetPosition.x - GlobalPosition.x, targetPosition.y - GlobalPosition.y).Normalized();
	}

	// SUPERPOWERS
	private void ActivatePower(int index, bool keyboardInitiated)
	{
		if (!powerTimers[index].IsStopped() || powerCooldowns[index] >= DateTime.Now) return;

		float timerDuration = 0f;
		switch (powers[index])
		{
			case "dash": timerDuration = Dash(keyboardInitiated); break;
			case "zawarudo": timerDuration = ZaWarudo(); break;
		}
		powerTimers[index].Start(timerDuration);
		StartParticlesTimer(index, timerDuration);
		powerCooldowns[index] = DateTime.Now.AddSeconds(powerCooldownTimes[index]);
	}
	private void PowerTimerTimeout(int index)
	{
		switch (powers[index])
        {
			case "dash": moveSpeed = lastSpeed; break;
			case "zawarudo": ZaWarudoTimeout(); break;
        }
	}
	private void StartParticlesTimer(int index, float powerDuration)
    {
		switch (powers[index])
		{
			case "dash":
				BurstDashParticles(index);
				particlesTimers[index].Start(powerDuration + 0.5f);
				break;
		}
	}
	private void ResetPowerParticles(int index)
    {
		switch (powers[index])
		{
			case "dash": ResetDashParticles(index); break;
		}
	}

	// Dash
	private float Dash(bool keyboardInitiated)
    {
        lastSpeed = moveSpeed;
        moveSpeed = (int)(moveSpeed * dashSpeedMultiplier);
        if (keyboardInitiated) direction = playerSpeed.Normalized();
		return dashTime;
    }
	private void BurstDashParticles(int powerIndex)
    {
		powerParticles[powerIndex].InitialVelocity = 100;
		powerParticles[powerIndex].LinearAccel = 100;
		powerParticles[powerIndex].Amount *= 10;
		powerParticles[powerIndex].ColorRamp.Colors[0] = Colors.DarkRed;
		powerParticles[powerIndex].ColorRamp.Colors[1] = Colors.OrangeRed;
		powerParticles[powerIndex].ScaleAmount = 3;
	}
	private void ResetDashParticles(int powerIndex)
	{
		try
        {
			powerParticles[powerIndex].InitialVelocity = 0;
			powerParticles[powerIndex].LinearAccel = 0;
			powerParticles[powerIndex].Amount /= 10;
			powerParticles[powerIndex].ColorRamp.Colors[0] = Colors.DarkRed;
			powerParticles[powerIndex].ColorRamp.Colors[1] = Colors.IndianRed;
			powerParticles[powerIndex].ScaleAmount = 2;
		}
		catch { }
	}

	// Za Warudo
	private float ZaWarudo()
    {
		for (int i = 1; i <= 2; i++)
			if (i != playerNumber) ((player)GetNode("../raquette" + i.ToString())).speedModifier = 1 / SLOWDOWNDIVIDER;
		((ball)GetNode("../ball")).speedModifier = 1/SLOWDOWNDIVIDER;
		return SLOWDOWNDURATION;
	}
	private void ZaWarudoTimeout()
    {
		for (int i = 1; i <= 2; i++)
			if (i != playerNumber) ((player)GetNode("../raquette" + i.ToString())).speedModifier = 1;
		((ball)GetNode("../ball")).speedModifier = 1;
	}

	//BlackHole
	private float BlackHole()
    {

		return BLACKHOLEDURATION;
	}
	
	// Animation
	internal void Ping()
	{
		sprite.Play("ping");
	}
	public void Idle()
	{
		sprite.Play("idle");
	}

	// Other
	internal Vector2 getSpeed()
	{
		return playerSpeed;
	}
}
