// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class PlayerState
    {
        public IController Controller;
        public string Name;
        public int VictoryCount;
        public int Index;

        public PlayerState(IController controller, int index)
        {
            Controller = controller;
            Index = index;
        }
    }
}
