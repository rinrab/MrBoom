﻿// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class PlayerState
    {
        public enum PlayerType
        {
            Human,
            Bot
        }

        public IController Controller;
        public string Name;
        public int VictoryCount;
        public int Index;
        public PlayerType Type;

        public PlayerState(IController controller, int index, PlayerType type, string name)
        {
            Controller = controller;
            Index = index;
            Type = type;
            Name = name;
        }
    }
}
