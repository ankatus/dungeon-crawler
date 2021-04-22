using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Guns;

namespace DungeonCrawler.GameObjects.Items
{
    public class MachineGunItem : Item
    {
        public MachineGunItem(Vector2 position) : base(position)
        {
        }

        protected override void ItemActivated(Player player)
        {
            var gun = new MachineGun(player);
            player.AddGun(gun);
        }
    }
}
