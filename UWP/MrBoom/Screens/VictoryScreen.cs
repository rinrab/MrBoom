// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class VictoryScreen : AbstractScreen
    {
        private readonly Team winner;
        private readonly Assets assets;
        private readonly List<IController> controllers;
        private readonly List<Team> teams;
        private readonly Settings settings;

        public VictoryScreen(Team winner, Assets assets, List<IController> controllers, List<Team> teams, Settings settings)
        {
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;
            this.teams = teams;
            this.settings = settings;
        }

        protected override void OnDraw(SpriteBatch ctx)
        {
            assets.Vic[CurrentTick / 5].Draw(ctx, 0, 0);

            for (int i = 0; i < winner.Players.Count; i++)
            {
                Image img = assets.Players[i].Normal[0][CurrentTick / 20];
                img.Draw(ctx, 320 / 2 - img.Width / 2 - winner.Players.Count * 5 + i * 20, 80 - img.Height);
            }
        }

        protected override void OnUpdate()
        {
            if (CurrentTick > 120 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                ScreenManager.SetScreen(new DemoScreen(teams, assets, settings, controllers));
            }
        }
    }
}
