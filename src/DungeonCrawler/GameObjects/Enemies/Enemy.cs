using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework.Graphics;
using DungeonCrawler.Guns;

namespace DungeonCrawler.GameObjects.Enemies
{
    public abstract class Enemy : GameObject
    {
        private const int PATH_UPDATE_INTERVAL = 1;

        private readonly Room _room;

        public float MaxHealth { get; protected set; }
        public float CurrentHealth { get; protected set; }
        protected int MovingSpeed { get; set; }
        protected Gun ActiveGun { get; set; }
        private int _ticksSincePathUpdate;
        private List<Point> _path;

        public Enemy(Room room, Vector2 position, int width, int height) : base(position, width, height)
        {
            _room = room;
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            MovingSpeed = 2;
            ActiveGun = new DefaultGun(this);
            _ticksSincePathUpdate = PATH_UPDATE_INTERVAL;
        }

        public void Update(GameObject target, bool noPathUpdate)
        {
            if (State != GameObjectState.Active) return;
            ChaseTarget(target, noPathUpdate);
            Shoot(target);
        }

        private void ChaseTarget(GameObject target, bool noPathUpdate)
        {
            // Rotate towards target
            var (x, y) = Vector2.Subtract(target.Position, Position);
            Rotation = (float) Math.Atan2(y, x);

            if (_ticksSincePathUpdate >= PATH_UPDATE_INTERVAL && !noPathUpdate)
            {
                // Calculate path
                const int MAX_TARGET_DISTANCE = 1000;
                const int MIN_TARGET_DISTANCE = 100;
                const int MOVE_BACK_DISTANCE = 90;
                const int MOVE_BACK_AMOUNT = 10;

                var localPosition = Position - _room.Position;
                var localTargetPosition = target.Position - _room.Position;
                var actualTarget = localTargetPosition;
                var distanceVector = Vector2.Subtract(localTargetPosition, localPosition);

                // If distance is too long, set actual target to MAX_TARGET_DISTANCE towards target
                if (distanceVector.Length() > MAX_TARGET_DISTANCE)
                {
                    actualTarget = localPosition + Vector2.Normalize(distanceVector) * MAX_TARGET_DISTANCE;
                }

                // If Distance is very short, move back
                if (distanceVector.Length() < MOVE_BACK_DISTANCE)
                {
                    actualTarget = localPosition - Vector2.Normalize(distanceVector) * MOVE_BACK_AMOUNT;
                }
                // If Distance is moderately short, stop
                else if (distanceVector.Length() < MIN_TARGET_DISTANCE)
                {
                    _path = new List<Point>();
                    _ticksSincePathUpdate = 0;
                    return;
                }

                var path = Pathfinding.FindPath((localPosition / _room.RoomGraph.TranslationFactor).ToPoint(), (actualTarget / _room.RoomGraph.TranslationFactor).ToPoint(), _room.RoomGraph.Graph);

                // If pathfinding failed, path will be empty
                if (path.Count > 0) _path = path;
            }
            else
            {
                _ticksSincePathUpdate++;
            }

            if (_path is null || _path.Count == 0) return;

            // Remove the first position to make movement smoother
            if (_path.Count > 1) _path.RemoveAt(0);

            var nextPosition = new Vector2(_path[0].X * _room.RoomGraph.TranslationFactor, _path[0].Y * _room.RoomGraph.TranslationFactor) + _room.Position;

            // Remove "used" position
            _path.RemoveAt(0);

            if (nextPosition == Position) return;

            // Move
            var travelDirection = Vector2.Normalize(Vector2.Subtract(nextPosition, Position));
            Position += travelDirection * MovingSpeed;

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
            // Shoots forward
            var projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

            _room.Projectiles.AddRange(ActiveGun.Shoot(Position, projectileTravelVector));
        }

        public void ProjectileCollision(Projectile projectile)
        {
            if (projectile.Source.Id == Id) return;
            if (projectile.Source is Enemy) return;

            CurrentHealth -= projectile.Damage;

            if (CurrentHealth <= 0)
            {
                State = GameObjectState.Inactive;
            }
        }
    }
}