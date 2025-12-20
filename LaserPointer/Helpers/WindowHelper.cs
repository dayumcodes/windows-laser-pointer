using System;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using LaserPointer.Helpers;
using Windows.Graphics;
using System.IO;
using System.Text.Json;

namespace LaserPointer.Helpers
{
    public static class WindowHelper
    {
        public static void MakeWindowTransparentAndClickThrough(IntPtr hwnd)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H1", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Window validation", data = new { hwnd = hwnd.ToInt64(), isValid = NativeMethods.IsWindow(hwnd), isVisible = NativeMethods.IsWindowVisible(hwnd) }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            if (hwnd == IntPtr.Zero || !NativeMethods.IsWindow(hwnd)) return;

            // Get current extended window style
            var exStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H1", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Current exStyle from GetWindowLong", data = new { exStyle = exStyle.ToInt64() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Step 1: Set WS_EX_LAYERED (required before SetLayeredWindowAttributes)
            var newExStyle = new IntPtr(exStyle.ToInt64() | NativeMethods.WS_EX_LAYERED);
            IntPtr setResult1;
            if (IntPtr.Size == 8)
            {
                setResult1 = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, newExStyle);
            }
            else
            {
                setResult1 = new IntPtr(NativeMethods.SetWindowLong32(hwnd, NativeMethods.GWL_EXSTYLE, (int)newExStyle.ToInt64()));
            }
            var error1 = NativeMethods.GetLastError();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H1", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Set WS_EX_LAYERED result", data = new { previousStyle = exStyle.ToInt64(), newStyle = newExStyle.ToInt64(), setResult = setResult1.ToInt64(), lastError = error1 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Step 2: Use per-pixel alpha transparency (H3: test per-pixel alpha for background transparency)
            // Alpha=255 allows per-pixel alpha from XAML, so transparent backgrounds show through
            bool lwaResult = NativeMethods.SetLayeredWindowAttributes(hwnd, 0, 255, NativeMethods.LWA_ALPHA);
            var errorAfterLWA = NativeMethods.GetLastError();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H3", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: SetLayeredWindowAttributes with alpha=255 (per-pixel)", data = new { success = lwaResult, alpha = 255, flags = "LWA_ALPHA", errorAfterLWA = errorAfterLWA }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Step 3: Set click-through and topmost flags individually (H2: test setting flags individually)
            var currentExStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            
            // Try WS_EX_TRANSPARENT individually
            var styleWithTransparent = new IntPtr(currentExStyle.ToInt64() | NativeMethods.WS_EX_TRANSPARENT);
            IntPtr setResult2;
            if (IntPtr.Size == 8)
            {
                setResult2 = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, styleWithTransparent);
            }
            else
            {
                setResult2 = new IntPtr(NativeMethods.SetWindowLong32(hwnd, NativeMethods.GWL_EXSTYLE, (int)styleWithTransparent.ToInt64()));
            }
            var error2 = NativeMethods.GetLastError();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H2", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Set WS_EX_TRANSPARENT individually", data = new { previousStyle = currentExStyle.ToInt64(), newStyle = styleWithTransparent.ToInt64(), setResult = setResult2.ToInt64(), lastError = error2 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Try WS_EX_NOACTIVATE individually
            currentExStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            var styleWithNoActivate = new IntPtr(currentExStyle.ToInt64() | NativeMethods.WS_EX_NOACTIVATE);
            IntPtr setResult3;
            if (IntPtr.Size == 8)
            {
                setResult3 = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, styleWithNoActivate);
            }
            else
            {
                setResult3 = new IntPtr(NativeMethods.SetWindowLong32(hwnd, NativeMethods.GWL_EXSTYLE, (int)styleWithNoActivate.ToInt64()));
            }
            var error3 = NativeMethods.GetLastError();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H2", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Set WS_EX_NOACTIVATE individually", data = new { previousStyle = currentExStyle.ToInt64(), newStyle = styleWithNoActivate.ToInt64(), setResult = setResult3.ToInt64(), lastError = error3 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Try WS_EX_TOPMOST individually (already set via SetWindowPos, but verify)
            currentExStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            var styleWithTopmost = new IntPtr(currentExStyle.ToInt64() | NativeMethods.WS_EX_TOPMOST);
            IntPtr setResult4;
            if (IntPtr.Size == 8)
            {
                setResult4 = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, styleWithTopmost);
            }
            else
            {
                setResult4 = new IntPtr(NativeMethods.SetWindowLong32(hwnd, NativeMethods.GWL_EXSTYLE, (int)styleWithTopmost.ToInt64()));
            }
            var error4 = NativeMethods.GetLastError();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H2", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Set WS_EX_TOPMOST individually", data = new { previousStyle = currentExStyle.ToInt64(), newStyle = styleWithTopmost.ToInt64(), setResult = setResult4.ToInt64(), lastError = error4 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // H1: Test WS_EX_NOREDIRECTIONBITMAP separately to see if it causes ERROR_INVALID_PARAMETER
            currentExStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            var styleWithNoRedirection = new IntPtr(currentExStyle.ToInt64() | NativeMethods.WS_EX_NOREDIRECTIONBITMAP);
            IntPtr setResult5;
            if (IntPtr.Size == 8)
            {
                setResult5 = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, styleWithNoRedirection);
            }
            else
            {
                setResult5 = new IntPtr(NativeMethods.SetWindowLong32(hwnd, NativeMethods.GWL_EXSTYLE, (int)styleWithNoRedirection.ToInt64()));
            }
            var error5 = NativeMethods.GetLastError();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H1", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Set WS_EX_NOREDIRECTIONBITMAP individually", data = new { previousStyle = currentExStyle.ToInt64(), newStyle = styleWithNoRedirection.ToInt64(), setResult = setResult5.ToInt64(), lastError = error5 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Force window to recalculate frame after style changes
            bool frameChanged = NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_FRAMECHANGED);
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H1", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: SetWindowPos with FRAMECHANGED", data = new { success = frameChanged }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            // Verify final styles
            var verifiedExStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            var verifiedLong = verifiedExStyle.ToInt64();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run3", hypothesisId = "H1", location = "WindowHelper.cs:MakeWindowTransparentAndClickThrough", message = "MakeWindowTransparent: Verified exStyle after all changes", data = new { verifiedExStyle = verifiedLong, hasLayered = (verifiedLong & NativeMethods.WS_EX_LAYERED) != 0, hasTransparent = (verifiedLong & NativeMethods.WS_EX_TRANSPARENT) != 0, hasNoActivate = (verifiedLong & NativeMethods.WS_EX_NOACTIVATE) != 0, hasTopmost = (verifiedLong & NativeMethods.WS_EX_TOPMOST) != 0, hasNoRedirection = (verifiedLong & NativeMethods.WS_EX_NOREDIRECTIONBITMAP) != 0 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
        }

