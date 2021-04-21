using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Guns
{
    public class ExplosionGun : Gun
    {
        public ExplosionGun(GameObject owner) : base(owner)
        {
            BaseDamage = 1.0f;
            BaseSpeed = 5.0f;
            BaseFirerate = 2.0f;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            projectiles.Add(new ExplodingProjectile(position, direction, Damage, Speed, Owner));
            LastShot = DateTime.Now;

            return projectiles;
        }
    }
}
