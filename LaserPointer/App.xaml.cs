using Microsoft.UI.Xaml;
using LaserPointer.Services;
using LaserPointer.Models;
using LaserPointer.Helpers;
using System;
using WinRT.Interop;
using Windows.UI.Core;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        // Static constructor - initialize bootstrap here since Program.cs is excluded
        static App()
        {
            LogDebugStatic("=== App Static Constructor Started ===");
            LogDebugStatic("Initializing Windows App SDK bootstrapper BEFORE App instance creation...");
            
            try
            {
                // Initialize Windows App SDK bootstrapper for unpackaged deployment
                // Version 1.5 = 0x00010005
                LogDebugStatic("Calling Bootstrap.TryInitialize(0x00010005)...");
                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"App.xaml.cs:33\",\"message\":\"About to call Bootstrap.TryInitialize in App static constructor\",\"data\":{{\"version\":\"0x00010005\"}},\"sessionId\":\"debug-session\",\"runId\":\"run7\",\"hypothesisId\":\"H8\"}}\n"); } catch { }
                // #endregion
                
                int hresult;
                bool success = Bootstrap.TryInitialize(0x00010005, out hresult);
                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"App.xaml.cs:36\",\"message\":\"Bootstrap.TryInitialize returned\",\"data\":{{\"success\":{success.ToString().ToLower()},\"hresult\":\"0x{hresult:X8}\"}},\"sessionId\":\"debug-session\",\"runId\":\"run7\",\"hypothesisId\":\"H8\"}}\n"); } catch { }
                // #endregion
                
                if (success)
                {
                    LogDebugStatic($"✓ Bootstrap initialization SUCCESS (HRESULT: 0x{hresult:X8})");
                }
                else
                {
                    string errorMsg = $"✗ Bootstrap initialization FAILED with HRESULT: 0x{hresult:X8}";
                    LogDebugStatic(errorMsg);
                    
                    // Decode common HRESULT errors
                    string hresultInfo = DecodeHResultStatic(hresult);
                    LogDebugStatic($"HRESULT details: {hresultInfo}");
                    
                    // Show error message for debugging using Win32 MessageBox
                    string message = $"Windows App SDK Bootstrap failed!\n\n" +
                        $"HRESULT: 0x{hresult:X8}\n" +
                        $"Details: {hresultInfo}\n\n" +
                        $"This usually means:\n" +
                        $"1. Windows App Runtime 1.5 is not installed\n" +
                        $"2. The installed version is incompatible\n" +
                        $"3. The bootstrapper DLL is missing\n\n" +
                        $"Check the Output window (Debug) for more details.";
                    
                    MessageBoxStatic(IntPtr.Zero, message, "Bootstrap Initialization Error", 0x10 | 0x0);
                }
            }
            catch (DllNotFoundException dllEx)
            {
                string errorMsg = $"DLL NOT FOUND: {dllEx.Message}";
                LogDebugStatic(errorMsg);
                LogDebugStatic($"Stack trace: {dllEx.StackTrace}");
                
                string message = $"Bootstrap DLL not found!\n\n" +
                    $"Message: {dllEx.Message}\n\n" +
                    $"This means the Microsoft.WindowsAppRuntime.Bootstrap.dll is missing.\n" +
                    $"Check the Output window for full details.";
                
                MessageBoxStatic(IntPtr.Zero, message, "Bootstrap DLL Missing", 0x10 | 0x0);
            }
            catch (Exception ex)
            {
                // #region agent log
                try { 
                    System.IO.File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"App.xaml.cs:88\",\"message\":\"SEHException caught in App static constructor\",\"data\":{{\"type\":\"{ex.GetType().Name}\",\"message\":\"{ex.Message.Replace("\"", "\\\"")}\",\"hresult\":\"0x{ex.HResult:X8}\",\"stackTrace\":\"{ex.StackTrace?.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "")}\"}},\"sessionId\":\"debug-session\",\"runId\":\"run7\",\"hypothesisId\":\"H8\"}}\n"); 
                } catch { }
                // #endregion
                string errorMsg = $"Bootstrap initialization EXCEPTION: {ex.GetType().Name} - {ex.Message}";
                LogDebugStatic(errorMsg);
                LogDebugStatic($"Stack trace: {ex.StackTrace}");
                
                string message = $"Exception during bootstrap initialization!\n\n" +
                    $"Type: {ex.GetType().Name}\n" +
                    $"Message: {ex.Message}\n\n" +
                    $"Check the Output window (Debug) for full details.";
                
                MessageBoxStatic(IntPtr.Zero, message, "Bootstrap Initialization Exception", 0x10 | 0x0);
            }
            
            LogDebugStatic("=== App Static Constructor Completed ===");
        }

        public App()
        {
            // Bootstrap is now initialized in Program.cs before this constructor runs
            LogDebug("=== App Constructor Started ===");
            
            LogDebug("Calling InitializeComponent...");
            try
            {
                this.InitializeComponent();
                LogDebug("✓ InitializeComponent completed successfully.");
            }
            catch (Exception ex)
            {
                LogDebug($"✗ InitializeComponent FAILED: {ex.GetType().Name} - {ex.Message}");
                LogDebug($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw as this is critical
            }
            
            LogDebug("=== App Constructor Completed ===");
        }
        
        private string DecodeHResult(int hresult)
        {
            // Common Windows App SDK HRESULT codes
            switch ((uint)hresult)
            {
                case 0x80073D54: return "ERROR_PACKAGE_NOT_FOUND - The Windows App Runtime package is not installed";
                case 0x80073CF9: return "ERROR_INSTALL_FAILED - Installation failed";
                case 0x80073CF0: return "ERROR_INSTALL_PREREQUISITE_FAILED - Prerequisites not met";
                case 0x80070002: return "ERROR_FILE_NOT_FOUND - File or DLL not found";
                case 0x8007007E: return "ERROR_MOD_NOT_FOUND - Module (DLL) not found";
                default: return $"Unknown error code. See: https://learn.microsoft.com/windows/win32/seccrypto/common-hresult-values";
            }
        }
        
        private void LogDebug(string message)
        {
            string logMessage = $"[LaserPointer] {DateTime.Now:HH:mm:ss.fff} - {message}";
            Debug.WriteLine(logMessage);
            System.Diagnostics.Trace.WriteLine(logMessage);
            
            // Also try to write to console if available
            try
            {
                Console.WriteLine(logMessage);
            }
            catch
            {
                // Console might not be available in WinUI apps
            }
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
        
        // Static versions for static constructor
        private static void LogDebugStatic(string message)
        {
            string logMessage = $"[App.Static] {DateTime.Now:HH:mm:ss.fff} - {message}";
            Debug.WriteLine(logMessage);
            System.Diagnostics.Trace.WriteLine(logMessage);
            
            try
            {
                Console.WriteLine(logMessage);
            }
            catch { }
        }
        
        private static string DecodeHResultStatic(int hresult)
        {
            switch ((uint)hresult)
            {
                case 0x80073D54: return "ERROR_PACKAGE_NOT_FOUND - The Windows App Runtime package is not installed";
                case 0x80073CF9: return "ERROR_INSTALL_FAILED - Installation failed";
                case 0x80073CF0: return "ERROR_INSTALL_PREREQUISITE_FAILED - Prerequisites not met";
                case 0x80070002: return "ERROR_FILE_NOT_FOUND - File or DLL not found";
                case 0x8007007E: return "ERROR_MOD_NOT_FOUND - Module (DLL) not found";
                default: return $"Unknown error code. See: https://learn.microsoft.com/windows/win32/seccrypto/common-hresult-values";
            }
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBoxStatic(IntPtr hWnd, string text, string caption, uint type);

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            LogDebug("OnLaunched called - application is starting");
            
            // Initialize services
            LogDebug("Initializing SettingsService...");
            _settingsService = new SettingsService();
            _settingsService.LoadSettings();
            LogDebug("SettingsService initialized");

            // Create and show main window (settings)
            _mainWindow = new MainWindow(_settingsService);
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
                var hwnd = WindowNative.GetWindowHandle(_mainWindow);
                NativeMethods.ShowWindow(hwnd, NativeMethods.SW_HIDE);
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

