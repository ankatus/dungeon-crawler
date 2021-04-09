using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Guns;

namespace DungeonCrawler.GameObjects.Items
{
    public class ShotgunItem : Item
    {
        public ShotgunItem(Vector2 position) : base(position)
        {
        }

        protected override void ItemActivated(Player player)
        {
            var shotgun = new Shotgun(player);
            player.Guns.Add(shotgun);
        }
    }
}
