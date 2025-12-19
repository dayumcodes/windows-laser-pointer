using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using LaserPointer.Helpers;

namespace LaserPointer.Services
{
    public class GlobalHotkeyService
    {
        private const int HotkeyId = 9000;
        private IntPtr _windowHandle;
        private bool _isRegistered = false;

        public event EventHandler? HotkeyPressed;

        public void RegisterHotkey(Window window)
        {
            _windowHandle = WindowNative.GetWindowHandle(window);
            RegisterHotkey();
        }

        public void RegisterHotkey()
        {
            if (_windowHandle == IntPtr.Zero)
                return;

            // Default: Ctrl+Shift+L
            uint modifiers = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;
            uint vk = 0x4C; // L key

            if (RegisterHotKey(_windowHandle, HotkeyId, modifiers, vk))
            {
                _isRegistered = true;
            }
        }

        public void UnregisterHotkey()
        {
            if (_isRegistered && _windowHandle != IntPtr.Zero)
            {
                UnregisterHotKey(_windowHandle, HotkeyId);
                _isRegistered = false;
            }
        }

        public void HandleHotkeyMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == NativeMethods.WM_HOTKEY && wParam.ToInt32() == HotkeyId)
            {
                HotkeyPressed?.Invoke(this, EventArgs.Empty);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}

