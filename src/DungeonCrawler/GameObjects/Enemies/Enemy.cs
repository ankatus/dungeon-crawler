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
        private const int DEFAULT_UPDATE_INTERVAL = 10;

        private int _ticksSincePathUpdate;

        protected int PathUpdateInterval { get; set; }
        protected Room Room { get; }
        protected List<Point> Path { get; set; }

        public float MaxHealth { get; protected set; }
        public float CurrentHealth { get; protected set; }
        protected int MovingSpeed { get; set; }
        protected Gun ActiveGun { get; set; }

        protected Enemy(Room room, Vector2 position, int width, int height) : base(position, width, height)
        {
            PathUpdateInterval = DEFAULT_UPDATE_INTERVAL;
            _ticksSincePathUpdate = PathUpdateInterval;
            Room = room;
            MaxHealth = 20;
            CurrentHealth = MaxHealth;
            MovingSpeed = 2;
            ActiveGun = new DefaultGun(this);
            Path = new List<Point>();
        }

        public void Update(GameObject target, List<GameObject> gameObjects, bool noPathUpdate)
        {
            if (State != GameObjectState.Active) return;
            ChaseTarget(target, gameObjects, noPathUpdate);
            Shoot(gameObjects);
        }

        protected void UpdatePath(GameObject targetObject, List<GameObject> otherObjects)
        {
            if (_ticksSincePathUpdate < PathUpdateInterval)
            {
                _ticksSincePathUpdate++;
                return;
            }

            _ticksSincePathUpdate = 0;

            var ownLocalPos = Position - Room.Position;
            var movementTarget = DecideMovement(targetObject, otherObjects);

            var newPath = Pathfinding.FindPath(
                (ownLocalPos / Room.RoomGraph.TranslationFactor).ToPoint(),
                (movementTarget / Room.RoomGraph.TranslationFactor).ToPoint(), 
                Room.RoomGraph.Graph);

            // If pathfinding failed, path will be empty
            if (newPath.Count == 0) return;

            // Remove the first position to make movement smoother
            if (newPath.Count > 1) newPath.RemoveAt(0);

            Path = newPath;
        }

        protected abstract Vector2 DecideMovement(GameObject targetObject, List<GameObject> otherObjects);

        private void ChaseTarget(GameObject target, List<GameObject> gameObjects, bool noPathUpdate)
        {
            RotateTowardsTarget(target);

            if (!noPathUpdate) UpdatePath(target, gameObjects);

            MoveTowardsNextPoint();
        }

        private Vector2 TranslatePathPoint(Point pathPoint)
        {
            var (x, y) = pathPoint;
            return new Vector2(x * Room.RoomGraph.TranslationFactor,
                y * Room.RoomGraph.TranslationFactor) + Room.Position;
        }

        private void MoveTowardsNextPoint()
        {
            if (Path.Count == 0) return;

            var nextPathPosition = TranslatePathPoint(Path[0]);
            var nextPathPositionVec = Vector2.Subtract(nextPathPosition, Position);

            while (MovingSpeed > nextPathPositionVec.Length())
            {
                // Will overshoot next path point, continue on towards the subsequent one
                Position = nextPathPosition;
                Path.RemoveAt(0);

                if (Path.Count == 0) return;

                nextPathPosition = TranslatePathPoint(Path[0]);
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
                Room.Projectiles.AddRange(ActiveGun.Shoot(Position, projectileTravelVector));
        }

        protected bool IsProjectileGoingToHitPlayer(List<GameObject> gameObjects)
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
        
        protected Vector2 GetRandomPointNearPosition(Vector2 position, int maxDistance)
        {
            var random = new Random();

            var distance = random.Next(maxDistance);

            var xLower = (int) Math.Min(Math.Max(position.X - distance, 0), Room.Width);
            var xUpper = (int) Math.Max(Math.Min(position.X + distance, Room.Width), 0);
            var yLower = (int) Math.Min(Math.Max(position.Y - distance, 0), Room.Height);
            var yUpper = (int) Math.Max(Math.Min(position.Y + distance, Room.Height), 0);


            var randomX = random.Next(xLower, xUpper + 1);
            var randomY = random.Next(yLower, yUpper + 1);

            return new Vector2(randomX, randomY);
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