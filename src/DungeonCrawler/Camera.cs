using System;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public class Camera
    {
        private int _width;
        private int _height;

        public Point TopLeft;

        public float AspectRatio { get; }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                _height = (int) Math.Ceiling(value / AspectRatio);
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                _width = (int) Math.Ceiling(value * AspectRatio);
            }
        }

        public Camera(float aspectRatio)
        {
            AspectRatio = aspectRatio;
        }

        public void ZoomTo(Point topLeft, int targetWidth, int targetHeight)
        {
            TopLeft = topLeft;

            if (AspectRatio > 1.0f)
            {
                Height = targetHeight;
            }
            else
            {
                Width = targetWidth;
            }
        }
    }
}