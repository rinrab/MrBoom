// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.Json;
using MrBoom.Screens;

namespace MrBoom
{
    public class OnlinePlayerListScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly IController currentPlayer;
        private readonly PlayFabResult<GetMatchResult> match;
        private readonly MultiplayerService multiplayerService;
        private readonly List<IPlayerState> players;
        private int tick;

        public Screen Next { get; private set; }

        public OnlinePlayerListScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                      Settings settings, IController currentPlayer, PlayFabResult<GetMatchResult> match, MultiplayerService multiplayerService)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.currentPlayer = currentPlayer;
            this.match = match;
            this.multiplayerService = multiplayerService;
            players = new List<IPlayerState>();

            var members = match.Result.Members;
            for (int i = 0; i < members.Count; i++)
            {
                JsonObject attributes = (JsonObject)members[i].Attributes.DataObject;
                attributes.TryGetValue("Name", out object playerName);

                attributes.TryGetValue("Ip", out object ip);
                attributes.TryGetValue("Port", out object port);
                if ((ulong)port != (ulong)multiplayerService.Port)
                {
                    multiplayerService.Connect(new PlayerConnectionData[]
                    {
                        new PlayerConnectionData((string)ip, (int)(ulong)port)
                    });
                }

                players.Add(new HumanPlayerState(null, i, (string)playerName));
            }
        }

        public void Update()
        {
            multiplayerService.SendInBackground(new byte[]
            {
                255, 255, 255, 255, 255, 255
            });

            var data = multiplayerService.GetData();
            if (data != null)
            {
                ScreenManager.SetScreen(new NetworkGameScreen(teams, assets, settings, controllers, Game.game, multiplayerService));
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
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
