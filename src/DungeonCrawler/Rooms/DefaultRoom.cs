using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using DungeonCrawler.GameObjects.Items;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Rooms
{
    public class DefaultRoom : Room
    {
        public DefaultRoom(Vector2 position, int width, int height)
            : base(position, width, height)
        {
            Items.Add(new HealthPack(Position + new Vector2(300, 300), 0.5f));
            Items.Add(new ShotgunItem(Position + new Vector2(400, 400)));
            Enemies.Add(new Enemy(this, Position + new Vector2(300, 300), 20, 60));
            CreateSurroundingWalls(new List<DoorPosition>
                {DoorPosition.Top, DoorPosition.Bottom, DoorPosition.Left, DoorPosition.Right});
            RoomGraph.Build();
        }
    }
}