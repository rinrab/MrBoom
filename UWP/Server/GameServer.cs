// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    public class GameServer : BackgroundService, IGameServer
    {
        private readonly IGameNetwork gameNetwork;
        private readonly List<Client> clients;

        public GameServer(IGameNetworkManager networkManager)
        {
            clients = new List<Client>();
            gameNetwork = networkManager.CreateNetwork();
            gameNetwork.MessageReceived += GameNetwork_MessageReceived;
            gameNetwork.ClientConnected += GameNetwork_ClientConnected;
        }

        private void GameNetwork_ClientConnected(IPEndPoint client, byte[] msg)
        {
            AddPlayerMessage addPlayerMsg;
            using (BinaryReader reader = new BinaryReader(new MemoryStream(msg)))
            {
                byte messageType = reader.ReadByte();

                addPlayerMsg = AddPlayerMessage.Decode(reader);
            }

            lock(clients)
            {
                clients.Add(new Client
                {
                    Name = addPlayerMsg.Name,
                    EndPoint = client
                });
            }
        }

        private void GameNetwork_MessageReceived(IPEndPoint client, byte[] msg)
        {
            // TODO: Secret
            gameNetwork.SendMessage(gameNetwork.GetAllExcept(client), msg, default);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await Task.Delay(1000 / 20, stoppingToken);

                foreach (var client in gameNetwork.GetAll())
                {
                     _ = gameNetwork.SendMessage(client, ClientsToBytes(clients, client), stoppingToken);
                }
            }
        }

        private static byte[] ClientsToBytes(List<Client> clients, IPEndPoint endPoint)
        {
            MemoryStream stream = new MemoryStream();

            stream.WriteByte(0); // type

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

        class Client
        {
            public required string Name { get; set; }
            public required IPEndPoint EndPoint { get; set; }
        }
    }
}
