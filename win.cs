using Godot;
using System;

public class win : CanvasLayer
{
    private Timer musicDelayedTimer;

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        GetNode("MarginContainer/VBoxContainer/rematchButton").Connect("pressed", this, nameof(Rematch));
        GetNode("MarginContainer/VBoxContainer/menuButton").Connect("pressed", this, nameof(Menu));
        GetNode("MarginContainer/VBoxContainer/quitButton").Connect("pressed", this, nameof(Quit));

        GetNode("ColorRect").Set("color", ((globalVariables)GetNode("/root/GlobalVariables")).winnerColor.Lightened(0.5f));
        GetNode("MarginContainer/VBoxContainer/VBoxContainer/Label").Set("custom_colors/font_color", ((globalVariables)GetNode("/root/GlobalVariables")).winnerColor);
        GetNode("MarginContainer/VBoxContainer/VBoxContainer/Label").Set("text", ((globalVariables)GetNode("/root/GlobalVariables")).winner + " wins !");
        GetNode("MarginContainer/VBoxContainer/VBoxContainer/Label2").Set("text", ((globalVariables)GetNode("/root/GlobalVariables")).score);

        (musicDelayedTimer = (Timer)GetNode("musicDelayedTimer")).Connect("timeout", this, nameof(PlayIdleTheme));

        ((globalVariables)GetNode("/root/GlobalVariables")).StopMusic();
        ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("victory.wav");
        musicDelayedTimer.Start(-1f);
    }
    private void PlayIdleTheme()
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).PlayMusic("victoryMenu.wav");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit")) GetTree().Quit();
    }

    private void Rematch()
    {
        GetTree().ChangeScene("mainscene.tscn");
    }

    private void Menu()
    {
        GetTree().ChangeScene("menu.tscn");
    }

    private void Quit()
    {
        GetTree().Quit();
    }
}
