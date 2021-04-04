using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects
{
    public class Door : GameObject
    {
        private bool _open;
        public bool Open { get { return _open; } set { _open = value; } }
        public bool Closed { get { return !_open; } set { _open = !value; } }
        public bool Activated { get; private set; }
        public DoorPosition DoorPosition { get; }

        public Door(int x, int y, int width, int height, DoorPosition doorDoorPosition) : base(x, y, width, height)
        {
            DoorPosition = doorDoorPosition;
            _open = false;
        }

        public Door(Vector2 position, int width, int height, DoorPosition doorDoorPosition) : base(position, width, height)
        {
            DoorPosition = doorDoorPosition;
            _open = false;
        }

        public void Update(Player player)
        {
            if (Open)
                Activated = CollisionDetection.IsThereCollision(player, this);
        }
    }
}