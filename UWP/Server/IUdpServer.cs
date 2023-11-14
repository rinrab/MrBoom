// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;

namespace MrBoom.Server
{
    public delegate void MessageReceivedDelegate(UdpReceiveResult msg);

    public interface IUdpServer
    {
        Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken);

        event MessageReceivedDelegate OnMesssageReceived;
    }
}
