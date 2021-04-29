using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects
{
    public class Dummy : GameObject
    {
        public Dummy(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public Dummy(Vector2 position, int width, int height) : base(position, width, height)
        {
        }
    }
}
