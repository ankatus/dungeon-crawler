using System.Collections.Generic;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Maps
{
    public class DefaultMap : GameMap
    {
        public DefaultMap() : base(1600, 900, 3, 3)
        {
            var doorPositions = new List<DoorPosition> {DoorPosition.Top};
            for (var y = 0; y < VerticalRooms; y++)
            {
                for (var x = 0; x < HorizontalRooms; x++)
                {
                    Rooms[y, x] = new DefaultRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight);
                }
            }
        }
    }
}