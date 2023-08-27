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

        private Terrain terrain;
        private readonly Assets assets;
        private readonly List<IController> controllers;

        private bool isPlayerJoined = false;
        private int bgTicks;
        private int level;
        private IController controller;
        private int toEnd = -1;

        public SoloGameScreen(Assets assets, List<IController> controllers)
        {
            this.assets = assets;
            this.controllers = controllers;

            startGame();
        }

        private void startGame()
        {
            terrain = new Terrain(assets, assets.Levels[level], Map.SoloMaps[level]);

            if (controller == null)
            {
                controller = new KeyboardController(Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None);
            }

            terrain.AddPlayer(assets.Players[0], controller, 0, 0);
            terrain.InitializeMonsters();
        }

        public void Update()
        {
            if (toEnd == -1 || toEnd > 140)
            {
                terrain.Update();
            }

            if (!isPlayerJoined)
            {
                if (Controller.IsKeyDown(controllers, PlayerKeys.Bomb))
                {
                    Human sprite = (Human)terrain.GetSprites().ToArray()[0];
                    controller = controllers.Find((a) => a.IsKeyDown(PlayerKeys.Bomb));
                    sprite.Controller = controller;
                    isPlayerJoined = true;
                    Controller.Reset(controllers);
                }
            }

            if (terrain.GetAliveMonsters().Count() == 0 && toEnd == -1)
            {
                toEnd = 60 * 3;
                Controller.Reset(controllers);
            }

            if (toEnd > 0)
            {
                toEnd--;
            }
            if (toEnd == 0 && Controller.IsKeyDown(controllers, PlayerKeys.Continue))
            {
                level++;
                startGame();
            }

            bgTicks++;
        }

        public void Draw(SpriteBatch ctx)
        {
            terrain.Draw(ctx, bgTicks);

            if (!isPlayerJoined || toEnd >= 0)
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

            if (toEnd >= 0)
            {
                Vector2 size = assets.MenuFontBig.MeasureString("YOU WIN!") * scale / 6;

                ctx.DrawString(assets.MenuFontBig, "YOU WIN!",
                               new Vector2(rect.X + 320 * scale - size.X / 2, rect.Y + 200 * scale - size.Y / 2),
                               Color.White, 0, Vector2.Zero, scale / 6, SpriteEffects.None, 0);
            }
        }
    }
}
