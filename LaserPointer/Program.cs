using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
// DO NOT import Microsoft.UI.Xaml here - it will load the DLL before bootstrap!
using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace LaserPointer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // CRITICAL: Initialize bootstrap BEFORE any WinUI code runs
            // This must happen before any Microsoft.UI.Xaml references
            LogDebug("=== Program.Main Started ===");
            LogDebug("Initializing Windows App SDK bootstrapper BEFORE WinUI starts...");
            
            try
            {
                // Initialize Windows App SDK bootstrapper for unpackaged deployment
                // Version 1.5 = 0x00010005
                LogDebug("Calling Bootstrap.TryInitialize(0x00010005)...");
                int hresult;
                bool success = Bootstrap.TryInitialize(0x00010005, out hresult);
                
                if (success)
                {
                    LogDebug($"✓ Bootstrap initialization SUCCESS (HRESULT: 0x{hresult:X8})");
                }
                else
                {
                    string errorMsg = $"✗ Bootstrap initialization FAILED with HRESULT: 0x{hresult:X8}";
                    LogDebug(errorMsg);
                    
                    // Decode common HRESULT errors
                    string hresultInfo = DecodeHResult(hresult);
                    LogDebug($"HRESULT details: {hresultInfo}");
                    
                    // Show error message for debugging using Win32 MessageBox
                    string message = $"Windows App SDK Bootstrap failed!\n\n" +
                        $"HRESULT: 0x{hresult:X8}\n" +
                        $"Details: {hresultInfo}\n\n" +
                        $"This usually means:\n" +
                        $"1. Windows App Runtime 1.5 is not installed\n" +
                        $"2. The installed version is incompatible\n" +
                        $"3. The bootstrapper DLL is missing\n\n" +
                        $"Check the Output window (Debug) for more details.";
                    
                    MessageBox(IntPtr.Zero, message, "Bootstrap Initialization Error", 0x10 | 0x0); // MB_ICONERROR | MB_OK
                    Environment.Exit(hresult);
                    return;
                }
            }
            catch (DllNotFoundException dllEx)
            {
                string errorMsg = $"DLL NOT FOUND: {dllEx.Message}";
                LogDebug(errorMsg);
                LogDebug($"Stack trace: {dllEx.StackTrace}");
                
                string message = $"Bootstrap DLL not found!\n\n" +
                    $"Message: {dllEx.Message}\n\n" +
                    $"This means the Microsoft.WindowsAppRuntime.Bootstrap.dll is missing.\n" +
                    $"Check the Output window for full details.";
                
                MessageBox(IntPtr.Zero, message, "Bootstrap DLL Missing", 0x10 | 0x0);
                Environment.Exit(1);
                return;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Bootstrap initialization EXCEPTION: {ex.GetType().Name} - {ex.Message}";
                LogDebug(errorMsg);
                LogDebug($"Stack trace: {ex.StackTrace}");
                
                string message = $"Exception during bootstrap initialization!\n\n" +
                    $"Type: {ex.GetType().Name}\n" +
                    $"Message: {ex.Message}\n\n" +
                    $"Check the Output window (Debug) for full details.";
                
                MessageBox(IntPtr.Zero, message, "Bootstrap Initialization Exception", 0x10 | 0x0);
                Environment.Exit(ex.HResult);
                return;
            }
            
            LogDebug("Starting WinUI Application...");
            // Now safe to use WinUI - bootstrap is initialized
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(
                    Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
                System.Threading.SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
            
            LogDebug("=== Program.Main Completed ===");
        }
        
        private static string DecodeHResult(int hresult)
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
        
        private static void LogDebug(string message)
        {
            string logMessage = $"[Program] {DateTime.Now:HH:mm:ss.fff} - {message}";
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
    }
}
