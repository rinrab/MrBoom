// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace MrBoom.Screens
{
    public class NetworkGameScreen : AbstractGameScreen
    {
        private readonly GameNetworkConnection gameNetworkConnection;
        private volatile NetworkParser.GameData lastGameData;

        public NetworkGameScreen(List<Team> teams, Assets assets, Settings settings,
                                 List<IController> controllers, GameNetworkConnection gameNetworkConnection,
                                 IEnumerable<IPlayerState> players) : base(teams, assets, settings, controllers)
        {
            this.gameNetworkConnection = gameNetworkConnection;
            this.gameNetworkConnection.MessageReceived += GameNetworkConnection_MessageReceived;

            Terrain.Random = new Random(1);
            terrain = new Terrain(0, assets);

            int i = 0;
            foreach (IPlayerState player in players)
            {
                terrain.AddPlayer(player.GetPlayer(terrain, i));
                i++;
            }
        }

        private void GameNetworkConnection_MessageReceived(ReadOnlyByteSpan msg)
        {
            try
            {
                byte[] data = msg.AsArray();

                if (data != null && data[0] == 1)
                {
                    Interlocked.Exchange(ref lastGameData, NetworkParser.GameData.Parse(new MemoryStream(data)));
                }
            }
            catch(Exception ex)
            {
                // Ignore invalid messages.
                Debug.WriteLine("Unexpecteted error while processing message from the server: {0}", ex.Message);
            }
        }

        protected override void OnUpdate()
        {
            NetworkParser.GameData parsedData = Interlocked.Exchange(ref lastGameData, null);
            if (parsedData != null)
            {
                terrain.Recieved(parsedData);
            }

            base.OnUpdate();

            if (CurrentTick % 1 == 0)
            {
                var dataToSend = terrain.GetDataToSend().ToArray();
                gameNetworkConnection.SendInBackground(dataToSend);
            }
        }

        protected override void OnDispose()
        {
            gameNetworkConnection.MessageReceived -= GameNetworkConnection_MessageReceived;

            base.OnDispose();
        }

        public override string GetAdditionDebugInfo()
        {
            return string.Format("Ping: {0}\n", gameNetworkConnection.Ping);
        }
    }
}
