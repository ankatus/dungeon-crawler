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
using System.Diagnostics;

namespace DungeonCrawler.GameObjects.Enemies
{
    public abstract class Enemy : GameObject
    {
        private const int PATH_UPDATE_INTERVAL = 1;

        private readonly Room _room;
        private int _ticksSincePathUpdate;
        private List<Point> _path;

        public float MaxHealth { get; protected set; }
        public float CurrentHealth { get; protected set; }
        protected int MovingSpeed { get; set; }
        protected Gun ActiveGun { get; set; }

        protected Enemy(Room room, Vector2 position, int width, int height) : base(position, width, height)
        {
            _room = room;
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            MovingSpeed = 2;
            ActiveGun = new DefaultGun(this);
            _ticksSincePathUpdate = PATH_UPDATE_INTERVAL;
            _path = new List<Point>();
        }

        public void Update(GameObject target, List<GameObject> gameObjects, bool noPathUpdate)
        {
            if (State != GameObjectState.Active) return;
            ChaseTarget(target, gameObjects, noPathUpdate);
            Shoot(gameObjects);
        }

        protected virtual void UpdatePath(GameObject target, List<GameObject> gameObjects)
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

            // Check if we are close to target
            if (distanceVector.Length() < MIN_TARGET_DISTANCE)
            {
                // Calculate if target is visible
                var targetIsVisible = IsProjectileGoingToHitPlayer(gameObjects);

                // Only if target is visible care about being too close to target
                if (targetIsVisible)
                {
                    if (distanceVector.Length() < MOVE_BACK_DISTANCE)
                    {
                        if (distanceVector.Length() == 0)
                        {
                            // If target is top off us, then move somewhere else
                            distanceVector = Vector2.UnitX;
                        }

                        // Move back, too close to target
                        actualTarget = localPosition - Vector2.Normalize(distanceVector) * MOVE_BACK_AMOUNT;
                    }
                    else
                    {
                        // Stop, close enough to target
                        _path = new List<Point>();
                        _ticksSincePathUpdate = 0;
                        return;
                    }
                }
            }

            var newPath = Pathfinding.FindPath((localPosition / _room.RoomGraph.TranslationFactor).ToPoint(),
                (actualTarget / _room.RoomGraph.TranslationFactor).ToPoint(), _room.RoomGraph.Graph);

            // If pathfinding failed, path will be empty
            if (newPath.Count == 0) return;

            // Remove the first position to make movement smoother
            if (newPath.Count > 1) newPath.RemoveAt(0);

            _path = newPath;
        }

        private void ChaseTarget(GameObject target, List<GameObject> gameObjects, bool noPathUpdate)
        {
            RotateTowardsTarget(target);

            if (_ticksSincePathUpdate >= PATH_UPDATE_INTERVAL && !noPathUpdate)
            {
                UpdatePath(target, gameObjects);
            }
            else
            {
                _ticksSincePathUpdate++;
            }

            if (_path.Count == 0) return;

            MoveTowardsNextPoint();
        }

        private Vector2 TranslatePathPoint(Point pathPoint)
        {
            var (x, y) = pathPoint;
            return new Vector2(x * _room.RoomGraph.TranslationFactor,
                y * _room.RoomGraph.TranslationFactor) + _room.Position;
        }

        private void MoveTowardsNextPoint()
        {
            var nextPathPosition = TranslatePathPoint(_path[0]);
            var nextPathPositionVec = Vector2.Subtract(nextPathPosition, Position);
            
            while (MovingSpeed > nextPathPositionVec.Length())
            {
                // Will overshoot next path point, continue on towards the subsequent one
                Position = nextPathPosition;
                _path.RemoveAt(0);
                nextPathPosition = TranslatePathPoint(_path[0]);
                nextPathPositionVec = Vector2.Subtract(nextPathPosition, Position);
            }
             
            var newPosition = Vector2.Normalize(nextPathPositionVec) * MovingSpeed + Position;
            Position = newPosition;
        }

        private void RotateTowardsTarget(GameObject target)
        {
            var (x, y) = Vector2.Subtract(target.Position, Position);
            Rotation = (float) Math.Atan2(y, x);
        }

        private void Shoot(List<GameObject> gameObjects)
        {
            var projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

            // Just fill ammo every time so that gun does not run out :)
            ActiveGun.FillAmmo();

            if (IsProjectileGoingToHitPlayer(gameObjects))
                _room.Projectiles.AddRange(ActiveGun.Shoot(Position, projectileTravelVector));
        }

        private bool IsProjectileGoingToHitPlayer(List<GameObject> gameObjects)
        {
            // Enemy is always facing player so travel vector can be calculated using rotation
            var projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

            var fakeProjectile = new Projectile(Position, projectileTravelVector, 0, 2, this);
            var projectileHit = false;
            var projectileHitPlayer = false;

            // Find if projectile is going to hit player (if player stays still)
            while (!projectileHit)
            {
                fakeProjectile.Position += fakeProjectile.Velocity;
                var collisions = CollisionDetection.GetCollisions(fakeProjectile, gameObjects);

                foreach (var gameObject in collisions)
                {
                    if (gameObject.Id == fakeProjectile.Source.Id)
                        continue;

                    if (gameObject is Wall or Door)
                    {
                        projectileHit = true;
                    }
                    else if (gameObject is Enemy)
                    {
                        projectileHit = true;
                    }
                    else if (gameObject is Player)
                    {
                        projectileHit = true;
                        projectileHitPlayer = true;
                    }
                }
            }

            return projectileHitPlayer;
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