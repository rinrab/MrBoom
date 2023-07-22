using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public interface IScreen
    {
        Screen Next { get; }

        void Update();
        void Draw(SpriteBatch ctx);
    }
}
