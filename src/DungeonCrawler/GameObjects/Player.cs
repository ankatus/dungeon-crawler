using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using DungeonCrawler.Guns;
using DungeonCrawler.Maps;

namespace DungeonCrawler.GameObjects
{
    public enum Direction { None, Up, Right, Down, Left };

    public class Player : GameObject
    {
        public List<Gun> Guns { get; }
        public Gun ActiveGun { get { return Guns[activeGunIndex]; } }
        public float MaxHealth { get; }
        public float CurrentHealth { get; set; }
        private readonly int _movingSpeed;
        private readonly GameMap _map;
        private int activeGunIndex;

        public Player(GameMap map, int x, int y) : base(x, y, 10, 30)
        {
            _map = map;
            _movingSpeed = 3;
            MaxHealth = 50;
            CurrentHealth = MaxHealth;
            var defaultGun = new DefaultGun(Id);
            Guns = new List<Gun>();
            Guns.Add(defaultGun);
            activeGunIndex = 0;
        }

        public void Update(float newFacing)
        {
            Turn(newFacing);

            if (InputHandler.Inputs[InputHandler.InputName.Up].IsActivated()) Move(Direction.Up);

            if (InputHandler.Inputs[InputHandler.InputName.Left].IsActivated()) Move(Direction.Left);

            if (InputHandler.Inputs[InputHandler.InputName.Down].IsActivated()) Move(Direction.Down);

            if (InputHandler.Inputs[InputHandler.InputName.Right].IsActivated()) Move(Direction.Right);

            if (InputHandler.Inputs[InputHandler.InputName.Shoot].IsActivated()) Shoot();

            if (InputHandler.Inputs[InputHandler.InputName.ChangeWeapon1].IsActivated()) ChangeWeapon(0);

            if (InputHandler.Inputs[InputHandler.InputName.ChangeWeapon2].IsActivated()) ChangeWeapon(1);

            if (InputHandler.Inputs[InputHandler.InputName.ChangeWeapon3].IsActivated()) ChangeWeapon(2);
        }

        private void ChangeWeapon(int gunIndex)
        {
            if (gunIndex < Guns.Count)
            {
                activeGunIndex = gunIndex;
            }
        }

        private void Move(Direction direction)
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
            while (CheckCollision())
            {
                Position -= Velocity / _movingSpeed;
            }
        }

        private bool CheckCollision()
        {
            var gameObjects = new List<GameObject>();
            gameObjects.AddRange(_map.CurrentRoom.Walls);

            foreach (var door in _map.CurrentRoom.Doors)
            {
                // Check collision only with closed doors
                if (door.Closed)
                {
                    gameObjects.Add(door);
                }
            }

            return CollisionDetection.GetOverlaps(this, gameObjects).Count > 0;
        }

        private void Turn(float newRotation)
        {
            var previousRotation = Rotation;

            // Rotate towards target
            Rotation = newRotation;

            // If there is overlap after rotation, undo rotation
            if (CheckCollision())
            {
                Rotation = previousRotation;
            }
        }

        private void Shoot()
        {
            // Create vector from player to target coordinates
            var projectileTravelVector = CollisionDetection.RotateVector(Vector2.UnitX, Rotation);

            _map.CurrentRoom.Projectiles.AddRange(ActiveGun.Shoot(Position, projectileTravelVector));
        }

        public void ProjectileCollision(Projectile projectile)
        {
            if (projectile.SourceId == Id) return;

            CurrentHealth -= projectile.Damage;

            if (CurrentHealth <= 0)
            {
                State = GameObjectState.Inactive;
            }
        }
    }
}
