namespace LaserPointer.Models
{
    public class LaserSettings
    {
        public string ColorPreset { get; set; } = "Red";
        public double FadeDurationSeconds { get; set; } = 2.0;
        public float LineThickness { get; set; } = 3.0f;
        public string HotkeyModifiers { get; set; } = "Control+Shift";
        public string HotkeyKey { get; set; } = "L";
    }
}

