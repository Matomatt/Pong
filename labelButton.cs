using Godot;
using System;

public class labelButton : TextureButton
{
    private Color fontColor;
    private Color fontOutlineColor;
    private Label textLabel;
    public override void _Ready()
    {
        textLabel = (Label)GetNode("label");
        fontColor = (Color)textLabel.Get("custom_colors/font_color");
        fontOutlineColor = (Color)textLabel.Get("custom_colors/font_outline_modulate");
        Connect("focus_entered", this, nameof(onFocusEntered));
        Connect("focus_exited", this, nameof(onFocusExited));
        Connect("mouse_entered", this, nameof(onFocusEntered));
        Connect("mouse_exited", this, nameof(onFocusExited));
        Connect("button_up", this, nameof(Sfx));
    }

    private void onFocusEntered()
    {
        textLabel.Set("custom_colors/font_color", fontOutlineColor);
        textLabel.Set("custom_colors/font_outline_modulate", fontColor);
    }
    private void onFocusExited()
    {
        textLabel.Set("custom_colors/font_color", fontColor);
        textLabel.Set("custom_colors/font_outline_modulate", fontOutlineColor);
    }

    private void Sfx()
    {
        ((globalVariables)GetNode("/root/GlobalVariables")).PlaySfx("blip.wav");
    }
}
