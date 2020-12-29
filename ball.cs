using Godot;
using System;
using System.Threading.Tasks;

public class ball : KinematicBody2D
{
    [Export]
    private int initialSpeed = 120;
    [Export]
    private float speedMultiplier = 0.5f;
    [Export]
    private int maxSpeed = 800;
    private Vector2 speed;
    private DateTime[] latestCollide = { DateTime.Now, DateTime.Now };
    [Export]
    private float timeBetweenCollisionCalculations = 0.1f;
    

    public override void _Ready()
    {
        GD.Randomize();
        resetSpeed();
    }

    private void resetSpeed()
    {
        speed = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
        speed *= initialSpeed;
    }
    public override void _PhysicsProcess(float delta)
    {
        if (GlobalPosition.x > 256 || GlobalPosition.x < -256 || GlobalPosition.y > 128 | GlobalPosition.y < -128)
            GlobalPosition = new Vector2(0, 0);

        KinematicCollision2D collision = MoveAndCollide(speed * delta, infiniteInertia: false);
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
    }
        
    internal void collideWith(player raquette, float delta, Vector2 normal)
    {
        normal = new Vector2(Math.Abs(normal.x), Math.Abs(normal.y));
        if (latestCollide[raquette.Name[raquette.Name.Length - 1] - '1'].CompareTo(DateTime.Now) < 0)
        {
            speed += speed * speedMultiplier * delta + normal * raquette.getSpeed();
            if (speed.Length() > maxSpeed) speed = speed.Normalized() * maxSpeed;
            latestCollide[raquette.Name[raquette.Name.Length - 1] - '1'] = DateTime.Now.AddSeconds(timeBetweenCollisionCalculations);
            raquette.Ping();
        }

    }
    
}
