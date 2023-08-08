using System;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class PauseWindow : IScreen
    {
        public Screen Next => Screen.None;

        private int tick = 0;
        private readonly Assets assets;

        public PauseWindow(Assets assets)
        {
            this.assets = assets;
        }

        public void Draw(SpriteBatch ctx)
        {
            var img = assets.Pause[tick / 20 % 4];
            img.Draw(ctx, 320 / 2 - img.Width / 2, 30);

            assets.PauseHelp.Draw(ctx, 320 / 2 - assets.PauseHelp.Width / 2, 130);
        }

        public void Update()
        {
            tick++;
        }
    }
}
