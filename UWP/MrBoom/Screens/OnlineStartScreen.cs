// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class OnlineStartScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;

        public Screen Next => Screen.None;

        public OnlineStartScreen(Assets assets, List<Team> teams, List<IController> controllers, Settings settings)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
        }

        public void Update()
        {
            // TODO:
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.MrFond.Draw(ctx, 0, 0);
            string text = "push   !!";
            assets.Controls[0].Draw(ctx, 320 / 2, 181);
            Game.DrawString(ctx, (320 - text.Length * 8) / 2, 186, text, assets.Alpha[1]);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
