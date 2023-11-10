// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MrBoom
{
    public class MultiplayerService
    {
        public byte[] Data { get; private set; }
        public int Port { get; private set; }

        private UdpClient[] clients;
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
            clients = new UdpClient[connectionDatas.Length];
            for (int i = 0; i < connectionDatas.Length; i++)
            {
                clients[i] = new UdpClient(connectionDatas[i].Ip, connectionDatas[i].Port);
            }
        }

        public void StartListen(int port)
        {
            Port = port;
            listenTask = Listen(port);
        }

        private async Task Listen(int port)
        {
            UdpClient server = new UdpClient(port);

            while (true)
            {
                UdpReceiveResult recieved = await server.ReceiveAsync();

                Data = recieved.Buffer;

                await Task.Delay(20);
            }
        }

        public async Task SendAsync(byte[] data)
        {
            foreach (var client in clients)
            {
                await client.SendAsync(data, data.Length);
            }
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
