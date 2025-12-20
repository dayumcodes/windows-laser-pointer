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

namespace LaserPointer.Controls
{
    public sealed partial class LaserCanvas : UserControl
    {
        private List<LaserStroke> _activeStrokes = new List<LaserStroke>();
        private DispatcherQueueTimer _fadeTimer;
        private LaserStroke? _currentStroke;
        private Point? _lastPoint;
        private double _fadeDurationSeconds = 2.0;

        public LaserCanvas()
        {
            this.InitializeComponent();
            InitializeTimer();
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
        }

        public void ContinueStroke(Point point)
        {
            if (_currentStroke != null && _lastPoint.HasValue)
            {
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
                Canvas.Invalidate();
            }
        }

        public void EndStroke()
        {
            _currentStroke = null;
            _lastPoint = null;
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

            if (needsRedraw)
            {
                Canvas.Invalidate();
            }
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            using (var session = args.DrawingSession)
            {
                foreach (var stroke in _activeStrokes)
                {
                    var color = stroke.Color;
                    color.A = (byte)(255 * stroke.Opacity);

                    session.DrawLine(
                        stroke.Start,
                        stroke.End,
                        color,
                        stroke.Thickness);
                }
            }
        }

        public void SetFadeDuration(double seconds)
        {
            _fadeDurationSeconds = seconds;
        }
    }
}

