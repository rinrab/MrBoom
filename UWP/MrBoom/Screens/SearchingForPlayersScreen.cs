// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;

namespace MrBoom
{
    public class SearchingForPlayersScreen : IScreen
    {
        private Task matchmakingTask;
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly IController currentPlayer;
        private readonly NameGenerator nameGenerator;
        private string status;

        public Screen Next => Screen.None;

        public SearchingForPlayersScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                         Settings settings, IController currentPlayer)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.currentPlayer = currentPlayer;
            this.nameGenerator = new NameGenerator(Terrain.Random);

            matchmakingTask = StartMatchmaking();
        }

        private async Task StartMatchmaking()
        {
            PlayFabSettings.staticSettings.TitleId = "E8B53";

            string playerId = settings.PlayerId;
            if (playerId == null)
            {
                Guid guid = Guid.NewGuid();
                playerId = guid.ToString("N");
                settings.PlayerId = playerId;
            }

            if (settings.IsDebug)
            {
                playerId += "-debug";
            }

            status = "logging in";

            PlayFabResult<LoginResult> login = await PlayFabClientAPI.LoginWithCustomIDAsync(new LoginWithCustomIDRequest
            {
                CustomId = playerId,
                CreateAccount = true
            });

            await PlayFabMultiplayerAPI.CancelAllMatchmakingTicketsForPlayerAsync(new CancelAllMatchmakingTicketsForPlayerRequest
            {
                Entity = new PlayFab.MultiplayerModels.EntityKey
                {
                    Id = login.Result.EntityToken.Entity.Id,
                    Type = login.Result.EntityToken.Entity.Type,
                },
                QueueName = "default",
            });

            //if (login.Result.NewlyCreated)
            //{
            //    await PlayFabClientAPI.UpdateUserTitleDisplayNameAsync(new UpdateUserTitleDisplayNameRequest
            //    {
            //        DisplayName = new NameGenerator(Terrain.Random).GenerateName()
            //    });
            //}

            status = "creating ticket";

            MultiplayerService multiplayerService = new MultiplayerService();
            string ip = multiplayerService.GetLocalIPAddress();
            int port = Terrain.Random.Next(65535);
            multiplayerService.StartListen(port);

            PlayFabResult<CreateMatchmakingTicketResult> ticket = await PlayFabMultiplayerAPI.CreateMatchmakingTicketAsync(
                new CreateMatchmakingTicketRequest
                {
                    Creator = new MatchmakingPlayer
                    {
                        Entity = new PlayFab.MultiplayerModels.EntityKey
                        {
                            Id = login.Result.EntityToken.Entity.Id,
                            Type = login.Result.EntityToken.Entity.Type,
                        },
                        Attributes = new MatchmakingPlayerAttributes
                        {
                            DataObject = new
                            {
                                Name = new NameGenerator(Terrain.Random).GenerateName(),
                                Ip = ip,
                                Port = port
                            }
                        },
                    },
                    GiveUpAfterSeconds = 120,
                    QueueName = "default",
                });

            while (true)
            {
                PlayFabResult<GetMatchmakingTicketResult> newTicket = await PlayFabMultiplayerAPI.GetMatchmakingTicketAsync(new GetMatchmakingTicketRequest
                {
                    TicketId = ticket.Result.TicketId,
                    QueueName = "default"
                });

                if (newTicket.Result.Status == "Matched")
                {
                    PlayFabResult<GetMatchResult> match = await PlayFabMultiplayerAPI.GetMatchAsync(new GetMatchRequest
                    {
                        MatchId = newTicket.Result.MatchId,
                        QueueName = "default",
                        ReturnMemberAttributes = true,
                    });

                    ScreenManager.SetScreen(new OnlinePlayerListScreen(assets, teams, controllers, settings, currentPlayer, match, multiplayerService));

                    break;
                }

                status = newTicket.Result.Status;

                await Task.Delay(6000);
            }
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.MrFond.Draw(ctx, 0, 0);

            string text = GetStatusDisplayString(status);

            Game.DrawString(ctx, (320 - text.Length * 8) / 2, 190, text, assets.Alpha[1]);
        }

        private string GetStatusDisplayString(string status)
        {
            switch (status)
            {
                case "Creating": return "creating...";
                case "Joining": return "joining...";
                case "WaitingForPlayers": return "waiting for players...";
                case "WaitingForMatch": return "waiting for match...";
                case "Matched": return "matched";
                case "Canceled": return "canceled";
                case "Failed": return "failed";
                default: return status;
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
