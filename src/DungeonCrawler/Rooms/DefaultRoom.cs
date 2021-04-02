using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Rooms
{
    public class DefaultRoom : Room
    {
        public DefaultRoom(Vector2 position, int width, int height)
            : base(position, width, height, new List<DoorPosition> {DoorPosition.Top, DoorPosition.Bottom, DoorPosition.Left, DoorPosition.Right})
        {
            Enemies.Add(new Enemy(this, Position + new Vector2(300, 300), 20, 60));
        }
    }
}