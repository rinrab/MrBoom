﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MrBoom
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public class Player
        {
            public IController Controller;
            public string Name;

            public Player(IController controller)
            {
                Controller = controller;
            }
        }

        public List<Player> Players;
        public Assets assets;
        public List<IController> Controllers;
        public Terrain terrain;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderTarget2D renderTarget;
        private State state;
        private IState menu;
        private int bgTick = 0;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;

            Controllers = new List<IController>()
            {
                new KeyboardController(Keys.W, Keys.S, Keys.A, Keys.D, Keys.LeftControl, Keys.X),
                new KeyboardController(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl, Keys.End),
                new GamepadController(PlayerIndex.One),
                new GamepadController(PlayerIndex.Two),
                new GamepadController(PlayerIndex.Three),
                new GamepadController(PlayerIndex.Four),
            };

            Players = new List<Player>();
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            assets = Assets.Load(Content);

            state = State.StartMenu;
            menu = new StartMenu(this);

            renderTarget = new RenderTarget2D(GraphicsDevice, 640, 400, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            base.Initialize();
        }

        public void StartGame()
        {
            terrain = new Terrain(0, assets);

            for (int i = 0; i < Players.Count; i++)
            {
                Sprite sprite = new Sprite(terrain, assets.Players[i], assets.Bomb)
                {
                    Controller = this.Players[i].Controller
                };
                terrain.LocateSprite(sprite);
            }

            state = State.Game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (state == State.Game)
            {
                bgTick++;

                terrain.Update();
            }
            else
            {
                menu.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (state == State.Game)
            {
                var bgs = assets.levels[0].Backgrounds;
                bgs[bgTick / 20 % bgs.Length].Draw(spriteBatch, 0, 0);

                for (int y = 0; y < terrain.Height; y++)
                {
                    for (int x = 0; x < terrain.Width; x++)
                    {
                        Cell cell = terrain.GetCell(x, y);
                        if (cell.Images != null)
                        {
                            int index = (cell.Index == -1) ? 0 : cell.Index;
                            var image = cell.Images[index];

                            image.Draw(spriteBatch, x * 16 + 8 + 8 - image.Width / 2, y * 16 + 16 - image.Height);
                        }
                    }
                }

                List<Sprite> spritesToDraw = new List<Sprite>(terrain.Players);
                spritesToDraw.Sort((a, b) => a.y - b.y);

                foreach (Sprite sprite in spritesToDraw)
                {
                    sprite.Draw(spriteBatch);
                }
            }
            else
            {
                menu.Draw(spriteBatch);
            }

            spriteBatch.End();
            
            GraphicsDevice.SetRenderTarget(null);

            float height = GraphicsDevice.PresentationParameters.Bounds.Height;
            float width = GraphicsDevice.PresentationParameters.Bounds.Width;
            float scale = Math.Min(height / 400, width / 640);
            Matrix matrix = Matrix.CreateScale(scale);
            matrix.Translation = new Vector3((width - 640 * scale) / 2, (height - 400 * scale) / 2, 0);

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                samplerState: SamplerState.PointWrap,
                transformMatrix: matrix);

            spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void DrawString(SpriteBatch ctx, int x, int y, string text, Assets.AssetImage[] images)
        {
            string alpha = "abcdefghijklmnopqrstuvwxyz0123456789!.-:/()?";

            for (int i = 0; i < text.Length; i++)
            {
                int index = alpha.IndexOf(text[i]);
                if (index != -1)
                {
                    images[index].Draw(ctx, x + i * 8, y);
                }
            }
        }
    }
}