using Godot;
using System;

//COntinue button
//https://www.youtube.com/watch?v=WaotOuDNio8

public class pause : CanvasLayer
{
    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        GetNode("MarginContainer/VBoxContainer/continueButton").Connect("pressed", this, nameof(TogglePause));
        GetNode("MarginContainer/VBoxContainer/menuButton").Connect("pressed", this, nameof(Back));
        GetNode("MarginContainer/VBoxContainer/quitButton").Connect("pressed", this, nameof(Quit));
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("pause")) TogglePause();
        if (@event.IsActionPressed("quit")) GetTree().Quit();
    }

    private void TogglePause()
    {
        if (((startCountdown)GetNode("/root/StartCountdown")).isCounting()) return;
        if (!GetTree().Paused) ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("titootitoo.wav");
        GetTree().Paused = !GetTree().Paused;
        ((ColorRect)GetNode("ColorRect")).Visible = !((ColorRect)GetNode("ColorRect")).Visible;
        ((MarginContainer)GetNode("MarginContainer")).Visible = !((MarginContainer)GetNode("MarginContainer")).Visible;
    }

    private void Back()
    {
        GetTree().ChangeScene("menu.tscn");
        TogglePause();
    }

    private void Quit()
    {
        GetTree().Quit();
    }
}
