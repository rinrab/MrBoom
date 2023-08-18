// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Xaml.Documents;

namespace MrBoom
{
    public class ResultScreen : IScreen
    {
        public Screen Next { get; private set; }

        private readonly Team[] teams;
        private readonly int winner;
        private readonly List<IController> controllers;
        private readonly Assets assets;

        private int tick;

        public ResultScreen(Team[] teams, int winner, Assets assets, List<IController> controllers, int teamMode)
        {
            this.teams = teams;
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;

            assets.Sounds.Victory.Play();
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Med.Draw(ctx, 0, 0);

            void drawCoins(int x, int y, int teamIndex)
            {
                if (teamIndex < teams.Length)
                {
                    Team team = teams[teamIndex];

                    for (int j = 0; j < team.VictoryCount; j++)
                    {
                        int index = tick / (8 + j) % assets.Coin.Length;

                        if (teamIndex == winner && j == team.VictoryCount - 1)
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
                            assets.Coin[index].Draw(ctx, x + j * 23, y);
                        }
                    }

                    for (int i = 0; i < team.Names.Length; i++)
                    {
                        Game.DrawString(ctx, x - 34, y + 26 - 10 + i * 8, team.Names[i], assets.Alpha[teamIndex + 2]);
                    }
                }
            }

            drawCoins(0 * 161 + 44, 0 * 42 + 27, 0);
            drawCoins(0 * 161 + 44, 1 * 42 + 27, 1);
            drawCoins(1 * 161 + 44, 0 * 42 + 27, 2);
            drawCoins(1 * 161 + 44, 1 * 42 + 27, 3);
            drawCoins(0 * 161 + 44, 2 * 42 + 27, 4);
            drawCoins(0 * 161 + 44, 3 * 42 + 27, 5);
            drawCoins(1 * 161 + 44, 2 * 42 + 27, 6);
            drawCoins(1 * 161 + 44, 3 * 42 + 27, 7);
        }

        public void Update()
        {
            if (this.tick > 120 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                if (teams[winner].VictoryCount >= 5)
                {
                    ScreenManager.SetScreen(new VictoryScreen(teams[winner], assets, controllers));
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
