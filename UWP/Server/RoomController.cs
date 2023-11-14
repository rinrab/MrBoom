// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.AspNetCore.Mvc;

namespace MrBoom.Server
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        [HttpPost("connect")]
        public IActionResult ConnectRoom()
        {
            string roomId = Guid.NewGuid().ToString("N");

            return Ok(new Room
            {
                Id = roomId,
                Hostname = "localhost",
                Port = 7333,
                // TODO: Secret
            });
        }
    }

    public class Room
    {
        public required string Id { get; set; }
        public required string Hostname { get; set; }
        public required int Port { get; set; }
    }
}
