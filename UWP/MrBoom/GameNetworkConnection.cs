// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using MrBoom.NetworkProtocol;

namespace MrBoom
{
    public class GameNetworkConnection
    {
        public int Ping { get; private set; }

        private byte[] Data;
        private readonly UdpClient udpClient;

        private GameNetworkConnection(string hostname, int port)
        {
            Ping = -1;

            udpClient = new UdpClient(0);

            udpClient.Connect(hostname, port);
        }

        public static async Task<GameNetworkConnection> Connect(string hostname, int port, byte[] msg)
        {
            GameNetworkConnection connection = new GameNetworkConnection(hostname, port);

            connection.StartListen();

            // TODO: Wait for ConnectAck.
            await connection.SendPacket(NetworkPacketType.ConnectReq, msg.AsByteSpan());

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

                    switch(packet.Type)
                    {
                        case NetworkPacketType.UnreliableData:
                            // TODO: Make data ReadOnlyByteSpan.
                            Data = packet.Data.AsArray();
                            break;
                    }
                }
            });
        }

        public Task SendAsync(byte[] data)
        {
            return SendPacket(NetworkPacketType.UnreliableData, data.AsByteSpan());
        }

        private async Task SendPacket(byte NetworkPacketType, ReadOnlyByteSpan payload)
        {
            NetworkPacket packet = new NetworkPacket(NetworkPacketType, payload);
            byte[] encodedPacket = packet.Encode().AsArray();

            await udpClient.SendAsync(encodedPacket, encodedPacket.Length);
        }

        public void SendInBackground(byte[] data)
        {
            _ = SendAsync(data);
        }

        public byte[] GetData()
        {
            byte[] tmp = Data;
            Data = null;
            return tmp;
        }

        //public void StartPinging()
        //{
        //    _ = PingAsync();
        //}

        //private async Task PingAsync()
        //{
        //    while (true)
        //    {
        //        pingSw = Stopwatch.StartNew();

        //        SendInBackground(new byte[]
        //        {
        //            2
        //        });
        //        await Task.Delay(200);
        //    }
        //}
    }
}
