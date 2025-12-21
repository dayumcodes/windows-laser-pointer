using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Windows.ApplicationModel;

namespace LaserPointer
{
    // Module initializer runs BEFORE any other code, including WinUI's auto-generated Program
    internal static class BootstrapInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"BootstrapInitializer.cs:16\",\"message\":\"ModuleInitializer entry\",\"data\":{{\"exePath\":\"{AppDomain.CurrentDomain.BaseDirectory}\"}},\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"H1\"}}\n"); } catch { }
            // #endregion
            LogDebug("=== Module Initializer Started ===");
            LogDebug("Initializing Windows App SDK bootstrapper BEFORE any other code runs...");
            
            // #region agent log
            try { 
                var exeDir = AppDomain.CurrentDomain.BaseDirectory;
                var bootstrapDll = Path.Combine(exeDir, "Microsoft.WindowsAppRuntime.Bootstrap.dll");
                var bootstrapDllExists = File.Exists(bootstrapDll);
                var dlls = Directory.GetFiles(exeDir, "*.dll").Length;
                var isTempDir = exeDir.Contains(@"\Temp\") || exeDir.Contains(@"\.net\");
                
                // Check installed runtime packages
                var runtimeInfo = GetInstalledRuntimeInfo();
                
                File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"BootstrapInitializer.cs:25\",\"message\":\"Before Bootstrap.TryInitialize - checking DLLs and runtime\",\"data\":{{\"exeDir\":\"{exeDir}\",\"bootstrapDllExists\":{bootstrapDllExists.ToString().ToLower()},\"totalDlls\":{dlls},\"isTempDir\":{isTempDir.ToString().ToLower()},\"runtimeInstalled\":{runtimeInfo.installed.ToString().ToLower()},\"runtimePackages\":\"{runtimeInfo.packages}\",\"runtimeVersions\":\"{runtimeInfo.versions}\"}},\"sessionId\":\"debug-session\",\"runId\":\"run4\",\"hypothesisId\":\"H1\"}}\n"); 
            } catch (Exception ex) { 
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"BootstrapInitializer.cs:25\",\"message\":\"Error checking DLLs\",\"data\":{{\"error\":\"{ex.Message}\"}},\"sessionId\":\"debug-session\",\"runId\":\"run4\",\"hypothesisId\":\"H1\"}}\n"); } catch { }
            }
            // #endregion
            
            // SKIP initialization in ModuleInitializer - it's failing with SEHException
            // Initialization will be done in Program.Main instead
            LogDebug("Skipping bootstrap initialization in ModuleInitializer (will be done in Program.Main)");
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", $"{{\"id\":\"log_{DateTime.Now.Ticks}\",\"timestamp\":{DateTimeOffset.Now.ToUnixTimeMilliseconds()},\"location\":\"BootstrapInitializer.cs:40\",\"message\":\"Skipping initialization in ModuleInitializer\",\"data\":{{\"reason\":\"Will initialize in Program.Main instead\"}},\"sessionId\":\"debug-session\",\"runId\":\"run6\",\"hypothesisId\":\"H7\"}}\n"); } catch { }
            // #endregion
            
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
        
        private static (bool installed, string packages, string versions) GetInstalledRuntimeInfo()
        {
            try
            {
                // Check if Windows App Runtime package is installed using PowerShell
                var startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Get-AppxPackage -Name Microsoft.WindowsAppRuntime* | Select-Object Name, Version, Architecture | ConvertTo-Json -Compress\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                
                using (var process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit(5000); // 5 second timeout
                        string output = process.StandardOutput.ReadToEnd().Trim();
                        string error = process.StandardError.ReadToEnd().Trim();
                        
                        if (!string.IsNullOrEmpty(output) && !output.StartsWith("Get-AppxPackage"))
                        {
                            // Parse JSON output to get package names and versions
                            var packages = output;
                            var versions = ExtractVersions(output);
                            return (true, packages, versions);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", "");
            }
            return (false, "No packages found", "");
        }
        
        private static string ExtractVersions(string json)
        {
            try
            {
                // Simple extraction - look for Version patterns in JSON
                var versions = new System.Collections.Generic.List<string>();
                var parts = json.Split(new[] { "\"Version\"" }, StringSplitOptions.None);
                for (int i = 1; i < parts.Length && i < 4; i++)
                {
                    var versionPart = parts[i].Split('"')[1];
                    if (!string.IsNullOrEmpty(versionPart))
                        versions.Add(versionPart);
                }
                return string.Join(", ", versions);
            }
            catch
            {
                return "Unable to parse";
            }
        }
        
        private static bool CheckWindowsAppRuntimeInstalled()
        {
            return GetInstalledRuntimeInfo().installed;
        }
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    }
}
