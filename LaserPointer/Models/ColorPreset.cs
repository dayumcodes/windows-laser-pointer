using Windows.UI;
using Microsoft.UI;
using System;
using System.Linq;

namespace LaserPointer.Models
{
    public class ColorPreset
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public static ColorPreset[] GetPresets()
        {
            return new ColorPreset[]
            {
                new ColorPreset { Name = "Red", Color = Colors.Red },
                new ColorPreset { Name = "Green", Color = Colors.Green },
                new ColorPreset { Name = "Blue", Color = Colors.Blue },
                new ColorPreset { Name = "Yellow", Color = Colors.Yellow },
                new ColorPreset { Name = "Purple", Color = Color.FromArgb(255, 128, 0, 128) },
                new ColorPreset { Name = "Orange", Color = Color.FromArgb(255, 255, 165, 0) }
            };
        }

        public static ColorPreset? GetPresetByName(string name)
        {
            var presets = GetPresets();
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run15", hypothesisId = "H6_ShowExecution", location = "ColorPreset.cs:GetPresetByName", message = "GetPresetByName: Searching for preset", data = new { searchedName = name, availablePresets = string.Join(",", presets.Select(p => p.Name)) }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            foreach (var preset in presets)
            {
                if (preset.Name == name)
                {
                    // #region agent log
                    try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run15", hypothesisId = "H6_ShowExecution", location = "ColorPreset.cs:GetPresetByName", message = "GetPresetByName: Found preset", data = new { foundName = preset.Name, color = $"{preset.Color.A},{preset.Color.R},{preset.Color.G},{preset.Color.B}" }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    return preset;
                }
            }
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run15", hypothesisId = "H6_ShowExecution", location = "ColorPreset.cs:GetPresetByName", message = "GetPresetByName: Not found, returning default", data = new { searchedName = name, defaultName = presets[0].Name, defaultColor = $"{presets[0].Color.A},{presets[0].Color.R},{presets[0].Color.G},{presets[0].Color.B}" }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            return presets[0]; // Default to Red
        }
    }
}

