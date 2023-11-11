// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Xaml;

namespace MrBoom
{
    public class MultiplayerStartScreen : IScreen
    {
        private int tick = 0;

        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly List<IController> unjoinedControllers;
        private readonly List<IController> joinedControllers;
        private readonly NameGenerator nameGenerator;

        private readonly string helpText =
            "welcome to mr.bomber " +
            $"v{Game.Version}!!!   " +
            "players can join using their drop bomb button second press or enter will start game   " +
            "gamepad controller: d-pad or left stick - move  a button - drop bomb  b button radio control   " +
            "right keyboard: arrows - move  ctrl - drop  bomb  shift - radio control   " +
            "left keyboard: wsad - move  ctrl - drop  bomb  shift - radio control   ";

        private int startTick = -1;
        private TeamMode teamMode = 0;
        private readonly List<IPlayerState> players;
        private Menu menu;

        public MultiplayerStartScreen(Assets assets, List<Team> teams, List<IController> controllers, Settings settings)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;

            unjoinedControllers = new List<IController>(controllers);
            joinedControllers = new List<IController>();
            nameGenerator = new NameGenerator(Terrain.Random);
            players = new List<IPlayerState>();
            teamMode = settings.TeamMode;

            teams.Clear();
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Start.Draw(ctx, 0, 0);

            int ox = 10;
            int oy = 10;

            assets.Controls[0].Draw(ctx, ox, oy);
            Game.DrawString(ctx, ox + 14 + 8, oy + 5, "or", assets.Alpha[1]);
            assets.Controls[1].Draw(ctx, ox + 14 + 8 * 4, oy + 1);
            Game.DrawString(ctx, ox + 14 + 25 + 8 * 5, oy + 5, "- join", assets.Alpha[1]);

            const int offset = 20;

            assets.Controls[2].Draw(ctx, ox, oy + offset);
            Game.DrawString(ctx, ox + 14 + 8, oy + 5 + offset, "or", assets.Alpha[1]);
            assets.Controls[3].Draw(ctx, ox + 14 + 8 * 4, oy + 1 + offset);
            Game.DrawString(ctx, ox + 14 * 2 + 8 * 5, oy + 5 + offset, "- add bot", assets.Alpha[1]);

