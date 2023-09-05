// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class PlayerState
    {
        public enum Type
        {
            Human,
            Bot
        }

        public IController Controller;
        public string Name;
        public int VictoryCount;
        public int Index;
        public Type type;

        public PlayerState(IController controller, int index, Type type, string name)
        {
            Controller = controller;
            Index = index;
            this.type = type;
            Name = name;
        }
    }
}
