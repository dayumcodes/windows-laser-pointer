using Windows.UI;

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
            foreach (var preset in presets)
            {
                if (preset.Name == name)
                    return preset;
            }
            return presets[0]; // Default to Red
        }
    }
}