            Game.DrawString(ctx, 320 - ox - 15 * 8, oy + 5,
                            "team mode: " + teamMode.ToString().ToLower(), assets.Alpha[1]);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    AnimatedImage images = assets.Alpha[index / 2 + 2];
                    if (index < players.Count)
                    {
                        IPlayerState player = players[index];

                        Game.DrawString(ctx, 13 + x * 80, 78 + y * 70, "name ?", images);
                        Game.DrawString(ctx, 21 + x * 80, 88 + y * 70, player.Name, images);
                    }
                    else
                    {
                        if (tick / 30 % 4 == 0)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "join", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 88, "us", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                        else if (tick / 30 % 4 == 2)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "push", images);
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 88, "fire", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                    }
                }
            }

            if (startTick < 600)
            {
                Game.DrawString(ctx, 320 - tick % (helpText.Length * 8 + 320), 192, helpText, assets.Alpha[1]);
            }

            if (startTick >= 0)
            {
                double scale = Math.Abs(Math.Sin((double)startTick / 15)) * 0.5 + 1;

                int width = (int)(assets.StartButton.Width * scale);
                int height = (int)(assets.StartButton.Height * scale);

                Rectangle rect = new Rectangle(325 - width / 2, 38 - height / 2, width, height);

                ctx.Draw(assets.StartButton, rect, Color.White);
            }
            if (startTick >= 600)
            {
                string text = "press a or enter";
                text = text.Substring(0, Math.Min((startTick - 600) / 6, text.Length));

                Game.DrawString(ctx, (320 - text.Length * 8) / 2, 200 - 10, text, assets.Alpha[1]);
            }

            menu?.Draw(ctx);
        }

        public void Update()
        {
            tick++;

            if (menu == null)
            {
                List<IController> toRemove = new List<IController>();
                foreach (IController controller in unjoinedControllers)
                {
                    if (controller.IsKeyDown(PlayerKeys.Bomb))
                    {
                        if (players.Count < 8)
                        {
                            players.Add(new HumanPlayerState(controller, players.Count, nameGenerator.GenerateName()));
                            assets.Sounds.Addplayer.Play();

                            toRemove.Add(controller);
                        }
                        else
                        {
                            for (int i = 0; i < players.Count; i++)
                            {
                                if (players[i].IsReplaceble)
                                {
                                    players[i] = new HumanPlayerState(controller, i, nameGenerator.GenerateName());
                                    assets.Sounds.Addplayer.Play();

                                    toRemove.Add(controller);
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (IController controller in toRemove)
                {
                    controller.Reset();
                    controller.Update();
                    unjoinedControllers.Remove(controller);
                    joinedControllers.Add(controller);
                }

                if (Controller.IsKeyDown(controllers, PlayerKeys.Menu))
                {
                    var options = new IMenuItem[] {
                        new TextMenuItem("START"),
                        new SelectMenuItem("TEAM", new string[] { "OFF", "COLOR", "SEX" })
                        {
                            SelectionIndex = (int)teamMode
                        },
                        new TextMenuItem("QUIT")
                    };

                    menu = new Menu(options, assets, controllers);
                    Controller.Reset(controllers);
                }

                if (Controller.IsKeyDown(controllers, PlayerKeys.AddBot) && players.Count < 8)
                {
                    assets.Sounds.Addbot.Play();
                    players.Add(new BotPlayerState(players.Count, "bot"));
                    Controller.Reset(controllers);
                }

                if (Controller.IsKeyDown(controllers, PlayerKeys.StartGame) ||
                    Controller.IsKeyDown(joinedControllers, PlayerKeys.Bomb))
                {
                    Start();
                }

                if (startTick == -1)
                {
                    if (players.Count >= 1)
                    {
                        startTick = 0;
                    }
                }
                else
                {
                    startTick++;
                }
            }
            else
            {
                menu.Update();

                teamMode = (TeamMode)((SelectMenuItem)menu.Items[1]).SelectionIndex;

                if (menu.Action == -2)
                {
                    menu = null;
                    Controller.Reset(controllers);
                }
                else if (menu.Action == 0)
                {
                    Start();
                }
                else if (menu.Action == 2)
                {
                    Application.Current.Exit();
                }
            }
        }

        private void Start()
        {
            if (players.Count == 1)
            {
                players.Add(new BotPlayerState(players.Count, "bot"));
            }
            else if (players.Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    players.Add(new BotPlayerState(players.Count, "bot"));
                }
            }

            if (players.Count >= 1)
            {
                if (players.Count == 1 && teamMode != 0)
                {
                    teamMode = 0;
                }

                settings.TeamMode = teamMode;

                teams.Clear();
                if (teamMode == TeamMode.Off)
                {
                    foreach (IPlayerState player in players)
                    {
                        teams.Add(new Team { Players = new List<IPlayerState> { player } });
                    }
                }
                if (teamMode == TeamMode.Color)
                {
                    if (players.Count == 2)
                    {
                        foreach (IPlayerState player in players)
                        {
                            teams.Add(new Team { Players = new List<IPlayerState> { player } });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < players.Count; i += 2)
                        {
                            var newPlayers = new List<IPlayerState> { players[i] };
                            if (i + 1 < players.Count)
                            {
                                newPlayers.Add(players[i + 1]);
                            }

                            teams.Add(new Team { Players = newPlayers });
                        }
                    }
                }
                if (teamMode == TeamMode.Sex)
                {
                    teams.Add(new Team { Players = new List<IPlayerState>() });
                    teams.Add(new Team { Players = new List<IPlayerState>() });

                    for (int i = 0; i < players.Count; i += 2)
                    {
                        teams[0].Players.Add(players[i]);
                        if (i + 1 < players.Count)
                        {
                            teams[1].Players.Add(players[i + 1]);
                        }
                    }
                }

                ScreenManager.SetScreen(new GameScreen(teams, assets, settings, controllers));
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            menu?.DrawHighDPI(ctx, rect, scale, graphicScale);
        }
    }
}
