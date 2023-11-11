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

        public NetworkGameScreen(List<Team> teams, Assets assets, Settings settings,
                                 List<IController> controllers, Game game, MultiplayerService multiplayerService,
                                 IEnumerable<IPlayerState> players) : base(teams, assets, settings, controllers, game)
        {
            this.multiplayerService = multiplayerService;
            Terrain.Random = new Random(1);
            terrain = new Terrain(0, assets);

            int i = 0;
            foreach (IPlayerState player in players)
            {
                terrain.AddPlayer(player.GetPlayer(terrain, i));
                i++;
            }
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
                var dataToSend = terrain.GetDataToSend().ToArray();
                multiplayerService.SendInBackground(dataToSend);
            }
        }
    }
}
