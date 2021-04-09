using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Guns
{
    public abstract class Gun
    {
        private const float BASE_DAMAGE = 1.0f;
        private const float BASE_SPEED = 1.0f;
        private const float BASE_FIRERATE = 1.0f;

        protected DateTime LastShot { get; set; }
        protected long OwnerId { get; }

        public float Damage => BASE_DAMAGE * DamageMultiplier;
        public float Speed => BASE_SPEED * SpeedMultiplier;
        public float FireRate => BASE_FIRERATE * FireRateMultiplier;
        public float DamageMultiplier { get; set; }
        public float SpeedMultiplier { get; set; }
        public float FireRateMultiplier { get; set; }
        protected bool CanFire => (DateTime.Now - LastShot > TimeSpan.FromSeconds(1) / FireRate);

        protected Gun(long ownerId)
        {
            OwnerId = ownerId;
            DamageMultiplier = 1.0f;
            SpeedMultiplier = 1.0f;
            FireRateMultiplier = 1.0f;
        }

        public abstract List<Projectile> Shoot(Vector2 position, Vector2 direction);
    }
}
