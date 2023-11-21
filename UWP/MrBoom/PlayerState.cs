// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Bot;

namespace MrBoom
{
    public interface IPlayerState
    {
        int Index { get; }
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        AbstractPlayer CreatePlayerObject(Terrain terrain, int team);
    }

    public class HumanPlayerState : IPlayerState
    {
        public IController Controller { get; }
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public HumanPlayerState(IController controller, int index, string name)
        {
            Controller = controller;
            Index = index;
            Name = name;
        }

        public AbstractPlayer CreatePlayerObject(Terrain terrain, int team)
        {
            return new Human(terrain, terrain.assets.Players[Index], Controller, team);
        }
    }

    public class BotPlayerState : IPlayerState
    {
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        public BotPlayerState(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public AbstractPlayer CreatePlayerObject(Terrain terrain, int team)
        {
            return new ComputerPlayer(terrain, terrain.assets.Players[Index], team, Index);
        }
    }

    public class RemotePlayerState : IPlayerState
    {
        public int Index { get; }
        public int RemoteIndex { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        public RemotePlayerState(int index, int remoteIndex, string name)
        {
            Index = index;
            RemoteIndex = remoteIndex;
            Name = name;
        }

        public AbstractPlayer CreatePlayerObject(Terrain terrain, int team)
        {
            return new RemotePlayer(terrain, terrain.assets.Players[Index], team);
        }
    }
}
