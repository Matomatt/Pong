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
    const float TIMEBETWEENCOLLISIONCALCULATIONS = 0.05f;
    public float speedModifier = 1f;
    [Export]
    private float timeBeforeUnblocking = 1f;
    private Vector2 lastPosition;
    private DateTime blockingStartedAt = DateTime.Now;
    private AnimatedSprite explosionAnimatedSprite;
    [Export]
    private float samplePositionInterval = 0.2f;
    private float samplePositionTime = 0f;

    public override void _Ready()
    {
        GD.Randomize();
        ResetSpeed();
        (explosionAnimatedSprite = (AnimatedSprite)GetNode("../explosion")).Connect("animation_finished", this, nameof(ExplosionEnded));
    }

    internal void ResetSpeed()
    {
        speed = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
        speed *= INITIALSPEED;
    }
    public override void _PhysicsProcess(float delta)
    {
        if (GlobalPosition.x > 256 || GlobalPosition.x < -256 || GlobalPosition.y > 128 | GlobalPosition.y < -128)
            GlobalPosition = new Vector2(0, 0);

        MoveAndBounce(speed * speedModifier, delta);
    }

    internal void MoveAndBounce(Vector2 _speed, float delta)
    {
        KinematicCollision2D collision = MoveAndCollide(_speed*delta, infiniteInertia: false);

        if (collision != null && latestCollide[0].CompareTo(DateTime.Now) < 0)
        {
            speed = speed.Bounce(collision.Normal);
            string colliderName = ((Node2D)collision.Collider).Name;
            if (colliderName.Contains("raquette"))
                collideWith((player)collision.Collider, delta, collision.Normal);
            else if (colliderName.Contains("goal"))
                ((mainscene)GetNode("/root/mainscene")).Goal(colliderName);
            latestCollide[0] = DateTime.Now.AddSeconds(TIMEBETWEENCOLLISIONCALCULATIONS);
        }

        StuckExplosionCalculations(delta);
    }

    private void StuckExplosionCalculations(float delta)
    {
        if (lastPosition.DistanceTo(GlobalPosition) > 0.5) blockingStartedAt = DateTime.Now;

        if (lastPosition.DistanceTo(GlobalPosition) <= 0.5 && blockingStartedAt.AddSeconds(timeBeforeUnblocking) < DateTime.Now)
        {
            explosionAnimatedSprite.GlobalPosition = GlobalPosition + new Vector2(256, 128);
            explosionAnimatedSprite.Play("boom");
            ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("explosion.wav");
            player[] rackets = ((mainscene)GetNode("/root/mainscene")).rackets;
            for (int i = 0; i < rackets.Length; i++)
                rackets[i].ApplyForce(new Vector2(rackets[i].GlobalPosition.x - GlobalPosition.x - 256, rackets[i].GlobalPosition.y - GlobalPosition.y - 128).Normalized());
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
        //((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("ping" + ((int)GD.RandRange(0, 5)).ToString()+".wav");
        normal = new Vector2(Math.Abs(normal.x), Math.Abs(normal.y));
        speed += speed * SPEEDMULTIPLIER * delta + normal * raquette.getSpeed();
        if (speed.Length() > MAXSPEED) speed = speed.Normalized() * MAXSPEED;
        raquette.Ping();
    }

    private void ExplosionEnded()
    {
        explosionAnimatedSprite.Play("default");
    }
}
