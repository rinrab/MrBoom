// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class OnlineStartScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly List<IPlayerState> players;
        private readonly List<IController> unjoinedControllers;
        private readonly List<IController> joinedControllers;
        private readonly NameGenerator nameGenerator;
        private int tick;

        public Screen Next => Screen.None;

        public OnlineStartScreen(Assets assets, List<Team> teams, List<IController> controllers, Settings settings)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            players = new List<IPlayerState>();
            unjoinedControllers = new List<IController>(controllers);
            joinedControllers = new List<IController>();
            nameGenerator = new NameGenerator(Terrain.Random);
        }

        public void Update()
        {
            List<IController> toRemove = new List<IController>();
            foreach (IController controller in unjoinedControllers)
            {
                if (controller.IsKeyDown(PlayerKeys.Bomb))
                {
                    if (players.Count < 4)
                    players.Add(new HumanPlayerState(controller, players.Count, nameGenerator.GenerateName()));
                    assets.Sounds.Addplayer.Play();

                    toRemove.Add(controller);
                }
            }

            foreach (IController controller in toRemove)
            {
                controller.Reset();
                controller.Update();
                unjoinedControllers.Remove(controller);
                joinedControllers.Add(controller);
            }

            if (Controller.IsKeyDown(controllers, PlayerKeys.StartGame) ||
                Controller.IsKeyDown(joinedControllers, PlayerKeys.Bomb))
            {
                Start();
            }

            tick++;
        }

        private void Start()
        {
            assets.Sounds.Bang.Play();
            ScreenManager.SetScreen(new SearchingForPlayersScreen(assets, teams, controllers, settings, (HumanPlayerState)players[0]));
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.OnlineMenu.Draw(ctx, 0, 0);
            string text = "push   !!";
            assets.Controls[0].Draw(ctx, 320 / 2, 181);
            Game.DrawString(ctx, (320 - text.Length * 8) / 2, 186, text, assets.Alpha[1]);

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 2 + x;
                    AnimatedImage images = assets.Alpha[2];
                    if (index < players.Count)
                    {
                        IPlayerState player = players[index];

                        Game.DrawString(ctx, 91 + 4 * 0 + x * 93, 78 + y * 54, "name ?", images);
                        Game.DrawString(ctx, 91 + 4 * 3 + x * 93, 88 + y * 54, player.Name, images);
                    }
                    else
                    {
                        if (tick / 30 % 4 == 0)
                        {
                            Game.DrawString(ctx, 91 + 4 * 2 + x * 93, 73 + y * 54, "join", images);
                            Game.DrawString(ctx, 91 + 4 * 4 + x * 93, 83 + y * 54, "us", images);
                            Game.DrawString(ctx, 91 + 4 * 4 + x * 93, 93 + y * 54, "!!", images);
                        }
                        else if (tick / 30 % 4 == 2)
                        {
                            Game.DrawString(ctx, 91 + 4 * 2 + x * 93, 73 + y * 54, "push", images);
                            Game.DrawString(ctx, 91 + 4 * 2 + x * 93, 83 + y * 54, "fire", images);
                            Game.DrawString(ctx, 91 + 4 * 4 + x * 93, 93 + y * 54, "!!", images);
                        }
                    }
                }
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
