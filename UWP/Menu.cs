// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class Menu
    {
        public int Action { get; private set; }

        private readonly string[] items;
        private readonly Assets assets;
        private readonly IEnumerable<IController> controllers;

        private int select;
        private int bombTick;

        public Menu(string[] items, Assets assets, IEnumerable<IController> controllers)
        {
            this.items = items;
            this.assets = assets;
            this.controllers = controllers;
            Action = -1;
        }

        public void Update()
        {
            if (Controller.IsKeyDown(controllers, PlayerKeys.Up))
            {
                if (select > 0) select--;
                else select = items.Length - 1;

                Controller.Reset(controllers);
                assets.Sounds.PoseBomb.Play();
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Down))
            {
                if (select < items.Length - 1) select++;
                else select = 0;

                Controller.Reset(controllers);
                assets.Sounds.PoseBomb.Play();
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Bomb) ||
                Controller.IsKeyDown(controllers, PlayerKeys.StartGame))
            {
                Action = select;
                assets.Sounds.Bang.Play();
                Controller.Reset(controllers);
            }
            bombTick++;
        }

        public void Draw(SpriteBatch ctx)
        {
            Color color = Color.White * 0.5f;
            ctx.Draw(assets.BlackPixel, Vector2.Zero, new Rectangle(0, 0, 1, 1),
                     color, 0, Vector2.Zero, new Vector2(640, 400), SpriteEffects.None, 1);

            const int scale = 2;
            const int bombOffset = 6;

            int maxWidth = 0;
            int maxHeight = 0;

            foreach (var item in items)
            {
                Vector2 size = assets.MenuFont.MeasureString(item) / scale;

                maxWidth = Math.Max(maxWidth, (int)size.X);
                maxHeight = Math.Max(maxHeight, (int)size.Y);
            }

            int x = (320 - maxWidth + assets.Bomb[bombTick / 15].Width + bombOffset) / 2;
            int y = (200 - maxHeight * items.Length) / 2;

            for (int i = 0; i < items.Length; i++)
            {
                ctx.DrawString(assets.MenuFont, items[i], new Vector2(x, y) * scale, Color.Blue);
                if (select == i)
                {
                    assets.Bomb[bombTick / 16].Draw(ctx, x - assets.Bomb[0].Width - bombOffset / 2,
                                                    y + maxHeight / 2 - assets.Bomb[0].Height / 2);
                }

                y += maxHeight;
            }
        }
    }
}
