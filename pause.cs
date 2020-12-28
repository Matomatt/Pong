using Godot;
using System;

//COntinue button
//https://www.youtube.com/watch?v=WaotOuDNio8

public class pause : CanvasLayer
{
    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause")) GetTree().Paused = !GetTree().Paused;
    }
}
