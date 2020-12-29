using Godot;
using System;

public class menu : Control
{
    private TextureButton startButton;
    private TextureButton settingsButton;

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        if (((globalVariables)GetNode("/root/GlobalVariables")).getMusicPlaying() != "Ping Pong The Animation OST Ping Pong Phase 1 & 2 [Extended].wav")
            ((globalVariables)GetNode("/root/GlobalVariables")).PlayMusic("Ping Pong The Animation OST Ping Pong Phase 1 & 2 [Extended].wav");
        startButton = (TextureButton)GetNode(new NodePath("MarginContainer/VBoxContainer/VBoxContainer/startButton"));
        startButton.Connect("pressed", this, nameof(StartGame));
        settingsButton = (TextureButton)GetNode(new NodePath("MarginContainer/VBoxContainer/VBoxContainer/settingsButton"));
        settingsButton.Connect("pressed", this, nameof(Settings));
        GetNode(new NodePath("MarginContainer/VBoxContainer/VBoxContainer/quitButton")).Connect("pressed", this, nameof(Quit));
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit")) Quit();
    }

    private void Quit()
    {
        GetTree().Quit();
    }
    private void StartGame()
    {
        GD.Print("Starting the game !");
        ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("start.wav");
        GetTree().ChangeScene("mainscene.tscn");
    }
    
    private void Settings()
    {
        GD.Print("Settings");
        GetTree().ChangeScene("settings.tscn");
    }

}
