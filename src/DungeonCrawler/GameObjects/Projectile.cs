using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DungeonCrawler.GameObjects
{
    public class Projectile : GameObject
    {
        public Projectile(int x, int y, Vector2 travelVector, int speed) : base(GameObjectType.DefaultProjectile, x, y, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float)((float)Math.Atan2(Velocity.Y, Velocity.X));
        }

        public void Update()
        {
            Position += Velocity;
        }
    }
}
