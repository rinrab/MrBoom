// Copyright (c) Timofei Zhakov. All rights reserved.

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
        private int select;
        private int bombTick;

        public Menu(IMenuItem[] items, Assets assets, IEnumerable<IController> controllers)
        {
            Items = items;
            this.assets = assets;
            this.controllers = controllers;
            Action = -1;
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

            const int bombOffset = 8;

            // 6 is font scale

            Vector2 maxSize = new Vector2(0, 0);

            foreach (var item in Items)
            {
                foreach (var text in item.GetDynamicTexts())
                {
                    Vector2 size = assets.MenuFontBig.MeasureString(text) / 6 / 2;
                    maxSize = Vector2.Max(maxSize, size);
                }
            }

            var img = assets.Bomb[bombTick / 16];

            img.Draw(ctx, (int)((320 - maxSize.X) / 2 - bombOffset - img.Width),
                          (int)(100 - maxSize.Y * Items.Length / 2 +
                                maxSize.Y * select + (maxSize.Y - img.Height) / 2));
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            Vector2 offset = new Vector2(rect.X, rect.Y);

            Vector2 maxSize = new Vector2(0, 0);

            foreach (var item in Items)
            {
                foreach (var text in item.GetDynamicTexts())
                {
                    Vector2 size = assets.MenuFontBig.MeasureString(text) / 6 * scale;
                    maxSize = Vector2.Max(maxSize, size);
                }
            }

            Vector2 pos = offset +
                new Vector2(rect.Width - maxSize.X, rect.Height - maxSize.Y * Items.Length) / 2;

            for (int i = 0; i < Items.Length; i++)
            {
                ctx.DrawString(assets.MenuFontBig,
                               Items[i].Text,
                               pos,
                               Color.White,
                               0,
                               Vector2.Zero,
                               scale / 6,
                               SpriteEffects.None,
                               0);

                pos.Y += maxSize.Y;
            }
        }
    }
}
