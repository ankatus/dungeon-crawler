using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.Guns
{
    public class Shotgun : Gun
    {
        private const int NUM_PROJECTILES = 9;

        public Shotgun(GameObject owner) : base(owner)
        {
            BaseDamage = 3.0f;
            BaseSpeed = 5.0f;
            BaseFirerate = 0.5f;
            MaxAmmo = 20;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();

            if (!CanFire) return new List<Projectile>();

            direction = CollisionDetection.RotateVector(direction, -Math.PI / 4);

            for (var i = 0; i < NUM_PROJECTILES; i++)
            {
                var rotation = Math.PI / 2 * (i + 1) / NUM_PROJECTILES;
                var projectileDirection = CollisionDetection.RotateVector(direction, rotation);
                var projectile = new Projectile(position, projectileDirection, Damage, Speed, Owner);
                projectiles.Add(projectile);
            }

            LastShot = DateTime.Now;
            Ammo -= 1;

            return projectiles;
        }
    }
}
