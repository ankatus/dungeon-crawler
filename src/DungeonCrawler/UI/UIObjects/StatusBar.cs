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

        public StatusBar(Vector2 position, int width, int height) : base(position, width, height)
        {
            _gunIndicators = new List<TextBlock>();
            Build();
        }

        public void Update(Player player)
        {
            _hpIndicator.Text = player.CurrentHealth.ToString(CultureInfo.InvariantCulture);

            // Gun indicators
            if (_gunIndicators.Count == player.Guns.Count) return;

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
            var bar = new TextBlock("100", position, 0, 0);
            Children.Add(bar);
            _hpIndicator = bar;

            // Guns label
            position = hpLabel.Position;
            position.X += 5;
            position.Y += 30;
            var gunsLabel = new TextBlock("Guns:", position, 0, 0);
            Children.Add(gunsLabel);

            position = gunsLabel.Position;
            position.X += 30;
            _gunIndicatorsStart = position;
        }
    }
}
