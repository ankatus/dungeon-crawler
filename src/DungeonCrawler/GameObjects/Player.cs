using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects
{
    public enum Direction { None, Up, Right, Down, Left };

    public class Player : GameObject
    {
        public override List<GameObject> Children => Projectiles.Cast<GameObject>().ToList();
        public List<Projectile> Projectiles { get; set; }
        private int _movingSpeed;
        private int _projectileSpeed;

        public Player(int x, int y) : base(GameObjectType.Player, x, y, 10, 30)
        {
            Projectiles = new List<Projectile>();
            _movingSpeed = 3;
            _projectileSpeed = 5;
        }

        public void Update(GameObject gameObjectTree)
        {
            var mousePosition = Mouse.GetState().Position;

            Turn(mousePosition, gameObjectTree);

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Move(Direction.Left, gameObjectTree);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Move(Direction.Right, gameObjectTree);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(Direction.Up, gameObjectTree);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(Direction.Down, gameObjectTree);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Shoot(mousePosition);
            }

            foreach (var projectile in Projectiles)
            {
                projectile.Update(gameObjectTree);
            }

            RemoveInactiveProjectiles();
        }

        private void Move(Direction direction, GameObject gameObjectTree)
        {
            Velocity = Vector2.Zero;
            switch (direction)
            {
                case Direction.Up:
                    Velocity = -Vector2.UnitY * _movingSpeed;
                    break;
                case Direction.Right:
                    Velocity = Vector2.UnitX * _movingSpeed;
                    break;
                case Direction.Down:
                    Velocity = Vector2.UnitY * _movingSpeed;
                    break;
                case Direction.Left:
                    Velocity = -Vector2.UnitX * _movingSpeed;
                    break;
                case Direction.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            Position += Velocity;

            // If overlapping, move back until not overlapping
            while (CheckOverlaps(gameObjectTree))
            {
                Position -= Velocity / _movingSpeed;
            }
        }

        private bool CheckOverlaps(GameObject gameObjectTree)
        {
            var overlaps = CollisionDetection.GetOverlaps(this, gameObjectTree);

            foreach (var gameObject in overlaps)
            {
                if (gameObject.Type == GameObjectType.Wall) return true;
            }

            return false;
        }

        private void Turn(Point target, GameObject gameObjectTree)
        {
            // Create vector from player to target coordinates
            var (x, y) = Vector2.Subtract(target.ToVector2(), Position);

            float previousRotation = Rotation;

            // Rotate towards target
            Rotation = (float)Math.Atan2(y, x);

            // If there is overlap after rotation, undo rotation
            if (CheckOverlaps(gameObjectTree))
            {
                Rotation = previousRotation;
            }
        }

        private void Shoot(Point targetCoordinates)
        {
            // Create vector from player to target coordinates
            Vector2 projectileTravelVector = Vector2.Subtract(targetCoordinates.ToVector2(), Position);

            Projectile projectile = new Projectile((int)Position.X, (int)Position.Y, projectileTravelVector, _projectileSpeed, this);

            Projectiles.Add(projectile);
        }

        private void RemoveInactiveProjectiles()
        {
            for (var i = 0; i < Projectiles.Count; i++)
            {
                if (Projectiles[i].State == GameObjectState.Inactive)
                {
                    Projectiles.RemoveAt(i);
                }
            }
        }
    }
}
