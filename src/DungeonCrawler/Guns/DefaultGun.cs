using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Guns
{
    public class DefaultGun : Gun
    {
        public DefaultGun(long ownerId) : base(ownerId)
        {
        }

        public override List<Projectile> Shoot(Vector2 position, Vector2 direction)
        {
            var projectiles = new List<Projectile>();
            if (!CanFire) return projectiles;

            projectiles.Add(new Projectile(position, direction, Speed, OwnerId));
            LastShot = DateTime.Now;

            return projectiles;
        }
    }
}