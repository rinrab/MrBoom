// Copyright (c) Timofei Zhakov. All rights reserved.

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

        public Screen Next => Screen.None;

        public SearchingForPlayersScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                         Settings settings, IController currentPlayer)
        {
            matchmakingTask = StartMatchmaking();
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.currentPlayer = currentPlayer;
        }

        private async Task StartMatchmaking()
        {
            PlayFabSettings.staticSettings.TitleId = "E8B53";

            PlayFabResult<LoginResult> login = await PlayFabClientAPI.LoginWithCustomIDAsync(new LoginWithCustomIDRequest
            {
                CustomId = "GettingStartedGuide", // TODO:
                CreateAccount = true
            });

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
                            },
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

                await Task.Delay(6000);
            }
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.MrFond.Draw(ctx, 0, 0);
            string text = "searching for players...";
            Game.DrawString(ctx, (320 - text.Length * 8) / 2, 190, text, assets.Alpha[1]);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
