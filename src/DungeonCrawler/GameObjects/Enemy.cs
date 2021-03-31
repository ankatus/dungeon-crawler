using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects
{
    public class Enemy : GameObject
    {
        private int _health;
        private float _movingSpeed;

        public Enemy(Vector2 position, int width, int height) : base(position, width, height)
        {
            _health = 10;
            _movingSpeed = 0.5F;
        }

        public void Update(GameObject gameObjectTree)
        {
            // Does not work yet because player does not exist in gameObjectTree
            foreach (var gameObject in gameObjectTree.Children)
            {
                if (gameObject is Player)
                {
                    MoveTowardsPlayer((Player)gameObject);
                }
            }
        }

        private void MoveTowardsPlayer(Player player)
        {
            Vector2 travelDirection = Vector2.Normalize(Vector2.Subtract(player.Position, this.Position));
            Position += travelDirection * _movingSpeed;
        }

        public void ProjectileCollision(Projectile projectile)
        {
            if (projectile.Source != this)
            {
                _health -= projectile.Damage;

                if (_health <= 0)
                {
                    State = GameObjectState.Inactive;
                }
            }
        }
    }
}
