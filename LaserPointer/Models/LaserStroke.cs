using System;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;

namespace LaserPointer.Models
{
    public class LaserStroke
    {
        public System.Numerics.Vector2 Start { get; set; }
        public System.Numerics.Vector2 End { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; } = 1.0f;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public float Thickness { get; set; } = 3.0f;
    }
}

