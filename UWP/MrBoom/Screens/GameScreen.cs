// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Xaml;

namespace MrBoom
{
    public class GameScreen : AbstractGameScreen
    {
        private Menu pauseMenu;

        public GameScreen(List<Team> teams, Assets assets, Settings settings,
                          List<IController> controllers) : base(teams, assets, settings, controllers)
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

            Controller.Reset(controllers);
        }

        public override void Update()
        {
            base.Update();

            if (terrain.Result == GameResult.Victory)
            {
                int winner = terrain.Winner;

                teams[winner].VictoryCount++;

                ScreenManager.SetScreen(new ResultScreen(teams, winner, assets, controllers, settings));
            }
            else if (terrain.Result == GameResult.Draw)
            {
                ScreenManager.SetScreen(new DrawScreen(teams, assets, settings, controllers));
            }

            if (isPause)
            {
                pauseMenu.Update();

                if (Controller.IsKeyDown(controllers, PlayerKeys.Menu) ||
                    Controller.IsKeyDown(controllers, PlayerKeys.Back))
                {
                    isPause = false;
                    Controller.Reset(controllers);
                }

                if (pauseMenu.Action == 0)
                {
                    isPause = false;
                }
                else if (pauseMenu.Action == 1)
                {
                    ScreenManager.NextSong(assets.Sounds, 3);
                    ScreenManager.SetScreen(new MultiplayerStartScreen(assets, teams, controllers, settings));
                }
                else if (pauseMenu.Action == 2)
                {
                    Application.Current.Exit();
                }
            }
            else
            {
                if (Controller.IsKeyDown(controllers, PlayerKeys.Menu))
                {
                    var options = new IMenuItem[]
                    {
                        new TextMenuItem("RESUME"),
                        new TextMenuItem("MAIN MENU"),
                        new TextMenuItem("QUIT")
                    };

                    pauseMenu = new Menu(options, assets, controllers);
                    isPause = true;

                    Controller.Reset(controllers);
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
