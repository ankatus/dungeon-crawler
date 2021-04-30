using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects.Enemies
{
    public class SmallEnemy : Enemy
    {
        private const int WIDTH = 30;
        private const int HEIGHT = 30;
        public SmallEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {

        }
    }
}
