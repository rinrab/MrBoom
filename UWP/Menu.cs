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
        public readonly IMenuItem[] Items;

        private readonly Assets assets;
        private readonly IEnumerable<IController> controllers;
        private readonly int width;
        private int select;
        private int bombTick;

        public Menu(IMenuItem[] items, Assets assets, IEnumerable<IController> controllers, int width)
        {
            Items = items;
            this.assets = assets;
            this.controllers = controllers;
            Action = -1;
            this.width = width;
        }

        public void Update()
        {
            if (Controller.IsKeyDown(controllers, PlayerKeys.Up))
            {
                if (select > 0) select--;
                else select = Items.Length - 1;

                Controller.Reset(controllers);
                assets.Sounds.PoseBomb.Play();
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Down))
            {
                if (select < Items.Length - 1) select++;
                else select = 0;

                Controller.Reset(controllers);
                assets.Sounds.PoseBomb.Play();
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Bomb) ||
                Controller.IsKeyDown(controllers, PlayerKeys.StartGame))
            {
                if (Items[select].OnEnter())
                {
                    Action = select;
                }
                assets.Sounds.Bang.Play();
                Controller.Reset(controllers);
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Left))
            {
                if (Items[select].OnLeft())
                {
                    assets.Sounds.PoseBomb.Play();
                }
                Controller.Reset(controllers);
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Right))
            {
                if (Items[select].OnRight())
                {
                    assets.Sounds.PoseBomb.Play();
                }
                Controller.Reset(controllers);
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Back))
            {
                Action = -2;
            }

            bombTick++;
        }

        public void Draw(SpriteBatch ctx)
        {
            ctx.Draw(assets.BlackPixel, new Rectangle(0, 0, 640, 400), new Rectangle(0, 0, 1, 1), Color.White * 0.7f);

            const int scale = 2;
            const int bombOffset = 8;

            int maxWidth = width;
            int maxHeight = 0;

            foreach (var item in Items)
            {
                Vector2 size = assets.MenuFont.MeasureString(item.Text) / scale;

                maxWidth = Math.Max(maxWidth, (int)size.X);
                maxHeight = Math.Max(maxHeight, (int)size.Y);
            }

            int x = (320 - maxWidth) / 2;
            int y = (200 - maxHeight * Items.Length) / 2;

            for (int i = 0; i < Items.Length; i++)
            {
                ctx.DrawString(assets.MenuFont, Items[i].Text, new Vector2(x, y) * scale, Color.White);
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
