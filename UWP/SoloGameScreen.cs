// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class SoloGameScreen : IScreen
    {
        public Screen Next { get; }

        private readonly Terrain terrain;
        private readonly Assets assets;
        private readonly List<IController> controllers;

        private int bgTicks;

        public SoloGameScreen(Assets assets, List<IController> controllers)
        {
            terrain = new Terrain(0, assets);

            this.assets = assets;
            this.controllers = controllers;
        }

        public void Update()
        {
            bgTicks++;
        }

        public void Draw(SpriteBatch ctx)
        {
            terrain.Draw(ctx, bgTicks);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale)
        {
        }
    }
}
