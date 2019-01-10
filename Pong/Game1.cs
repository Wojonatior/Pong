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

    public struct myRectangle {
        public float width;
        public float height;
    }

    class Utilities {
        public static myRectangle combineSizesOfGameObjects(GameObject targetObject, GameObject sourceObject){
            return new myRectangle{
                width = targetObject.texture.Width + sourceObject.texture.Width,
                height = targetObject.texture.Height + sourceObject.texture.Height,
            };
        }

        public static Vector2 getMajorAxis(Vector2 inVector) {
            if (abs(inVector.X) > abs(inVector.Y))
                return new Vector2(inVector.X, 0);
                // Scalar.Sign these values?
            else
                return new Vector2(0, inVector.Y);
                // Scalar.Sign these values?
        }

        public static bool atLeftBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics) {
            return coordinates.X <= texture.Width / 2;
        }
        public static bool atRightBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics) {
            return coordinates.X >= graphics.PreferredBackBufferWidth - texture.Width / 2;
        }
        public static bool atBottomBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics) {
            return coordinates.Y <= texture.Height / 2;
        }
        public static bool atTopBorder(Vector2 coordinates, Texture2D texture, GraphicsDeviceManager graphics) {
            return coordinates.Y >= graphics.PreferredBackBufferHeight - texture.Height / 2;
        }
        public static float getSpeedMultiplier(System.Random random) {
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

        public static BoundingBox getBBfromGameObject(GameObject gObject){
            var min = new Vector3(gObject.position.X - gObject.texture.Width / 2, gObject.position.Y - gObject.texture.Height / 2, 0);
            var max = new Vector3(gObject.position.X + gObject.texture.Width / 2, gObject.position.Y + gObject.texture.Height / 2, 0);
            return new BoundingBox(min, max);
        }

        public static Rectangle getRectanglefromGameObject(GameObject gObject) {
            var min = new Vector2(gObject.position.X - gObject.texture.Width / 2, gObject.position.Y - gObject.texture.Height / 2);
            var max = new Vector2(gObject.position.X + gObject.texture.Width / 2, gObject.position.Y + gObject.texture.Height / 2);
            var size = new Vector2(max.X - min.X, max.Y - min.Y);

            var minPoint = new Point((int)min.X, (int)min.Y);
            var sizePoint = new Point((int)size.X, (int)size.Y);
            return new Rectangle(minPoint, sizePoint);
        }
    }

    public class Game1 : Game {
        GameObject ball;
        GameObject leftPaddle;
        GameObject rightPaddle;
        Texture2D pixel;
        SpriteFont font;
        SpriteFont smol_font;

        const float MAX_BALL_SPEED = 50f;
        const float MIN_BALL_SPEED = 500f;
        const float PADDLE_SPEED = 600f;

        int leftScore = 0;
        int rightScore = 0;
        bool gameOver = false;
        bool notStarted = true;

        float? leftCollision = null;

        System.Random random;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            setupGame();
            random = new System.Random();
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ball.texture = Content.Load<Texture2D>("ball");
            leftPaddle.texture = Content.Load<Texture2D>("paddle");
            rightPaddle.texture = Content.Load<Texture2D>("paddle");
            font = Content.Load<SpriteFont>("Aldo");
            smol_font = Content.Load<SpriteFont>("44px_Aldo");

            pixel = new Texture2D(this.GraphicsDevice,1,1);
            Color[] colourData = new Color[1];
            colourData[0] = Color.White;
            pixel.SetData<Color>(colourData);
        }

        protected override void UnloadContent() { }

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

        private bool checkScoreOnLeft(GameObject ball){
            return ball.position.X - ball.texture.Width / 2 <= 0;
        }

        private bool checkScoreOnRight(GameObject ball){
            return ball.position.X + ball.texture.Width / 2 >= graphics.PreferredBackBufferWidth;
        }

        private float clampBallSpeed(float speed){
            return MathHelper.Clamp(speed, MIN_BALL_SPEED, MAX_BALL_SPEED);
        }

        private bool checkTwoObjectCollision(GameObject obj1, GameObject obj2){
            return obj1.position.X < obj2.position.X + obj2.texture.Width / 2 &&
            obj1.position.X + obj1.texture.Width / 2 > obj2.position.X &&
            obj1.position.Y < obj2.position.Y + obj2.texture.Height / 2 &&
            obj1.position.Y + obj1.texture.Height / 2 > obj2.position.Y;
        }

        private bool altTwoBoxCollision(GameObject obj1, GameObject obj2) {
            return (Utilities.abs(obj1.position.X - obj2.position.X) * 2 < (obj1.texture.Width + obj2.texture.Width)) &&
                   (Utilities.abs(obj1.position.Y - obj2.position.Y) * 2 < (obj1.texture.Height + obj2.texture.Height));
        }

        private void ballAndPaddleCollision(){
            var posVector = new Vector3(ball.position, 0);
            var velVector = new Vector3(ball.velocity, 0);
            var ballRay = new Ray(posVector, velVector);

            var leftPaddleBB = Utilities.getBBfromGameObject(leftPaddle);

            var leftPaddleCollisionPoint = ballRay.Intersects(leftPaddleBB);
            leftCollision = leftPaddleCollisionPoint;

            if (altTwoBoxCollision(ball, leftPaddle)){
                ball.position.X = leftPaddle.position.X + (leftPaddle.texture.Width / 2) + (ball.texture.Width / 2);
                ball.velocity.X *= -1;
            }

            if (altTwoBoxCollision(ball, rightPaddle)){
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

        private Vector2 getCenteredVector() {
            return new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
        }

        private bool checkForGameEnd() {
            return leftScore >= 5 || rightScore >= 5;
        }

        private void setupGame() {
            leftScore = 0;
            rightScore = 0;
            gameOver = false;

            ball.position = getCenteredVector();
            leftPaddle.position = new Vector2(
                32 + 16, // 32 + half the texture width
                graphics.PreferredBackBufferHeight / 2
            );
            rightPaddle.position = new Vector2(
                graphics.PreferredBackBufferWidth - (32 + 16), // 32 + half the texture width
                graphics.PreferredBackBufferHeight / 2
            );

            ball.velocity = new Vector2(
                500f,
                0f
            );
            leftPaddle.velocity = new Vector2(0, 0);
            rightPaddle.velocity = new Vector2(0, 0);

        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameOver) {
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                    setupGame();
            } else if (notStarted) {
                if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                    setupGame();
                    notStarted = false;
                }
            } else {
                leftScore += checkScoreOnRight(ball) ? 1 : 0;
                rightScore += checkScoreOnLeft(ball) ? 1 : 0;
                ballAndPaddleCollision();
                moveAndBoundBall(gameTime);
                leftPaddle.position.Y += getLeftPaddlePositionYDiff(PADDLE_SPEED, gameTime);
                leftPaddle.position.Y = getBoundY(leftPaddle);
                rightPaddle.position.Y = getRightPaddlePositionY();
                rightPaddle.position.Y = getBoundY(rightPaddle);
                ballAndPaddleCollision();
                gameOver = checkForGameEnd();
            }
            // TODO: Check for collision during movement, not just before/after
            // TODO: Render Middle Dashed line 
            // TODO: Maybe remove random ball speed and replace with geometry
            // TODO: Fix ball velocity jumping to max
            // TODO: Adjust ball velocity on paddle hits
            // TODO: Reset ball to center after score
            // TODO: functionalize ball movement


            base.Update(gameTime);
        }

        private void drawScore(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(font, leftScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth * 1 / 4, 50), Color.White);
            spriteBatch.DrawString(font, rightScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth * 3 / 4 - 37, 50), Color.White);
            spriteBatch.DrawString(font, leftCollision.ToString(), new Vector2(graphics.PreferredBackBufferWidth /  2 , graphics.PreferredBackBufferHeight / 2), Color.White);
        }

        private void drawStartAndEndText(SpriteBatch spriteBatch) {
            if(notStarted || gameOver)
                spriteBatch.DrawString(
                    smol_font,
                    "Press The S Key to Start",
                    new Vector2(
                        graphics.PreferredBackBufferWidth / 4 - 65,
                        150),
                    Color.White);
            if (gameOver) {
                string winningSide = leftScore >= 5 ? "Left" : "Right";
                spriteBatch.DrawString(
                    smol_font,
                    winningSide + " Has Won!",
                    new Vector2(
                        graphics.PreferredBackBufferWidth / 4 + 40,
                        250),
                    Color.White);
            }
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

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.Opaque);
            //RasterizerState state = new RasterizerState();
            //state.FillMode = FillMode.WireFrame;
            //spriteBatch.GraphicsDevice.RasterizerState = state;


            var oldScore = leftScore + rightScore;
            drawScore(spriteBatch);
            drawCollisionObjects(spriteBatch);
            drawStartAndEndText(spriteBatch);
            spriteBatch.Draw(pixel, Utilities.getRectanglefromGameObject(leftPaddle), Color.Red);
            var newScore = leftScore + rightScore;
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
