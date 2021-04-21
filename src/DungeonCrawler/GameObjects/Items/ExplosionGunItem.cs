using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Guns;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects.Items
{
    public class ExplosionGunItem : Item
    {
        public ExplosionGunItem(Vector2 position) : base(position)
        {
        }

        protected override void ItemActivated(Player player)
        {
            var explosionGun = new ExplosionGun(player);
            player.Guns.Add(explosionGun);
        }
    }
}
