// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server
{
    public interface IGameServerManager
    {
        IGameServer FindOrCreateGameServer();
    }
}
