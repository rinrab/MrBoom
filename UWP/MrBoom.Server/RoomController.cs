// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using Microsoft.AspNetCore.Mvc;

namespace MrBoom.Server
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        private readonly IGameServerManager gameServerManager;

        public RoomController(IGameServerManager gameServerManager) 
        {
            this.gameServerManager = gameServerManager;
        }

        [HttpPost("connect")]
        public IActionResult ConnectRoom()
        {
            IGameServer gameServer = gameServerManager.FindOrCreateGameServer();

            return Ok(new Room
            {
                Id = gameServer.GetNetwork().Id,
                Hostname = "localhost",
                Port = 7333,
                // TODO: Secret
            });
        }
    }

    public class Room
    {
        public required UInt32 Id { get; set; }
        public required string Hostname { get; set; }
        public required int Port { get; set; }
    }
}
