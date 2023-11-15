// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSingleton<IUdpServer, UdpServer>();
            builder.Services.AddHostedService(serviceProvider => (UdpServer)serviceProvider.GetRequiredService<IUdpServer>());
            builder.Services.AddSingleton<IGameServer, GameServer>();
            builder.Services.AddHostedService(serviceProvider => (GameServer)serviceProvider.GetRequiredService<IGameServer>());
            builder.Services.AddSingleton<IGameNetworkManager, GameNetworkManager>();

            WebApplication app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
