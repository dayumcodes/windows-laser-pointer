using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LaserPointer.Services;
using LaserPointer.Models;

namespace LaserPointer
{
    public sealed partial class MainWindow : Window
    {
        private SettingsService _settingsService;

        public MainWindow()
        {
            this.InitializeComponent();
            _settingsService = new SettingsService();
            _settingsService.LoadSettings();
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

            // Set fade duration
            FadeDurationSlider.Value = settings.FadeDurationSeconds;
            FadeDurationValue.Text = settings.FadeDurationSeconds.ToString("F1");

            // Set line thickness
            LineThicknessSlider.Value = settings.LineThickness;
            LineThicknessValue.Text = ((int)settings.LineThickness).ToString();
        }

        private void ColorPresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Settings will be saved on Apply
        }

        private void FadeDurationSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            FadeDurationValue.Text = e.NewValue.ToString("F1");
        }

        private void LineThicknessSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            LineThicknessValue.Text = ((int)e.NewValue).ToString();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new LaserSettings
            {
                ColorPreset = (ColorPresetComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Red",
                FadeDurationSeconds = FadeDurationSlider.Value,
                LineThickness = (float)LineThicknessSlider.Value,
                HotkeyModifiers = "Control+Shift",
                HotkeyKey = "L"
            };

            _settingsService.UpdateSettings(settings);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var defaultSettings = new LaserSettings();
            _settingsService.UpdateSettings(defaultSettings);
            LoadSettingsIntoUI();
        }
    }
}

