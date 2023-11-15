// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public interface IScreen
    {
        void Update();
        void Draw(SpriteBatch ctx);
        void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale);
    }
}
