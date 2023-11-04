// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class GameScreen : AbstractGameScreen
    {
        private Menu pauseMenu;

        public GameScreen(List<Team> teams, Assets assets, Settings settings, Game game) : base(teams, assets, settings, game)
        {
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = 0; j < teams[i].Players.Count; j++)
                {
                    IPlayerState playerState = teams[i].Players[j];
                    AbstractPlayer player = playerState.GetPlayer(terrain, i);
                    terrain.AddPlayer(player);
                }
            }

            terrain.InitializeMonsters();
        }

        public override void Update()
        {
            base.Update();

            if (terrain.Result == GameResult.Victory)
            {
                int winner = terrain.Winner;

                game.Teams[winner].VictoryCount++;

                ScreenManager.SetScreen(new ResultScreen(game.Teams, winner, assets, game.Controllers, settings));
            }
            else if (terrain.Result == GameResult.Draw)
            {
                ScreenManager.SetScreen(new DrawScreen(assets, game.Controllers));
            }

            if (isPause)
            {
                pauseMenu.Update();

                if (Controller.IsKeyDown(game.Controllers, PlayerKeys.Menu) ||
                    Controller.IsKeyDown(game.Controllers, PlayerKeys.Back))
                {
                    isPause = false;
                    Controller.Reset(game.Controllers);
                }

                if (pauseMenu.Action == 0)
                {
                    isPause = false;
                }
                else if (pauseMenu.Action == 1)
                {
                    game.NextSong(3);
                    ScreenManager.SetScreen(new MultiplayerStartScreen(assets, game.Teams, game.Controllers, settings));
                }
                else if (pauseMenu.Action == 2)
                {
                    game.Exit();
                }
            }
            else
            {
                if (Controller.IsKeyDown(game.Controllers, PlayerKeys.Menu))
                {
                    var options = new IMenuItem[]
                    {
                        new TextMenuItem("RESUME"),
                        new TextMenuItem("MAIN MENU"),
                        new TextMenuItem("QUIT")
                    };

                    pauseMenu = new Menu(options, assets, game.Controllers);
                    isPause = true;

                    Controller.Reset(game.Controllers);
                }
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            base.Draw(ctx);

            if (isPause)
            {
                pauseMenu.Draw(ctx);
            }
        }

        public override void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            base.DrawHighDPI(ctx, rect, scale, graphicScale);

            if (isPause)
            {
                pauseMenu.DrawHighDPI(ctx, rect, scale, graphicScale);
            }
        }
    }
}
