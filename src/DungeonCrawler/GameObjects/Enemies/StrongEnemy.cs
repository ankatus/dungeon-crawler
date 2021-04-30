using DungeonCrawler.Guns;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects.Enemies
{
    public class StrongEnemy : Enemy
    {
        private const int WIDTH = 30;
        private const int HEIGHT = 60;

        public StrongEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {
            // Give this slow dude a bit more time to move before calculating a new path
            PathUpdateInterval = 20;
            MaxHealth = 60;
            CurrentHealth = MaxHealth;
            MovingSpeed = 1;
            ActiveGun = new Shotgun(this);
        }

        protected override Vector2 DecideMovement(GameObject targetObject, List<GameObject> otherObjects)
        {
            const int MAX_TARGET_DISTANCE = 1000;
            const int MIN_TARGET_DISTANCE = 100;
            const int MOVE_BACK_DISTANCE = 90;
            const int MOVE_BACK_AMOUNT = 10;
            const int RANDOM_POS_MAX_DISTANCE = 1000;

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
                if (!targetIsVisible) return GetRandomPointNearPosition(targetObjLocalPos, RANDOM_POS_MAX_DISTANCE);

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

            return GetRandomPointNearPosition(targetObjLocalPos, RANDOM_POS_MAX_DISTANCE);
        }
    }
}