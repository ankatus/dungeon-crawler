using System;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public class Camera
    {
        private int _width;
        private int _height;
        private int _travelSpeed;
        private Point _travelTarget;
        private int _travelTargetWidth;

        public Point TopLeft;
        public bool Travelling { get; private set; }

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

        public void StartTravel(Point topLeft, int targetWidth, int speed)
        {
            if (AspectRatio < 1.0f) throw new NotImplementedException();

            Travelling = true;
            _travelSpeed = speed;
            _travelTarget = topLeft;
            _travelTargetWidth = targetWidth;
        }

        public void UpdateTravel()
        {
            if (!Travelling) throw new InvalidOperationException();

            var done = true;

            if (TopLeft.X < _travelTarget.X)
            {
                TopLeft.X = Math.Min(_travelTarget.X, TopLeft.X + _travelSpeed);
                done = false;
            }
            else if (TopLeft.X > _travelTarget.X)
            {
                TopLeft.X = Math.Max(_travelTarget.X, TopLeft.X - _travelSpeed);
                done = false;
            }

            if (TopLeft.Y < _travelTarget.Y)
            {
                TopLeft.Y = Math.Min(_travelTarget.Y, TopLeft.Y + _travelSpeed);
                done = false;
            }
            else if (TopLeft.Y > _travelTarget.Y)
            {
                TopLeft.Y = Math.Max(_travelTarget.Y, TopLeft.Y - _travelSpeed);
                done = false;
            }

            if (Width < _travelTargetWidth)
            {
                Width = Math.Min(_travelTargetWidth, Width + _travelSpeed);
                done = false;
            }
            else if (Width > _travelTargetWidth)
            {
                Width = Math.Max(_travelTargetWidth, Width - _travelSpeed);
                done = false;
            }

            if (done) Travelling = false;
        }
    }
}