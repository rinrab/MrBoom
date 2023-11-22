// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.AspNetCore.Mvc;

namespace MrBoom.Server
{
    [ApiController]
    [Route("lobby")]
    public class LobbyController : ControllerBase
    {
        private readonly IGameServerManager gameServerManager;

        public LobbyController(IGameServerManager gameServerManager)
        {
            this.gameServerManager = gameServerManager;
        }

        [HttpGet()]
        public IActionResult GetLobbyNetwork()
        {
            IGameNetwork lobbyNetwork = gameServerManager.GetLobbyNetwork();

            return Ok(new LobbyNetwork
            {
                Id = lobbyNetwork.Id,
                Hostname = "localhost",
                Port = 7333,
                // TODO: Secret
            });
        }

        private class LobbyNetwork
        {
            public required uint Id { get; set; }
            public required string Hostname { get; set; }
            public required int Port { get; set; }
        }
    }
}
