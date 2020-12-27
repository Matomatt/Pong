using Godot;
using System;

public class score : Label
{
    private int leftScore = 0;
    private int rigthScore = 0;

    public override void _Ready()
    {
        
    }

    public void goal(bool left)
    {
        if (left) leftScore += 1;
        else rigthScore += 1;

        this.Text = leftScore + " : " + rigthScore;
    }

}
