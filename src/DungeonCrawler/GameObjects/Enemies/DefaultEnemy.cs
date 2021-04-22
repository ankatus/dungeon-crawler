using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects.Enemies
{
    public class DefaultEnemy : Enemy
    {
        private const int WIDTH = 30;
        private const int HEIGHT = 30;
        public DefaultEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {

        }
    }
}
