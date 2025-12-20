using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LaserPointer.Services;
using LaserPointer.Models;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using System;

namespace LaserPointer
{
    public sealed partial class MainWindow : Window
    {
        private SettingsService _settingsService;

        public MainWindow(SettingsService settingsService)
        {
            this.InitializeComponent();
            
            // Set window size (WinUI 3 requires AppWindow API)
            this.AppWindow.Resize(new SizeInt32(500, 700));
            
            _settingsService = settingsService;
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run2", hypothesisId = "F", location = "MainWindow.xaml.cs:MainWindow", message = "MainWindow: Received SettingsService", data = new { settingsServiceInstance = _settingsService.GetHashCode() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            LoadSettingsIntoUI();
        }

        private void LoadSettingsIntoUI()
        {
            var settings = _settingsService.Settings;

            // Set color preset
            foreach (ComboBoxItem item in ColorPresetComboBox.Items)
            {
                if (item.Tag?.ToString() == settings.ColorPreset)
                {
                    ColorPresetComboBox.SelectedItem = item;
                    break;
                }
            }

            // Set window background color
            foreach (ComboBoxItem item in WindowBackgroundColorComboBox.Items)
            {
                if (item.Tag?.ToString() == settings.WindowBackgroundColor)
                {
                    WindowBackgroundColorComboBox.SelectedItem = item;
                    break;
                }
            }

            // Temporarily unsubscribe from events to prevent firing during initialization
            FadeDurationSlider.ValueChanged -= FadeDurationSlider_ValueChanged;
            LineThicknessSlider.ValueChanged -= LineThicknessSlider_ValueChanged;

            // Set fade duration
            FadeDurationSlider.Value = settings.FadeDurationSeconds;
            if (FadeDurationValue != null)
            {
                FadeDurationValue.Text = settings.FadeDurationSeconds.ToString("F1");
            }

            // Set line thickness
            LineThicknessSlider.Value = settings.LineThickness;
            if (LineThicknessValue != null)
            {
                LineThicknessValue.Text = ((int)settings.LineThickness).ToString();
            }

            // Re-subscribe to events
            FadeDurationSlider.ValueChanged += FadeDurationSlider_ValueChanged;
            LineThicknessSlider.ValueChanged += LineThicknessSlider_ValueChanged;
        }

        private void ColorPresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Settings will be saved on Apply
        }

        private void WindowBackgroundColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Settings will be saved on Apply
        }

        private void FadeDurationSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            // Null check to prevent exception if control isn't initialized yet
            if (FadeDurationValue != null)
            {
                FadeDurationValue.Text = e.NewValue.ToString("F1");
            }
        }

        private void LineThicknessSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            // Null check to prevent exception if control isn't initialized yet
            if (LineThicknessValue != null)
            {
                LineThicknessValue.Text = ((int)e.NewValue).ToString();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run2", hypothesisId = "A", location = "MainWindow.xaml.cs:ApplyButton_Click", message = "ApplyButton_Click: Button clicked", data = new { settingsServiceInstance = _settingsService.GetHashCode() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            var settings = new LaserSettings
            {
                ColorPreset = (ColorPresetComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Red",
                FadeDurationSeconds = FadeDurationSlider.Value,
                LineThickness = (float)LineThicknessSlider.Value,
                HotkeyModifiers = "Control+Shift",
                HotkeyKey = "L",
                WindowBackgroundColor = (WindowBackgroundColorComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Transparent"
            };
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run2", hypothesisId = "A", location = "MainWindow.xaml.cs:ApplyButton_Click", message = "ApplyButton_Click: Created settings object", data = new { colorPreset = settings.ColorPreset, windowBackgroundColor = settings.WindowBackgroundColor, fadeDuration = settings.FadeDurationSeconds, lineThickness = settings.LineThickness }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            _settingsService.UpdateSettings(settings);
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run2", hypothesisId = "A", location = "MainWindow.xaml.cs:ApplyButton_Click", message = "ApplyButton_Click: Called UpdateSettings", data = new { settingsServiceInstance = _settingsService.GetHashCode() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var defaultSettings = new LaserSettings();
            _settingsService.UpdateSettings(defaultSettings);
            LoadSettingsIntoUI();
        }
    }
}

