﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class DrawScreen : IScreen
    {
        public Screen Next { get; private set; }
        public List<IController> controllers;

        private readonly Assets assets;
        private int tick;

        public DrawScreen(Assets assets, List<IController> controllers)
        {
            this.assets = assets;
            this.controllers = controllers;

            assets.Sounds.Draw.Play();
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Draw[tick / 30 % assets.Draw.Length].Draw(ctx, 0, 0);
        }

        public void Update()
        {
            tick++;

            if (tick > 120 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                Next = Screen.Game;
            }
        }
    }
}
