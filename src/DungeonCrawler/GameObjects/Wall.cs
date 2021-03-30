using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects
{
    public class Wall : GameObject
    {
        public Wall(Vector2 position, int width, int height) : base(position, width, height) { }
    }
}
