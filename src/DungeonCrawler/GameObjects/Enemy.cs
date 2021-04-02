using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Rooms;

namespace DungeonCrawler.GameObjects
{
    public class Enemy : GameObject
    {
        private Room _room;

        public float MaxHealth { get; }
        public float CurrentHealth { get; private set; }
        private readonly float _movingSpeed;
        private DateTime _lastShotTime;
        private readonly TimeSpan _minTimeBetweenShots;

        public Enemy(Room room, Vector2 position, int width, int height) : base(position, width, height)
        {
            _room = room;
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            _movingSpeed = 0.5F;
            _minTimeBetweenShots = TimeSpan.FromMilliseconds(100);
        }

        public void Update(GameObject target)
        {
            if (State != GameObjectState.Active) return;
            ChaseTarget(target);
            Shoot(target);
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
            if (DateTime.Now - _lastShotTime <= _minTimeBetweenShots) return;

            // Create vector from player to target coordinates
            var projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

            var projectile =
                new Projectile((int) Position.X, (int) Position.Y, projectileTravelVector, 5, this);

            _room.Projectiles.Add(projectile);
            _lastShotTime = DateTime.Now;
        }

        public void ProjectileCollision(Projectile projectile)
        {
            if (projectile.Source == this) return;

            CurrentHealth -= projectile.Damage;

            if (CurrentHealth <= 0)
            {
                State = GameObjectState.Inactive;
            }
        }
    }
}