// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net.Sockets;
using System.Threading.Tasks;

namespace MrBoom
{
    public class GameNetworkConnection
    {
        public int Ping { get; private set; }

        private byte[] Data;
        private readonly UdpClient udpClient;

        public GameNetworkConnection(string hostname, int port)
        {
            Ping = -1;

            udpClient = new UdpClient(0);

            udpClient.Connect(hostname, port);
        }

        public void StartListen()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    UdpReceiveResult response = await udpClient.ReceiveAsync();
                    Data = response.Buffer;
                }
            });
        }

        public async Task SendAsync(byte[] data)
        {
            await udpClient.SendAsync(data, data.Length);
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
