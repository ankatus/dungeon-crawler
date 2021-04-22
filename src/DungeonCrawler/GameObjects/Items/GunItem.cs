using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Guns;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects.Items
{
    public class GunItem : Item
    {
        public Type GunType { get; }

        public GunItem(Type gunType, Vector2 position) : base(position)
        {
            GunType = gunType;
        }

        protected override void ItemActivated(Player player)
        {
            Gun gun;
            if (GunType == typeof(ExplosionGun))
            {
                gun = new ExplosionGun(player);
            }
            else if (GunType == typeof(MachineGun))
            {
                gun = new MachineGun(player);
            }
            else if (GunType == typeof(Shotgun))
            {
                gun = new Shotgun(player);
            }
            else if (GunType == typeof(SniperGun))
            {
                gun = new SniperGun(player);
            }
            else
            {
                throw new InvalidOperationException("Unknown gun type");
            }

            player.AddGun(gun);
        }
    }
}
