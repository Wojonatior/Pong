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
        public static float abs(float val){
            return val >= 0 ? val : val * -1;
        }

        public static float getSign(float val){
            return val >= 0 ? 1 : -1;
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
        SpriteFont font;
        int leftScore = 0;
        int rightScore = 0;
        System.Random random;
        const float MAX_BALL_SPEED = 50f;
        const float MIN_BALL_SPEED = 500f;
        const float PADDLE_SPEED = 250f;


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
            font = Content.Load<SpriteFont>("Aldo");
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

        private float getBoundX(GameObject gameObject) {
            return MathHelper.Clamp(gameObject.position.X, gameObject.texture.Width / 2, graphics.PreferredBackBufferWidth - (gameObject.texture.Width / 2));
        }

        private bool checkLeftScore(GameObject ball){
            return ball.position.X - ball.texture.Width / 2 <= 0;
        }

        private bool checkRightScore(GameObject ball){
            return ball.position.X + ball.texture.Width / 2 >= graphics.PreferredBackBufferWidth;
        }

        private float clampBallSpeed(float speed){
            return MathHelper.Clamp(speed, MIN_BALL_SPEED, MAX_BALL_SPEED);
        }

        private bool checkTwoObjectCollision(GameObject obj1, GameObject obj2){
            return obj1.position.X < obj2.position.X + obj2.texture.Width &&
            obj1.position.X + obj1.texture.Width > obj2.position.X &&
            obj1.position.Y < obj2.position.Y + obj2.texture.Height &&
            obj1.position.Y + obj1.texture.Height > obj2.position.Y;
        }

        private void ballAndPaddleCollision(){
            if (checkTwoObjectCollision(leftPaddle, ball)){
                ball.position.X = leftPaddle.position.X + (leftPaddle.texture.Width / 2) + (ball.texture.Width / 2);
                ball.velocity.X *= -1;
            }

            if (checkTwoObjectCollision(rightPaddle, ball)){
                ball.position.X = rightPaddle.position.X - (rightPaddle.texture.Width / 2) - (ball.texture.Width / 2);
                ball.velocity.X *= -1;
            }
        }

        private void moveAndBoundBall(GameTime gameTime){
            ball.position.X += ball.velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ball.position.Y += ball.velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Utilities.atLeftBorder(ball.position, ball.texture, graphics) || Utilities.atRightBorder(ball.position, ball.texture, graphics)) {
                var absNewVelocity = Utilities.abs(ball.velocity.X * Utilities.getSpeedMultiplier(random));
                ball.velocity.X = Utilities.getSign(ball.velocity.X) * -1 * clampBallSpeed(absNewVelocity);
                ball.position.X = getBoundX(ball);
            }

            if (Utilities.atTopBorder(ball.position, ball.texture, graphics) || Utilities.atBottomBorder(ball.position, ball.texture, graphics)) {
                var absNewVelocity = Utilities.abs(ball.velocity.Y * Utilities.getSpeedMultiplier(random));
                ball.velocity.Y = Utilities.getSign(ball.velocity.Y) * -1 * clampBallSpeed(absNewVelocity);
                ball.position.Y = getBoundY(ball);
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

            leftScore += checkLeftScore(ball) ? 1 : 0;
            rightScore += checkRightScore(ball) ? 1 : 0;
            moveAndBoundBall(gameTime);
            ballAndPaddleCollision();
            leftPaddle.position.Y += getLeftPaddlePositionYDiff(PADDLE_SPEED, gameTime);
            leftPaddle.position.Y = getBoundY(leftPaddle);
            rightPaddle.position.Y = getRightPaddlePositionY();
            rightPaddle.position.Y = getBoundY(rightPaddle);
            // TODO: Check for Ball/Paddle Collision
            // TODO: Render Middle Dashed line 
            // TODO: Maybe remove random ball speed and replace with geometry
            // TODO: Fix ball velocity immediatly jumping to max
            // TODO: Reset ball to center after score
            // TODO: Add end game state
            // TODO: Allow game to reset
            // TODO: Start game after pressing arrows
            // TODO: functionalize ball movement


            base.Update(gameTime);
        }

        private void drawScore(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(font, leftScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth * 1 / 4, 50), Color.White);
            spriteBatch.DrawString(font, rightScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth * 3 / 4 - 37, 50), Color.White);
        }

        private void drawCollisionObjects(SpriteBatch spriteBatch) {
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
            var oldScore = leftScore + rightScore;
            drawScore(spriteBatch);
            drawCollisionObjects(spriteBatch);
            var newScore = leftScore + rightScore;
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
