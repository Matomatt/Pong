using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace pong
{
    class LoadingBar
    {
        private double maxValue, currentValue;
        private Color loadingColor, maxColor;
        private RectanglePrimitive bar;

        public LoadingBar(GraphicsDevice graphicsDevice, double _maxValue, Color _loadingColor, Color _maxColor, double _startingValue = 0)
        {
            maxValue = _maxValue;
            currentValue = _startingValue;
            maxColor = _maxColor;
            loadingColor = _loadingColor;
            bar = new RectanglePrimitive(graphicsDevice, new Vector2(10, 10), Color.White, 255);
        }

        public void setValue(double value)
        {
            currentValue = value;
        }

        public void Draw(SpriteBatch _spriteBatch, float X, float Y, float height, float width)
        {
            _spriteBatch.Draw(bar.texture, new Vector2(X, Y), null, new Color((currentValue == maxValue) ? maxColor:loadingColor, 255), 0f, new Vector2(0, 0), new Vector2(width/10, (float)(height * (currentValue / maxValue))/10f), SpriteEffects.None, 0f);
        }
    }
}
