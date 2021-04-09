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
                    if (x == 0 && y == 0)
                        Rooms[y, x] = new CornerRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight);
                    else if (x == HorizontalRooms - 1 && y == 0)
                        Rooms[y, x] = new CornerRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            1);
                    else if (x == HorizontalRooms - 1 && y == VerticalRooms - 1)
                        Rooms[y, x] = new CornerRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            2);
                    else if (x == 0 && y == VerticalRooms - 1)
                        Rooms[y, x] = new CornerRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            3);
                    else if (x == 1 && y == 1)
                        Rooms[y, x] = new DefaultRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth,
                            RoomHeight);
                    else if (x == 1 && y == 0)
                        Rooms[y, x] = new SideRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            0);
                    else if (x == 2 && y == 1)
                        Rooms[y, x] = new SideRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            1);
                    else if (x == 1 && y == 2)
                        Rooms[y, x] = new SideRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            2);
                    else if (x == 0 && y == 1)
                        Rooms[y, x] = new SideRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight,
                            3);
                }
            }
        }
    }
}