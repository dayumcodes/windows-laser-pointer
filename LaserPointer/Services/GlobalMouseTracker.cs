using System;
using System.Runtime.InteropServices;
using LaserPointer.Helpers;
using Windows.Foundation;

namespace LaserPointer.Services
{
    public class GlobalMouseTracker : IDisposable
    {
        private IntPtr _hookId = IntPtr.Zero;
        private NativeMethods.LowLevelMouseProc _proc;
        private bool _isTracking = false;
        private bool _isDrawing = false;

        public event EventHandler<Point>? MouseMove;
        public event EventHandler<Point>? DrawingStart;
        public event EventHandler<Point>? DrawingEnd;

        public bool IsTracking => _isTracking;

        private Point _lastPoint;

        public GlobalMouseTracker()
        {
            _proc = HookCallback;
        }

        public void StartTracking()
        {
            if (_isTracking)
                return;

            _hookId = SetHook(_proc);
            _isTracking = true;
        }

        public void StopTracking()
        {
            if (!_isTracking)
                return;

            if (_hookId != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
            _isTracking = false;
        }

        private IntPtr SetHook(NativeMethods.LowLevelMouseProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(
                    NativeMethods.WH_MOUSE_LL,
                    proc,
                    NativeMethods.GetModuleHandle(curModule?.ModuleName ?? "LaserPointer"),
                    0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)NativeMethods.WM_LBUTTONDOWN)
                {
                    var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                    var point = new Point(hookStruct.pt.X, hookStruct.pt.Y);
                    _lastPoint = point;
                    DrawingStart?.Invoke(this, point);
                }
                else if (wParam == (IntPtr)NativeMethods.WM_LBUTTONUP)
                {
                    var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                    var point = new Point(hookStruct.pt.X, hookStruct.pt.Y);
                    DrawingEnd?.Invoke(this, point);
                }
                else if (wParam == (IntPtr)NativeMethods.WM_MOUSEMOVE)
                {
                    var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                    var point = new Point(hookStruct.pt.X, hookStruct.pt.Y);
                    
                    if (_lastPoint.X != point.X || _lastPoint.Y != point.Y)
                    {
                        MouseMove?.Invoke(this, point);
                        _lastPoint = point;
                    }
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            StopTracking();
        }
    }
}

