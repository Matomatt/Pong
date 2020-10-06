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

        ScreenConsole scc;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D circleTexture;
        Texture2D vectorTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            ballSpeed = new Vector2(50f, 0f);

            raquette1 = new Raquette(new Vector2(_graphics.PreferredBackBufferWidth - 64, _graphics.PreferredBackBufferHeight / 2));
            raquette2 = new Raquette(new Vector2(64, _graphics.PreferredBackBufferHeight / 2), false);

            base.Initialize();
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

            raquette1.Move(kstate.IsKeyDown(Keys.Up), kstate.IsKeyDown(Keys.Down), kstate.IsKeyDown(Keys.Left), kstate.IsKeyDown(Keys.Right), gameTime);
            raquette2.Move(kstate.IsKeyDown(Keys.Z), kstate.IsKeyDown(Keys.S), kstate.IsKeyDown(Keys.Q), kstate.IsKeyDown(Keys.D), gameTime);

            ballPosition += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;


            //       COLLISIONS       \\

            ballSpeed = raquette1.CalculateBallCollisions(new Circle(ballPosition, ballTexture.Width / 2), ballSpeed);
            ballSpeed = raquette2.CalculateBallCollisions(new Circle(ballPosition, ballTexture.Width / 2), ballSpeed);

            if (Collisions.RectangleCircle(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), ballPosition, ballTexture.Width / 2))
                ballSpeed = Collisions.CircleBounceOnRectangle(new Circle(ballPosition, ballTexture.Width / 2), new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), ballSpeed, Vector2.Zero);

            if (raquette1.Lost(ballPosition) || raquette2.Lost(ballPosition))
                lost = 1;


            //       ANIMATIONS       \\

            raquette1.Animate(gameTime);
            raquette2.Animate(gameTime);


            //       LAST UPDATES       \\

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Bisque);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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
