// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.Server
{
    public interface IGameServer : IDisposable
    {
        IGameNetwork GetNetwork();
    }
}
