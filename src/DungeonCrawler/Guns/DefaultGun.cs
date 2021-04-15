﻿using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Guns
{
    public class DefaultGun : Gun
    {
        public DefaultGun(GameObject owner) : base(owner)
        {
            BaseDamage = 1.0f;
            BaseSpeed = 5.0f;
            BaseFirerate = 10.0f;
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            projectiles.Add(new Projectile(position, direction, Damage, Speed, Owner));
            LastShot = DateTime.Now;

            return projectiles;
        }
    }
}