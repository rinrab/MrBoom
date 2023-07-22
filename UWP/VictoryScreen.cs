using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using static MrBoom.Game;

namespace MrBoom
{
    public class VictoryScreen : IScreen
    {
        public Screen Next { get; private set; }
        public Sound SoundsToPlay { get; private set; }

        private int tick;

        private readonly Player[] players;
        private readonly int winner;
        private readonly Assets assets;
        private readonly List<IController> controllers;

        public VictoryScreen(Player[] players, int winner, Assets assets, List<IController> controllers)
        {
            this.players = players;
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Vic[tick / 5 % assets.Vic.Length].Draw(ctx, 0, 0);

            Assets.AssetImage img = assets.Players[winner][0][tick / 20 % assets.Players[winner][0].Length];
            img.Draw(ctx, 320 / 2 - img.Width / 2, 80 - img.Height);
        }

        public void Update()
        {
            SoundsToPlay = 0;

            tick++;
            if (tick > 120 && Game.IsAnyKeyPressed(controllers))
            {
                foreach (Player player in players)
                {
                    player.VictoryCount = 0;
                }
                Next = Screen.StartMenu;
            }
        }
    }
}
