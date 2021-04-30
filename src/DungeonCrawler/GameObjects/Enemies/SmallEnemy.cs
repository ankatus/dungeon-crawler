using System;
using System.Collections.Generic;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.GameObjects.Enemies
{
    public class SmallEnemy : Enemy
    {
        private enum ChaseSide
        {
            Top, Right, Bottom, Left
        }

        private const int WIDTH = 30;
        private const int HEIGHT = 30;
        private const int KEEP_OLD_PATH_TIMES = 5;

        private int _keptOldPath;
        private readonly ChaseSide _chaseSide;

        public SmallEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {
            _chaseSide = PickRandomChaseSide();
            _keptOldPath = KEEP_OLD_PATH_TIMES;
        }

        protected override void UpdatePath(GameObject targetObject, List<GameObject> otherObjects)
        {
            if (_keptOldPath < KEEP_OLD_PATH_TIMES)
            {
                _keptOldPath++;
                return;
            }

            _keptOldPath = 0;

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

        private Vector2 DecideMovement(GameObject targetObject, List<GameObject> gameObjects)
        {
            const int MAX_TARGET_DISTANCE = 1000;
            const int MIN_TARGET_DISTANCE = 100;
            const int MOVE_BACK_DISTANCE = 90;
            const int MOVE_BACK_AMOUNT = 10;

            var ownLocalPos = Position - Room.Position;
            var targetObjLocalPos = targetObject.Position - Room.Position;

            var distanceVector = Vector2.Subtract(targetObjLocalPos, ownLocalPos);

            // If distance is too long, set actual target to MAX_TARGET_DISTANCE towards target
            if (distanceVector.Length() > MAX_TARGET_DISTANCE)
            {
                return ownLocalPos + Vector2.Normalize(distanceVector) * MAX_TARGET_DISTANCE;
            }

            // Check if we are close to target
            if (distanceVector.Length() < MIN_TARGET_DISTANCE)
            {
                // Calculate if target is visible
                var targetIsVisible = IsProjectileGoingToHitPlayer(gameObjects);

                // Only if target is visible care about being too close to target
                if (!targetIsVisible) return PickMovementTarget(targetObjLocalPos);

                if (distanceVector.Length() < MOVE_BACK_DISTANCE)
                {
                    if (distanceVector.Length() == 0)
                    {
                        // If target is top off us, then move somewhere else
                        distanceVector = Vector2.UnitX;
                    }

                    // Move back, too close to target
                    return ownLocalPos - Vector2.Normalize(distanceVector) * MOVE_BACK_AMOUNT;
                }
                else
                {
                    // Stop, close enough to target
                    return ownLocalPos;
                }
            }

            return PickMovementTarget(targetObjLocalPos);
        }

        private Vector2 PickMovementTarget(Vector2 targetObjLocation)
        {
            return _chaseSide switch
            {
                ChaseSide.Top => GetRandomPointNearPosition(new Vector2(targetObjLocation.X, targetObjLocation.Y - 200)),
                ChaseSide.Right => GetRandomPointNearPosition(new Vector2(targetObjLocation.X + 200, targetObjLocation.Y)),
                ChaseSide.Bottom => GetRandomPointNearPosition(new Vector2(targetObjLocation.X, targetObjLocation.Y + 200)),
                ChaseSide.Left => GetRandomPointNearPosition(new Vector2(targetObjLocation.X - 200, targetObjLocation.Y)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector2 GetRandomPointNearPosition(Vector2 position)
        {
            var random = new Random();

            var distance = random.Next(200);

            var xLower = (int) Math.Min(Math.Max(position.X - distance, 0), Room.Width);
            var xUpper = (int) Math.Max(Math.Min(position.X + distance, Room.Width), 0);
            var yLower = (int) Math.Min(Math.Max(position.Y - distance, 0), Room.Height);
            var yUpper = (int) Math.Max(Math.Min(position.Y + distance, Room.Height), 0);


            var randomX = random.Next(xLower, xUpper + 1);
            var randomY = random.Next(yLower, yUpper + 1);

            return new Vector2(randomX, randomY);
        }

        private static ChaseSide PickRandomChaseSide()
        {
            var random = new Random();
            var values = Enum.GetValues(typeof(ChaseSide));
            return (ChaseSide) values.GetValue(random.Next(4));
        }
    }
}
