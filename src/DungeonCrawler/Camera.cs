using System;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public class Camera
    {
        private int _width;
        private int _height;
        private const float ASPECT_RATIO = 16f / 9f;

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                _height = (int) Math.Ceiling(value / ASPECT_RATIO);
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                _width = (int) Math.Ceiling(value * ASPECT_RATIO);
            }
        }

        public Point TopLeft;

        public bool IsObjectVisible(GameObject gameObject)
        {
            // TODO: Implement
            return true;
        }
    }
}
