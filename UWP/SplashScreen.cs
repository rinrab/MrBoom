using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    internal class SplashScreen : IScreen
    {
        public Screen Next => Screen.None;

        private int tick = 0;

        private readonly Assets assets;
        private readonly List<PlayerState> players;
        private readonly List<IController> controllers;

        public SplashScreen(Assets assets, List<PlayerState> players, List<IController> controllers)
        {
            this.assets = assets;
            this.players = players;
            this.controllers = controllers;
        }

        public void Update()
        {
            tick++;
            if (tick > 300)
            {
                ScreenManager.SetScreen(new StartScreen(assets, players, controllers));
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Splash.Draw(ctx, 0, 0);
        }
    }
}
