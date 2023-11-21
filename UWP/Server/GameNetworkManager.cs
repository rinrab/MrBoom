// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    internal partial class GameNetworkManager : IGameNetworkManager
    {
        private readonly Dictionary<UInt32, GameNetwork> networks;
        private volatile UInt32 nextNetworkId;
        private readonly IUdpServer udpServer;

        public GameNetworkManager(IUdpServer udpServer)
        {
            networks = new Dictionary<UInt32, GameNetwork>();

            udpServer.OnMesssageReceived += UdpServer_OnMesssageReceived;
            this.udpServer = udpServer;
        }

        private void UdpServer_OnMesssageReceived(UdpReceiveResult msg)
        {
            GameNetwork? network = null;

            NetworkPacket packet;
            try
            {
                packet = NetworkPacket.Decode(msg.Buffer.AsByteSpan());
            }
            catch
            {
                return;
            }

            lock (networks)
            {
                if (!networks.TryGetValue(packet.NetworkId, out network))
                {
                    network = null;
                }
            }

            if (network != null)
            {
                network.ProcessPacket(msg.RemoteEndPoint, packet);
            }
        }

        private async Task SendPacket(byte packetType, UInt32 networkId, IEnumerable<IPEndPoint> clients, ReadOnlyByteSpan message, CancellationToken cancellationToken)
        {
            NetworkPacket packet = new NetworkPacket(packetType, networkId, message);
            ReadOnlyByteSpan encodedPacket = packet.Encode();

            List<Task> tasks = new List<Task>();

            foreach (IPEndPoint client in clients)
            {
                tasks.Add(udpServer.SendMessage(encodedPacket.AsArray(), client, cancellationToken));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        public IGameNetwork CreateNetwork()
        {
            UInt32 networkId = Interlocked.Increment(ref nextNetworkId);
            GameNetwork network = new GameNetwork(this, networkId);

            lock(networks)
            {
                networks.Add(network.Id, network);
            }

            return network;
        }
    }
}
