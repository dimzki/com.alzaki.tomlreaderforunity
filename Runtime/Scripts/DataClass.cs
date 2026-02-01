using System;
using UnityEngine;

[Serializable]
public class GameSettings
{
    public GraphicsSettings Graphics { get; set; }
    public AudioSettings Audio { get; set; }

    public GameSettings()
    {
        Graphics = new GraphicsSettings();
        Audio = new AudioSettings();
    }
}

[Serializable]
public class GraphicsSettings
{
    public bool Fullscreen { get; set; } = true;

    [Range(800, 7680)]
    public int Width { get; set; } = 1920;

    [Range(600, 4320)]
    public int Height { get; set; } = 1080;

    public bool VSync { get; set; } = false;
}

[Serializable]
public class AudioSettings
{
    [Range(0f, 1f)]
    public float MasterVolume { get; set; } = 1f;

    [Range(0f, 1f)]
    public float MusicVolume { get; set; } = 1f;

    [Range(0f, 1f)]
    public float SfxVolume { get; set; } = 1f;
}


