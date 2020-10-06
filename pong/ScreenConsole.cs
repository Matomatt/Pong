using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace pong
{
    class ScreenConsole
    {
        readonly List<string> log = new List<string>();
        private readonly int width;
        private readonly int height;
        readonly SpriteFont font;

        public ScreenConsole(int _width, int _height, SpriteFont _font)
        {
            width = _width;
            height = _height;
            font = _font;
        }

        public void write(string text)
        {
            log.Add(text);
            System.Diagnostics.Debug.WriteLine(text);
        }

        internal void Draw(SpriteBatch _spriteBatch)
        {
            float totH = 0;
            for (int i=0; i<log.Count; i++)
            {
                float llength = font.MeasureString(log[i]).X;
                if (llength > width)
                    log[i] = log[i].Substring(0, (int)((float)log[i].Length*((float)width/ llength)))+"\n"+ log[i].Substring((int)((float)log[i].Length * ((float)width / llength)));

                totH += font.MeasureString(log[i]).Y;
            }

            int y = 0;
            for (int i = 0; i < log.Count; i++)
            {
                _spriteBatch.DrawString(font, log[i], new Vector2(0, y + ((height < totH) ? (height - totH) : 0)), Color.DarkSlateBlue);
                y += (int)font.MeasureString(log[i]).Y;
            }
        }

        internal void clear()
        {
            log.Clear();
        }
    }
}
