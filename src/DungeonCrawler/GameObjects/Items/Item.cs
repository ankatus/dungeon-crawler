using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects.Items
{
    public abstract class Item : GameObject
    {
        public Item(Vector2 position) : base(position, 10, 10)
        {
        }

        protected abstract void ItemActivated(Player player);

        public void Update(Player player)
        {
            if (CollisionDetection.IsThereCollision(this, player))
            {
                ItemActivated(player);
                State = GameObjectState.Inactive;
            }
        }
    }
}
