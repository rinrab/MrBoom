// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.UI.Xaml;

namespace MrBoom
{
    public class StartScreen : IScreen
    {
        public Screen Next => Screen.None;

        public Menu menu;

        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;

        public StartScreen(Assets assets, List<Team> teams, List<IController> controllers)
        {
            IMenuItem[] menuItems = new IMenuItem[]
            {
                new TextMenuItem("MULTIPLAYER"),
                new TextMenuItem("SOLO"),
                new TextMenuItem("QUIT GAME"),
            };

            menu = new Menu(menuItems, assets, controllers, 0);

            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
        }

        public void Update()
        {
            menu.Update();
            if (menu.Action == 0)
            {
                ScreenManager.SetScreen(new MultiplayerStartScreen(assets, teams, controllers));
            }
            else if (menu.Action == 1)
            {
                ScreenManager.SetScreen(new SoloGameScreen(assets, controllers));
            }
            else if (menu.Action == 2)
            {
                Application.Current.Exit();
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Splash.Draw(ctx, 0, 0);

            menu.Draw(ctx);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale)
        {
            menu.DrawHighDPI(ctx, rect, scale);
        }
    }
}
