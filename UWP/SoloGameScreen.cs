// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MrBoom
{
    public class SoloGameScreen : IScreen
    {
        public Screen Next { get; }

        private readonly Terrain terrain;
        private readonly Assets assets;
        private readonly List<IController> controllers;

        private bool isPlayerJoined = false;
        private int bgTicks;

        public SoloGameScreen(Assets assets, List<IController> controllers)
        {
            terrain = new Terrain(assets, assets.Levels[0], Map.SoloMaps[0]);

            var controller = new KeyboardController(Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None);

            terrain.AddPlayer(assets.Players[0], controller, 0, 0);
            terrain.InitializeMonsters();

            this.assets = assets;
            this.controllers = controllers;
        }

        public void Update()
        {
            terrain.Update();

            if (!isPlayerJoined)
            {
                if (Controller.IsKeyDown(controllers, PlayerKeys.Bomb))
                {
                    Human sprite = (Human)terrain.GetSprites().ToArray()[0];
                    sprite.Controller = controllers.Find((a) => a.IsKeyDown(PlayerKeys.Bomb));
                    isPlayerJoined = true;
                    Controller.Reset(controllers);
                }
            }

            bgTicks++;
        }

        public void Draw(SpriteBatch ctx)
        {
            terrain.Draw(ctx, bgTicks);

            if (!isPlayerJoined)
            {
                ctx.Draw(assets.BlackPixel, new Rectangle(0, 0, 640, 400), Color.White * 0.7f);
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale)
        {
            if (!isPlayerJoined)
            {
                Vector2 size = assets.MenuFontBig.MeasureString("FIRE TO JOIN") * scale / 6;

                ctx.DrawString(assets.MenuFontBig, "FIRE TO JOIN",
                               new Vector2(rect.X + 320 * scale - size.X / 2, rect.Y + 200 * scale - size.Y / 2),
                               Color.White, 0, Vector2.Zero, scale / 6, SpriteEffects.None, 0);
            }
        }
    }
}
