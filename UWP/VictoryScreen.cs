// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class VictoryScreen : IScreen
    {
        public Screen Next { get; private set; }

        private int tick;

        private readonly PlayerState[] players;
        private readonly Team winner;
        private readonly Assets assets;
        private readonly List<IController> controllers;

        public VictoryScreen(Team winner, Assets assets, List<IController> controllers)
        {
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Vic[tick / 5].Draw(ctx, 0, 0);

            Image img = assets.Players[winner.Players[0].Index].Normal[0][tick / 20];
            img.Draw(ctx, 320 / 2 - img.Width / 2, 80 - img.Height);
        }

        public void Update()
        {
            tick++;
            if (tick > 120 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                foreach (PlayerState player in players)
                {
                    player.VictoryCount = 0;
                }
                Next = Screen.StartMenu;
            }
        }
    }
}
