// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
            builder.Services.AddSingleton<IGameServerManager, GameServerManager>();
            builder.Services.AddHostedService(serviceProvider => (GameServerManager)serviceProvider.GetRequiredService<IGameServerManager>());
            builder.Services.AddSingleton<IGameNetworkManager, GameNetworkManager>();

            WebApplication app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
