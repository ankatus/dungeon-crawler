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
        private readonly int _movingSpeed;
        private readonly int _projectileSpeed;

        public Player(int x, int y) : base(x, y, 10, 30)
        {
            Projectiles = new List<Projectile>();
            _movingSpeed = 3;
            _projectileSpeed = 5;
        }

        public void Update(float newFacing, List<GameObject> gameObjects)
        {
            Turn(newFacing, gameObjects);

            if (InputHandler.Inputs[InputHandler.InputName.Up].IsActivated()) Move(Direction.Up, gameObjects);

            if (InputHandler.Inputs[InputHandler.InputName.Left].IsActivated()) Move(Direction.Left, gameObjects);

            if (InputHandler.Inputs[InputHandler.InputName.Down].IsActivated()) Move(Direction.Down, gameObjects);

            if (InputHandler.Inputs[InputHandler.InputName.Right].IsActivated()) Move(Direction.Right, gameObjects);

            if (InputHandler.Inputs[InputHandler.InputName.Shoot].IsActivated()) Shoot();

            foreach (var projectile in Projectiles)
            {
                projectile.Update(gameObjects);
            }

            RemoveInactiveProjectiles();
        }

        private void Move(Direction direction, List<GameObject> gameObjects)
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
            while (CheckOverlaps(gameObjects))
            {
                Position -= Velocity / _movingSpeed;
            }
        }

        private bool CheckOverlaps(List<GameObject> gameObjects)
        {
            var overlaps = CollisionDetection.GetOverlaps(this, gameObjects);

            foreach (var gameObject in overlaps)
            {
                if (gameObject is Wall) return true;
            }

            return false;
        }

        private void Turn(float newRotation, List<GameObject> gameObjects)
        {
            float previousRotation = Rotation;

            // Rotate towards target
            Rotation = newRotation;

            // If there is overlap after rotation, undo rotation
            if (CheckOverlaps(gameObjects))
            {
                Rotation = previousRotation;
            }
        }

        private void Shoot()
        {
            // Create vector from player to target coordinates
            Vector2 projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

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
