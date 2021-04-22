using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Guns
{
    public class ExplosionGun : Gun
    {
        public ExplosionGun(GameObject owner) : base(owner)
        {
            BaseDamage = 1.0f;
            BaseSpeed = 3.0f;
            BaseFirerate = 1.0f;
            MaxAmmo = 5;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire || Ammo == 0) return projectiles;

            projectiles.Add(new ExplodingProjectile(position, direction, Damage, Speed, Owner));
            LastShot = DateTime.Now;

            Ammo -= 1;

            return projectiles;
        }
    }
}
