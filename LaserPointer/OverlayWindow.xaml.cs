using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using LaserPointer.Helpers;
using LaserPointer.Services;
using LaserPointer.Models;
using Windows.Graphics;
using System;

namespace LaserPointer
{
    public sealed partial class OverlayWindow : Window
    {
        private SettingsService _settingsService;
        private GlobalMouseTracker? _mouseTracker;

        public OverlayWindow(SettingsService settingsService)
        {
            this.InitializeComponent();
            _settingsService = settingsService;
            SetupWindow();
            InitializeMouseTracker();
        }

        private void SetupWindow()
        {
            // Set window to cover all displays
            var bounds = WindowHelper.GetVirtualScreenBounds();
            this.AppWindow.MoveAndResize(bounds);

            // Make window always on top
            var hwnd = WindowNative.GetWindowHandle(this);
            WindowHelper.SetWindowAlwaysOnTop(hwnd);

            // Make window transparent and click-through
            WindowHelper.MakeWindowTransparentAndClickThrough(hwnd);

            // Set window to be non-activatable
            this.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            // Subscribe to settings changes
            _settingsService.SettingsChanged += OnSettingsChanged;
        }

        private void InitializeMouseTracker()
        {
            _mouseTracker = new GlobalMouseTracker();
            _mouseTracker.DrawingStart += OnDrawingStart;
            _mouseTracker.MouseMove += OnMouseMove;
            _mouseTracker.DrawingEnd += OnDrawingEnd;
        }

        protected override void OnActivated(Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                // When activated, make click-through
                var hwnd = WindowNative.GetWindowHandle(this);
                WindowHelper.MakeWindowTransparentAndClickThrough(hwnd);
            }
        }

        public new void Show()
        {
            base.Show();
            _mouseTracker?.StartTracking();
        }

        public new void Hide()
        {
            _mouseTracker?.StopTracking();
            base.Hide();
        }

        private void OnDrawingStart(object? sender, Windows.Foundation.Point point)
        {
            var settings = _settingsService.Settings;
            var preset = ColorPreset.GetPresetByName(settings.ColorPreset);
            var color = preset?.Color ?? Colors.Red;

            LaserCanvas.StartStroke(point, color, settings.LineThickness, settings.FadeDurationSeconds);
        }

        private void OnMouseMove(object? sender, Windows.Foundation.Point point)
        {
            LaserCanvas.ContinueStroke(point);
        }

        private void OnDrawingEnd(object? sender, Windows.Foundation.Point point)
        {
            LaserCanvas.EndStroke();
        }

        private void OnSettingsChanged(object? sender, LaserSettings settings)
        {
            // Update canvas with new settings if needed
        }
    }
}

