using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using LaserPointer.Helpers;

namespace LaserPointer.Services
{
    public class WindowMessageService
    {
        private const int GWLP_WNDPROC = -4;
        private IntPtr _windowHandle;
        private WndProcDelegate? _wndProc;
        private IntPtr _originalWndProc;
        private GlobalHotkeyService? _hotkeyService;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public void Initialize(Window window, GlobalHotkeyService hotkeyService)
        {
            _windowHandle = WindowNative.GetWindowHandle(window);
            _hotkeyService = hotkeyService;
            _wndProc = WndProc;
            
            var procPtr = Marshal.GetFunctionPointerForDelegate(_wndProc);
            
            if (IntPtr.Size == 8)
                _originalWndProc = SetWindowLongPtr(_windowHandle, GWLP_WNDPROC, procPtr);
            else
                _originalWndProc = new IntPtr(SetWindowLong32(_windowHandle, GWLP_WNDPROC, procPtr.ToInt32()));
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == NativeMethods.WM_HOTKEY)
            {
                _hotkeyService?.HandleHotkeyMessage(hWnd, msg, wParam, lParam);
            }

            return CallWindowProc(_originalWndProc, hWnd, msg, wParam, lParam);
        }
    }
}

