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
    public enum Direction { Up, Right, Down, Left };

    public class Player : GameObject
    {
        public override List<GameObject> Children
        {
            get => Projectiles.Cast<GameObject>().ToList();
        }
        public List<Projectile> Projectiles { get; set; }
        public int MovingSpeed { get; set; }
        public int ShootingSpeed { get; set; }

        public Player(int x, int y) : base(GameObjectType.Player, x, y, 10, 30)
        {
            Projectiles = new List<Projectile>();
            MovingSpeed = 5;
            ShootingSpeed = 1;
        }

        public void Update(GameObject gameObjectTree)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Move(Direction.Left);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Move(Direction.Right);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(Direction.Up);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(Direction.Down);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Shoot(Mouse.GetState().Position);  
            }

            foreach (Projectile projectile in Projectiles)
            {
                projectile.Update();
            }
        }

        private void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    Position.Y -= MovingSpeed;
                    break;
                case Direction.Right:
                    Position.X += MovingSpeed;
                    break;
                case Direction.Down:
                    Position.Y += MovingSpeed;
                    break;
                case Direction.Left:
                    Position.X -= MovingSpeed;
                    break;
            }
        }

        private void Shoot(Point targetCoordinates)
        {
            // Create vector from player to target coordinates
            Vector2 projectileTravelVector = Vector2.Subtract(targetCoordinates.ToVector2(), Position);

            // Rotate player texture towards projectile target
            Rotation = (float)((float)Math.Atan2(projectileTravelVector.Y, projectileTravelVector.X));

            Projectile projectile = new Projectile((int)Position.X, (int)Position.Y, projectileTravelVector, ShootingSpeed);

            Projectiles.Add(projectile);
        }
    }
}
