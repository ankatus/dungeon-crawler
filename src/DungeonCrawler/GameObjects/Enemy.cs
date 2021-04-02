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
        public override List<GameObject> Children => Projectiles.Cast<GameObject>().ToList();
        public List<Projectile> Projectiles { get; set; }
        public float MaxHealth { get; }
        public float CurrentHealth { get; private set; }
        private float _movingSpeed;
        private DateTime _lastShotTime;
        private TimeSpan _minTimeBetweenShots;

        public Enemy(Vector2 position, int width, int height) : base(position, width, height)
        {
            Projectiles = new List<Projectile>();
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            _movingSpeed = 0.5F;
            _minTimeBetweenShots = TimeSpan.FromMilliseconds(100);
        }

        public void Update(GameObject target, List<GameObject> otherGameObjectsInRoom)
        {
            if (State == GameObjectState.Active)
            {
                ChaseTarget(target);
                Shoot(target);
            }

            foreach (var projectile in Projectiles)
            {
                projectile.Update(otherGameObjectsInRoom);
            }

            RemoveInactiveProjectiles();
        }

        private void ChaseTarget(GameObject target)
        {
            // Move towards target
            Vector2 travelDirection = Vector2.Normalize(Vector2.Subtract(target.Position, this.Position));
            Position += travelDirection * _movingSpeed;

            // Rotate towards target
            var (x, y) = Vector2.Subtract(target.Position, Position);
            Rotation = (float) Math.Atan2(y, x);
        }

        private void Shoot(GameObject target)
        {
            if (DateTime.Now - _lastShotTime > _minTimeBetweenShots)
            {
                // Create vector from player to target coordinates
                Vector2 projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

                Projectile projectile =
                    new Projectile((int) Position.X, (int) Position.Y, projectileTravelVector, 5, this);

                Projectiles.Add(projectile);
                _lastShotTime = DateTime.Now;
            }
        }

        private void RemoveInactiveProjectiles()
        {
            for (var i = 0; i < Projectiles.Count; i++)
            {
                if (Projectiles[i].State == GameObjectState.Inactive)
                {
                    Projectiles.RemoveAt(i);
                }
            }
        }

        public void ProjectileCollision(Projectile projectile)
        {
            if (projectile.Source != this)
            {
                CurrentHealth -= projectile.Damage;

                if (CurrentHealth <= 0)
                {
                    State = GameObjectState.Inactive;
                }
            }
        }
    }
}