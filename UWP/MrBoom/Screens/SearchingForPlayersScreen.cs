// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MrBoom.NetworkProtocol;
using Newtonsoft.Json;

namespace MrBoom
{
    public class SearchingForPlayersScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly List<HumanPlayerState> players;
        private string status;

        public SearchingForPlayersScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                         Settings settings, List<HumanPlayerState> players)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.settings = settings;
            this.players = players;
            status = "finding room";

            Task.Run(() => StartMatchmaking());
        }

        private async Task StartMatchmaking()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5191");

                HttpContent content = new StringContent(JsonConvert.SerializeObject(new
                {
                    Name = new NameGenerator(Terrain.Random).GenerateName(),
                }));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage res = await client.PostAsync("/room/connect", content);

                Room room = JsonConvert.DeserializeObject<Room>(await res.Content.ReadAsStringAsync());

                MemoryStream stream = new MemoryStream();
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    new AddPlayerMessage()
                    {
                        Name = players[0].Name
                    }.Encode(writer);
                }

                GameNetworkConnection gameNetworkConnection = await GameNetworkConnection.Connect(room.Hostname, room.Port, stream.ToArray());

                ScreenManager.SetScreen(new OnlinePlayerListScreen(assets, teams, controllers, settings, players, gameNetworkConnection));
            }
            catch (Exception ex)
            {
                await MessageBox.Show("Cannot connect to the server.", ex.Message, new string[]
                {
                    "Ok"
                });
                ScreenManager.SetScreen(new OnlineStartScreen(assets, teams, controllers, players, settings));
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

        private class Room
        {
            public string Id { get; set; }
            public string Hostname { get; set; }
            public int Port { get; set; }
        };
    }
}
