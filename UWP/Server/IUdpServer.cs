// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MrBoom.Server
{
    public delegate void MessageReceivedDelegate(UdpReceiveResult msg);

    public interface IUdpServer
    {
        Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken);

        event MessageReceivedDelegate OnMesssageReceived;
    }
}
