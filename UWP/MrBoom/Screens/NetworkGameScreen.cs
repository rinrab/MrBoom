// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MrBoom.Screens
{
    public class NetworkGameScreen : AbstractGameScreen
    {
        private readonly MultiplayerService multiplayerService;
        private int tick = 0;

        public NetworkGameScreen(List<Team> teams, Assets assets, Settings settings, List<IController> controllers,
                                 Game game, MultiplayerService multiplayerService) : base(teams, assets, settings, controllers, game)
        {
            this.multiplayerService = multiplayerService;
            Terrain.Random = new Random(1);
            terrain = new Terrain(0, assets);

            terrain.AddPlayer(new RemotePlayer(terrain, assets.Players[0], 0));
            terrain.AddPlayer(new Human(terrain, assets.Players[1], controllers[0], 1));
        }

        public override void Update()
        {
            tick++;

            byte[] data = multiplayerService.GetData();

            if (data != null)
            {
                terrain.Recieved(data);
            }

            base.Update();

            if (tick % 1 == 0)
            {
                multiplayerService.SendInBackground(terrain.GetDataToSend());
            }
        }
    }
}
