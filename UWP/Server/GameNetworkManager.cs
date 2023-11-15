// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;

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
                network.ProcessMessage(msg);
            }
        }

        private async Task SendMessage(string networkId, IEnumerable<IPEndPoint> clients, byte[] message, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();

            foreach (IPEndPoint client in clients)
            {
                tasks.Add(udpServer.SendMessage(message, client, cancellationToken));
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
