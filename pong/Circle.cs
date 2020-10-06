using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace pong
{
    class Circle
    {
        public Vector2 centerPosition;
        public float radius;

        public Circle(Vector2 _centerPosition, float _radius)
        {
            centerPosition = _centerPosition;
            radius = _radius;
        }
    }
}
