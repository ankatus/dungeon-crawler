using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects
{
    public class Door : GameObject
    {
        public bool Activated { get; private set; }
        public DoorPosition Position { get; }

        public Door(int x, int y, int width, int height, DoorPosition doorPosition) : base(x, y, width, height)
        {
            Position = doorPosition;
        }

        public Door(Vector2 position, int width, int height, DoorPosition doorPosition) : base(position, width, height)
        {
            Position = doorPosition;
        }

        public void Update(Player player)
        {
            Activated = CollisionDetection.IsThereCollision(player, this);
        }
    }
}