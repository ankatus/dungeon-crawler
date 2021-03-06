using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Guns
{
    public abstract class Gun
    {
        protected GameObject Owner { get; }
        protected DateTime LastShot { get; set; }
        
        public int MaxAmmo { get; protected set; }
        public int Ammo { get; protected set; }
        public float BaseDamage { get; protected set; }
        public float BaseSpeed { get; protected set; }
        public float BaseFirerate { get; protected set; }
        public float DamageMultiplier { get; set; }
        public float SpeedMultiplier { get; set; }
        public float FireRateMultiplier { get; set; }
        public float Damage => BaseDamage * DamageMultiplier;
        public float Speed => BaseSpeed * SpeedMultiplier;
        public float FireRate => BaseFirerate * FireRateMultiplier;

        protected bool CanFire => (DateTime.Now - LastShot > TimeSpan.FromSeconds(1) / FireRate) && (this is DefaultGun || Ammo > 0);

        protected Gun(GameObject owner)
        {
            Owner = owner;
            BaseDamage = 1.0f;
            BaseSpeed = 1.0f;
            BaseFirerate = 1.0f;
            DamageMultiplier = 1.0f;
            SpeedMultiplier = 1.0f;
            FireRateMultiplier = 1.0f;
            MaxAmmo = 0;
        }

        public abstract List<Projectile> Shoot(Vector2 position, Vector2 direction);

        public void FillAmmo()
        {
            Ammo = MaxAmmo;
        }
    }
}
