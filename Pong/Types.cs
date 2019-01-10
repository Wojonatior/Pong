using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    public struct GameObject
    {
        public Vector2 position;
        public Vector2 velocity;
        public Texture2D texture;
    }

    public struct GameState
    {
        public int leftScore;
        public int rightScore;
        public bool gameOver;
        public bool notStarted;
        public float leftCollision;
    }
}
