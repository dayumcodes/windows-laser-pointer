using System;
using Microsoft.UI.Xaml;

namespace LaserPointer.Services
{
    // Note: System tray icon implementation for WinUI 3 requires additional setup
    // For now, this is a placeholder. Full implementation would require:
    // - Windows App SDK extension for system tray
    // - Or using a third-party library
    // The app will work without tray icon, just using the hotkey
    public class SystemTrayIcon : IDisposable
    {
        private bool _disposed = false;

        public event Action? ShowSettingsRequested;
        public event Action? ExitRequested;

        public void Initialize()
        {
            // TODO: Implement system tray icon when Windows App SDK supports it
            // For now, the app can be minimized and controlled via hotkey
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}

