using DungeonCrawler.GameObjects.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Rooms;
using DungeonCrawler.Guns;

namespace DungeonCrawler.GameObjects
{
    public class Projectile : GameObject
    {
        private const int DEFAULT_WIDTH = 6;
        private const int DEFAULT_HEIGH = 2;
        public GameObject Source { get; }
        public Gun SourceGun { get; }
        public float Damage { get; set; }

        public Projectile(Vector2 position, Vector2 travelVector, float damage, float speed, GameObject source) : base(position, DEFAULT_WIDTH, DEFAULT_HEIGH)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            Source = source;
            Damage = damage;
        }

        public Projectile(Vector2 position, Vector2 travelVector, float damage, float speed, GameObject source, Gun sourceGun) : base(position, DEFAULT_WIDTH, DEFAULT_HEIGH)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            Source = source;
            Damage = damage;
            SourceGun = sourceGun;

            if (SourceGun is ExplosionGun)
            {
                // If shot from ExplosionGun make projectile larger
                Width = 12;
                Height = 5;
            }
        }

        public virtual void Update(Room room, Player player)
        {
            Position += Velocity;
            var collisions = CollisionDetection.GetCollisions(this, room.AllObjects.Concat(new[] { player }).ToList());

            foreach (var gameObject in collisions)
            {
                if (gameObject.Id == Source.Id)
                    continue;

                if (gameObject is Wall or Door)
                {
                    State = GameObjectState.Inactive;
                }
                else if (gameObject is Enemy)
                {
                    ((Enemy) gameObject).ProjectileCollision(this);
                    State = GameObjectState.Inactive;
                }
                else if (gameObject is Player)
                {
                    ((Player) gameObject).ProjectileCollision(this);
                    State = GameObjectState.Inactive;
                }
            }
        }
    }
}