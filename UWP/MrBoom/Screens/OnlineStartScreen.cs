// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace MrBoom
{
    public class OnlineStartScreen : IScreen
    {
        private readonly Assets assets;
        private readonly List<Team> teams;
        private readonly List<IController> controllers;
        private readonly Settings settings;
        private readonly List<HumanPlayerState> players;
        private readonly List<IController> unjoinedControllers;
        private readonly List<IController> joinedControllers;
        private readonly NameGenerator nameGenerator;
        private int tick;
        private int state = 0;

        public OnlineStartScreen(Assets assets, List<Team> teams, List<IController> controllers,
                                 List<HumanPlayerState> players, Settings settings)
        {
            this.assets = assets;
            this.teams = teams;
            this.controllers = controllers;
            this.players = players;
            this.settings = settings;
            unjoinedControllers = new List<IController>(controllers);
            joinedControllers = new List<IController>();
            foreach (HumanPlayerState playerState in players)
            {
                unjoinedControllers.Remove(playerState.Controller);
                joinedControllers.Add(playerState.Controller);
            }
            nameGenerator = new NameGenerator(Terrain.Random);
        }

        public void Update()
        {
            if (state == 0)
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
                else if (Controller.IsKeyDown(controllers, PlayerKeys.Back))
                {
                    Controller.Reset(controllers);
                    ScreenManager.SetScreen(new DemoScreen(teams, assets, settings, controllers));
                }
            }

            tick++;
        }

        private void Start()
        {
            if (players.Count > 0)
            {
                assets.Sounds.Bang.Play();
                state = 1;

                Task.Run(async () =>
                {
                    try
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://localhost:5191");

                        HttpContent content = new StringContent(JsonConvert.SerializeObject(new
                        {
                        }));

                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        HttpResponseMessage res = await client.PostAsync("/room/connect", content);

                        Room room = JsonConvert.DeserializeObject<Room>(await res.Content.ReadAsStringAsync());

                        GameNetworkConnection gameNetworkConnection = GameNetworkConnection.Connect(room.Hostname, room.Port);

                        ScreenManager.SetScreen(new OnlinePlayerListScreen(assets, teams, controllers, settings, players, gameNetworkConnection));
                    }
                    catch (Exception ex)
                    {
                        await MessageBox.Show("Cannot connect to the server.", ex.Message, new string[] { "Ok" });

                        state = 0;
                    }
                });

            }
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.OnlineMenu.Draw(ctx, 0, 0);
            
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

            if (state == 0)
            {
                string text = "push   !!";
                assets.Controls[0].Draw(ctx, 320 / 2, 181);
                Game.DrawString(ctx, (320 - text.Length * 8) / 2, 186, text, assets.Alpha[1]);
            }
            else
            {
                ctx.Draw(assets.BlackPixel, new Rectangle(0, 0, 640, 400), new Rectangle(0, 0, 1, 1), Color.White * 0.5f);
                string text = "loading...";
                Game.DrawString(ctx, (320 - text.Length * 8) / 2, 186, text, assets.Alpha[1]);
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
