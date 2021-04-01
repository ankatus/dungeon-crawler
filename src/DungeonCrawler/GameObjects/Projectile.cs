using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.GameObjects
{
    public class Projectile : GameObject
    {
        public GameObject Source { get; }
        public int Damage { get; set; }

        public Projectile(int x, int y, Vector2 travelVector, int speed, GameObject source) : base(x, y, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);

            Source = source;
            Damage = 1;
        }

        public void Update(List<GameObject> gameObjects)
        {
            Position += Velocity;

            var collisions = CollisionDetection.GetCollisions(this, gameObjects);

            foreach (var gameObject in collisions)
            {
                if (gameObject.Id == Source.Id)
                    continue;

                if (gameObject is Wall)
                {
                    State = GameObjectState.Inactive;
                }
                else if (gameObject is Enemy)
                {
                    ((Enemy)gameObject).ProjectileCollision(this);
                    State = GameObjectState.Inactive;
                }
                else if (gameObject is Player)
                {
                    // Do stuff
                }
            }
        }
    }
}