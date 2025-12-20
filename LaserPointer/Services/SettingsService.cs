using System;
using System.IO;
using System.Text.Json;
using Windows.Storage; // Still needed for ApplicationData.Current.LocalFolder.Path
using LaserPointer.Models;

namespace LaserPointer.Services
{
    public class SettingsService
    {
        private LaserSettings? _settings;
        private const string SettingsFileName = "LaserSettings.json";
        private readonly string _settingsFilePath;

        public event EventHandler<LaserSettings>? SettingsChanged;

        public LaserSettings Settings => _settings ??= LoadSettings();

        public SettingsService()
        {
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:SettingsService", message = "SettingsService constructor entry", data = new { serviceInstance = this.GetHashCode(), currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            try
            {
                _settingsFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, SettingsFileName);
                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:SettingsService", message = "Settings file path determined (ApplicationData)", data = new { path = _settingsFilePath, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            catch (Exception ex)
            {
                _settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaserPointer", SettingsFileName);
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath)!);
                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:SettingsService", message = "Settings file path determined (Fallback)", data = new { path = _settingsFilePath, error = ex.Message, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            
            _settings = LoadSettings();
        }

        public LaserSettings LoadSettings()
        {
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:LoadSettings", message = "LoadSettings: Attempting to load settings", data = new { path = _settingsFilePath, fileExists = File.Exists(_settingsFilePath), currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            if (File.Exists(_settingsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonSerializer.Deserialize<LaserSettings>(json);
                    // #region agent log
                    try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:LoadSettings", message = "LoadSettings: Successfully loaded settings", data = new { path = _settingsFilePath, windowBackgroundColor = settings?.WindowBackgroundColor, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    return settings ?? new LaserSettings();
                }
                catch (Exception ex)
                {
                    // #region agent log
                    try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:LoadSettings", message = "LoadSettings: Error deserializing settings", data = new { error = ex.Message, stackTrace = ex.StackTrace, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }
            }
            
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:LoadSettings", message = "LoadSettings: Returning default settings", data = new { currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            return new LaserSettings();
        }

        public void UpdateSettings(LaserSettings newSettings)
        {
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "SettingsService.cs:UpdateSettings", message = "UpdateSettings: Called", data = new { serviceInstance = this.GetHashCode(), newWindowBackgroundColor = newSettings.WindowBackgroundColor, newColorPreset = newSettings.ColorPreset, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            _settings = newSettings;
            SaveSettings();

            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "SettingsService.cs:UpdateSettings", message = "UpdateSettings: After SaveSettings", data = new { serviceInstance = this.GetHashCode(), currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
        }

        public void SaveSettings()
        {
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:SaveSettings", message = "SaveSettings: Attempting to save settings", data = new { path = _settingsFilePath, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsFilePath, json);

                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "SettingsService.cs:SaveSettings", message = "SaveSettings: Before invoking SettingsChanged", data = new { serviceInstance = this.GetHashCode(), subscriberCount = SettingsChanged?.GetInvocationList()?.Length ?? 0, windowBackgroundColor = _settings?.WindowBackgroundColor, hasEvent = SettingsChanged != null, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                if (SettingsChanged != null)
                {
                    try
                    {
                        SettingsChanged.Invoke(this, _settings!);
                        // #region agent log
                        try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "SettingsService.cs:SaveSettings", message = "SaveSettings: After invoking SettingsChanged", data = new { serviceInstance = this.GetHashCode(), currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }
                    catch (Exception ex)
                    {
                        // #region agent log
                        try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "SettingsService.cs:SaveSettings", message = "SaveSettings: Exception invoking SettingsChanged", data = new { error = ex.Message, stackTrace = ex.StackTrace, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }
                }
                else
                {
                    // #region agent log
                    try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "SettingsService.cs:SaveSettings", message = "SaveSettings: SettingsChanged is null, no subscribers", data = new { serviceInstance = this.GetHashCode(), currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }
            }
            catch (Exception ex)
            {
                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H1_FileStorage", location = "SettingsService.cs:SaveSettings", message = "SaveSettings: Exception saving settings to file", data = new { error = ex.Message, stackTrace = ex.StackTrace, path = _settingsFilePath, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }
    }
}

