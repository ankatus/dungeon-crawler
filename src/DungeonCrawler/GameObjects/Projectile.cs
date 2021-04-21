using DungeonCrawler.GameObjects.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.Rooms;

namespace DungeonCrawler.GameObjects
{
    public class Projectile : GameObject
    {
        public GameObject Source { get; }
        public float Damage { get; set; }

        public Projectile(int x, int y, Vector2 travelVector, float speed, GameObject source) : base(x, y, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            Source = source;
            Damage = 1;
        }

        public Projectile(Vector2 position, Vector2 travelVector, float damage, float speed, GameObject source) : base(position, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            Source = source;
            Damage = damage;
        }


        public virtual void Update(Room room)
        {
            Position += Velocity;
            var collisions = CollisionDetection.GetCollisions(this, room.AllObjects);

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