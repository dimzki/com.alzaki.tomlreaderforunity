using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Tomlyn;

namespace Alzaki.TomlReader
{
    public class TomlSettingsManager : MonoBehaviour
    {
        public static TomlSettingsManager Instance {get; private set;}

        public static GameSettings Current { get; private set; }

        public static event Action<GameSettings> OnSettingsReloaded;

        [Tooltip("Enable file watching for hot-reload")]
        public bool enableFileWatcher = true;

        private FileSystemWatcher watcher;
        private string filePath;
        private float reloadDelay = 0.2f;
        private float lastChangeTime;
        private bool fileChanged = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);


            filePath = Path.Combine(Application.streamingAssetsPath, "settings.ini");

            EnsureFileExists();
            LoadSettings();

            // Only enable file watcher if explicitly enabled
            if (enableFileWatcher)
            {
                StartWatcher();
            }
        }

        private void Update()
    {
        // Skip Update check if watcher is disabled
        if (!enableFileWatcher) return;

        // Handle file change from background thread
        if (fileChanged)
        {
            fileChanged = false;
            lastChangeTime = Time.realtimeSinceStartup;
            Debug.Log($"<color=yellow>Settings file changed, will reload in {reloadDelay}s</color>");
        }

        // Small delay prevents double reload spam
        if (lastChangeTime > 0 && Time.realtimeSinceStartup - lastChangeTime > reloadDelay)
        {
            lastChangeTime = 0;
            LoadSettings();
        }
    }

        private void OnDestroy()
    {
        watcher?.Dispose();
    }

        // ---------------- LOADING ----------------

        public void ReloadSettingsManually()
    {
        LoadSettings();
    }

        private void LoadSettings()
    {
        try
        {
            string text = File.ReadAllText(filePath);
            var table = Toml.ToModel(text);

            var settings = new GameSettings();
            PopulateObject(settings, table);

            Validate(settings);
            Current = settings;

            Debug.Log("<color=cyan>Settings reloaded</color>");
            OnSettingsReloaded?.Invoke(Current);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to reload settings:\n" + ex.Message);
        }
    }

        private void PopulateObject(object target, IDictionary<string, object> data)
    {
        var type = target.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!prop.CanWrite || !data.ContainsKey(prop.Name))
                continue;

            var value = data[prop.Name];

            // Handle nested objects (tables)
            if (value is IDictionary<string, object> nestedTable)
            {
                var nestedObj = prop.GetValue(target);
                if (nestedObj != null)
                {
                    PopulateObject(nestedObj, nestedTable);
                }
            }
            // Handle primitive types
            else
            {
                try
                {
                    var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(target, convertedValue);
                }
                catch
                {
                    Debug.LogWarning($"Failed to set property {prop.Name} with value {value}");
                }
            }
        }
    }

        // ---------------- FILE WATCHER ----------------

        private void StartWatcher()
    {
        try
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                Debug.LogWarning($"Cannot watch settings: directory does not exist: {directory}");
                return;
            }

            watcher = new FileSystemWatcher
            {
                Path = directory,
                Filter = Path.GetFileName(filePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += OnFileChanged;
            watcher.Error += OnWatcherError;
            watcher.EnableRaisingEvents = true;

            Debug.Log($"<color=green>File watcher started for: {filePath}</color>");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to start file watcher: {ex.Message}");
        }
    }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // This runs on a background thread, so just set a flag
        fileChanged = true;
    }

        private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        Debug.LogError($"File watcher error: {e.GetException()?.Message}");
    }

        // ---------------- DEFAULT FILE ----------------

        private void EnsureFileExists()
    {
        if (File.Exists(filePath)) return;

        var defaultSettings = new GameSettings();
        string toml = Toml.FromModel(defaultSettings);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, toml);
    }

        // ---------------- VALIDATION ----------------

        private void Validate(GameSettings settings)
    {
        ValidateObject(settings);
    }

        private void ValidateObject(object target)
    {
        if (target == null) return;

        var type = target.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!prop.CanWrite) continue;

            var value = prop.GetValue(target);

            // Ensure nested objects exist
            if (value == null && prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
            {
                var newInstance = Activator.CreateInstance(prop.PropertyType);
                prop.SetValue(target, newInstance);
                value = newInstance;
            }

            // Apply Range validation
            var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null && value != null)
            {
                if (prop.PropertyType == typeof(int))
                {
                    var intValue = (int)value;
                    var clamped = Mathf.Clamp(intValue, (int)rangeAttr.min, (int)rangeAttr.max);
                    prop.SetValue(target, clamped);
                }
                else if (prop.PropertyType == typeof(float))
                {
                    var floatValue = (float)value;
                    var clamped = Mathf.Clamp(floatValue, rangeAttr.min, rangeAttr.max);
                    prop.SetValue(target, clamped);
                }
            }

            // Recursively validate nested objects
            if (value != null && prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
            {
                ValidateObject(value);
            }
        }
    }
    }
}
