using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class ResultScreen : IScreen
    {
        public Screen Next { get; private set; }
        public Sound SoundsToPlay { get; private set; }

        private readonly Game.Player[] players;
        private readonly int winner;
        private readonly List<IController> controllers;
        private readonly Assets assets;

        private int tick;

        public ResultScreen(Game.Player[] players, int winner, Assets assets, List<IController> controllers)
        {
            this.players = players;
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;

            SoundsToPlay |= Sound.Victory;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Med.Draw(ctx, 0, 0);

            Point[] positions = new Point[] {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 0),
                new Point(1, 1),
                new Point(0, 3),
                new Point(0, 4),
                new Point(1, 3),
                new Point(1, 4),
            };

            for (int i = 0; i < players.Length; i++)
            {
                for (int j = 0; j < players[i].VictoryCount; j++)
                {
                    int index = (tick / (8 + j)) % assets.Coin.Length;
                    if (i == this.winner && j == players[i].VictoryCount - 1)
                    {
                        if (tick % 60 < 30)
                        {
                            index = 0;
                        }
                        else
                        {
                            index = -1;
                        }
                    }

                    if (index != -1)
                    {
                        assets.Coin[index].Draw(ctx, positions[i].X * 161 + 44 + j * 23, positions[i].Y * 42 + 27);
                    }
                }
            }


            for (int i = 0; i < positions.Length; i++)
            {
                if (i < players.Length)
                {
                    Game.DrawString(ctx, positions[i].X * 161 + 10, positions[i].Y * 42 + 44,
                        players[i].Name, assets.Alpha[i / 2 + 2]);
                }
            }
        }

        public void Update()
        {
            SoundsToPlay = 0;

            if (this.tick > 120 && Game.IsAnyKeyPressed(controllers))
            {
                if (players[winner].VictoryCount >= 5)
                {
                    Next = Screen.Victory;
                }
                else
                {
                    Next = Screen.Game;
                }
            }

            this.tick++;
        }
    }
}
