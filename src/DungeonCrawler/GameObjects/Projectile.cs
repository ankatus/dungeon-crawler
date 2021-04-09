using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonCrawler.GameObjects
{
    public class Projectile : GameObject
    {
        public long SourceId { get; }
        public float Damage { get; set; }

        public Projectile(int x, int y, Vector2 travelVector, float speed, long sourceId) : base(x, y, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            SourceId = sourceId;
            Damage = 1;
        }

        public Projectile(Vector2 position, Vector2 travelVector, float damage, float speed, long sourceId) : base(position, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            SourceId = sourceId;
            Damage = damage;
        }


        public void Update(List<GameObject> gameObjects)
        {
            Position += Velocity;
            var collisions = CollisionDetection.GetCollisions(this, gameObjects);

            foreach (var gameObject in collisions)
            {
                if (gameObject.Id == SourceId)
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