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
        
        private readonly ChaseSide _chaseSide;

        public SmallEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {
            _chaseSide = PickRandomChaseSide();
            MovingSpeed = 4;
            MaxHealth = 15;
        }

        protected override Vector2 DecideMovement(GameObject targetObject, List<GameObject> otherObjects)
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
                var targetIsVisible = IsProjectileGoingToHitPlayer(otherObjects);

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
            const int RANDOM_POS_MAX_DISTANCE = 200;
            const int SIDE_POS_OFFSET_FROM_TARGET = 200;

            return _chaseSide switch
            {
                ChaseSide.Top => GetRandomPointNearPosition(new Vector2(targetObjLocation.X, targetObjLocation.Y - SIDE_POS_OFFSET_FROM_TARGET), RANDOM_POS_MAX_DISTANCE),
                ChaseSide.Right => GetRandomPointNearPosition(new Vector2(targetObjLocation.X + SIDE_POS_OFFSET_FROM_TARGET, targetObjLocation.Y), RANDOM_POS_MAX_DISTANCE),
                ChaseSide.Bottom => GetRandomPointNearPosition(new Vector2(targetObjLocation.X, targetObjLocation.Y + SIDE_POS_OFFSET_FROM_TARGET), RANDOM_POS_MAX_DISTANCE),
                ChaseSide.Left => GetRandomPointNearPosition(new Vector2(targetObjLocation.X - SIDE_POS_OFFSET_FROM_TARGET, targetObjLocation.Y), RANDOM_POS_MAX_DISTANCE),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static ChaseSide PickRandomChaseSide()
        {
            var random = new Random();
            var values = Enum.GetValues(typeof(ChaseSide));
            return (ChaseSide) values.GetValue(random.Next(4));
        }
    }
}
