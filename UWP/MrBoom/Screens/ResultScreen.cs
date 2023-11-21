// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class ResultScreen : AbstractScreen
    {
        private readonly List<Team> teams;
        private readonly int winner;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly Assets assets;

        public ResultScreen(List<Team> teams, int winner, Assets assets, List<IController> controllers, Settings settings)
        {
            this.teams = teams;
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;
            this.settings = settings;
            assets.Sounds.Victory.Play();
        }

        protected override void OnDraw(SpriteBatch ctx)
        {
            if (settings.TeamMode == TeamMode.Off)
            {
                assets.Med.Draw(ctx, 0, 0);
            }
            else if (settings.TeamMode == TeamMode.Color)
            {
                assets.MedC.Draw(ctx, 0, 0);
            }
            else if (settings.TeamMode == TeamMode.Sex)
            {
                assets.MedG.Draw(ctx, 0, 0);
            }

            void drawCoins(int x, int y, int teamIndex)
            {
                if (teamIndex < teams.Count)
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
                        int tx = (settings.TeamMode == TeamMode.Sex) ? x - 42 : x - 34;
                        int ty = (settings.TeamMode == TeamMode.Sex) ? y : y + 26 - 10;
                        int color = 0;
                        if (settings.TeamMode == TeamMode.Off) color = teamIndex / 2 + 2;
                        if (settings.TeamMode == TeamMode.Color) color = teamIndex + 2;
                        if (settings.TeamMode == TeamMode.Sex) color = (teamIndex == 0) ? 4 : 3;

                        Game.DrawString(ctx, tx, ty + i * 8, team.Names[i], assets.Alpha[color]);
                    }
                }
            }

            if (settings.TeamMode == TeamMode.Off)
            {
                drawCoins(0 * 161 + 44, 0 * 42 + 27, 0);
                drawCoins(0 * 161 + 44, 1 * 42 + 27, 1);
                drawCoins(1 * 161 + 44, 0 * 42 + 27, 2);
                drawCoins(1 * 161 + 44, 1 * 42 + 27, 3);
                drawCoins(0 * 161 + 44, 2 * 42 + 27, 4);
                drawCoins(0 * 161 + 44, 3 * 42 + 27, 5);
                drawCoins(1 * 161 + 44, 2 * 42 + 27, 6);
                drawCoins(1 * 161 + 44, 3 * 42 + 27, 7);
            }
            else if (settings.TeamMode == TeamMode.Color)
            {
                drawCoins(0 * 161 + 44, 0 * 64 + 53, 0);
                drawCoins(1 * 161 + 44, 0 * 64 + 53, 1);
                drawCoins(0 * 161 + 44, 1 * 64 + 53, 2);
                drawCoins(1 * 161 + 44, 1 * 64 + 53, 3);
            }
            else if (settings.TeamMode == TeamMode.Sex)
            {
                drawCoins(126, 0 * 68 + 60, 0);
                drawCoins(126, 1 * 68 + 60, 1);
            }
        }

        protected override void OnUpdate()
        {
            if (tick > 120 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                if (teams[winner].VictoryCount >= 5)
                {
                    ScreenManager.SetScreen(new VictoryScreen(teams[winner], assets, controllers, teams, settings));
                }
                else
                {
                    ScreenManager.SetScreen(new GameScreen(teams, assets, settings, controllers));
                }
            }
        }
    }
}
