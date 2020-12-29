using Godot;
using System;

public class globalVariables : CanvasLayer
{
    [Export]
    private float windowScale = 3f;
    [Export]
    public int winningScore = 11;
    AudioStreamPlayer musicPlayer;
    AudioStreamPlayer sfxPlayer;
    public bool countdownOnlyOnStart = true;
    public string winner = "";
    public string score = "";
    public Color winnerColor;

    public override void _Ready()
    {
        OS.WindowSize *= windowScale;
        musicPlayer = (AudioStreamPlayer)GetNode("musicPlayer");
        sfxPlayer = (AudioStreamPlayer)GetNode("sfxPlayer");
    }

    internal void PlayMusic(string name)
    {
        musicPlayer.PlaySample(name, true);
    }

    internal void StopMusic()
    {
        musicPlayer.StopPlaying();
    }

    internal void PlaySfx(string name)
    {
        sfxPlayer.PlaySample(name, false);
    }

    internal void setMusicVolume(float volume)
    {
        musicPlayer.VolumeDb = volume;
    }
    internal void setSfxVolume(float volume)
    {
        sfxPlayer.VolumeDb = volume;
    }
    internal float getMusicVolume()
    {
        return musicPlayer.VolumeDb;
    }
    internal float getSfxVolume()
    {
        return sfxPlayer.VolumeDb;
    }

    internal string getMusicPlaying()
    {
        if (musicPlayer.Stream == null) return "none";
        if (musicPlayer.Stream.ResourcePath.Contains("res://Music and sounds/"))
            return musicPlayer.Stream.ResourcePath.Replace("res://Music and sounds/", "");
        return "unknown";
    }
}
