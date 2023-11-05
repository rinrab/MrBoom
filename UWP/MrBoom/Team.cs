// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom
{
    public class Team
    {
        public List<IPlayerState> Players;
        public int VictoryCount;

        public string[] Names
        {
            get
            {
                List<string> names = new List<string>();
                foreach (IPlayerState player in Players)
                {
                    names.Add(player.Name);
                }
                return names.ToArray();
            }
        }

        public static IPlayerState[] GetPlayers(IEnumerable<Team> teams)
        {
            List<IPlayerState> players = new List<IPlayerState>();
            foreach (Team team in teams)
            {
                players.AddRange(team.Players);
            }
            return players.ToArray();
        }
    }
}
