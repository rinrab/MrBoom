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

        public TimeSpan? Ping { get; private set; }
        public event MessageReceivedDelegate MessageReceived;

        private readonly UdpClient udpClient;
        private readonly uint networkId;

        private GameNetworkConnection(string hostname, int port, UInt32 networkId)
        {
            Ping = null;

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

                    Ping = TimeSpan.FromTicks((Stopwatch.GetTimestamp() - payload.TimeStamp) * 10000 * 1000 / Stopwatch.Frequency);
                    break;
            }
        }

        private void StartPing(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ReadOnlyByteSpan payload = new EchoPayload(Stopwatch.GetTimestamp()).Encode();

                    await Task.WhenAll(SendPacket(NetworkPacketType.EchoRequest, payload),
                                       Task.Delay(TimeSpan.FromSeconds(1), cancellationToken));
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
