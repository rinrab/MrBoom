// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Human : AbstractPlayer
    {
        public readonly IController Controller;

        public Human(Terrain map, Assets.MovingSpriteAssets animations,
            int x, int y,
            IController controller, int maxBoom, int maxBombs, int team) :
            base(map, animations, x, y, maxBoom, maxBombs, team)
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
            this.Direction = null;
            if (this.Controller.IsKeyDown(PlayerKeys.Up))
            {
                this.Direction = Directions.Up;
            }
            else if (this.Controller.IsKeyDown(PlayerKeys.Left))
            {
                this.Direction = Directions.Left;
            }
            else if (this.Controller.IsKeyDown(PlayerKeys.Right))
            {
                this.Direction = Directions.Right;
            }
            else if (this.Controller.IsKeyDown(PlayerKeys.Down))
            {
                this.Direction = Directions.Down;
            }
            dropBombButton = Controller.IsKeyDown(PlayerKeys.Bomb);
            rcDitonateButton = Controller.IsKeyDown(PlayerKeys.RcDitonate);

            base.Update();
        }
    }
}
