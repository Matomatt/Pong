using Godot;
using System;

public class startCountdown : CanvasLayer
{
    [Export]
    private float intervalTime = 0.5f;
    [Export]
    private int startingAt = 3;
    private int countdown;
    private Label label;
    private ColorRect colorRect;
    private Timer timer;
    public override void _Ready()
    {
        countdown = startingAt;
        label = (Label)GetNode("Label");
        colorRect = (ColorRect)GetNode("ColorRect");
        timer = (Timer)GetNode("countdownTimer");
        timer.WaitTime = intervalTime;
        timer.Connect("timeout", this, nameof(Countdown));
    }

    internal void Countdown()
    {
        label.Text = countdown.ToString();
        if (countdown == startingAt)
        {
            if (GetTree().Paused) return;
            GetTree().Paused = true;
            colorRect.Visible = true;
            label.Visible = true;

        }
        else if (countdown == 0)
        {
            timer.Stop();
            colorRect.Visible = false;
            label.Visible = false;
            GetTree().Paused = false;
            countdown = startingAt;
            return;
        }
        countdown -= 1;
        timer.Start(intervalTime);
    }

    internal bool isCounting()
    {
        return (countdown != startingAt);
    }
}
