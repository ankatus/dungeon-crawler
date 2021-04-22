using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.Guns
{
    public class MachineGun : Gun
    {
        public MachineGun(GameObject owner) : base(owner)
        {
            BaseDamage = 0.5f;
            BaseSpeed = 10.0f;
            BaseFirerate = 30.0f;
            MaxAmmo = 120;
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
