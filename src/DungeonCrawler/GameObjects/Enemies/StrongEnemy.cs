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
        private const int KEEP_OLD_PATH_TIMES = 10;

        private int _keptOldPath;

        public StrongEnemy(Room room, Vector2 position) : base(room, position, WIDTH, HEIGHT)
        {
            MaxHealth = 60;
            CurrentHealth = MaxHealth;
            MovingSpeed = 1;
            ActiveGun = new Shotgun(this);
            _keptOldPath = KEEP_OLD_PATH_TIMES;
        }

        protected override void UpdatePath(GameObject target, List<GameObject> gameObjects)
        {
            const int MAX_TARGET_DISTANCE = 1000;
            const int MIN_TARGET_DISTANCE = 100;
            const int MOVE_BACK_DISTANCE = 90;
            const int MOVE_BACK_AMOUNT = 10;

            if (_keptOldPath < KEEP_OLD_PATH_TIMES)
            {
                _keptOldPath++;
                return;
            }

            _keptOldPath = 0;

            var localPosition = Position - Room.Position;
            var localTargetPosition = target.Position - Room.Position;
            var actualTarget = GetRandomPointNearPosition(localTargetPosition);
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
                        Path = new List<Point>();
                        return;
                    }
                }
            }

            var newPath = Pathfinding.FindPath((localPosition / Room.RoomGraph.TranslationFactor).ToPoint(),
                (actualTarget / Room.RoomGraph.TranslationFactor).ToPoint(), Room.RoomGraph.Graph);

            // If pathfinding failed, path will be empty
            if (newPath.Count == 0) return;

            // Remove the first position to make movement smoother
            if (newPath.Count > 1) newPath.RemoveAt(0);

            Path = newPath;
        }

        private Vector2 GetRandomPointNearPosition(Vector2 targetPos)
        {
            var random = new Random();

            var distance = random.Next(1000);

            var xLower = (int) Math.Min(Math.Max(targetPos.X - distance, Room.Position.X), Room.Position.X + Room.Width);
            var xUpper = (int) Math.Max(Math.Min(targetPos.X + distance, Room.Position.X + Room.Width), 0);
            var yLower = (int) Math.Min(Math.Max(targetPos.Y - distance, Room.Position.Y), Room.Position.Y + Room.Height);
            var yUpper = (int) Math.Max(Math.Min(targetPos.Y + distance, Room.Position.Y + Room.Height), 0);


            var randomX = random.Next(xLower, xUpper + 1);
            var randomY = random.Next(yLower, yUpper + 1);

            return new Vector2(randomX, randomY);
        }
    }
}