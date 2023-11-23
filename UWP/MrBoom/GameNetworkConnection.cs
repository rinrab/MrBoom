// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MrBoom.NetworkProtocol;

namespace MrBoom
{
    public class GameNetworkConnection
    {
        public delegate void MessageReceivedDelegate(ReadOnlyByteSpan msg);

        public TimeSpan? Ping
        {
            get
            {
                int count = 0;
                TimeSpan sum = TimeSpan.Zero;
                foreach (PingRequests request in pingRequests)
                {
                    if (request.SendTime.HasValue && request.ReceiveTime.HasValue)
                    {
                        sum += request.ReceiveTime.Value - request.SendTime.Value;
                        count++;
                    }
                }

                if (count > 0)
                {
                    return sum / count;
                }
                else
                {
                    return null;
                }
            }
        }
        public event MessageReceivedDelegate MessageReceived;

        private readonly UdpClient udpClient;
        private readonly uint networkId;

        struct PingRequests
        {
            public TimeSpan? SendTime;
            public TimeSpan? ReceiveTime;
        }

        private readonly PingRequests[] pingRequests;
        private UInt64 pingIndex;

        private GameNetworkConnection(string hostname, int port, UInt32 networkId)
        {
            pingRequests = new PingRequests[5];
            pingIndex = 0;
            udpClient = new UdpClient(0);

            udpClient.Connect(hostname, port);
            this.networkId = networkId;
        }

        public static async Task<GameNetworkConnection> Connect(string hostname, int port, UInt32 networkId, byte[] msg)
        {
            GameNetworkConnection connection = new GameNetworkConnection(hostname, port, networkId);

            connection.StartListen();

            // TODO: Wait for ConnectAck.
            await connection.SendPacket(NetworkPacketType.ConnectReq, msg.AsByteSpan());

            connection.StartPing(default);

            return connection;
        }

        private void StartListen()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    UdpReceiveResult response = await udpClient.ReceiveAsync();

                    NetworkPacket packet;
                    try
                    {
                        packet = NetworkPacket.Decode(response.Buffer.AsByteSpan());
                    }
                    catch
                    {
                        continue;
                    }

                    ProcessNetworkPacket(packet);
                }
            });
        }

        private void ProcessNetworkPacket(NetworkPacket packet)
        {
            switch (packet.Type)
            {
                case NetworkPacketType.UnreliableData:
                    MessageReceivedDelegate messageReceivedDelegate = MessageReceived;
                    messageReceivedDelegate?.Invoke(packet.Data);
                    break;

                case NetworkPacketType.EchoResponse:
                    EchoPayload payload;
                    try
                    {
                        payload = EchoPayload.Decode(packet.Data);
                    }
                    catch
                    {
                        return;
                    }

                    pingRequests[payload.TimeStamp % pingRequests.Length].ReceiveTime = TimeSpan.FromTicks(Stopwatch.GetTimestamp() * 10000 * 1000 / Stopwatch.Frequency);

                    break;
            }
        }

        private void StartPing(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    pingIndex++;

                    int idx = (int)(pingIndex % (uint)pingRequests.Length);
                    pingRequests[idx].SendTime = TimeSpan.FromTicks(Stopwatch.GetTimestamp() * 10000 * 1000 / Stopwatch.Frequency);
                    pingRequests[idx].ReceiveTime = null;

                    await SendPacket(NetworkPacketType.EchoRequest, new EchoPayload((long)pingIndex).Encode());

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            });
        }

        public Task SendAsync(byte[] data)
        {
            return SendPacket(NetworkPacketType.UnreliableData, data.AsByteSpan());
        }

        private async Task SendPacket(byte NetworkPacketType, ReadOnlyByteSpan payload)
        {
            NetworkPacket packet = new NetworkPacket(NetworkPacketType, networkId, payload);
            byte[] encodedPacket = packet.Encode().AsArray();

            await udpClient.SendAsync(encodedPacket, encodedPacket.Length);
        }

        public void SendInBackground(byte[] data)
        {
            _ = SendAsync(data);
        }
    }
}
