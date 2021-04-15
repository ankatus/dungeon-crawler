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
        public DefaultEnemy(Room room, Vector2 position, int width, int height) : base(room, position, width, height)
        {

        }
    }
}
