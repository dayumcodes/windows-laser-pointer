using Microsoft.UI.Xaml;
using LaserPointer.Services;
using LaserPointer.Models;
using System;
using WinRT.Interop;
using Windows.UI.Core;

namespace LaserPointer
{
    public partial class App : Application
    {
        private Window? _mainWindow;
        private OverlayWindow? _overlayWindow;
        private GlobalHotkeyService? _hotkeyService;
        private SettingsService? _settingsService;
        private SystemTrayIcon? _trayIcon;
        private WindowMessageService? _windowMessageService;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Initialize services
            _settingsService = new SettingsService();
            _settingsService.LoadSettings();

            // Create and show main window (settings)
            _mainWindow = new MainWindow();
            _mainWindow.Activate();

            // Create overlay window (hidden initially)
            _overlayWindow = new OverlayWindow(_settingsService);
            _overlayWindow.Hide();

            // Initialize global hotkey service
            _hotkeyService = new GlobalHotkeyService();
            _hotkeyService.HotkeyPressed += OnHotkeyPressed;
            _hotkeyService.RegisterHotkey(_mainWindow);
            
            // Set up window message handler for hotkeys
            _windowMessageService = new WindowMessageService();
            _windowMessageService.Initialize(_mainWindow, _hotkeyService);

            // Initialize system tray icon
            _trayIcon = new SystemTrayIcon();
            _trayIcon.ShowSettingsRequested += () => _mainWindow?.Activate();
            _trayIcon.ExitRequested += OnExitRequested;
            _trayIcon.Initialize();

            // Handle window closing
            _mainWindow.Closed += (sender, e) =>
            {
                // Minimize to tray instead of closing
                _mainWindow.Hide();
                e.Handled = true;
            };
        }

        private void OnHotkeyPressed(object? sender, EventArgs e)
        {
            if (_overlayWindow != null)
            {
                if (_overlayWindow.Visible)
                {
                    _overlayWindow.Hide();
                }
                else
                {
                    _overlayWindow.Show();
                }
            }
        }

        private void OnExitRequested()
        {
            _hotkeyService?.UnregisterHotkey();
            _trayIcon?.Dispose();
            _overlayWindow?.Close();
            _mainWindow?.Close();
            Exit();
        }
    }
}

