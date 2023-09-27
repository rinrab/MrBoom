// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom
{
    public enum TeamMode
    {
        Off,
        Color,
        Sex
    }

    public class Team
    {
        public static TeamMode Mode;

        public List<PlayerState> Players;
        public int VictoryCount;

        public string[] Names
        {
            get
            {
                List<string> names = new List<string>();
                foreach (PlayerState player in Players)
                {
                    names.Add(player.Name);
                }
                return names.ToArray();
            }
        }

        public static PlayerState[] GetPlayers(IEnumerable<Team> teams)
        {
            List<PlayerState> players = new List<PlayerState>();
            foreach (Team team in teams)
            {
                players.AddRange(team.Players);
            }
            return players.ToArray();
        }
    }
}
