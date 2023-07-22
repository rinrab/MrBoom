using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public interface IScreen
    {
        Screen Next { get; }
        Sound SoundsToPlay { get; }

        void Update();
        void Draw(SpriteBatch ctx);
    }
}
