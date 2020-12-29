using Godot;
using System;

public class AudioStreamPlayer : Godot.AudioStreamPlayer
{
    [Export]
    private AudioStreamSample[] playables;
    [Export]
    private bool looping = false;

    public override void _Process(float delta)
    {
        if (!this.Playing && looping) this.Play();
    }

    internal void PlaySample(string name, bool loop)
    {
        looping = loop;
        foreach (AudioStreamSample audio in playables)
        {
            if (audio.ResourcePath == "res://Music and sounds/" + name)
            {
                this.Stream = audio;
                break;
            }
        }
        Play();
    }

    public void StopPlaying()
    {
        looping = false;
        Stop();
    }
}
