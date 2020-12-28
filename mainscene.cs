using Godot;
using System;

public class mainscene : Node2D
{
    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("quit")) GetTree().Quit();
    }
}
