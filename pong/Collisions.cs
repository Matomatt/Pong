using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace pong
{
    static class Collisions
    {
        public static bool RectangleCircle(Rectangle rect, Vector2 centerPosition, float radius)
        {
            //Inside
            if (centerPosition.Y < rect.Y + rect.Height && centerPosition.Y > rect.Y && centerPosition.X > rect.X && centerPosition.X < rect.X + rect.Width)
            {
                float cx = (centerPosition.X - rect.X < rect.X + rect.Width - centerPosition.X) ? centerPosition.X - rect.X : rect.X + rect.Width - centerPosition.X;
                float cy = (centerPosition.Y - rect.Y < rect.Y + rect.Height - centerPosition.Y) ? centerPosition.Y - rect.Y : rect.Y + rect.Height - centerPosition.Y;

                if (cx > radius && cy > radius)
                    return false;
                return true;
            }

            //Outside

            if (rect.X > centerPosition.X + radius || rect.X + rect.Width < centerPosition.X - radius ||
                rect.Y > centerPosition.Y + radius || rect.Y + rect.Height < centerPosition.Y - radius)
                return false;

            

            if (
                centerPosition.X < rect.X && centerPosition.Y < rect.Y &&
                Vector2.Distance(centerPosition, new Vector2(rect.X, rect.Y)) > radius ||

                centerPosition.X > rect.X + rect.Width && centerPosition.Y < rect.Y &&
                Vector2.Distance(centerPosition, new Vector2(rect.X+rect.Width, rect.Y)) > radius ||

                centerPosition.X > rect.X + rect.Width && centerPosition.Y > rect.Y + rect.Height &&
                Vector2.Distance(centerPosition, new Vector2(rect.X + rect.Width, rect.Y + rect.Height)) > radius ||

                centerPosition.X < rect.X && centerPosition.Y + rect.Height < rect.Y &&
                Vector2.Distance(centerPosition, new Vector2(rect.X, rect.Y + rect.Height)) > radius
               )
                return false;

            return true;
        }

        public static Vector2 RectangleCircleNormal(Circle circle, Rectangle rect)
        {
            Vector2 centerPosition = circle.centerPosition;

            //Inside
            if (centerPosition.Y < rect.Y + rect.Height && centerPosition.Y > rect.Y && centerPosition.X > rect.X && centerPosition.X < rect.X + rect.Width)
            {
                float cx = (centerPosition.X - rect.X < rect.X + rect.Width - centerPosition.X) ? centerPosition.X - rect.X : rect.X + rect.Width - centerPosition.X;
                float cy = (centerPosition.Y - rect.Y < rect.Y + rect.Height - centerPosition.Y) ? centerPosition.Y - rect.Y : rect.Y + rect.Height - centerPosition.Y;

                return new Vector2((cx < cy) ? -1 : 0, (cx < cy) ? 0 : -1);
            }

            //Outside

            if (centerPosition.Y > rect.Y && centerPosition.Y < rect.Y + rect.Height)
                return new Vector2((centerPosition.X < rect.X)?-1:1, 0);
            if (centerPosition.X > rect.X && centerPosition.X < rect.X + rect.Width)
                return new Vector2(0, (centerPosition.Y < rect.Y) ? -1 : 1);

            if (centerPosition.X < rect.X && centerPosition.Y < rect.Y)
                return Vector2.Normalize(new Vector2(centerPosition.X - rect.X, centerPosition.Y - rect.Y));

            if (centerPosition.X > rect.X + rect.Width && centerPosition.Y < rect.Y)
                return Vector2.Normalize(new Vector2(rect.X + rect.Width - centerPosition.X, rect.Y - centerPosition.Y));

            if (centerPosition.X > rect.X + rect.Width && centerPosition.Y > rect.Y + rect.Height)
                return Vector2.Normalize(new Vector2(rect.X + rect.Width - centerPosition.X, rect.Y + rect.Height - centerPosition.Y));

            if (centerPosition.X < rect.X && centerPosition.Y > rect.Y + rect.Height)
                return Vector2.Normalize(new Vector2(centerPosition.X - rect.X, centerPosition.Y - rect.Y - rect.Height));

            return new Vector2(0, 0);
        }

        public static Vector2 CircleCircleNormal(Circle circle1, Circle circle2)
        {
            Vector2 normal = Vector2.Normalize(new Vector2(circle2.centerPosition.X - circle1.centerPosition.X, circle2.centerPosition.Y - circle1.centerPosition.Y));
            //inside
            if (Vector2.Distance(circle1.centerPosition, circle2.centerPosition) < circle2.radius) return normal*-1f;
            //Outside
            return normal;
        }

        private static Vector2 PushedBySpeed(Vector2 receivingSpeed, Vector2 pushingForce, Vector2 normal)
        {
            normal *= -1;
            return receivingSpeed + new Vector2(pushingForce.X*normal.X, pushingForce.Y*normal.Y);
        }

        public static Vector2 CircleBounceOnRectangle(Circle circle, Rectangle rect, Vector2 circleSpeed, Vector2 rectangleSpeed)
        {
            Vector2 angleVect = RectangleCircleNormal(circle, rect);
            return PushedBySpeed((circleSpeed - 2 * Vector2.Dot(angleVect, circleSpeed) * angleVect), rectangleSpeed, angleVect);
        }       

        public static Vector2 CircleBounceOnCircle(Circle circle1, Circle circle2, Vector2 circle1Speed, Vector2 circle2Speed)
        {
            Vector2 angleVect = CircleCircleNormal(circle1, circle2);
            return PushedBySpeed((circle1Speed - 2 * Vector2.Dot(angleVect, circle1Speed) * angleVect), circle2Speed, angleVect);
        }
    }
}
