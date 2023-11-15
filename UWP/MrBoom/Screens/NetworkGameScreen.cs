﻿// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MrBoom.Screens
{
    public class NetworkGameScreen : AbstractGameScreen
    {
        private readonly GameNetworkConnection gameNetworkConnection;
        private int tick = 0;

        public NetworkGameScreen(List<Team> teams, Assets assets, Settings settings,
                                 List<IController> controllers, GameNetworkConnection gameNetworkConnection,
                                 IEnumerable<IPlayerState> players) : base(teams, assets, settings, controllers)
        {
            this.gameNetworkConnection = gameNetworkConnection;
            Terrain.Random = new Random(1);
            terrain = new Terrain(0, assets);

            int i = 0;
            foreach (IPlayerState player in players)
            {
                terrain.AddPlayer(player.GetPlayer(terrain, i));
                i++;
            }
        }

        public override void Update()
        {
            tick++;

            byte[] data = gameNetworkConnection.GetData();

            if (data != null && data[0] == 1)
            {
                NetworkParser.GameData parsedData = NetworkParser.GameData.Parse(new MemoryStream(data));
                terrain.Recieved(parsedData);
            }

            base.Update();

            if (tick % 1 == 0)
            {
                var dataToSend = terrain.GetDataToSend().ToArray();
                gameNetworkConnection.SendInBackground(dataToSend);
            }
        }

        public override string GetAdditionDebugInfo()
        {
            return string.Format("Ping: {0}\n", gameNetworkConnection.Ping);
        }
    }
}
