using Godot;
using System;

public class settings : Control
{
    private HSlider musicSlider;
    private HSlider sfxSlider;
    private HSlider scoreSlider;
    private CheckButton countdownCheck;
    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        musicSlider = (HSlider)GetNode("MarginContainer/VBoxContainer/musicVolume");
        musicSlider.Connect("value_changed", this, nameof(ChangeMusicVolume));
        musicSlider.Value = ((globalVariables)GetNode("/root/GlobalVariables")).getMusicVolume();
        sfxSlider = (HSlider)GetNode("MarginContainer/VBoxContainer/sfxVolume");
        sfxSlider.Connect("value_changed", this, nameof(ChangeSfxVolume));
        sfxSlider.Value = ((globalVariables)GetNode("/root/GlobalVariables")).getSfxVolume();
        scoreSlider = (HSlider)GetNode("MarginContainer/VBoxContainer/winningScore");
        scoreSlider.Connect("value_changed", this, nameof(ChangeWinningScore));
        scoreSlider.Value = ((globalVariables)GetNode("/root/GlobalVariables")).winningScore;
        countdownCheck = (CheckButton)GetNode("MarginContainer/VBoxContainer/countdownCheck");
        countdownCheck.Connect("toggled", this, nameof(toggleCountdown));
        countdownCheck.Pressed = ((globalVariables)GetNode("/root/GlobalVariables")).countdownOnlyOnStart;
        GetNode("MarginContainer/VBoxContainer/backButton").Connect("pressed", this, nameof(Back));
    }
    private void ChangeMusicVolume(float score)
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).setMusicVolume(score);
    }
    private void ChangeSfxVolume(float score)
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).setSfxVolume(score);
    }
    private void ChangeWinningScore(float score)
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).winningScore = (int)score;
        ((Label)scoreSlider.GetNode("Label")).Text = ((int)score).ToString();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit")) GetTree().Quit();
    }

    private void Back()
    {
        GetTree().ChangeScene("menu.tscn");
    }

    private void toggleCountdown(bool val)
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("blip.wav");
        ((globalVariables)GetNode("/root/GlobalVariables")).countdownOnlyOnStart = val;
    }
}
