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
    public player[] rackets = new player[2];
    private globalVariables globalVariables;

    public override void _Ready()
    {
        (globalVariables = (globalVariables)GetNode("/root/GlobalVariables")).PlayMusic("pongMainTheme2.wav");
        leftGoalColor = ((ColorRect)GetNode("goalLeftColor")).Color;
        rightGoalColor = ((ColorRect)GetNode("goalRightColor")).Color;
        field = (ColorRect)GetNode("field");
        fieldColor = field.Color;
        score = (score)GetNode("score");
        (resetFieldColorTimer = (Timer)GetNode("resetFieldColorTimer")).Connect("timeout", this, nameof(ResetFieldColor));
        ((startCountdown)GetNode("/root/StartCountdown")).Countdown();
        for (int i = 0; i<rackets.Length; i++)
            rackets[i] = (player)GetNode("raquette"+(i+1).ToString());
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit")) GetTree().Quit();
    }

    internal void Goal(string goalName)
    {
        ((ball)GetNode("ball")).ResetSpeed();
        ((ball)GetNode("ball")).GlobalPosition = new Vector2(0, 0);
        GoalVisualEffect(!goalName.Contains("Left"));
        Score(!goalName.Contains("Left"));
        if (!globalVariables.countdownOnlyOnStart)
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
            globalVariables.winnerColor = (leftSide) ? leftGoalColor : rightGoalColor;
            globalVariables.winner = (leftSide) ? "Blue" : "Red";
            globalVariables.score = score.get();
            GetTree().ChangeScene("win.tscn");
        }
    }

    private void ResetFieldColor()
    {
        try { field.Color = fieldColor; } catch { }
    }
}
