// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    public class GameServer : IGameServer
    {
        private readonly IGameNetwork gameNetwork;
        private readonly List<Client> clients;
        private readonly CancellationTokenSource stoppingTokenSource;

        public GameServer(IGameNetworkManager networkManager)
        {
            stoppingTokenSource = new CancellationTokenSource();
            clients = new List<Client>();
            gameNetwork = networkManager.CreateNetwork();
            gameNetwork.MessageReceived += GameNetwork_MessageReceived;
            gameNetwork.ClientConnected += GameNetwork_ClientConnected;

            _ = Run(stoppingTokenSource.Token);
        }

        private void GameNetwork_ClientConnected(IPEndPoint client, ReadOnlyByteSpan msg)
        {
            AddPlayerMessage addPlayerMsg;
            using (BinaryReader reader = new BinaryReader(msg.AsStream()))
            {
                addPlayerMsg = AddPlayerMessage.Decode(reader);
            }

            lock (clients)
            {
                clients.Add(new Client
                {
                    Name = addPlayerMsg.Name,
                    EndPoint = client
                });
            }
        }

        private void GameNetwork_MessageReceived(IPEndPoint client, ReadOnlyByteSpan msg)
        {
            // TODO: Secret
            gameNetwork.SendMessage(gameNetwork.GetAllExcept(client), msg, default);
        }

        private async Task Run(CancellationToken stoppingToken)
        {
            while (true)
            {
                await Task.Delay(1000 / 20, stoppingToken);

                foreach (var client in gameNetwork.GetAll())
                {
                    _ = gameNetwork.SendMessage(client, ClientsToBytes(clients, client).AsByteSpan(), stoppingToken);
                }
            }
        }

        private static byte[] ClientsToBytes(List<Client> clients, IPEndPoint endPoint)
        {
            MemoryStream stream = new MemoryStream();

            stream.WriteByte(GameMessageType.ServerConnectedPlayersState); // type

            stream.WriteByte((byte)clients.Count);
            foreach (Client client in clients)
            {
                stream.WriteByte((byte)((client.EndPoint.ToString() == endPoint.ToString()) ? 0 : 1));

                stream.WriteByte((byte)client.Name.Length);
                foreach (char c in client.Name)
                {
                    stream.WriteByte((byte)c);
                }
            }

            return stream.ToArray();
        }

        public IGameNetwork GetNetwork()
        {
            return gameNetwork;
        }

        public void Dispose()
        {
            stoppingTokenSource.Cancel();
        }

        class Client
        {
            public required string Name { get; set; }
            public required IPEndPoint EndPoint { get; set; }
        }
    }
}
