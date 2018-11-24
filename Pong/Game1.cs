using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong.Desktop
{

    public struct GameObject {
        public Vector2 position;
        public Vector2 velocity;
        public Texture2D texture;
    }

    class Utilities
    {
        public static bool atLeftBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics)
        {
            return coordinates.X <= texture.Width / 2;
        }
        public static bool atRightBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics)
        {
            return coordinates.X >= graphics.PreferredBackBufferWidth - texture.Width / 2;
        }
        public static bool atBottomBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics)
        {
            return coordinates.Y <= texture.Height / 2;
        }
        public static bool atTopBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics)
        {
            return coordinates.Y >= graphics.PreferredBackBufferHeight - texture.Height / 2;
        }
        public static float getSpeedMultiplier(System.Random random)
        {
            // Generates a random number between .5 and 2.0 to double or halve speed
            // TODO: Make the number of > and < 1 multipliers the same
            return (float)random.Next(5, 21) / 10;
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GameObject ball;
        GameObject leftPaddle;
        GameObject rightPaddle;
        System.Random random;
        const float MAX_BALL_SPEED = 50f;
        const float MIN_SPEED = 500f;
        const float PADDLE_SPEED = 100f;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ball.position = new Vector2(
                graphics.PreferredBackBufferWidth / 2,
                graphics.PreferredBackBufferHeight / 2
            );
            ball.velocity = new Vector2(
                200f,
                160f
            );

            leftPaddle.position = new Vector2(
                32 + 16, // 32 + half the texture width
                graphics.PreferredBackBufferHeight / 2
            );
            leftPaddle.velocity = new Vector2(0,0);

            rightPaddle.position = new Vector2(
                graphics.PreferredBackBufferWidth - (32 + 16), // 32 + half the texture width
                graphics.PreferredBackBufferHeight / 2
            );

            rightPaddle.velocity = new Vector2(0,0);
            random = new System.Random();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ball.texture = Content.Load<Texture2D>("ball");
            leftPaddle.texture = Content.Load<Texture2D>("paddle");
            rightPaddle.texture = Content.Load<Texture2D>("paddle");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private float getLeftPaddlePositionYDiff(float paddleSpeed, GameTime gameTime){
            float yPosDiff = 0;
            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Up))
                yPosDiff -= paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if(kstate.IsKeyDown(Keys.Down))
                yPosDiff +=  paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            return yPosDiff;
        }
        private float getRightPaddlePositionY(){
            return ball.position.Y;
        }
        private void boundRightPaddle(){
            //rightPaddle.position.Y = MathHelper.Clamp(rightPaddle.position.Y, rightPaddle.texture.Height / 2, graphics.PreferredBackBufferHeight - (rightPaddle.texture.Height / 2));
        }
        private float getBoundY(GameObject gameObject){
            return MathHelper.Clamp(gameObject.position.Y, gameObject.texture.Height / 2, graphics.PreferredBackBufferHeight - (gameObject.texture.Height / 2));
        }

        private void moveAndBoundBall(GameTime gameTime){
            ball.position.X += ball.velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ball.position.Y += ball.velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Utilities.atLeftBorder(ball.position, ball.texture, graphics) || Utilities.atRightBorder(ball.position, ball.texture, graphics)) {
                ball.velocity.X *= -1 * Utilities.getSpeedMultiplier(random);
                ball.position.X = MathHelper.Clamp(ball.position.X, ball.texture.Width / 2, graphics.PreferredBackBufferWidth - (ball.texture.Width / 2));
            }

            if (Utilities.atTopBorder(ball.position, ball.texture, graphics) || Utilities.atBottomBorder(ball.position, ball.texture, graphics)) {
                ball.velocity.Y *= -1 * Utilities.getSpeedMultiplier(random);
                ball.position.Y = MathHelper.Clamp(ball.position.Y, ball.texture.Height / 2, graphics.PreferredBackBufferHeight - (ball.texture.Height / 2));
            }
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            moveAndBoundBall(gameTime);
            leftPaddle.position.Y += getLeftPaddlePositionYDiff(PADDLE_SPEED, gameTime);
            leftPaddle.position.Y = getBoundY(leftPaddle);
            rightPaddle.position.Y = getRightPaddlePositionY();
            rightPaddle.position.Y = getBoundY(rightPaddle);
            // TODO: Check for Scoring
            // TODO: Render Score
            // TODO: Check for Ball/Paddle Collision
            // TODO: Render Middle Dashed line 
            // TODO: Cap Ball Speed
            // TODO: Maybe remove random ball speed and replace with geometry


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(
                ball.texture,
                ball.position,
                null,
                Color.White,
                0f,
                new Vector2(ball.texture.Width / 2, ball.texture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                leftPaddle.texture,
                leftPaddle.position,
                null,
                Color.White,
                0f,
                new Vector2(leftPaddle.texture.Width / 2, leftPaddle.texture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                rightPaddle.texture,
                rightPaddle.position,
                null,
                Color.White,
                0f,
                new Vector2(rightPaddle.texture.Width / 2, rightPaddle.texture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
