using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DungeonCrawler.GameObjects
{
    public class Projectile : GameObject
    {
        private GameObject _source;

        public Projectile(int x, int y, Vector2 travelVector, int speed, GameObject source) : base(
            GameObjectType.DefaultProjectile, x, y, 6, 2)
        {
            Velocity = Vector2.Normalize(travelVector) * speed;

            Rotation = (float) Math.Atan2(Velocity.Y, Velocity.X);

            _source = source;
        }

        public void Update(GameObject gameObjectTree)
        {
            Position += Velocity;

            var collisions = CollisionDetection.GetCollisions(this, gameObjectTree);

            foreach (var gameObject in collisions)
            {
                if (gameObject.Id == _source.Id)
                    continue;

                if (gameObject.Type == GameObjectType.Wall)
                {
                    State = GameObjectState.Inactive;
                }
                else if (gameObject.Type == GameObjectType.Enemy)
                {
                    // Do stuff
                }
                else if (gameObject.Type == GameObjectType.Player)
                {
                    // Do stuff
                }
            }
        }
    }
}