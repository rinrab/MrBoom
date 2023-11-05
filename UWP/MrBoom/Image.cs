// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class Image
    {
        private readonly Texture2D texture;
        private readonly Rectangle rect;
        private readonly int scale;

        public int Width => rect.Width / scale;
        public int Height => rect.Height / scale;
        public int X => rect.X / scale;
        public int Y => rect.Y / scale;

        public Texture2D Texture => texture;

        public Image(Texture2D texture, int x, int y, int width, int height, int scale)
        {
            this.texture = texture;
            this.scale = scale;
            rect = new Rectangle(x * scale, y * scale, width * scale, height * scale);
        }

        public void Draw(SpriteBatch ctx, int x, int y, Color? color = null)
        {
            ctx.Draw(texture,
                     new Vector2(x * scale, y * scale),
                     rect,
                     color.HasValue ? color.Value : Color.White);
        }
    }
}
