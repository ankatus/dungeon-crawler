using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects
{
    public class Wall : GameObject
    {
        public Wall(int x, int y, int width, int height) : base(ObjectType.Wall, x, y, width, height) { }
    }
}
