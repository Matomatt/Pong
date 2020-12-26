using Godot;
using System;
using System.Threading.Tasks;

public class player : KinematicBody2D
{
    [Export]
    private int movespeed = 500;
    private int lastspeed = 500;
    [Export]
    private float dashtime = 0.2f;
    private bool dashing = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("quit")) GetTree().Quit();
        if (Input.IsActionPressed("spacebar") && !dashing)
        {
            dashing = true;
            lastspeed = movespeed;
            movespeed *= 3;
            DashTimeout(dashtime);
        }
        Vector2 movement = new Vector2(
            Input.GetActionStrength("moveright") - Input.GetActionStrength("moveleft"),
            Input.GetActionStrength("movedown") - Input.GetActionStrength("moveup")
        );
        MoveAndCollide(movement.Normalized() * movespeed * delta);
    }

    private async void DashTimeout(float time)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(time * 1000f));
        movespeed = lastspeed;
        dashing = false;
    }
}