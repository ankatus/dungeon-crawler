using System.Collections.Generic;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Maps
{
    public class DefaultMap : GameMap
    {
        public DefaultMap() : base(1600, 900, 3, 3)
        {
            for (var y = 0; y < VerticalRooms; y++)
            {
                for (var x = 0; x < HorizontalRooms; x++)
                {
                    if (x == 0 && y == 0)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.UpperLeftCorner);
                    else if (x == HorizontalRooms - 1 && y == 0)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.UpperRightCorner);
                    else if (x == HorizontalRooms - 1 && y == VerticalRooms - 1)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.LowerRightCorner);
                    else if (x == 0 && y == VerticalRooms - 1)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.LowerLeftCorner);
                    else if (x != 0 && y != 0 && x != HorizontalRooms - 1 && y != VerticalRooms - 1)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.Normal);
                    else if (y == 0)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.UpperEdge);
                    else if (y == VerticalRooms - 1)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.LowerEdge);
                    else if (x == 0)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.LeftEdge);
                    else if (x == HorizontalRooms - 1)
                        Rooms[y, x] = new RandomNormalRoom(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight, RoomLocation.RightEdge);
                }
            }
        }
    }
}