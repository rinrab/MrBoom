// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;

namespace MrBoom.Server
{
    internal partial class GameNetworkManager
    {
        private class GameNetwork : IGameNetwork
        {
            private readonly string networkId;
            private readonly GameNetworkManager networkManager;
            private readonly List<IPEndPoint> clients;

            public GameNetwork(GameNetworkManager networkManager)
            {
                networkId = Guid.NewGuid().ToString("N");
                this.networkManager = networkManager;
                clients = new List<IPEndPoint>();
            }

            public string Id
            {
                get
                {
                    return networkId;
                }
            }

            public event GameNetworkMessageReceivedDelegate? MessageReceived;
            public event ClientConnectedDelegate? ClientConnected;

            public IEnumerable<IPEndPoint> GetAll()
            {
                var result = new List<IPEndPoint>();

                lock (clients)
                {
                    result.AddRange(clients);
                }

                return result;
            }

            public IEnumerable<IPEndPoint> GetAllExcept(IPEndPoint exclude)
            {
                var result = new List<IPEndPoint>();

                lock (clients)
                {
                    foreach (IPEndPoint client in clients)
                    {
                        if (!client.Equals(exclude))
                        {
                            result.Add(exclude);
                        }
                    }
                }

                return result;
            }

            public Task SendMessage(IEnumerable<IPEndPoint> clients, byte[] message, CancellationToken cancellationToken)
            {
                return networkManager.SendMessage(networkId, clients, message, cancellationToken);
            }

            public Task SendMessage(IPEndPoint client, byte[] message, CancellationToken cancellationToken)
            {
                return networkManager.SendMessage(networkId, new IPEndPoint[] { client }, message, cancellationToken);
            }

            internal void ProcessMessage(UdpReceiveResult msg)
            {
                if (msg.Buffer.Length >= 1 && msg.Buffer[0] == 2)
                {
                    bool found = false;

                    lock (clients)
                    {
                        foreach (IPEndPoint client in clients)
                        {
                            if (client.Equals(msg.RemoteEndPoint))
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            clients.Add(msg.RemoteEndPoint);
                        }
                    }

                    if (!found)
                    {
                        ClientConnectedDelegate? clientConnected = ClientConnected;
                        clientConnected?.Invoke(msg.RemoteEndPoint, msg.Buffer);
                    }
                }
                else
                {
                    GameNetworkMessageReceivedDelegate? messageReceived = MessageReceived;
                    messageReceived?.Invoke(msg.RemoteEndPoint, msg.Buffer);
                }
            }
        }
    }
}
