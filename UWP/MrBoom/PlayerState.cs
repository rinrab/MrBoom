// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Bot;

namespace MrBoom
{
    public interface IPlayerState
    {
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        AbstractPlayer CreatePlayerObject(Terrain terrain, Assets.MovingSpriteAssets assets, int team);
    }

    public class HumanPlayerState : IPlayerState
    {
        public IController Controller { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public HumanPlayerState(IController controller, string name)
        {
            Controller = controller;
            Name = name;
        }

        public AbstractPlayer CreatePlayerObject(Terrain terrain, Assets.MovingSpriteAssets assets, int team)
        {
            return new Human(terrain, assets, Controller, team);
        }
    }

    public class BotPlayerState : IPlayerState
    {
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        // TODO: Remove this property
        private readonly int index;

        public BotPlayerState(int index, string name)
        {
            this.index = index;
            Name = name;
        }

        public AbstractPlayer CreatePlayerObject(Terrain terrain, Assets.MovingSpriteAssets assets, int team)
        {
            return new ComputerPlayer(terrain, assets, team, index);
        }
    }

    public class RemotePlayerState : IPlayerState
    {
        public int RemoteIndex { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        public RemotePlayerState(int remoteIndex, string name)
        {
            RemoteIndex = remoteIndex;
            Name = name;
        }

        public AbstractPlayer CreatePlayerObject(Terrain terrain, Assets.MovingSpriteAssets assets, int team)
        {
            return new RemotePlayer(terrain, assets, team);
        }
    }
}
