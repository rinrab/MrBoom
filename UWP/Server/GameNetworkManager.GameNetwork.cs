// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MrBoom.NetworkProtocol;

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
                            result.Add(client);
                        }
                    }
                }

                return result;
            }

            public Task SendMessage(IEnumerable<IPEndPoint> clients, ReadOnlyByteSpan message, CancellationToken cancellationToken)
            {
                return networkManager.SendPacket(NetworkPacketType.UnreliableData, networkId, clients, message, cancellationToken);
            }

            public Task SendMessage(IPEndPoint client, ReadOnlyByteSpan message, CancellationToken cancellationToken)
            {
                return networkManager.SendPacket(NetworkPacketType.UnreliableData, networkId, new IPEndPoint[] { client }, message, cancellationToken);
            }

            internal void ProcessPacket(IPEndPoint remoteEndPoint, NetworkPacket packet)
            {
                switch (packet.Type)
                {
                    case NetworkPacketType.ConnectReq:
                        {
                            bool found = false;

                            lock (clients)
                            {
                                foreach (IPEndPoint client in clients)
                                {
                                    if (client.Equals(remoteEndPoint))
                                    {
                                        found = true;
                                    }
                                }

                                if (!found)
                                {
                                    clients.Add(remoteEndPoint);
                                }
                            }

                            if (!found)
                            {
                                ClientConnectedDelegate? clientConnected = ClientConnected;
                                clientConnected?.Invoke(remoteEndPoint, packet.Data);
                            }
                        }
                        break;

                    case NetworkPacketType.UnreliableData:
                        {
                            GameNetworkMessageReceivedDelegate? messageReceived = MessageReceived;
                            messageReceived?.Invoke(remoteEndPoint, packet.Data);
                        }
                        break;

                    case NetworkPacketType.EchoRequest:
                        _ = networkManager.SendPacket(NetworkPacketType.EchoResponse, networkId, new IPEndPoint[] { remoteEndPoint }, packet.Data, default);
                        break;
                }
            }
        }
    }
}
