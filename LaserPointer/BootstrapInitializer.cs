using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace LaserPointer
{
    // Module initializer runs BEFORE any other code, including WinUI's auto-generated Program
    internal static class BootstrapInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            LogDebug("=== Module Initializer Started ===");
            LogDebug("Initializing Windows App SDK bootstrapper BEFORE any other code runs...");
            
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
                    
                    MessageBox(IntPtr.Zero, message, "Bootstrap Initialization Error", 0x10 | 0x0);
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
            }
            
            LogDebug("=== Module Initializer Completed ===");
        }
        
        private static string DecodeHResult(int hresult)
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
        
        private static void LogDebug(string message)
        {
            string logMessage = $"[BootstrapInit] {DateTime.Now:HH:mm:ss.fff} - {message}";
            Debug.WriteLine(logMessage);
            System.Diagnostics.Trace.WriteLine(logMessage);
            
            try
            {
                Console.WriteLine(logMessage);
            }
            catch { }
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    }
}
