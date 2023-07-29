﻿namespace MrBoom
{
    public class PlayerState
    {
        public IController Controller;
        public string Name;
        public int VictoryCount;

        public PlayerState(IController controller)
        {
            Controller = controller;
        }
    }
}