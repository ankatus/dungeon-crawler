using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DungeonCrawler
{
    public static class InputHandler
    {
        public enum InputName
        {
            None,
            Up,
            Left,
            Down,
            Right,
            Shoot,
            ChangeWeapon1,
            ChangeWeapon2,
            ChangeWeapon3
        };

        public enum MouseButton
        {
            None,
            LeftButton,
            RightButton
        };

        public class Input
        {
            private readonly Keys _key;
            private readonly MouseButton _mouseButton;

            public Input(Keys key)
            {
                _key = key;
                _mouseButton = MouseButton.None;
            }

            public Input(MouseButton button)
            {
                _mouseButton = button;
                _key = Keys.None;
            }

            public bool IsActivated()
            {
                if (_key != Keys.None)
                {
                    return Keyboard.GetState().IsKeyDown(_key);
                }

                return _mouseButton switch
                {
                    MouseButton.LeftButton => Mouse.GetState().LeftButton == ButtonState.Pressed,
                    MouseButton.RightButton => Mouse.GetState().RightButton == ButtonState.Pressed,
                    _ => false
                };
            }
        }

        public static readonly Dictionary<InputName, Input> Inputs;

        static InputHandler()
        {
            Inputs = new Dictionary<InputName, Input>
            {
                {InputName.Up, new Input(Keys.W)},
                {InputName.Left, new Input(Keys.A)},
                {InputName.Down, new Input(Keys.S)},
                {InputName.Right, new Input(Keys.D)},
                {InputName.Shoot, new Input(MouseButton.LeftButton)},
                {InputName.ChangeWeapon1, new Input(Keys.D1)},
                {InputName.ChangeWeapon2, new Input(Keys.D2)},
                {InputName.ChangeWeapon3, new Input(Keys.D3)}
            };
        }
    }
}