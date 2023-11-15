// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Screens;
using System.IO;
using System.Text;

namespace MrBoom
{
    public class OnlinePlayerListScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly HumanPlayerState currentPlayer;
        private readonly MultiplayerService multiplayerService;
        private List<IPlayerState> players;
        private int tick;
        private int toStart = -1;

        public Screen Next { get; private set; }

        public OnlinePlayerListScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                      Settings settings, HumanPlayerState currentPlayer, MultiplayerService multiplayerService)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.currentPlayer = currentPlayer;
            this.multiplayerService = multiplayerService;
            players = new List<IPlayerState>();

            string name = new NameGenerator(Terrain.Random).GenerateName();
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(2);
            foreach (char c in name)
            {
                stream.WriteByte((byte)c);
            }
            // TODO: resend if not delivered
            multiplayerService.SendInBackground(stream.ToArray());

            //multiplayerService.StartPinging();
        }

        public void Update()
        {
            var data = multiplayerService.GetData();
            if (data != null)
            {
                if (data[0] == 0)
                {
                    MemoryStream stream = new MemoryStream(data);

                    stream.ReadByte(); // Type

                    int playersCount = stream.ReadByte();
                    players = new List<IPlayerState>(playersCount);
                    for (int i = 0; i < playersCount; i++)
                    {
                        int type = stream.ReadByte();

                        StringBuilder name = new StringBuilder();
                        int nameLength = stream.ReadByte();
                        for (int j = 0; j < nameLength; j++)
                        {
                            name.Append((char)stream.ReadByte());
                        }
                        if (type == 0)
                        {
                            players.Add(currentPlayer);
                        }
                        else
                        {
                            players.Add(new RemotePlayerState(i, i, name.ToString()));
                        }
                    }

                    if (players.Count >= 2 && toStart == -1)
                    {
                        toStart = 60 * 3;
                    }
                }
            }

            if (toStart == 0)
            {
                var newScreen = new NetworkGameScreen(teams, assets, settings, controllers,
                                                      multiplayerService, players);
                ScreenManager.SetScreen(newScreen);
            }

            if (toStart != -1)
            {
                toStart--;
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Start.Draw(ctx, 0, 0);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    AnimatedImage images = assets.Alpha[index / 2 + 2];
                    if (index < players.Count)
                    {
                        IPlayerState player = players[index];

                        Game.DrawString(ctx, 21 + x * 80, 78 + y * 70, "name", images);
                        Game.DrawString(ctx, 21 + x * 80, 88 + y * 70, player.Name, images);
                    }
                    //else
                    //{
                    //    if (tick / 30 % 4 == 0)
                    //    {
                    //        Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "join", images);
                    //        Game.DrawString(ctx, x * 80 + 28, y * 70 + 88, "us", images);
                    //        Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                    //    }
                    //    else if (tick / 30 % 4 == 2)
                    //    {
                    //        Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "push", images);
                    //        Game.DrawString(ctx, x * 80 + 20, y * 70 + 88, "fire", images);
                    //        Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                    //    }
                    //}
                }
            }

            string ping;
            if (multiplayerService.Ping == -1)
            {
                ping = "loading...";
            }
            else
            {
                ping = multiplayerService.Ping.ToString();
            }

            Game.DrawString(ctx, 0, 0, "ping: " + ping, assets.Alpha[1]);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
