using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects
{
    public class Enemy : GameObject
    {
        public Enemy(int x, int y, int width, int height) : base(GameObjectType.Enemy, x, y, width, height)
        {

        }

        public void Update(GameObject gameObjectTree)
        {

        }
    }
}
