// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;

namespace MrBoom.Server
{
    public class UdpServer : BackgroundService, IUdpServer
    {
        private readonly ILogger<UdpServer> logger;
        // TOOD: Add configuration.
        private readonly int port = 7333;
        private UdpClient udpClient;

        public UdpServer(ILogger<UdpServer> logger)
        {
            this.logger = logger;
        }

        public event MessageReceivedDelegate OnMesssageReceived;

        public async Task SendMessage(byte[] msg, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            await udpClient.SendAsync(msg, endPoint, cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("UdpServer is starting");

            try
            {
                using (UdpClient udpClient = new UdpClient(port))
                {
                    this.udpClient = udpClient;

                    logger.LogInformation("Udp server binded to port {ListenPort}", port);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        UdpReceiveResult msg = await udpClient.ReceiveAsync(stoppingToken);
                        logger.LogDebug("Received message from {RemoteEndPoint}", msg.RemoteEndPoint);

                        MessageReceivedDelegate handler = OnMesssageReceived;
                        handler?.Invoke(msg);
                    }
                }
            }
            catch (Exception)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
            }

            logger.LogInformation("UdpServer stopped");
        }
    }
}
