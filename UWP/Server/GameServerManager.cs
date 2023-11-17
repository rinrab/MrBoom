// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MrBoom.Server
{
    public class GameServerManager : BackgroundService, IGameServerManager
    {
        private GameServer gameServer;

        public GameServerManager(IGameNetworkManager networkManager)
        {
            gameServer = new GameServer(networkManager);
        }

        public IGameServer FindOrCreateGameServer()
        {
            return gameServer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => gameServer.Dispose());
        }
    }
}
