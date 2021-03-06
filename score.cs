using Godot;
using System;

public class score : Label
{
    private int leftScore = 0;
    private int rigthScore = 0;

    public override void _Ready()
    {
        
    }

    public bool Goal(bool left)
    {
        if (left) leftScore += 1;
        else rigthScore += 1;

        this.Text = leftScore + " : " + rigthScore;

        if (Math.Max(leftScore, rigthScore) >= ((globalVariables)GetNode("/root/GlobalVariables")).winningScore)
            return true;
        return false;
    }

    internal string get()
    {
        return leftScore + " : " + rigthScore;
    }
}
