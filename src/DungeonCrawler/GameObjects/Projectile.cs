using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DungeonCrawler
{
    public class Projectile : GameObject
    {
        public Projectile(int x, int y, Vector2 travelVector, int speed) : base(GameObjectType.DefaultProjectile, x, y, 2, 6)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float)((float)Math.Atan2(Velocity.Y, Velocity.X) - Math.PI / 2);
        }

        public void Move()
        {
            Position += Velocity;
        }
    }
}
