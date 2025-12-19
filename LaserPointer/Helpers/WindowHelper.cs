using System;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using LaserPointer.Helpers;
using Windows.Graphics;

namespace LaserPointer.Helpers
{
    public static class WindowHelper
    {
        public static void MakeWindowTransparentAndClickThrough(IntPtr hwnd)
        {
            var exStyle = NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, IntPtr.Zero);
            exStyle = new IntPtr(exStyle.ToInt64() | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_NOACTIVATE);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, exStyle);
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
    }
}

