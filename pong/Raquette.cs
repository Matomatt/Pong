using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace pong
{
    class Raquette
    {
        readonly Texture2D[] sprites = new Texture2D[3];
        int actualSprite = 0;
        bool isAnimating = false;
        bool animateOnce = false;
        readonly int FPS;
        double countDown;

        public Vector2 position;
        public Vector2 baseSpeed;
        private Vector2 originalBaseSpeed;
        public Vector2 speed;
        public float catchUpSpeed;

        public bool direction;
        private bool colliding = false;
        private const double COLLIDINGCOOLDOWNTIME = 0.5;
        private double collidingCooldown = 0;

        public RectanglePrimitive hitbox;

        private const double EFFECTDURATION = 0.2;
        public double COOLDOWNTIME = 6;
        private const float BOOSTSPEEDRATIO = 2f;
        private double effectDurationCooldown = 0;
        public double coolDown = 0;

        private const double TICKLENGTH = 2;
        private double tickTime = 0;

        private const float BALLMAXPSPEED = 1200;

        public Raquette(Vector2 pos, bool _direction = true, float _baseSpeed = 600f, float _catchUpSpeed = 0.3f, int _FPS = 12)
        {
            position = pos;
            direction = _direction;
            baseSpeed = new Vector2(_baseSpeed, _baseSpeed);
            originalBaseSpeed = baseSpeed;
            speed = Vector2.Zero;
            catchUpSpeed = _catchUpSpeed;
            FPS = _FPS;
            ResetCountdown();
        }

        private void ResetCountdown()
        {
            countDown = 1.0 / (double)FPS;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            for(int i = 0; i< sprites.Length; i++)
                sprites[i] = Content.Load<Texture2D>("raquette"+ (i+1).ToString());

            hitbox = new RectanglePrimitive(new Vector2(sprites[0].Width/4, sprites[0].Height*7/8), Color.Red, 50);
        }

        internal Texture2D Texture()
        {
            return sprites[actualSprite];
        }

        public bool StartAnimation(bool _animateOnce = false)
        {
            if (!isAnimating)
                isAnimating = true;
            else
                return false;

            animateOnce = _animateOnce;

            return isAnimating;
        }

        public void Update(GameTime gameTime)
        {
            Animate(gameTime);

            if (collidingCooldown > 0) collidingCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
            if (collidingCooldown < 0)
            {
                collidingCooldown = 0;
                if (colliding) colliding = false;
            }

            if (coolDown > 0)
            {
                coolDown -= gameTime.ElapsedGameTime.TotalSeconds;

                if (effectDurationCooldown > 0)
                    effectDurationCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                if (effectDurationCooldown < 0)
                {
                    effectDurationCooldown = 0;
                    baseSpeed = originalBaseSpeed;
                }
            }
            if (coolDown < 0) coolDown = 0;
            
            
            tickTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (tickTime>TICKLENGTH)
            {
                tickTime = 0;
                //baseSpeed.X = (float)((-4 * Math.Pow(baseSpeed.X, 3) * Math.Pow(10, -7)) + 5 * Math.Pow(baseSpeed.X, 2) * Math.Pow(10, -4) + 0.87 * baseSpeed.X + 20);
                //baseSpeed.Y = (float)((-4 * Math.Pow(baseSpeed.Y, 3) * Math.Pow(10, -7)) + 5 * Math.Pow(baseSpeed.Y, 2) * Math.Pow(10, -4) + 0.87 * baseSpeed.Y + 20);
            }

        }

        public void Animate(GameTime gameTime)
        {
            if (isAnimating)
                countDown -= gameTime.ElapsedGameTime.TotalSeconds;

            if (isAnimating && countDown <= 0)
            {
                actualSprite = (actualSprite + 1) % sprites.Length;
                ResetCountdown();
                if (animateOnce && actualSprite == 0)
                    isAnimating = false;
            }
        }

        public void Move(bool keyUp, bool keyDown, bool keyLeft, bool keyRight, GameTime gameTime)
        {
            Vector2 speedGoal = Vector2.Zero;
            if (keyUp) speedGoal.Y -= baseSpeed.Y;
            if (keyDown) speedGoal.Y += baseSpeed.Y;
            if (keyLeft) speedGoal.X -= baseSpeed.X;
            if (keyRight) speedGoal.X += baseSpeed.X;

            speed = Vector2.Lerp(speed, speedGoal, catchUpSpeed);

            if (Vector2.Distance(speed, speedGoal) < 1)
                speed = speedGoal;

            position += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Boost()
        {
            if (coolDown == 0)
            {
                baseSpeed *= BOOSTSPEEDRATIO;
                speed = baseSpeed * Vector2.Normalize(speed);
                coolDown = COOLDOWNTIME;
                effectDurationCooldown = EFFECTDURATION;
            }
        }

        public Rectangle HitboxRectangle()
        {
            Point p = new Point((int)(position.X - hitbox.size.X / 2), (int)(position.Y - hitbox.size.Y / 2));
            return new Rectangle(p, hitbox.size.ToPoint());
        }

        public Vector2 CalculateBallCollisions(Circle ball, Vector2 ballSpeed, Vector2 exBallPosition)
        {
            if (Collisions.RectangleCircle(HitboxRectangle(), ball.centerPosition, ball.radius).contact)
            {
                
                if (!colliding)
                {
                    colliding = true;
                    collidingCooldown = COLLIDINGCOOLDOWNTIME;

                    if (ball.centerPosition.X < position.X - hitbox.size.X / 2 && direction || ball.centerPosition.X > position.X + hitbox.size.X / 2 && !direction)
                        ballSpeed = Collisions.CircleBounceOnCircle(ball, CircularBounceHitbox(), ballSpeed, speed);
                    else
                        ballSpeed = Collisions.CircleBounceOnRectangle(new Circle(exBallPosition, ball.radius), HitboxRectangle(), ballSpeed, speed);
                    
                    float sp = Vector2.Distance(new Vector2(0, 0), ballSpeed);
                    if (sp > BALLMAXPSPEED) sp = BALLMAXPSPEED;
                    ballSpeed = Vector2.Normalize(ballSpeed) * sp;//(float)((-4 * Math.Pow(sp, 3) * Math.Pow(10, -7)) + 5 * Math.Pow(sp, 2) * Math.Pow(10, -4) + 0.87 * sp + 20);
                }
                     
            }
            else if (colliding)
            {
                StartAnimation(true);
                colliding = false;
            }

            return ballSpeed;
        }

        public Circle CircularBounceHitbox()
        {
            return new Circle(new Vector2(position.X + hitbox.size.X * 3f * ((direction)?1f:-1f), position.Y), hitbox.size.X * 3);
        }

        public bool Lost(Vector2 ballPos)
        {
            return (ballPos.X > position.X+sprites[0].Width) && direction || (ballPos.X < position.X-sprites[0].Width) && !direction;
        }

        public void Reset()
        {
            coolDown = 0;
            effectDurationCooldown = 0;
            tickTime = 0;
            actualSprite = 0;
            ResetCountdown();
            baseSpeed = originalBaseSpeed;
            speed = Vector2.Zero;
        }
    }
}
