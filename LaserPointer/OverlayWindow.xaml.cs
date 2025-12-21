using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using WinRT.Interop;
using LaserPointer.Helpers;
using LaserPointer.Services;
using LaserPointer.Models;
using Windows.Graphics;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using System;
using System.IO;
using System.Text.Json;

namespace LaserPointer
{
    public sealed partial class OverlayWindow : Window
    {
        private SettingsService _settingsService;
        private GlobalMouseTracker? _mouseTracker;
        private RectInt32 _windowBounds;
        private bool _isWindowVisible = false;

        public new bool Visible => _isWindowVisible;

        public OverlayWindow(SettingsService settingsService)
        {
            this.InitializeComponent();
            _settingsService = settingsService;
            SetupWindow();
            InitializeMouseTracker();
            
            // Subscribe to Activated event instead of overriding OnActivated
            this.Activated += OnWindowActivated;
        }

        private void SetupWindow()
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "I", location = "OverlayWindow.xaml.cs:35", message = "SetupWindow: Getting virtual screen bounds", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            // Set window to cover all displays FIRST (before getting hwnd)
            _windowBounds = WindowHelper.GetVirtualScreenBounds();
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "I", location = "OverlayWindow.xaml.cs:37", message = "SetupWindow: Window bounds", data = new { x = _windowBounds.X, y = _windowBounds.Y, width = _windowBounds.Width, height = _windowBounds.Height }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            this.AppWindow.MoveAndResize(_windowBounds);
            
            // Get window handle after moving/resizing
            var hwnd = WindowNative.GetWindowHandle(this);
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "OverlayWindow.xaml.cs:42", message = "SetupWindow: Got window handle", data = new { hwnd = hwnd.ToInt64() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            
            // Remove title bar completely - make window borderless
            var titleBar = this.AppWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;
            
            // Hide the title bar buttons
            titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
            
            // Set AppWindow background to transparent (CRITICAL for WinUI 3)
            this.AppWindow.TitleBar.BackgroundColor = Microsoft.UI.Colors.Transparent;
            this.AppWindow.TitleBar.InactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
            
            // Make window always on top
            WindowHelper.SetWindowAlwaysOnTop(hwnd);

            // Remove window borders FIRST (before setting transparency)
            WindowHelper.RemoveWindowBorders(hwnd);
            
            // Make window transparent and click-through
            WindowHelper.MakeWindowTransparentAndClickThrough(hwnd);
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "A", location = "OverlayWindow.xaml.cs:60", message = "SetupWindow: Called MakeWindowTransparentAndClickThrough", data = new { hwnd = hwnd.ToInt64() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            
            // Set window background to transparent for per-pixel alpha
            this.SystemBackdrop = null;
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "E", location = "OverlayWindow.xaml.cs:64", message = "SetupWindow: Set SystemBackdrop to null", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            ApplyWindowBackgroundColor();

            // Set window to be non-activatable
            this.AppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            // Subscribe to settings changes
            _settingsService.SettingsChanged += OnSettingsChanged;
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run2", hypothesisId = "E", location = "OverlayWindow.xaml.cs:SetupWindow", message = "SetupWindow: Subscribed to SettingsChanged", data = new { settingsServiceInstance = _settingsService.GetHashCode() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
        }
        
        // Convert screen coordinates to window-relative coordinates
        // ScreenToClient is not working correctly - manually convert using DPI
        private Point ScreenToWindow(Point screenPoint)
        {
            var hwnd = WindowNative.GetWindowHandle(this);
            var dpi = NativeMethods.GetDpiForWindow(hwnd);
            double dpiScale = dpi / 96.0; // Convert DPI to scale factor (120 DPI = 1.25x)
            
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "J", location = "OverlayWindow.xaml.cs:88", message = "ScreenToWindow: Input screen coordinates", data = new { screenX = screenPoint.X, screenY = screenPoint.Y, dpi = dpi, dpiScale = dpiScale, windowBounds = new { x = _windowBounds.X, y = _windowBounds.Y, w = _windowBounds.Width, h = _windowBounds.Height } }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            
            // Mouse hook gives physical screen coordinates
            // Window bounds are in physical pixels
            // WinUI uses logical pixels, so convert physical to logical
            double logicalScreenX = screenPoint.X / dpiScale;
            double logicalScreenY = screenPoint.Y / dpiScale;
            double logicalWindowX = _windowBounds.X / dpiScale;
            double logicalWindowY = _windowBounds.Y / dpiScale;
            
            var result = new Point(
                logicalScreenX - logicalWindowX,
                logicalScreenY - logicalWindowY
            );
            
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "J", location = "OverlayWindow.xaml.cs:105", message = "ScreenToWindow: Manual DPI conversion result", data = new { windowX = result.X, windowY = result.Y, logicalScreenX = logicalScreenX, logicalScreenY = logicalScreenY, logicalWindowX = logicalWindowX, logicalWindowY = logicalWindowY }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            
            return result;
        }

        private void InitializeMouseTracker()
        {
            _mouseTracker = new GlobalMouseTracker();
            _mouseTracker.MouseMove += OnMouseMove;
        }

        private void OnWindowActivated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            // When activated, make click-through and refresh window bounds
            var hwnd = WindowNative.GetWindowHandle(this);
            WindowHelper.MakeWindowTransparentAndClickThrough(hwnd);
            
            // Update window bounds in case display configuration changed
            _windowBounds = WindowHelper.GetVirtualScreenBounds();
            this.AppWindow.MoveAndResize(_windowBounds);
        }

