using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects
{
    public class Door : GameObject
    {
        public bool Activated { get; private set; }
        public DoorPosition DoorPosition { get; }

        public Door(int x, int y, int width, int height, DoorPosition doorDoorPosition) : base(x, y, width, height)
        {
            DoorPosition = doorDoorPosition;
        }

        public Door(Vector2 position, int width, int height, DoorPosition doorDoorPosition) : base(position, width, height)
        {
            DoorPosition = doorDoorPosition;
        }

        public void Update(Player player)
        {
            Activated = CollisionDetection.IsThereCollision(player, this);
        }
    }
}