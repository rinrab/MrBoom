// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Screens;
using System.IO;
using System.Text;
using MrBoom.NetworkProtocol;
using System;
using System.Threading;
using System.Diagnostics;

namespace MrBoom
{
    public class OnlinePlayerListScreen : AbstractScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly List<HumanPlayerState> currentPlayers;
        private readonly GameNetworkConnection gameNetworkConnection;
        private List<IPlayerState> players;
        private int toStart = -1;
        private byte[] lastGameMessage;

        public OnlinePlayerListScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                      Settings settings, List<HumanPlayerState> currentPlayers,
                                      GameNetworkConnection gameNetworkConnection)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.currentPlayers = currentPlayers;
            this.gameNetworkConnection = gameNetworkConnection;
            this.gameNetworkConnection.MessageReceived += GameNetworkConnection_MessageReceived;
            players = new List<IPlayerState>();

            //multiplayerService.StartPinging();
        }

        private void GameNetworkConnection_MessageReceived(ReadOnlyByteSpan msg)
        {
            try
            {
                byte[] data = msg.AsArray();

                if (data != null && data[0] == 0)
                {
                    Interlocked.Exchange(ref lastGameMessage, data);
                }
            }
            catch (Exception ex)
            {
                // Ignore invalid messages.
                Debug.WriteLine("Unexpecteted error while processing message from the server: {0}", ex.Message);
            }
        }

        protected override void OnUpdate()
        {
            var data = Interlocked.Exchange(ref lastGameMessage, null);
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
                            players.Add(currentPlayers[0]); // TODO:
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
                                                      gameNetworkConnection, players);
                ScreenManager.SetScreen(newScreen);
            }

            if (toStart != -1)
            {
                toStart--;
            }

            if (Controller.IsKeyDown(controllers, PlayerKeys.Back))
            {
                Controller.Reset(controllers);
                ScreenManager.SetScreen(new OnlineStartScreen(assets, teams, controllers, currentPlayers, settings));
            }
        }

        protected override void OnDraw(SpriteBatch ctx)
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
            TimeSpan? currentRtt = gameNetworkConnection.Ping;

            if (currentRtt.HasValue)
            {
                ping = currentRtt.Value.TotalMilliseconds.ToString();
            }
            else
            {
                ping = "loading...";
            }

            Game.DrawString(ctx, 0, 0, "ping: " + ping, assets.Alpha[1]);
        }

        protected override void OnDispose()
        {
            gameNetworkConnection.MessageReceived -= GameNetworkConnection_MessageReceived;

            base.OnDispose();
        }
    }
}
