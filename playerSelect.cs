using Godot;
using System;

public class playerSelect : Control
{
    private TextureButton startButton;
    private TextureButton backButton;
    private TextureButton settingsButton;
    private globalVariables globalVariables;
    private OptionButton[] optionButtonsPower1 = new OptionButton[2];
    private OptionButton[] optionButtonsPower2 = new OptionButton[2];

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        globalVariables = (globalVariables)GetNode("/root/GlobalVariables");

        startButton = (TextureButton)GetNode(new NodePath("startButton"));
        startButton.Connect("pressed", this, nameof(StartGame));
        backButton = (TextureButton)GetNode(new NodePath("backButton"));
        backButton.Connect("pressed", this, nameof(Back));

        for(int i = 1; i<=2; i++)
        {
            optionButtonsPower1[i - 1] = (OptionButton)GetNode("MarginContainer/HBoxContainer/VBoxContainer" + i.ToString() + "/OptionButton1");
            optionButtonsPower2[i - 1] = (OptionButton)GetNode("MarginContainer/HBoxContainer/VBoxContainer" + i.ToString() + "/OptionButton2");
        }

        foreach (Godot.Collections.Dictionary power in globalVariables.powers)
            for (int i = 0; i < 2; i++)
                if (power["type"].ToString() == "1")
                    optionButtonsPower1[i].AddItem(power["name"].ToString());
                else if (power["type"].ToString() == "2")
                    optionButtonsPower2[i].AddItem(power["name"].ToString());
        //GetNode(new NodePath("MarginContainer/VBoxContainer/VBoxContainer/quitButton")).Connect("pressed", this, nameof(Quit));
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit")) Quit();
    }

    private void Quit()
    {
        GetTree().Quit();
    }
    private void Back()
    {
        GetTree().ChangeScene("menu.tscn");
    }
    private void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (Godot.Collections.Dictionary power in globalVariables.powers)
                if (power["name"].ToString() == optionButtonsPower1[i].Text)
                    globalVariables.selectedPowers[i*2] = power["id"].ToString();
                else if (power["name"].ToString() == optionButtonsPower2[i].Text)
                    globalVariables.selectedPowers[i*2+1] = power["id"].ToString();
        }
             
        GD.Print("Starting the game !");
        ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("start.wav");
        GetTree().ChangeScene("mainscene.tscn");
    }
}
