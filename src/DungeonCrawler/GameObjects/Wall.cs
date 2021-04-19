using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Rooms;

namespace DungeonCrawler.GameObjects
{
    public class Wall : GameObject
    {
        public Wall(Vector2 position, int width, int height) : base(position, width, height)
        {
        }

        public static Wall FromCoordinates(Point a, Point b, int wallWidth)
        {
            if (a.X != b.X && a.Y != b.Y)
                throw new ArgumentException("Coordinates have to line up on either dimension");

            if (a.X != b.X)
            {
                // Horizontal wall
                var width = Math.Abs(a.X - b.X);
                var x = Math.Min(a.X,b.X) + width / 2;
                var y = a.Y;

                return new Wall(new Vector2(x, y), width, wallWidth);
            }
            else
            {
                // Vertical wall
                var height = Math.Abs(a.Y - b.Y);
                var x = a.X;
                var y = Math.Min(a.Y, b.Y) + height / 2;

                return new Wall(new Vector2(x, y), wallWidth, height);
            }
        }
    }
}