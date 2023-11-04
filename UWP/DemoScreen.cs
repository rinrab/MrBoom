// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Bot;

namespace MrBoom
{
    public class DemoScreen : AbstractGameScreen
    {
        private readonly Menu demoMenu;

        public DemoScreen(List<Team> teams, Assets assets, Game game) : base(teams, assets, game)
        {
            demoMenu = new Menu(new IMenuItem[]
            {
                new TextMenuItem("PLAY"),
                new SelectMenuItem("TEAM", new string[]
                {
                    "OFF",
                    "COLOR",
                    "SEX",
                }),
                new TextMenuItem("QUIT"),
            }, assets, game.Controllers);

            for (int i = 0; i < 4; i++)
            {
                terrain.AddPlayer(new ComputerPlayer(terrain, assets.Players[i], i, i));
            }

            terrain.InitializeMonsters();
        }

        public override void Update()
        {
            base.Update();
            if (terrain.Result == GameResult.Victory || terrain.Result == GameResult.Draw)
            {
                int levelIndex = game.LevelRandom.Next(MapData.Data.Length);

                terrain = new Terrain(levelIndex, assets);

                game.NextSong(MapData.Data[levelIndex].Song);

                for (int i = 0; i < 4; i++)
                {
                    terrain.AddPlayer(new ComputerPlayer(terrain, assets.Players[i], i, i));
                }

                terrain.InitializeMonsters();
            }

            demoMenu.Update();

            if (demoMenu.Action == 0)
            {
                ScreenManager.SetScreen(new MultiplayerStartScreen(assets, game.Teams, game.Controllers));
            }
            else if (demoMenu.Action == 2)
            {
                game.Exit();
            }

            Team.Mode = (TeamMode)((SelectMenuItem)demoMenu.Items[1]).SelectionIndex;

            if (Controller.IsKeyDown(game.Controllers, PlayerKeys.Continue))
            {
                 Controller.Reset(game.Controllers);
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            base.Draw(ctx);

            demoMenu.Draw(ctx);
        }

        public override void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            base.DrawHighDPI(ctx, rect, scale, graphicScale);

            demoMenu.DrawHighDPI(ctx, rect, scale, graphicScale);
        }
    }
}
