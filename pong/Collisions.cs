using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace pong
{
    static class Collisions
    {
        public static Collision RectangleCircle(Rectangle rect, Vector2 centerPosition, float radius)
        {
            //Inside
            if (centerPosition.Y - radius < rect.Y + rect.Height && centerPosition.Y + radius > rect.Y && centerPosition.X + radius > rect.X && centerPosition.X - radius < rect.X + rect.Width)
            {
                float cx = (centerPosition.X - rect.X < rect.X + rect.Width - centerPosition.X) ? centerPosition.X - rect.X : rect.X + rect.Width - centerPosition.X;
                float cy = (centerPosition.Y - rect.Y < rect.Y + rect.Height - centerPosition.Y) ? centerPosition.Y - rect.Y : rect.Y + rect.Height - centerPosition.Y;

                if (cx > radius && cy > radius)
                    return new Collision(false, RectangleCircleNormal(new Circle(centerPosition, radius), rect));
                //if (rect.X != 0)
                //    System.Diagnostics.Debug.WriteLine("INSIDE");

                return new Collision(true, RectangleCircleNormal(new Circle(centerPosition, radius), rect));
            }

            //Outside

            if (rect.X > centerPosition.X + radius || rect.X + rect.Width < centerPosition.X - radius ||
                rect.Y > centerPosition.Y + radius || rect.Y + rect.Height < centerPosition.Y - radius)
                return new Collision(false, RectangleCircleNormal(new Circle(centerPosition, radius), rect));
            
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
                return new Collision(false, RectangleCircleNormal(new Circle(centerPosition, radius), rect));

            return new Collision(true, RectangleCircleNormal(new Circle(centerPosition, radius), rect));
        }

        public static Vector2 RectangleCircleNormal(Circle circle, Rectangle rect, bool aff = false)
        {
            Vector2 centerPosition = circle.centerPosition;
            float radius = circle.radius;

            //Inside
            //if (centerPosition.Y + radius < rect.Y + rect.Height && centerPosition.Y - radius > rect.Y && centerPosition.X + radius > rect.X && centerPosition.X - radius < rect.X + rect.Width)
            if (centerPosition.Y < rect.Y + rect.Height && centerPosition.Y > rect.Y && centerPosition.X > rect.X && centerPosition.X < rect.X + rect.Width)
            {
                float xDir = (centerPosition.X - rect.X < rect.X + rect.Width - centerPosition.X) ? 1 : -1;
                float cx = (xDir == 1) ? centerPosition.X - rect.X : rect.X + rect.Width - centerPosition.X;
                float yDir = (centerPosition.Y - rect.Y < rect.Y + rect.Height - centerPosition.Y) ? 1 : -1;
                float cy = (yDir == 1) ? centerPosition.Y - rect.Y : rect.Y + rect.Height - centerPosition.Y;

                return new Vector2((cx < cy) ? xDir : 0, (cx < cy) ? 0 : yDir);
            }

            //Outside

            if (centerPosition.Y > rect.Y && centerPosition.Y < rect.Y + rect.Height)
            {
                if (aff)
                    System.Diagnostics.Debug.WriteLine("HAI");
                if (aff)
                    System.Diagnostics.Debug.WriteLine(centerPosition.X - radius < rect.X);
                return new Vector2((centerPosition.X - radius < rect.X) ? -1 : 1, 0);
                
            }
            if (aff)
                System.Diagnostics.Debug.WriteLine("YAA");
            if (centerPosition.X > rect.X && centerPosition.X < rect.X + rect.Width)
                return new Vector2(0, (centerPosition.Y < rect.Y+rect.Height/2) ? -1 : 1);

            if (centerPosition.X < rect.X && centerPosition.Y < rect.Y)
                return Vector2.Normalize(new Vector2(centerPosition.X - rect.X, centerPosition.Y - rect.Y));

            if (centerPosition.X > rect.X + rect.Width && centerPosition.Y < rect.Y)
                return Vector2.Normalize(new Vector2(centerPosition.X - rect.X - rect.Width, centerPosition.Y - rect.Y));

            if (centerPosition.X > rect.X + rect.Width && centerPosition.Y > rect.Y + rect.Height)
                return Vector2.Normalize(new Vector2(centerPosition.X - rect.X - rect.Width, centerPosition.Y - rect.Y - rect.Height));

            if (centerPosition.X < rect.X && centerPosition.Y > rect.Y + rect.Height)
                return Vector2.Normalize(new Vector2(centerPosition.X - rect.X, centerPosition.Y - rect.Y - rect.Height));

            //Vector2 centerToCenter = new Vector2(centerPosition.X - (rect.X + rect.Width / 2), centerPosition.Y - (rect.Y + rect.Height / 2));

            
            return new Vector2(0, 0);
        }
        
        public static Vector2 changeDirectionOfVectTo(Vector2 from, Vector2 to)
        {
            //System.Diagnostics.Debug.WriteLine(from);

            if (to.X != 0)
                from.X = Math.Abs(from.X) * (to.X / Math.Abs(to.X));
            if (to.Y != 0)
                from.Y = Math.Abs(from.Y) * (to.Y / Math.Abs(to.Y));

            //System.Diagnostics.Debug.WriteLine("=> " + from);
            return from;
        }

        public static Vector2 CircleCircleNormal(Circle circle1, Circle circle2)
        {
            Vector2 normal = Vector2.Normalize(new Vector2(circle1.centerPosition.X - circle2.centerPosition.X, circle1.centerPosition.Y - circle2.centerPosition.Y));
            //inside
            //if (Vector2.Distance(circle1.centerPosition, circle2.centerPosition) < circle2.radius) return normal*-1f;
            //Outside
            return normal;
        }

        private static Vector2 PushedBySpeed(Vector2 receivingSpeed, Vector2 pushingForce, Vector2 normal)
        {
            //System.Diagnostics.Debug.WriteLine(receivingSpeed + " + " + pushingForce.X * Math.Abs(normal.X) + "," + pushingForce.Y * Math.Abs(normal.Y));
            //normal *= -1;
            
            return receivingSpeed + new Vector2(pushingForce.X*Math.Abs(normal.X), pushingForce.Y*Math.Abs(normal.Y));
            //return receivingSpeed;
        }

        public static Vector2 CircleBounceOnRectangle(Circle circle, Rectangle rect, Vector2 circleSpeed, Vector2 rectangleSpeed)
        {
            Vector2 normal = RectangleCircleNormal(circle, rect);
            Vector2 refractedSpeed = changeDirectionOfVectTo((circleSpeed - 2 * Vector2.Dot(normal, circleSpeed) * normal), normal);
            //System.Diagnostics.Debug.WriteLine("r : " + rectangleSpeed);
            return PushedBySpeed(refractedSpeed, rectangleSpeed, normal);
        }       

        public static Vector2 CircleBounceOnCircle(Circle circle1, Circle circle2, Vector2 circle1Speed, Vector2 circle2Speed)
        {
            Vector2 normal = CircleCircleNormal(circle1, circle2);
            Vector2 refractedSpeed = changeDirectionOfVectTo((circle1Speed - 2 * Vector2.Dot(normal, circle1Speed) * normal), normal);
            
            return PushedBySpeed(refractedSpeed, circle2Speed, normal);
        }
    }
}
