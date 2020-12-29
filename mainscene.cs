using Godot;
using System;

public class mainscene : Node2D
{

    private Color leftGoalColor;
    private Color rightGoalColor;
    private ColorRect field;
    private Color fieldColor;
    private score score;
    private Timer resetFieldColorTimer;

    public override void _Ready()
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).PlayMusic("pongMainTheme2.wav");
        leftGoalColor = ((ColorRect)GetNode("goalLeftColor")).Color;
        rightGoalColor = ((ColorRect)GetNode("goalRightColor")).Color;
        field = (ColorRect)GetNode("field");
        fieldColor = field.Color;
        score = (score)GetNode("score");
        (resetFieldColorTimer = (Timer)GetNode("resetFieldColorTimer")).Connect("timeout", this, nameof(ResetFieldColor));
        ((startCountdown)GetNode("/root/StartCountdown")).Countdown();
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit")) GetTree().Quit();
    }

    internal void Goal(string goalName)
    {
        GoalVisualEffect(!goalName.Contains("Left"));
        Score(!goalName.Contains("Left"));
        if (!((globalVariables)GetNode("/root/GlobalVariables")).countdownOnlyOnStart)
            ((startCountdown)GetNode("/root/StartCountdown")).Countdown();
    }

    private void GoalVisualEffect(bool leftSide)
    {
        field.Color = (leftSide) ? leftGoalColor : rightGoalColor;
        resetFieldColorTimer.Start(0.25f);
    }
    private void Score(bool leftSide)
    {
        if (score.Goal(leftSide))
        {
            ((globalVariables)GetNode("/root/GlobalVariables")).winnerColor = (leftSide) ? leftGoalColor : rightGoalColor;
            ((globalVariables)GetNode("/root/GlobalVariables")).winner = (leftSide) ? "Blue" : "Red";
            ((globalVariables)GetNode("/root/GlobalVariables")).score = score.get();
            GetTree().ChangeScene("win.tscn");
        }
            
    }

    private void ResetFieldColor()
    {
        try { field.Color = fieldColor; } catch { }
    }
}
