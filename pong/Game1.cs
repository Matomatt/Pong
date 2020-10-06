using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;

namespace pong
{
    public class Game1 : Game
    {
        Texture2D ballTexture;
        Vector2 ballPosition;
        Vector2 ballSpeed;
        Raquette raquette1;
        Raquette raquette2;
        int lost = 0;
        int score1 = 0, score2 = 0;

        ScreenConsole scc;
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D circleTexture;
        Texture2D vectorTexture;

        RectanglePrimitive field;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 1200
            };

            this.Window.AllowUserResizing = true;
            this.Window.AllowAltF4 = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.ApplyChanges();

            InitBall();
            InitRaquette();

            field = new RectanglePrimitive(GraphicsDevice, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.LawnGreen, 100);

            base.Initialize();
        }

        private void InitBall(bool toTheRight = true)
        {
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            ballSpeed = new Vector2(50f * ((toTheRight)?1:-1), 0f);
        }

        private void InitRaquette()
        {
            raquette1 = new Raquette(new Vector2(_graphics.PreferredBackBufferWidth - 64, _graphics.PreferredBackBufferHeight / 2));
            raquette2 = new Raquette(new Vector2(64, _graphics.PreferredBackBufferHeight / 2), false);
        }

        private void ReinitRaquettePos()
        {
            raquette1.position = new Vector2(_graphics.PreferredBackBufferWidth - 64, _graphics.PreferredBackBufferHeight / 2);
            raquette2.position = new Vector2(64, _graphics.PreferredBackBufferHeight / 2);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ballTexture = Content.Load<Texture2D>("ball");
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            raquette1.LoadContent(Content);
            raquette2.LoadContent(Content);

            scc = new ScreenConsole(_graphics.PreferredBackBufferWidth/3, _graphics.PreferredBackBufferHeight, Content.Load<SpriteFont>("8bitFont"));

            circleTexture = Content.Load<Texture2D>("circle");
            vectorTexture = Content.Load<Texture2D>("vector");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (lost > 0)
            {
                scc.write("GAME OVER !");
                lost++;
                if (lost > 50)
                    Exit();
                return;
            }

            //       MOVEMENTS       \\

            var kstate = Keyboard.GetState();

            raquette1.Move(kstate.IsKeyDown(Keys.O), kstate.IsKeyDown(Keys.L), kstate.IsKeyDown(Keys.K), kstate.IsKeyDown(Keys.M), gameTime);
            raquette2.Move(kstate.IsKeyDown(Keys.Z), kstate.IsKeyDown(Keys.S), kstate.IsKeyDown(Keys.Q), kstate.IsKeyDown(Keys.D), gameTime);

            if (kstate.IsKeyDown(Keys.Enter))
                raquette1.Boost();
            if (kstate.IsKeyDown(Keys.Space))
                raquette2.Boost();

            ballPosition += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;


            //       COLLISIONS       \\

            ballSpeed = raquette1.CalculateBallCollisions(new Circle(ballPosition, ballTexture.Width / 2), ballSpeed);
            ballSpeed = raquette2.CalculateBallCollisions(new Circle(ballPosition, ballTexture.Width / 2), ballSpeed);

            Collision collision;
            if ((collision = Collisions.RectangleCircle(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), ballPosition, ballTexture.Width / 2)).contact)
            {
                ballSpeed = Collisions.CircleBounceOnRectangle(new Circle(ballPosition, ballTexture.Width / 2), new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), ballSpeed, Vector2.Zero);
                if (collision.normal.X != 0)
                {
                    if (collision.normal.X == 1)
                        score2++;
                    else
                        score1++;
                    InitBall(raquette1.Lost(ballPosition));
                    ReinitRaquettePos();
                    scc.clear();
                    scc.write(score2 + " : " + score1);
                    //lost = 1;
                }
            }


            //       OBJECT UPDATES       \\

            raquette1.Update(gameTime);
            raquette2.Update(gameTime);


            //       LAST UPDATES       \\

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Bisque);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawRectangle(field);

            DrawRaquette(raquette1);
            DrawRaquette(raquette2);

            _spriteBatch.Draw(ballTexture, ballPosition, null, Color.White, 0f, new Vector2(ballTexture.Width / 2, ballTexture.Height / 2), Vector2.One, SpriteEffects.None, 0f);

            //DrawCircle(raquette1.CircularBounceHitbox());
            //DrawCircle(raquette2.CircularBounceHitbox());
            //DrawVector(Collisions.CircleCircleNormal(new Circle(ballPosition, ballTexture.Width / 2), raquette1.CircularBounceHitbox())*50f, raquette1.CircularBounceHitbox().centerPosition);
            //DrawVector(Collisions.CircleCircleNormal(new Circle(ballPosition, ballTexture.Width / 2), raquette2.CircularBounceHitbox()) * 50f, raquette2.CircularBounceHitbox().centerPosition);

            //DrawVector(Collisions.CircleBounceOnRectangle(new Circle(ballPosition, ballTexture.Width / 2), raquette1.HitboxRectangle(), ballSpeed, Vector2.Zero), ballPosition);

            scc.Draw(_spriteBatch);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawRectangle(RectanglePrimitive field)
        {
            _spriteBatch.Draw(field.texture, new Vector2(0,0), null, field.color, 0f, new Vector2(0,0), new Vector2(1, 1), SpriteEffects.None, 0f);
        }

        private void DrawCircle(Circle circle)
        {
            _spriteBatch.Draw(circleTexture, circle.centerPosition, null, Color.White, 0f, new Vector2(circleTexture.Width / 2, circleTexture.Height / 2), Vector2.One * circle.radius*2/circleTexture.Height, SpriteEffects.None, 0f);
        }

        private void DrawVector(Vector2 vector, Vector2 startingPos)
        {
            _spriteBatch.Draw(vectorTexture, startingPos, null, Color.White, (float)Math.Atan2(vector.Y, vector.X), new Vector2(0, vectorTexture.Height / 2), new Vector2(vector.Length() / vectorTexture.Width /2, 0.8f), SpriteEffects.None, 0f);
        }

        private void DrawRaquette(Raquette raquette)
        {
            _spriteBatch.Draw(raquette.Texture(), raquette.position, null, Color.White, (float)((raquette.direction)?0:Math.PI), new Vector2(raquette.Texture().Width / 2, raquette.Texture().Height / 2), new Vector2(1, 1), SpriteEffects.None, 0f);
            //_spriteBatch.Draw(raquette.hitbox.Texture(GraphicsDevice), raquette.position, null, raquette.hitbox.color, 0f, raquette.hitbox.size / 2, new Vector2(1, 1), SpriteEffects.None, 0f);
        }
    }
}
