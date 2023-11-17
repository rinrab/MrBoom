// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using Microsoft.AspNetCore.Mvc;

namespace MrBoom.Server
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        private readonly IGameServer gameServer;

        public RoomController(IGameServer gameServer) 
        {
            this.gameServer = gameServer;
        }

        [HttpPost("connect")]
        public IActionResult ConnectRoom()
        {
            IGameNetwork network = gameServer.GetNetwork();

            return Ok(new Room
            {
                Id = network.Id,
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
