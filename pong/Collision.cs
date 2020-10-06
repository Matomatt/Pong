using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pong
{
    class Collision
    {
        public bool contact;
        public Vector2 normal;

        public Collision(bool _contact, Vector2 _normal)
        {
            contact = _contact;
            normal = _normal;
        }
    }
}
