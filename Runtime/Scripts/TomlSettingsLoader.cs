using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Tomlyn;

namespace Alzaki.TomlReader
{
    public static class TomlSettingsLoader
    {
        private static string FilePath => Path.Combine(Application.streamingAssetsPath, "settings.ini");

        public static GameSettings Load()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("Settings file not found. Using defaults.");
                return new GameSettings();
            }

            string text = File.ReadAllText(FilePath);
            var table = Toml.ToModel(text);

            var settings = new GameSettings();
            PopulateObject(settings, table);

            Validate(settings);

            return settings;
        }

        private static void PopulateObject(object target, IDictionary<string, object> data)
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

        private static void Validate(GameSettings settings)
        {
            ValidateObject(settings);
        }

        private static void ValidateObject(object target)
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

        public static void EnsureFileExists()
        {
            if (File.Exists(FilePath))
                return;

            var defaultSettings = new GameSettings();
            string toml = Toml.FromModel(defaultSettings);

            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, toml);

            Debug.Log("Default settings file created at: " + FilePath);
        }

    }
}