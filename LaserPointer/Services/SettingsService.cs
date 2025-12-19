using System;
using Windows.Storage;
using LaserPointer.Models;
using System.Text.Json;

namespace LaserPointer.Services
{
    public class SettingsService
    {
        private const string SettingsKey = "LaserSettings";
        private LaserSettings? _settings;

        public LaserSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    LoadSettings();
                }
                return _settings ?? new LaserSettings();
            }
        }

        public event EventHandler<LaserSettings>? SettingsChanged;

        public void LoadSettings()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (localSettings.Values.ContainsKey(SettingsKey))
                {
                    var json = localSettings.Values[SettingsKey]?.ToString();
                    if (!string.IsNullOrEmpty(json))
                    {
                        _settings = JsonSerializer.Deserialize<LaserSettings>(json);
                    }
                }
            }
            catch
            {
                // If loading fails, use defaults
            }

            if (_settings == null)
            {
                _settings = new LaserSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                var json = JsonSerializer.Serialize(_settings);
                localSettings.Values[SettingsKey] = json;
                SettingsChanged?.Invoke(this, _settings!);
            }
            catch
            {
                // Handle save error
            }
        }

        public void UpdateSettings(LaserSettings newSettings)
        {
            _settings = newSettings;
            SaveSettings();
        }
    }
}

