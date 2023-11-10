// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MrBoom
{
    public class MultiplayerService
    {
        private byte[] Data;
        public int Port { get; private set; }

        private UdpClient client;
        private Task listenTask;

        public MultiplayerService()
        {
        }

        public string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void Connect(PlayerConnectionData[] connectionDatas)
        {
            client = new UdpClient(connectionDatas[0].Ip, connectionDatas[0].Port);
        }

        public void StartListen(int port)
        {
            Port = port;
            _ = ListenAsync();
        }

        private async Task ListenAsync()
        {
            UdpClient server = new UdpClient(Port);

            while (true)
            {
                var result = await server.ReceiveAsync();
                Data = result.Buffer;
            }
        }

        public async Task SendAsync(byte[] data)
        {
            await client.SendAsync(data, data.Length);
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
    }

    public class PlayerConnectionData
    {
        public string Ip;
        public int Port;

        public PlayerConnectionData(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }
}
