A lightweight and easy-to-use TOML configuration loader for Unity projects.
Load structured .toml files into C# data classes at runtime or in the editor.

Designed for game settings, balancing data, configs, and tools.


✨ Features
- Load TOML files into strongly typed C# classes
- Works in runtime builds and inside the Unity Editor
- Simple API — no complex setup required
- Includes a sample demo scene
- Distributed as a UPM Git package

✨ Installation (UPM via Git)
Open Package Manager → Add package from Git URL…

✨ Quick Start
1️⃣ Create a TOML file
Example settings.toml:

[graphics]
resolutionWidth = 1920
resolutionHeight = 1080
fullscreen = true

[audio]
masterVolume = 0.8
musicVolume = 0.6
sfxVolume = 0.7

2️⃣ Create Matching Data Classes
[Serializable]
public class GraphicsSettings
{
    public int resolutionWidth;
    public int resolutionHeight;
    public bool fullscreen;
}

[Serializable]
public class AudioSettings
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
}

[Serializable]
public class GameSettings
{
    public GraphicsSettings graphics;
    public AudioSettings audio;
}

3️⃣ Load the TOML File
using Alzaki.TomlReader;

public class SettingsLoader : MonoBehaviour
{
    private void Start()
    {
        var settings = TomlSettingsLoader.Load<GameSettings>("settings.toml");

        Debug.Log($"Resolution: {settings.graphics.resolutionWidth}x{settings.graphics.resolutionHeight}");
        Debug.Log($"Master Volume: {settings.audio.masterVolume}");
    }
}
