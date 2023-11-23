// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using MrBoom.NetworkProtocol;

namespace MrBoom.Screens
{
    public class NetworkGameScreen : AbstractGameScreen
    {
        private readonly GameNetworkConnection gameNetworkConnection;
        private volatile ClientGameStateMessage.GameData lastGameData;

        public NetworkGameScreen(List<Team> teams, Assets assets, Settings settings,
                                 List<IController> controllers, GameNetworkConnection gameNetworkConnection,
                                 IEnumerable<IPlayerState> players) : base(teams, assets, settings, controllers)
        {
            this.gameNetworkConnection = gameNetworkConnection;
            this.gameNetworkConnection.MessageReceived += GameNetworkConnection_MessageReceived;

            terrain = new Terrain(0, assets, 1);

            int i = 0;
            foreach (IPlayerState player in players)
            {
                terrain.AddPlayer(player.CreatePlayerObject(terrain, i));
                i++;
            }
        }

        private void GameNetworkConnection_MessageReceived(ReadOnlyByteSpan msg)
        {
            try
            {
                if (msg[0] == GameMessageType.ClientGameState)
                {
                    Interlocked.Exchange(ref lastGameData, ClientGameStateMessage.GameData.Decode(msg));
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
            ClientGameStateMessage.GameData parsedData = Interlocked.Exchange(ref lastGameData, null);
            if (parsedData != null)
            {
                terrain.Recieved(parsedData);
            }

            base.OnUpdate();

            if (CurrentTick % 1 == 0)
            {
                ReadOnlyByteSpan dataToSend = terrain.GetDataToSend().Encode();
                gameNetworkConnection.SendInBackground(dataToSend.AsArray());
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
