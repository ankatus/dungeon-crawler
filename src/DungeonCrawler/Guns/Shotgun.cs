using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.Guns
{
    public class Shotgun : Gun
    {
        public Shotgun(GameObject owner) : base(owner)
        {
            BaseDamage = 1.0f;
            BaseSpeed = 5.0f;
            BaseFirerate = 2.0f;
            MaxAmmo = 20;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            projectiles.Add(new Projectile(position, CollisionDetection.RotateVector(direction, Math.PI / 8), Damage, Speed, Owner));
            projectiles.Add(new Projectile(position, CollisionDetection.RotateVector(direction, Math.PI / 16), Damage, Speed, Owner));
            projectiles.Add(new Projectile(position, direction, Damage, Speed, Owner));
            projectiles.Add(new Projectile(position, CollisionDetection.RotateVector(direction, -Math.PI / 16), Damage, Speed, Owner));
            projectiles.Add(new Projectile(position, CollisionDetection.RotateVector(direction, -Math.PI / 8), Damage, Speed, Owner));
            LastShot = DateTime.Now;

            Ammo -= 1;

            return projectiles;
        }
    }
}
