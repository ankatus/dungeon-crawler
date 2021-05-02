using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.Guns
{
    public class SniperGun : Gun
    {
        public SniperGun(GameObject owner) : base(owner)
        {
            BaseDamage = 12.0f;
            BaseSpeed = 40.0f;
            BaseFirerate = 1.0f;
            MaxAmmo = 12;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            var projectile = new Projectile(position, direction, Damage, Speed, Owner)
            {
                // Make projectile larger than default
                Width = 12,
                Height = 4
            };

            projectiles.Add(projectile);
            LastShot = DateTime.Now;

            Ammo -= 1;

            return projectiles;
        }
    }
}
