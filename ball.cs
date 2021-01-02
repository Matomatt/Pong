using Godot;
using System;
using System.Threading.Tasks;

public class ball : KinematicBody2D
{
    const int INITIALSPEED = 120;
    const float SPEEDMULTIPLIER = 0.5f;
    const int MAXSPEED = 800;
    private Vector2 speed;
    private DateTime[] latestCollide = { DateTime.Now, DateTime.Now };
    const float TIMEBETWEENCOLLISIONCALCULATIONS = 0.1f;
    public float speedModifier = 1f;
    [Export]
    private float timeBeforeUnblocking = 1f;
    private Vector2 lastPosition;
    private DateTime blockingStartedAt = DateTime.Now;
    [Export]
    private float samplePositionInterval = 0.2f;
    private float samplePositionTime = 0f;

    public override void _Ready()
    {
        GD.Randomize();
        resetSpeed();

    }

    private void resetSpeed()
    {
        speed = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
        speed *= INITIALSPEED;
    }
    public override void _PhysicsProcess(float delta)
    {
        if (GlobalPosition.x > 256 || GlobalPosition.x < -256 || GlobalPosition.y > 128 | GlobalPosition.y < -128)
            GlobalPosition = new Vector2(0, 0);

        KinematicCollision2D collision = MoveAndCollide(speed * delta * speedModifier, infiniteInertia: false);

        if (collision != null)
        {
            speed = speed.Bounce(collision.Normal);
            string colliderName = ((Node2D)collision.Collider).Name;
            if (colliderName.Contains("raquette"))
                collideWith((player)collision.Collider, delta, collision.Normal);
            else if (colliderName.Contains("goal"))
            {
                resetSpeed();
                GlobalPosition = new Vector2(0, 0);
                ((mainscene)GetNode("/root/mainscene")).Goal(colliderName);
            }
        }

        if (lastPosition.DistanceTo(GlobalPosition)>0.5) blockingStartedAt = DateTime.Now;
        GD.Print(lastPosition.DistanceTo(GlobalPosition));
        //GD.Print(touchingBall.ToString() + " + " + (int)lastPosition.x + " == " + (int)GlobalPosition.x + " && " + ((int)lastPosition.y == (int)GlobalPosition.y) + " - " + blockingStartedAt);
        if (lastPosition.DistanceTo(GlobalPosition) <= 0.5 && blockingStartedAt.AddSeconds(timeBeforeUnblocking) < DateTime.Now)
        {
            GD.Print("BOOM");
            player[] rackets = ((mainscene)GetNode("/root/mainscene")).rackets;
            for (int i = 0; i < rackets.Length; i++)
            {
                GD.Print(rackets[i].GlobalPosition.x + " - " + GlobalPosition.x);
                rackets[i].ApplyForce(new Vector2(rackets[i].GlobalPosition.x - GlobalPosition.x-256, rackets[i].GlobalPosition.y - GlobalPosition.y-128).Normalized());
            }
                
        }
        samplePositionTime -= delta;
        if (samplePositionTime <= 0)
        {
            lastPosition = GlobalPosition;
            samplePositionTime = samplePositionInterval;
        }
        
    }
        
    internal void collideWith(player raquette, float delta, Vector2 normal)
    {
        normal = new Vector2(Math.Abs(normal.x), Math.Abs(normal.y));
        if (latestCollide[raquette.Name[raquette.Name.Length - 1] - '1'].CompareTo(DateTime.Now) < 0)
        {
            speed += speed * SPEEDMULTIPLIER * delta + normal * raquette.getSpeed();
            if (speed.Length() > MAXSPEED) speed = speed.Normalized() * MAXSPEED;
            latestCollide[raquette.Name[raquette.Name.Length - 1] - '1'] = DateTime.Now.AddSeconds(TIMEBETWEENCOLLISIONCALCULATIONS);
            raquette.Ping();
        }

    }
}
