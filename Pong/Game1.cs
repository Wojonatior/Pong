using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong.Desktop
{

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
        Texture2D ballTexture;
        Vector2 ballPosition;
        Vector2 ballVelocity;
        System.Random random;
        const float MAX_SPEED = 50f;
        const float MIN_SPEED = 500f;


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
            ballPosition = new Vector2(
                graphics.PreferredBackBufferWidth / 2,
                graphics.PreferredBackBufferHeight / 2
            );

            ballVelocity = new Vector2(
                200f,
                160f
            );
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
            ballTexture = Content.Load<Texture2D>("ball");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            ballPosition.X += ballVelocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ballPosition.Y += ballVelocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Utilities.atLeftBorder(ballPosition, ballTexture, graphics) || Utilities.atRightBorder(ballPosition, ballTexture, graphics)) {
                ballVelocity.X *= -1 * Utilities.getSpeedMultiplier(random);
                ballPosition.X = MathHelper.Clamp(ballPosition.X, ballTexture.Width / 2, graphics.PreferredBackBufferWidth - (ballTexture.Width / 2));
            }

            if (Utilities.atTopBorder(ballPosition, ballTexture, graphics) || Utilities.atBottomBorder(ballPosition, ballTexture, graphics)) {
                ballVelocity.Y *= -1 * Utilities.getSpeedMultiplier(random);
                ballPosition.Y = MathHelper.Clamp(ballPosition.Y, ballTexture.Height / 2, graphics.PreferredBackBufferHeight - (ballTexture.Height / 2));
            }

            // Get the max between half of the width of the ball, and the x position
            // get the min between that and the prefered width minus half the width of the ball
            // if x is bigger than width - ballWidth/2, it's over the right border
            // if x is less than ballWidth/2 it's over the left border 

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
                ballTexture,
                ballPosition,
                null,
                Color.White,
                0f,
                new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
