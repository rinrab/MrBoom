// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Text;

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

            if (msg.Buffer.Length >= 1 && msg.Buffer[0] == 2)
            {
                StringBuilder name = new StringBuilder();
                for (int i = 1; i < msg.Buffer.Length; i++)
                {
                    name.Append((char)msg.Buffer[i]);
                }

                bool found = false;
                lock (clients)
                {
                    foreach (Client client in clients)
                    {
                        if (client.EndPoint.ToString() == msg.RemoteEndPoint.ToString())
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        clients.Add(new Client
                        {
                            EndPoint = msg.RemoteEndPoint,
                            Name = name.ToString(),
                        });
                    }
                }
            }
            else
            {
                // TODO: Secret

                lock (clients)
                {
                    foreach (Client client in clients)
                    {
                        if (client.EndPoint.ToString() != msg.RemoteEndPoint.ToString())
                        {
                            toSend.Add(client.EndPoint);
                        }
                    }
                }

                foreach (IPEndPoint client in toSend)
                {
                    udpServer.SendMessage(msg.Buffer, client, default);
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await Task.Delay(1000 / 20, stoppingToken);

                lock (clients)
                {
                    foreach (Client client in clients)
                    {
                        _ = udpServer.SendMessage(ClientsToBytes(clients, client.EndPoint), client.EndPoint, stoppingToken);
                    }
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

        class Client
        {
            public required string Name { get; set; }
            public required IPEndPoint EndPoint { get; set; }
        }
    }
}
