// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    internal partial class GameNetworkManager : IGameNetworkManager
    {
        private Dictionary<string, GameNetwork> networks;
        private readonly IUdpServer udpServer;

        public GameNetworkManager(IUdpServer udpServer)
        {
            networks = new Dictionary<string, GameNetwork>();

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
                // TODO: Add NetworkId to packet.
                foreach(var net in networks.Values)
                {
                    network = net;
                    break;
                }
            }

            if (network != null)
            {
                network.ProcessPacket(msg.RemoteEndPoint, packet);
            }
        }

        private async Task SendPacket(byte packetType, string networkId, IEnumerable<IPEndPoint> clients, ReadOnlyByteSpan message, CancellationToken cancellationToken)
        {
            NetworkPacket packet = new NetworkPacket(packetType, message);
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
            GameNetwork network = new GameNetwork(this);

            lock(networks)
            {
                networks.Add(network.Id, network);
            }

            return network;
        }
    }
}
