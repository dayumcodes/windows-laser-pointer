using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using LaserPointer.Models;
using Windows.UI;
using Windows.Foundation;
using Windows.Graphics.Display;
using System.IO;
using System.Text.Json;

namespace LaserPointer.Controls
{
    public sealed partial class LaserCanvas : UserControl
    {
        private List<LaserStroke> _activeStrokes = new List<LaserStroke>();
        private DispatcherQueueTimer _fadeTimer;
        private LaserStroke? _currentStroke;
        private Point? _lastPoint;
        private double _fadeDurationSeconds = 2.0;
        private CanvasSwapChain? _swapChain;
        private CanvasDevice? _canvasDevice;
        private bool _isInitialized = false;
        public Color WindowDrawBackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);

        public LaserCanvas()
        {
            this.InitializeComponent();
            InitializeTimer();
            this.Loaded += LaserCanvas_Loaded;
            this.Unloaded += LaserCanvas_Unloaded;
        }

        private void LaserCanvas_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Clean up swap chain
            if (_swapChain != null)
            {
                _swapChain.Dispose();
                _swapChain = null;
            }
            _isInitialized = false;
        }

        private void LaserCanvas_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:LaserCanvas_Loaded", message = "LaserCanvas: Loaded event", data = new { actualWidth = this.ActualWidth, actualHeight = this.ActualHeight }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            InitializeSwapChain();
        }

        private void InitializeSwapChain()
        {
            try
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Entry", data = new { actualWidth = this.ActualWidth, actualHeight = this.ActualHeight, hasCanvasDevice = _canvasDevice != null }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                if (_canvasDevice == null)
                {
                    _canvasDevice = CanvasDevice.GetSharedDevice();
                    // #region agent log
                    try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Created CanvasDevice", data = new { device = _canvasDevice?.GetHashCode() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                }

                var width = (float)Math.Max(1, this.ActualWidth);
                var height = (float)Math.Max(1, this.ActualHeight);

                if (width > 0 && height > 0)
                {
                    // Get DPI from XamlRoot.RasterizationScale (works without CoreWindow context)
                    // Fallback to 96 DPI (standard) if XamlRoot is not available
                    float dpi = 96.0f; // Default DPI
                    try
                    {
                        if (this.XamlRoot != null)
                        {
                            // RasterizationScale gives us the DPI scale factor (e.g., 1.25 for 120 DPI)
                            // Multiply by 96 to get actual DPI
                            double scale = this.XamlRoot.RasterizationScale;
                            dpi = (float)(96.0 * scale);
                            // #region agent log
                            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Got DPI from XamlRoot", data = new { dpi = dpi, rasterizationScale = scale }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                            // #endregion
                        }
                        else
                        {
                            // #region agent log
                            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: XamlRoot is null, using default DPI", data = new { defaultDpi = dpi }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                            // #endregion
                        }
                    }
                    catch (Exception dpiEx)
                    {
                        // #region agent log
                        try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: DPI retrieval failed, using default", data = new { error = dpiEx.Message, defaultDpi = dpi }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }
                    // #region agent log
                    try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Final DPI value", data = new { dpi = dpi }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion

                    if (_swapChain == null)
                    {
                        _swapChain = new CanvasSwapChain(_canvasDevice, width, height, dpi);
                        CanvasSwapChainPanel.SwapChain = _swapChain;
                        // #region agent log
                        try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Created CanvasSwapChain", data = new { width = width, height = height, dpi = dpi, swapChain = _swapChain?.GetHashCode() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }
                    else
                    {
                        _swapChain.ResizeBuffers(width, height);
                        // #region agent log
                        try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Resized swap chain", data = new { width = width, height = height }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion
                    }

                    _isInitialized = true;
                    Draw();
                }
            }
            catch (Exception ex)
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:InitializeSwapChain", message = "InitializeSwapChain: Exception", data = new { error = ex.Message, stackTrace = ex.StackTrace }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }

        private void CanvasSwapChainPanel_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:CanvasSwapChainPanel_SizeChanged", message = "CanvasSwapChainPanel: Size changed", data = new { newWidth = e.NewSize.Width, newHeight = e.NewSize.Height }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            if (_isInitialized && _swapChain != null)
            {
                _swapChain.ResizeBuffers((float)e.NewSize.Width, (float)e.NewSize.Height);
                Draw();
            }
            else if (!_isInitialized)
            {
                InitializeSwapChain();
            }
        }

        private void InitializeTimer()
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _fadeTimer = dispatcherQueue.CreateTimer();
            _fadeTimer.Interval = TimeSpan.FromMilliseconds(16); // ~60fps
            _fadeTimer.Tick += OnFadeTick;
            _fadeTimer.Start();
        }

        public void StartStroke(Point start, Color color, float thickness, double fadeDurationSeconds)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run17", hypothesisId = "H2_ParameterPassing", location = "LaserCanvas.xaml.cs:StartStroke", message = "StartStroke: Received parameters", data = new { color = $"{color.A},{color.R},{color.G},{color.B}", thickness = thickness, fadeDuration = fadeDurationSeconds, startX = start.X, startY = start.Y }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            _fadeDurationSeconds = fadeDurationSeconds;
            _currentStroke = new LaserStroke
            {
                Start = new System.Numerics.Vector2((float)start.X, (float)start.Y),
                End = new System.Numerics.Vector2((float)start.X, (float)start.Y),
                Color = color,
                Thickness = thickness,
                Opacity = 1.0f,
                CreatedAt = DateTime.Now
            };
            _lastPoint = start;
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run17", hypothesisId = "H3_StrokeApplication", location = "LaserCanvas.xaml.cs:StartStroke", message = "StartStroke: Created _currentStroke", data = new { strokeColor = $"{_currentStroke.Color.A},{_currentStroke.Color.R},{_currentStroke.Color.G},{_currentStroke.Color.B}", strokeThickness = _currentStroke.Thickness, storedFadeDuration = _fadeDurationSeconds }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            Draw();
        }

        public void ContinueStroke(Point point)
        {
            if (_currentStroke != null && _lastPoint.HasValue)
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run14", hypothesisId = "H5_ShowSettings", location = "LaserCanvas.xaml.cs:ContinueStroke", message = "ContinueStroke: Using _currentStroke values", data = new { strokeColor = $"{_currentStroke.Color.A},{_currentStroke.Color.R},{_currentStroke.Color.G},{_currentStroke.Color.B}", strokeThickness = _currentStroke.Thickness, pointX = point.X, pointY = point.Y }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                // Add segment from last point to current point
                var newStroke = new LaserStroke
                {
                    Start = new System.Numerics.Vector2((float)_lastPoint.Value.X, (float)_lastPoint.Value.Y),
                    End = new System.Numerics.Vector2((float)point.X, (float)point.Y),
                    Color = _currentStroke.Color,
                    Thickness = _currentStroke.Thickness,
                    Opacity = 1.0f,
                    CreatedAt = DateTime.Now
                };
                _activeStrokes.Add(newStroke);
                _lastPoint = point;
                Draw();
            }
            else
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run14", hypothesisId = "H5_ShowSettings", location = "LaserCanvas.xaml.cs:ContinueStroke", message = "ContinueStroke: Skipped (no current stroke or last point)", data = new { hasCurrentStroke = _currentStroke != null, hasLastPoint = _lastPoint.HasValue }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }

        public void EndStroke()
        {
            _currentStroke = null;
            _lastPoint = null;
        }

        public void StopStroke()
        {
            _currentStroke = null;
            _lastPoint = null;
            _activeStrokes.Clear();
            Draw();
        }

        private void OnFadeTick(object? sender, object e)
        {
            var now = DateTime.Now;
            bool needsRedraw = false;

            // Update opacity for all strokes based on their age
            for (int i = _activeStrokes.Count - 1; i >= 0; i--)
            {
                var stroke = _activeStrokes[i];
                var age = now - stroke.CreatedAt;
                
                // Get fade duration from settings
                var fadeDuration = TimeSpan.FromSeconds(_fadeDurationSeconds);
                
                if (age < fadeDuration)
                {
                    stroke.Opacity = Math.Max(0, 1.0f - (float)(age.TotalMilliseconds / fadeDuration.TotalMilliseconds));
                    needsRedraw = true;
                }
                else
                {
                    _activeStrokes.RemoveAt(i);
                    needsRedraw = true;
                }
            }

            if (needsRedraw && _isInitialized)
            {
                Draw();
            }
        }

        private void Draw()
        {
            if (!_isInitialized || _swapChain == null)
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "LaserCanvas.xaml.cs:Draw", message = "Draw: Skipped (not initialized)", data = new { isInitialized = _isInitialized, swapChainIsNull = _swapChain == null }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                return;
            }

            try
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "LaserCanvas.xaml.cs:Draw", message = "Draw: Starting", data = new { strokeCount = _activeStrokes.Count, clearColor = WindowDrawBackgroundColor.ToString() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                // Create drawing session with configurable background color
                // Use WindowDrawBackgroundColor property to support both transparent and opaque backgrounds
                using (var session = _swapChain.CreateDrawingSession(WindowDrawBackgroundColor))
                {
                    foreach (var stroke in _activeStrokes)
                    {
                        var color = stroke.Color;
                        color.A = (byte)(255 * stroke.Opacity);
                        // #region agent log
                        try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run17", hypothesisId = "H3_StrokeApplication", location = "LaserCanvas.xaml.cs:Draw", message = "Draw: Drawing stroke segment", data = new { strokeColor = $"{color.A},{color.R},{color.G},{color.B}", strokeThickness = stroke.Thickness, strokeOpacity = stroke.Opacity }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                        // #endregion

                        session.DrawLine(
                            stroke.Start,
                            stroke.End,
                            color,
                            stroke.Thickness);
                    }
                }

                // Present the swap chain
                _swapChain.Present();
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run13", hypothesisId = "H1_ClearColor", location = "LaserCanvas.xaml.cs:Draw", message = "Draw: Completed and presented", data = new { strokeCount = _activeStrokes.Count }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            catch (Exception ex)
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run12", hypothesisId = "H9_SwapChain", location = "LaserCanvas.xaml.cs:Draw", message = "Draw: Exception", data = new { error = ex.Message, stackTrace = ex.StackTrace }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }

        public void SetFadeDuration(double seconds)
        {
            _fadeDurationSeconds = seconds;
        }
        
        public void UpdateCurrentStroke(Color color, float thickness)
        {
            // #region agent log
            try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run14", hypothesisId = "H5_ShowSettings", location = "LaserCanvas.xaml.cs:UpdateCurrentStroke", message = "UpdateCurrentStroke: Called", data = new { hasCurrentStroke = _currentStroke != null, newColor = $"{color.A},{color.R},{color.G},{color.B}", newThickness = thickness }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            if (_currentStroke != null)
            {
                _currentStroke.Color = color;
                _currentStroke.Thickness = thickness;
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run14", hypothesisId = "H5_ShowSettings", location = "LaserCanvas.xaml.cs:UpdateCurrentStroke", message = "UpdateCurrentStroke: Updated _currentStroke", data = new { updatedColor = $"{_currentStroke.Color.A},{_currentStroke.Color.R},{_currentStroke.Color.G},{_currentStroke.Color.B}", updatedThickness = _currentStroke.Thickness }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
            else
            {
                // #region agent log
                try { File.AppendAllText(@"c:\Users\mfuza\Downloads\laser pointer\.cursor\debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run14", hypothesisId = "H5_ShowSettings", location = "LaserCanvas.xaml.cs:UpdateCurrentStroke", message = "UpdateCurrentStroke: No current stroke to update", data = new { }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
            }
        }
    }
}

