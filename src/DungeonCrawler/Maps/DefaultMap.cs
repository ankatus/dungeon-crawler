using Microsoft.Xna.Framework;

namespace DungeonCrawler.Maps
{
    public class DefaultMap : GameMap
    {
        public DefaultMap() : base(500, 500, 3, 3)
        {
            for (var y = 0; y < VerticalRooms; y++)
            {
                for (var x = 0; x < HorizontalRooms; x++)
                {
                    Rooms[y, x] = new Room(new Vector2(x * RoomWidth, y * RoomHeight), RoomWidth, RoomHeight);
                }
            }
        }
    }
}