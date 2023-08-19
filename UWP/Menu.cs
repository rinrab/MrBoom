// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public interface IMenuItem
    {
        string Text { get; }

        bool OnLeft();
        bool OnRight();
        bool OnEnter();
    }

    public class TextMenuItem : IMenuItem
    {
        public string Text { get; }

        public TextMenuItem(string text)
        {
            Text = text;
        }

        public bool OnLeft()
        {
            return false;
        }

        public bool OnRight()
        {
            return false;
        }

        public bool OnEnter()
        {
            return true;
        }
    }

    public class SelectMenuItem : IMenuItem
    {
        private readonly string header;
        private readonly string[] options;
        public int selectionIndex;

        public string Text => $"{header} {options[selectionIndex]}";

        public SelectMenuItem(string header, string[] options)
        {
            this.header = header;
            this.options = options;
        }

        public bool OnLeft()
        {
            selectionIndex--;
            if (selectionIndex < 0)
            {
                selectionIndex = options.Length - 1;
            }
            return true;
        }

        public bool OnRight()
        {
            selectionIndex++;
            if (selectionIndex >= options.Length)
            {
                selectionIndex = 0;
            }
            return true;
        }

        public bool OnEnter()
        {
            OnRight();
            return false;
        }
    }

    public class Menu
    {
        public int Action { get; private set; }

        private readonly IMenuItem[] items;
        private readonly Assets assets;
        private readonly IEnumerable<IController> controllers;

        private int select;
        private int bombTick;

        public Menu(IMenuItem[] items, Assets assets, IEnumerable<IController> controllers)
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
                if (items[select].OnEnter())
                {
                    Action = select;
                }
                assets.Sounds.Bang.Play();
                Controller.Reset(controllers);
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Left))
            {
                if (items[select].OnLeft())
                {
                    assets.Sounds.PoseBomb.Play();
                }
                Controller.Reset(controllers);
            }
            if (Controller.IsKeyDown(controllers, PlayerKeys.Right))
            {
                if (items[select].OnRight())
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

            int maxWidth = 0;
            int maxHeight = 0;

            foreach (var item in items)
            {
                Vector2 size = assets.MenuFont.MeasureString(item.Text) / scale;

                maxWidth = Math.Max(maxWidth, (int)size.X);
                maxHeight = Math.Max(maxHeight, (int)size.Y);
            }

            int x = (320 - maxWidth) / 2;
            int y = (200 - maxHeight * items.Length) / 2;

            for (int i = 0; i < items.Length; i++)
            {
                ctx.DrawString(assets.MenuFont, items[i].Text, new Vector2(x, y) * scale, Color.White);
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
