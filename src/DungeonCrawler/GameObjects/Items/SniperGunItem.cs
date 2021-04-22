using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Guns;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects.Items
{
    public class SniperGunItem : Item
    {
        public SniperGunItem(Vector2 position) : base(position)
        {
        }

        protected override void ItemActivated(Player player)
        {
            var gun = new SniperGun(player);
            player.AddGun(gun);
        }
    }
}
