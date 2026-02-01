# Toml Reader For Unity

A lightweight TOML configuration loader for Unity.

Load structured `.toml` files into C# data classes at runtime or in the editor.

---

## ✨ Features

- Reads TOML files into strongly typed C# classes  
- Works in both runtime and Unity Editor  
- Simple API and easy to use  
- Includes sample demo scene  
- Install as a Git package

---

## 📦 Installation (UPM via Git)

In Unity's **Package Manager → Add package from git URL…**:


---

## 🚀 Quick Start

### 1. Create a TOML file

Example `settings.toml`:

```toml
[graphics]
resolutionWidth = 1920
resolutionHeight = 1080
fullscreen = true

[audio]
masterVolume = 0.8
musicVolume = 0.6
```

### 2. Create Data Classes

```
[Serializable]
public class GraphicsSettings
{
    public int resolutionWidth;
    public int resolutionHeight;
    public bool fullscreen;
}
```

### 3. Load the File

```
var settings = TomlSettingsLoader.Load<GameSettings>("settings.toml");
```
