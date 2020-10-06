using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace pong
{
    class RectanglePrimitive
    {
        Color[] data;

        public Vector2 size;
        public Color color;
        int transparency;

        public RectanglePrimitive(Vector2 _size, Color _color, int _transparency = 255)
        {
            size = _size;
            color = _color;
            transparency = _transparency;

            data = new Color[(int)(size.X * size.Y)];
            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(color, transparency);
        }

        public Texture2D Texture(GraphicsDevice GraphicsDevice)
        {
            Texture2D rectTexture = new Texture2D(GraphicsDevice, (int)size.X, (int)size.Y);
            rectTexture.SetData(data);
            return rectTexture;
        }
    }
}
