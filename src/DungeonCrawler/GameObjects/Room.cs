using DungeonCrawler.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects
{
    public class Room : GameObject
    {
        public override List<GameObject> Children
        {
            get => Walls.Cast<GameObject>().ToList()
                .Concat(
                    Enemies.Cast<GameObject>().ToList()
                    ).ToList();
        }

        public List<Wall> Walls { get; set; }
        public List<Enemy> Enemies { get; set; }

        public Room() : base(GameObjectType.Room, 0, 0, 0, 0)
        {
            Walls = new List<Wall>();
            Enemies = new List<Enemy>();
            Walls.Add(new Wall(200, 200, 100, 10));
            Walls.Add(new Wall(200, 5, 100, 10));
            Enemies.Add(new Enemy(300, 300, 20, 60));
        }

        public void Update()
        {
            Enemies.ForEach(enemy => enemy.Update(this));
        }
    }
}
