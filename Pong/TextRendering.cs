using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pong
{
    public class TextRendering
    {
        public static void drawScore(SpriteBatch spriteBatch, GameState state, GraphicsDeviceManager graphics, SpriteFont font)
        {
            spriteBatch.DrawString(font, state.leftScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth * 1 / 4, 50), Color.White);
            spriteBatch.DrawString(font, state.rightScore.ToString(), new Vector2(graphics.PreferredBackBufferWidth * 3 / 4 - 37, 50), Color.White);
        }

        public static void drawStartAndEndText(SpriteBatch spriteBatch, GameState state, GraphicsDeviceManager graphics, SpriteFont smol_font)
        {
            if (state.notStarted || state.gameOver)
                spriteBatch.DrawString(
                    smol_font,
                    "Press The S Key to Start",
                    new Vector2(
                        graphics.PreferredBackBufferWidth / 4 - 65,
                        150),
                    Color.White);
            if (state.gameOver)
            {
                string winningSide = state.leftScore >= 5 ? "Left" : "Right";
                spriteBatch.DrawString(
                    smol_font,
                    winningSide + " Has Won!",
                    new Vector2(
                        graphics.PreferredBackBufferWidth / 4 + 40,
                        250),
                    Color.White);
            }
        }

    }
}
