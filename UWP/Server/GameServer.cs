// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;

namespace MrBoom.Server
{
    public class GameServer : BackgroundService, IGameServer
    {
        private readonly IUdpServer udpServer;

        private readonly List<Client> clients;

        public GameServer(IUdpServer udpServer)
        {
            this.udpServer = udpServer;
            clients = new List<Client>();

            udpServer.OnMesssageReceived += UdpServer_OnMesssageReceived;
        }

        private void UdpServer_OnMesssageReceived(UdpReceiveResult msg)
        {
            List<IPEndPoint> toSend = new List<IPEndPoint>();
            bool found = false;

            lock (clients)
            {
                foreach (Client client in clients)
                {
                    if (client.EndPoint.ToString() == msg.RemoteEndPoint.ToString())
                    {
                        found = true;
                    }
                    else
                    {
                        toSend.Add(client.EndPoint);
                    }
                }

                if (!found)
                {
                    clients.Add(new Client
                    {
                        EndPoint = msg.RemoteEndPoint,
                        Name = "name",
                    });
                }
            }

            foreach (IPEndPoint client in toSend)
            {
                udpServer.SendMessage(msg.Buffer, client, default);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        }

        class Client
        {
            public required string Name { get; set; }
            public required IPEndPoint EndPoint { get; set; }
        }
    }
}