        public void Show()
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H2_ContinuousDrawing", location = "OverlayWindow.xaml.cs:Show", message = "Show: Entry", data = new { currentIsVisible = _isWindowVisible, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            var hwnd = WindowNative.GetWindowHandle(this);
            NativeMethods.ShowWindow(hwnd, NativeMethods.SW_SHOW);
            _isWindowVisible = true;
            _mouseTracker?.StartTracking();
            
            // Start continuous drawing at current cursor position
            NativeMethods.POINT currentCursorPos;
            NativeMethods.GetCursorPos(out currentCursorPos);
            var windowPoint = ScreenToWindow(new Point(currentCursorPos.X, currentCursorPos.Y));
            LaserCanvas.StartStroke(windowPoint, GetLaserColor(), GetLaserThickness(), GetFadeDuration());
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H2_ContinuousDrawing", location = "OverlayWindow.xaml.cs:Show", message = "Show: Started continuous drawing", data = new { initialWindowX = windowPoint.X, initialWindowY = windowPoint.Y, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
        }

        public void Hide()
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H2_ContinuousDrawing", location = "OverlayWindow.xaml.cs:Hide", message = "Hide: Entry", data = new { currentIsVisible = _isWindowVisible, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            _mouseTracker?.StopTracking();
            // Stop drawing and clear strokes when hiding the window
            LaserCanvas.StopStroke();
            _isWindowVisible = false;
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H2_ContinuousDrawing", location = "OverlayWindow.xaml.cs:Hide", message = "Hide: Stopped drawing", data = new { currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            var hwnd = WindowNative.GetWindowHandle(this);
            NativeMethods.ShowWindow(hwnd, NativeMethods.SW_HIDE);
        }

        private void OnMouseMove(object? sender, Windows.Foundation.Point screenPoint)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H2_ContinuousDrawing", location = "OverlayWindow.xaml.cs:OnMouseMove", message = "OnMouseMove: Mouse hook screen coordinates", data = new { screenX = screenPoint.X, screenY = screenPoint.Y, isVisible = _isWindowVisible }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            // Draw continuously if the window is visible (hotkey activated)
            if (LaserCanvas != null && _isWindowVisible)
            {
                var windowPoint = ScreenToWindow(screenPoint);
                LaserCanvas.ContinueStroke(windowPoint);
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H2_ContinuousDrawing", location = "OverlayWindow.xaml.cs:OnMouseMove", message = "OnMouseMove: Final window coordinates sent to canvas (continuous)", data = new { windowX = windowPoint.X, windowY = windowPoint.Y }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }

        private Windows.UI.Color GetLaserColor()
        {
            var settings = _settingsService.Settings;
            var preset = ColorPreset.GetPresetByName(settings.ColorPreset);
            return preset?.Color ?? Colors.Red;
        }

        private float GetLaserThickness()
        {
            return _settingsService.Settings.LineThickness;
        }

        private double GetFadeDuration()
        {
            return _settingsService.Settings.FadeDurationSeconds;
        }

        private void OnSettingsChanged(object? sender, LaserSettings settings)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "OverlayWindow.xaml.cs:OnSettingsChanged", message = "OnSettingsChanged: Event received (entry)", data = new { senderInstance = sender?.GetHashCode(), settingsServiceInstance = _settingsService.GetHashCode(), windowBackgroundColor = settings.WindowBackgroundColor, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            
            // Ensure UI updates happen on the UI thread
            var dispatcherQueue = this.DispatcherQueue;
            if (dispatcherQueue != null)
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "OverlayWindow.xaml.cs:OnSettingsChanged", message = "OnSettingsChanged: Dispatching to UI thread", data = new { hasDispatcher = dispatcherQueue != null, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                
                dispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    // #region agent log
                    try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "OverlayWindow.xaml.cs:OnSettingsChanged", message = "OnSettingsChanged: On UI thread, calling ApplyWindowBackgroundColor", data = new { windowBackgroundColor = settings.WindowBackgroundColor, uiThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    try
                    {
                        ApplyWindowBackgroundColor();
                        // #region agent log
                        try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "OverlayWindow.xaml.cs:OnSettingsChanged", message = "OnSettingsChanged: ApplyWindowBackgroundColor completed", data = new { currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }
                    catch (Exception ex)
                    {
                        // #region agent log
                        try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "OverlayWindow.xaml.cs:OnSettingsChanged", message = "OnSettingsChanged: Exception in ApplyWindowBackgroundColor", data = new { error = ex.Message, stackTrace = ex.StackTrace, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }
                });
            }
            else
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run7", hypothesisId = "H2_EventPropagation", location = "OverlayWindow.xaml.cs:OnSettingsChanged", message = "OnSettingsChanged: No DispatcherQueue, calling directly", data = new { currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                ApplyWindowBackgroundColor();
            }
        }

        private void ApplyWindowBackgroundColor()
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Entry", data = new { currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            var settings = _settingsService.Settings;
            var colorName = settings.WindowBackgroundColor ?? "Transparent";
            var color = GetColorFromName(colorName);
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Got color", data = new { colorName = colorName, colorA = color.A, colorR = color.R, colorG = color.G, colorB = color.B, hasContent = this.Content != null, hasLaserCanvas = LaserCanvas != null, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            
            // Set window background (WinUI 3 requires setting on multiple levels)
            try
            {
                // Set on the root Grid (this is the Window's content, which controls the window background)
                if (this.Content is Microsoft.UI.Xaml.Controls.Grid grid)
                {
                    if (colorName == "Transparent")
                    {
                        grid.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                        LaserCanvas.WindowDrawBackgroundColor = Color.FromArgb(0, 0, 0, 0); // Pass transparent color to Win2D
                    }
                    else
                    {
                        grid.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(color);
                        LaserCanvas.WindowDrawBackgroundColor = color; // Pass opaque color to Win2D
                    }
                    // #region agent log
                    try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Set Grid and LaserCanvas background color for drawing", data = new { color = colorName, gridBackgroundSet = true, clearColor = LaserCanvas.WindowDrawBackgroundColor.ToString(), currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }
                else
                {
                    // #region agent log
                    try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Content is not Grid", data = new { contentType = this.Content?.GetType().Name, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }
                
                // H9: Update window transparency attributes for CanvasSwapChainPanel
                // CanvasSwapChainPanel provides better transparency support than CanvasControl
                // For transparent backgrounds: use DwmExtendFrameIntoClientArea, DWM backdrop, and LWA_ALPHA for per-pixel alpha
                // For non-transparent backgrounds: ensure full opacity so background color shows
                var hwnd = WindowNative.GetWindowHandle(this);
                var isTransparent = colorName == "Transparent" || color.A == 0;
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Checking if transparency update needed (with SwapChain)", data = new { isTransparent = isTransparent, colorName = colorName, colorA = color.A, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                
                // Update window transparency attributes
                // This uses DwmExtendFrameIntoClientArea, DWM backdrop settings, and LWA_ALPHA
                // to enable per-pixel alpha from XAML/SwapChain, allowing transparent content (alpha=0) to show through
                WindowHelper.UpdateWindowTransparency(hwnd, isTransparent);
                
                // Force window to update by invalidating layout on the Content element
                if (this.Content is Microsoft.UI.Xaml.UIElement contentElement)
                {
                    contentElement.InvalidateArrange();
                    contentElement.InvalidateMeasure();
                    // #region agent log
                    try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Invalidated layout on Content", data = new { currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }
            }
            catch (Exception ex)
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "OverlayWindow.xaml.cs:ApplyWindowBackgroundColor", message = "ApplyWindowBackgroundColor: Exception", data = new { error = ex.Message, stackTrace = ex.StackTrace, currentThread = System.Threading.Thread.CurrentThread.ManagedThreadId }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }

        private Windows.UI.Color GetColorFromName(string colorName)
        {
            // Microsoft.UI.Colors constants return Windows.UI.Color values
            Windows.UI.Color color = colorName switch
            {
                "Transparent" => Microsoft.UI.Colors.Transparent,
                "Black" => Microsoft.UI.Colors.Black,
                "White" => Microsoft.UI.Colors.White,
                "DarkGray" => Microsoft.UI.Colors.DarkGray,
                "LightGray" => Microsoft.UI.Colors.LightGray,
                _ => Microsoft.UI.Colors.Transparent
            };
            return color;
        }
    }
}

