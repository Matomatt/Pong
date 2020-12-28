using Godot;
using System;

public class Menu : Control
{
    [Export]
    private float windowScale = 3f;
    private TextureButton startButton;
    private TextureButton settingsButton;

    public override void _Ready()
    {
        OS.WindowSize *= windowScale;
        startButton = (TextureButton)GetNode(new NodePath("MarginContainer/VBoxContainer/VBoxContainer/startButton"));
        startButton.Connect("pressed", this, nameof(StartGame));
        settingsButton = (TextureButton)GetNode(new NodePath("MarginContainer/VBoxContainer/VBoxContainer/settingsButton"));
        settingsButton.Connect("pressed", this, nameof(Settings));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("quit")) GetTree().Quit();
    }

    private void StartGame()
    {
        GD.Print("Starting the game !");
        GetTree().ChangeScene("mainscene.tscn");
    }
    
    private void Settings()
    {
        GD.Print("Settings");
    }

}
