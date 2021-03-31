using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public class Room
    {
        private const int WALL_THICKNESS = 10;

        public Vector2 Position;

        public int Width { get; init; }
        public int Height { get; init; }

        public List<GameObject> AllObjects => Walls.Cast<GameObject>().Concat(Enemies).ToList();
        public List<Wall> Walls { get; set; }
        public List<Enemy> Enemies { get; set; }

        public Room(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            Walls = new List<Wall>();
            Enemies = new List<Enemy>
            {
                new(Position + new Vector2(300,300), 20, 60)
            };
            CreateSurroundingWalls();
        }

        public void Update(Player player)
        {
            Enemies.ForEach(enemy => enemy.Update(player, AllObjects));
        }

        public void CreateSurroundingWalls()
        {
            // Top
            Walls.Add(new Wall(Position + new Vector2(Width / 2, WALL_THICKNESS / 2), Width, WALL_THICKNESS));

            // Bottom
            Walls.Add(new Wall(Position + new Vector2(Width / 2, Height - WALL_THICKNESS / 2), Width, WALL_THICKNESS));

            // Left
            Walls.Add(new Wall(Position + new Vector2(WALL_THICKNESS / 2, Height / 2), WALL_THICKNESS, Height));

            // Right
            Walls.Add(new Wall(Position + new Vector2(Width - WALL_THICKNESS / 2, Height / 2), WALL_THICKNESS, Height));
        }
    }
}