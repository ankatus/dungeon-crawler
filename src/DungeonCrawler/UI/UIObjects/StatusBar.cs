using System.Collections.Generic;
using System.Globalization;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.UI.UIObjects
{
    public class StatusBar : UIObject
    {
        private Vector2 _gunIndicatorsStart;
        private readonly List<TextBlock> _gunIndicators;
        private TextBlock _hpIndicator;
        private TextBlock _ammoIndicator;

        public StatusBar(Vector2 position, int width, int height) : base(position, width, height)
        {
            _gunIndicators = new List<TextBlock>();
            Build();
        }

        public void Update(Player player)
        {
            _hpIndicator.Text = player.CurrentHealth.ToString(CultureInfo.InvariantCulture);

            // Gun indicators
            // Add indicators if necessary
            if (_gunIndicators.Count != player.Guns.Count)
            {
                var startPos = _gunIndicatorsStart;
                startPos.X += _gunIndicators.Count * 15;

                for (var index = _gunIndicators.Count; index < player.Guns.Count; index++)
                {
                    var position = startPos;
                    position.X += index * 15;
                    var indicator = new TextBlock((index + 1).ToString(), position, 0, 0);
                    Children.Add(indicator);
                    _gunIndicators.Add(indicator);
                }
            }

            // Change color of indicator corresponding to equipped gun
            for (var i = 0; i < _gunIndicators.Count; i++)
            {
                if (player.Guns[i].GetType() == player.ActiveGun.GetType())
                {
                    _gunIndicators[i].Color = TextColor.Blue;
                }
                else
                {
                    _gunIndicators[i].Color = TextColor.Red;
                }
            }
            

            // Ammo indicator
            var current = player.ActiveGun.Ammo;
            var max = player.ActiveGun.MaxAmmo;
            _ammoIndicator.Text = $"{current}/{max}";
        }

        private void Build()
        {
            // Health label
            var position = Position;
            position.X -= Width / 2 - 10;
            position.Y -= 60;
            var hpLabel = new TextBlock("HP:", position, 0, 0);
            Children.Add(hpLabel);

            // Health indicator
            position = hpLabel.Position;
            position.X += 40;
            var hp = new TextBlock("100", position, 0, 0);
            Children.Add(hp);
            _hpIndicator = hp;

            // Guns label
            position = hpLabel.Position;
            position.X += 5;
            position.Y += 30;
            var gunsLabel = new TextBlock("Guns:", position, 0, 0);
            Children.Add(gunsLabel);

            position = gunsLabel.Position;
            position.X += 30;
            _gunIndicatorsStart = position;

            // Ammo label
            position = gunsLabel.Position;
            position.Y += 30;
            var ammoLabel = new TextBlock("Ammo:", position, 0, 0);
            Children.Add(ammoLabel);

            // Ammo indicator
            position = ammoLabel.Position;
            position.X += 40;
            var ammo = new TextBlock("", position, 0, 0);
            Children.Add(ammo);
            _ammoIndicator = ammo;
        }
    }
}
