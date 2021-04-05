using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler.GameObjects
{
    public class Enemy : GameObject
    {
        private const int PATH_UPDATE_INTERVAL = 1;

        private readonly Room _room;

        public float MaxHealth { get; }
        public float CurrentHealth { get; private set; }
        private readonly int _movingSpeed;
        private DateTime _lastShotTime;
        private readonly TimeSpan _minTimeBetweenShots;
        private int _ticksSincePathUpdate;
        private List<Point> _path;

        public Enemy(Room room, Vector2 position, int width, int height) : base(position, width, height)
        {
            _room = room;
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            _movingSpeed = 2;
            _minTimeBetweenShots = TimeSpan.FromMilliseconds(100);
            _ticksSincePathUpdate = PATH_UPDATE_INTERVAL;
        }

        public void Update(GameObject target)
        {
            if (State != GameObjectState.Active) return;
            ChaseTarget(target);
            Shoot(target);
        }

        private void ChaseTarget(GameObject target)
        {
            // Rotate towards target
            var (x, y) = Vector2.Subtract(target.Position, Position);
            Rotation = (float) Math.Atan2(y, x);

            if (_ticksSincePathUpdate >= PATH_UPDATE_INTERVAL)
            {
                // Calculate path
                const int MAX_TARGET_DISTANCE = 1000;

                var localPosition = Position - _room.Position;
                var localTargetPosition = target.Position - _room.Position;
                var actualTarget = localTargetPosition;
                var distanceVector = Vector2.Subtract(localTargetPosition, localPosition);
                if (distanceVector.Length() > MAX_TARGET_DISTANCE)
                {
                    actualTarget = localPosition + distanceVector / 2;
                }
                var path = Pathfinding.FindPath((localPosition / _room.RoomGraph.TranslationFactor).ToPoint(), (actualTarget / _room.RoomGraph.TranslationFactor).ToPoint(), _room.RoomGraph.Graph);

                // If pathfinding failed, path will be empty
                if (path.Count > 0) _path = path;

                _ticksSincePathUpdate = 0;
            }
            else
            {
                _ticksSincePathUpdate++;
            }

            if ( _path is null || _path.Count == 0) return;
            
            // Remove the first position to make movement smoother
            if (_path.Count > 1) _path.RemoveAt(0);

            var nextPosition = new Vector2(_path[0].X * _room.RoomGraph.TranslationFactor, _path[0].Y * _room.RoomGraph.TranslationFactor);

            // Remove "used" position
            _path.RemoveAt(0);

            if (nextPosition == Position) return;

            // Move
            var travelDirection = Vector2.Normalize(Vector2.Subtract(nextPosition, Position));
            Position += travelDirection * _movingSpeed;
            
            // // If overlapping, move back until not overlapping
            // while (CheckWalls())
            // {
            //     Position -= travelDirection * _movingSpeed;
            // }
        }
        
        private bool CheckWalls()
        {
            return CollisionDetection.GetOverlaps(this, _room.Walls.Cast<GameObject>().ToList()).Count > 0;
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