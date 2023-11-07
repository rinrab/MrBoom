// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class SearchingForPlayersScreen : IScreen
    {
        private Task matchmakingTask;
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly IController currentPlayer;

        public Screen Next => Screen.None;

        public SearchingForPlayersScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                         Settings settings, IController currentPlayer)
        {
            matchmakingTask = StartMatchmaking();
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.currentPlayer = currentPlayer;
        }

        private async Task StartMatchmaking()
        {
            // TODO:
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.MrFond.Draw(ctx, 0, 0);
            string text = "searching for players...";
            Game.DrawString(ctx, (320 - text.Length * 8) / 2, 190, text, assets.Alpha[1]);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
