using Godot;
using System;

public class mainscene : Node2D
{
    [Export]
    private float windowScale = 3f;
    public override void _Ready()
    {
        OS.WindowSize *= windowScale;
    }
}
