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
            BaseDamage = 10.0f;
            BaseSpeed = 20.0f;
            BaseFirerate = 1.0f;
            MaxAmmo = 10;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            projectiles.Add(new Projectile(position, direction, Damage, Speed, Owner));
            LastShot = DateTime.Now;

            Ammo -= 1;

            return projectiles;
        }
    }
}
