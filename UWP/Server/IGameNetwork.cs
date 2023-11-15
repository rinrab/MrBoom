// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;

namespace MrBoom.Server
{
    public delegate void GameNetworkMessageReceivedDelegate(IPEndPoint client, byte[] msg);
    public delegate void ClientConnectedDelegate(IPEndPoint client, byte[] msg);

    public interface IGameNetwork
    {
        string Id { get; }

        Task SendMessage(IEnumerable<IPEndPoint> clients, byte[] message, CancellationToken cancellationToken);
        Task SendMessage(IPEndPoint client, byte[] message, CancellationToken cancellationToken);
        event GameNetworkMessageReceivedDelegate MessageReceived;
        event ClientConnectedDelegate ClientConnected;

        IEnumerable<IPEndPoint> GetAll();
        IEnumerable<IPEndPoint> GetAllExcept(IPEndPoint client);
    }
}
