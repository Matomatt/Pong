using Godot;
using System;
using System.Threading.Tasks;

public class player : KinematicBody2D
{
    [Export]
    private int playerNumber = 1;
    private string pn = "";
    [Export]
    private bool flip = true;
    [Export]
    private int moveSpeed = 500;
    private int lastSpeed = 500;
    [Export]
    private float dashTime = 0.05f;
    private bool dashing = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        pn = playerNumber.ToString();
        this.GetChild<Sprite>(0).FlipH = flip;
        CollisionShape2D hitbox = this.GetChild<CollisionShape2D>(1);
        hitbox.Transform = new Godot.Transform2D(0, new Vector2(5 * ((flip) ? -1 : 1), hitbox.Transform.origin.y));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("quit")) GetTree().Quit();
        if (Input.IsActionPressed("dash"+pn) && !dashing)
        {
            dashing = true;
            lastSpeed = moveSpeed;
            moveSpeed *= 3;
            DashTimeout(dashTime);
        }
        Vector2 movement = new Vector2(
            Input.GetActionStrength("moveright" + pn) - Input.GetActionStrength("moveleft" + pn),
            Input.GetActionStrength("movedown" + pn) - Input.GetActionStrength("moveup" + pn)
        );
        MoveAndCollide(movement.Normalized() * moveSpeed * delta);
    }

    private async void DashTimeout(float time)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(time * 1000f));
        moveSpeed = lastSpeed;
        dashing = false;
    }
}