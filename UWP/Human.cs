// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Human : AbstractPlayer
    {
        public IController Controller;

        public Human(Terrain map, Assets.MovingSpriteAssets animations,
            IController controller, int maxBoom, int maxBombs, int team) :
            base(map, animations, maxBoom, maxBombs, team)
        {
            Controller = controller;
        }

        public override void Update()
        {
            this.Direction = Directions.None;
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
