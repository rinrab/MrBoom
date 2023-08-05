using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public interface ISprite
    {
        void Update();
        void Draw(SpriteBatch ctx);
        void SetSkull(SkullType skullType);
    }
}
