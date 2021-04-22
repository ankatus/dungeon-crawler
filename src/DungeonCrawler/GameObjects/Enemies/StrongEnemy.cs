using DungeonCrawler.Guns;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects.Enemies
{
    public class StrongEnemy : Enemy
    {
        private const int WIDTH = 30;
        private const int HEIGHT = 60;
        public StrongEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {
            MaxHealth = 60;
            CurrentHealth = MaxHealth;
            MovingSpeed = 1;
            ActiveGun = new Shotgun(this);
        }
    }
}
