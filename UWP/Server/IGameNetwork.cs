// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MrBoom.Server
{
    public delegate void GameNetworkMessageReceivedDelegate(IPEndPoint client, ReadOnlyByteSpan msg);
    public delegate void ClientConnectedDelegate(IPEndPoint client, ReadOnlyByteSpan msg);

    public interface IGameNetwork
    {
        string Id { get; }

        Task SendMessage(IEnumerable<IPEndPoint> clients, ReadOnlyByteSpan msg, CancellationToken cancellationToken);
        Task SendMessage(IPEndPoint client, ReadOnlyByteSpan msg, CancellationToken cancellationToken);
        event GameNetworkMessageReceivedDelegate MessageReceived;
        event ClientConnectedDelegate ClientConnected;

        IEnumerable<IPEndPoint> GetAll();
        IEnumerable<IPEndPoint> GetAllExcept(IPEndPoint client);
    }
}
