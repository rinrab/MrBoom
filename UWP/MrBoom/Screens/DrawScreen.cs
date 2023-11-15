// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class DrawScreen : IScreen
    {
        public List<IController> controllers;
        private readonly List<Team> teams;
        private readonly Assets assets;
        private readonly Settings settings;
        private int tick;

        public DrawScreen(List<Team> teams, Assets assets, Settings settings, List<IController> controllers)
        {
            this.teams = teams;
            this.assets = assets;
            this.settings = settings;
            this.controllers = controllers;

            assets.Sounds.Draw.Play();
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Draw[tick / 30].Draw(ctx, 0, 0);
        }

        public void Update()
        {
            tick++;

            if (tick > 120 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                ScreenManager.SetScreen(new GameScreen(teams, assets, settings, controllers));
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