        public static void MakeWindowNormal(IntPtr hwnd)
        {
            var exStyle = NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, IntPtr.Zero);
            exStyle = new IntPtr(exStyle.ToInt64() | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_NOACTIVATE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, exStyle);
        }

        public static void SetWindowAlwaysOnTop(IntPtr hwnd)
        {
            NativeMethods.SetWindowPos(
                hwnd,
                NativeMethods.HWND_TOPMOST,
                0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
        }

        public static RectInt32 GetVirtualScreenBounds()
        {
            var x = NativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN);
            var y = NativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN);
            var width = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN);
            var height = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN);

            return new RectInt32(x, y, width, height);
        }
        
        public static void RemoveWindowBorders(IntPtr hwnd)
        {
            // Remove window borders and title bar by setting window style
            const int GWL_STYLE = -16;
            const int WS_BORDER = 0x00800000;
            const int WS_DLGFRAME = 0x00400000;
            const int WS_CAPTION = 0x00C00000;
            const int WS_SYSMENU = 0x00080000;
            const int WS_THICKFRAME = 0x00040000;
            const int WS_MINIMIZE = 0x20000000;
            const int WS_MAXIMIZE = 0x01000000;
            
            // Get current style
            var style = NativeMethods.GetWindowLong(hwnd, GWL_STYLE);
            
            // Remove borders, caption, and system menu
            style = new IntPtr(style.ToInt64() & ~(WS_BORDER | WS_DLGFRAME | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE));
            
            NativeMethods.SetWindowLong(hwnd, GWL_STYLE, style);
        }

        // H8: Update window transparency attributes based on background color
        // For transparent backgrounds: use DwmExtendFrameIntoClientArea, DWM backdrop, and LWA_ALPHA for per-pixel alpha
        // For non-transparent backgrounds: ensure full opacity so background color shows
        public static void UpdateWindowTransparency(IntPtr hwnd, bool isTransparent)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: Entry", data = new { hwnd = hwnd.ToInt64(), isTransparent = isTransparent }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion

            if (hwnd == IntPtr.Zero || !NativeMethods.IsWindow(hwnd)) return;

            // Ensure WS_EX_LAYERED is set (required for SetLayeredWindowAttributes)
            var exStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            if ((exStyle.ToInt64() & NativeMethods.WS_EX_LAYERED) == 0)
            {
                var newExStyle = new IntPtr(exStyle.ToInt64() | NativeMethods.WS_EX_LAYERED);
                IntPtr setResult;
                if (IntPtr.Size == 8)
                {
                    setResult = NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, newExStyle);
                }
                else
                {
                    setResult = new IntPtr(NativeMethods.SetWindowLong32(hwnd, NativeMethods.GWL_EXSTYLE, (int)newExStyle.ToInt64()));
                }
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: Set WS_EX_LAYERED", data = new { setResult = setResult.ToInt64(), lastError = NativeMethods.GetLastError() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }

            // For transparent windows, use multiple DWM and layered window techniques
            if (isTransparent)
            {
                // H8: Try DwmExtendFrameIntoClientArea with negative margins to extend frame (glass effect)
                var margins = new NativeMethods.MARGINS
                {
                    cxLeftWidth = -1,
                    cxRightWidth = -1,
                    cyTopHeight = -1,
                    cyBottomHeight = -1
                };
                int dwmExtendResult = NativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref margins);
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: DwmExtendFrameIntoClientArea", data = new { dwmExtendResult = dwmExtendResult, margins = margins, lastError = NativeMethods.GetLastError() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                // Set DWM backdrop to transparent
                int backdropType = NativeMethods.DWM_SYSTEMBACKDROP_TYPE_NONE; // Transparent backdrop
                int dwmResult = NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, sizeof(int));
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: Set DWM backdrop to transparent", data = new { dwmResult = dwmResult, backdropType = backdropType, lastError = NativeMethods.GetLastError() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                // Disable DWM non-client area rendering for better transparency
                int ncrPolicy = NativeMethods.DWMNCRP_DISABLED;
                int dwmNcrResult = NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_NCRENDERING_POLICY, ref ncrPolicy, sizeof(int));
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: Disable DWM non-client rendering", data = new { dwmNcrResult = dwmNcrResult, ncrPolicy = ncrPolicy, lastError = NativeMethods.GetLastError() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                // H8: Use LWA_ALPHA with alpha=255 to enable per-pixel alpha from XAML
                // This allows transparent XAML content (alpha=0) to show through
                // LWA_COLORKEY only works if pixels are exactly black, but compositor may render different default color
                bool lwaAlphaResult = NativeMethods.SetLayeredWindowAttributes(hwnd, 0, 255, NativeMethods.LWA_ALPHA);
                var errorAfterAlpha = NativeMethods.GetLastError();
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: SetLayeredWindowAttributes with LWA_ALPHA (per-pixel alpha)", data = new { success = lwaAlphaResult, alpha = 255, flags = "LWA_ALPHA", errorAfterAlpha = errorAfterAlpha }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            else
            {
                // For non-transparent windows, use LWA_ALPHA for per-pixel alpha
                bool lwaResult = NativeMethods.SetLayeredWindowAttributes(hwnd, 0, 255, NativeMethods.LWA_ALPHA);
                var errorAfterLWA = NativeMethods.GetLastError();
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: SetLayeredWindowAttributes with LWA_ALPHA", data = new { success = lwaResult, alpha = 255, flags = "LWA_ALPHA", isTransparent = isTransparent, errorAfterLWA = errorAfterLWA }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }

            // Force window to recalculate frame
            bool frameChanged = NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_FRAMECHANGED);
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run11", hypothesisId = "H8_Transparency", location = "WindowHelper.cs:UpdateWindowTransparency", message = "UpdateWindowTransparency: SetWindowPos FRAMECHANGED", data = new { success = frameChanged, isTransparent = isTransparent }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
        }
    }
}

