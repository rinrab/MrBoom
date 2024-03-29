﻿// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Human : AbstractPlayer
    {
        public readonly IController Controller;

        public Human(Terrain map,
                     Assets.MovingSpriteAssets animations,
                     IController controller,
                     int team) : base(map, animations, team)
        {
            Controller = controller;
        }

        public override string GetDebugInfo()
        {
            string position = string.Format("({0,3},{1,3})/({2,2},{3,2})", X, Y, (X + 8) / 16, (Y + 8) / 16);
            if (IsDie)
            {
                position = "DEAD";
            }

            return $"H:{position}";
        }

        public override void Update()
        {
            Direction = null;
            if (Controller.IsKeyDown(PlayerKeys.Up))
            {
                Direction = Directions.Up;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Left))
            {
                Direction = Directions.Left;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Right))
            {
                Direction = Directions.Right;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Down))
            {
                Direction = Directions.Down;
            }
            dropBombButton = Controller.IsKeyDown(PlayerKeys.Bomb);
            rcDitonateButton = Controller.IsKeyDown(PlayerKeys.RcDitonate);

            base.Update();
        }
    }
}
