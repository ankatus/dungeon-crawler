using System;
using System.Collections.Generic;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects
{
    public class ExplodingProjectile : Projectile
    {
        private const int NUM_PROJECTILES = 64;
        private const float PROJECTILE_DAMAGE = 5.0f;
        private const float PROJECTILE_SPEED = 5.0f;

        public ExplodingProjectile(int x, int y, Vector2 travelVector, float speed, GameObject source) : base(x, y, travelVector, speed, source)
        {
        }

        public ExplodingProjectile(Vector2 position, Vector2 travelVector, float damage, float speed, GameObject source) : base(position, travelVector, damage, speed, source)
        {
        }

        public override void Update(Room room, Player player)
        {
            var previousState = State;
            base.Update(room, player);

            if (State == GameObjectState.Inactive && previousState == GameObjectState.Active)
            {
                // Hit something, explode
                Explode(room);
            }
        }

        private void Explode(Room room)
        {
            var projectiles = new List<Projectile>();

            for (var i = 0; i < NUM_PROJECTILES; i++)
            {
                var rotation = 2 * Math.PI * (i + 1) / NUM_PROJECTILES;
                var direction = CollisionDetection.RotateVector(Velocity, rotation);
                var projectile = new Projectile(Position, direction, PROJECTILE_DAMAGE, PROJECTILE_SPEED, Source);
                projectiles.Add(projectile);
            }

            room.Projectiles.AddRange(projectiles);
        }
    }
}