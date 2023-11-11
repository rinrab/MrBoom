// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MrBoom
{
    public class MultiplayerService
    {
        public int Port { get; private set; }
        public int Ping { get; private set; }

        private byte[] Data;
        private UdpClient client;
        private Task listenTask;
        private Stopwatch pingSw;

        public MultiplayerService()
        {
            Ping = -1;
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
                UdpReceiveResult result = await server.ReceiveAsync();
                var data = result.Buffer;
                if (data[0] == 2)
                {
                    if (client != null)
                    {
                        SendInBackground(new byte[] { 3 });
                    }
                }
                else if (data[0] == 3)
                {
                    if (pingSw != null)
                    {
                        Ping = (int)pingSw.ElapsedMilliseconds;
                    }
                    else
                    {
                        Ping = -1;
                    }
                }
                else
                {
                    Data = result.Buffer;
                }
            }
        }

        public void StartPinging()
        {
            _ = PingAsync();
        }

        private async Task PingAsync()
        {
            while (true)
            {
                pingSw = Stopwatch.StartNew();

                SendInBackground(new byte[]
                {
                    2
                });
                await Task.Delay(200);
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
