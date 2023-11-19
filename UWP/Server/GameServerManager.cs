// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MrBoom.Server
{
    public class GameServerManager : BackgroundService, IGameServerManager
    {
        private GameServer gameServer;
        private IGameNetwork lobbyNetwork;

        public GameServerManager(IGameNetworkManager networkManager)
        {
            lobbyNetwork = networkManager.CreateNetwork();
            gameServer = new GameServer(networkManager);
        }

        public IGameServer FindOrCreateGameServer()
        {
            return gameServer;
        }

        public IGameNetwork GetLobbyNetwork()
        {
            return lobbyNetwork;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => gameServer.Dispose());
        }
    }
}
