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

        public float MaxHealth { get; protected set; }
        public float CurrentHealth { get; protected set; }
        protected int MovingSpeed { get; set; }
        protected Gun ActiveGun { get; set; }
        private int _ticksSincePathUpdate;
        private List<Point> _path;

        protected Enemy(Room room, Vector2 position, int width, int height) : base(position, width, height)
        {
            _room = room;
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            MovingSpeed = 2;
            ActiveGun = new DefaultGun(this);
            _ticksSincePathUpdate = PATH_UPDATE_INTERVAL;
        }

        public void Update(GameObject target, List<GameObject> gameObjects, bool noPathUpdate)
        {
            if (State != GameObjectState.Active) return;
            ChaseTarget(target, gameObjects, noPathUpdate);
            Shoot(gameObjects);
        }

        private void ChaseTarget(GameObject target, List<GameObject> gameObjects, bool noPathUpdate)
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

                var path = Pathfinding.FindPath((localPosition / _room.RoomGraph.TranslationFactor).ToPoint(),
                    (actualTarget / _room.RoomGraph.TranslationFactor).ToPoint(), _room.RoomGraph.Graph);

                // If pathfinding failed, path will be empty
                if (path.Count > 0)
                {
                    _path = path;

                    // Remove the first position to make movement smoother
                    if (_path.Count > 1) _path.RemoveAt(0);
                }
            }
            else
            {
                _ticksSincePathUpdate++;
            }

            if (_path is null || _path.Count == 0) return;


            var nextPosition = new Vector2(_path[0].X * _room.RoomGraph.TranslationFactor,
                _path[0].Y * _room.RoomGraph.TranslationFactor) + _room.Position;

            // Remove "used" position
            _path.RemoveAt(0);

            if (nextPosition == Position) return;

            // Move
            var travelDirection = Vector2.Normalize(Vector2.Subtract(nextPosition, Position));
            var tentativePosition = Position + travelDirection * MovingSpeed;

            if (Vector2.Distance(Position, tentativePosition) > Vector2.Distance(Position, nextPosition))
            {
                // Next position reached
                Position = nextPosition;
                _path.RemoveAt(0);
            }
            else
            {
                Position = tentativePosition;
            }
        }

        private bool CheckWalls(Vector2 position)
        {
            var collider = new Dummy(position, 10, 10);
            return CollisionDetection.GetCollisions(collider, _room.Walls.Cast<GameObject>().ToList()).Count > 0;
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