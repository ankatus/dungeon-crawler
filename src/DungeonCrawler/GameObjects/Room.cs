using System.Collections.Generic;
using System.Linq;

namespace DungeonCrawler.GameObjects
{
    public class Room : GameObject
    {
        private const int WALL_THICKNESS = 10;

        public override List<GameObject> Children =>
            Walls.Cast<GameObject>().ToList()
                .Concat(
                    Enemies.Cast<GameObject>().ToList()
                ).ToList();

        public List<Wall> Walls { get; set; }
        public List<Enemy> Enemies { get; set; }

        public Room(int width, int height) : base(0, 0, 0, 0)
        {
            Width = width;
            Height = height;
            Walls = new List<Wall>();
            Enemies = new List<Enemy>
            {
                new(300, 300, 20, 60)
            };
            CreateSurroundingWalls();
        }

        public void Update()
        {
            Enemies.ForEach(enemy => enemy.Update(this));
        }

        public void CreateSurroundingWalls()
        {
            // Top
            Walls.Add(new Wall(Width / 2, WALL_THICKNESS / 2, Width, WALL_THICKNESS));
            
            // Bottom
            Walls.Add(new Wall(Width / 2, Height - WALL_THICKNESS / 2, Width, WALL_THICKNESS));
            
            // Left
            Walls.Add(new Wall(WALL_THICKNESS / 2, Height / 2, WALL_THICKNESS, Height));
            
            // Right
            Walls.Add(new Wall(Width - WALL_THICKNESS  / 2, Height / 2, WALL_THICKNESS, Height));
        }
    }
}