using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.Guns
{
    public class MachineGun : Gun
    {
        private Random randomGen;

        public MachineGun(GameObject owner) : base(owner)
        {
            BaseDamage = 1.0f;
            BaseSpeed = 10.0f;
            BaseFirerate = 40.0f;
            MaxAmmo = 300;
            randomGen = new Random();
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            var randomMultiplier = randomGen.NextDouble();

            var projectileSpreadAngle = Math.PI / 16;

            // Rotate direction vector to start of spread angle
            direction = CollisionDetection.RotateVector(direction, -projectileSpreadAngle / 2);

            // Rotate random amount towards end of spread angle
            direction = CollisionDetection.RotateVector(direction, projectileSpreadAngle * randomMultiplier);

            projectiles.Add(new Projectile(position, direction, Damage, Speed, Owner));
            LastShot = DateTime.Now;

            Ammo -= 1;

            return projectiles;
        }
    }
}
