// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Windows.ApplicationModel;

namespace MrBoom
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public List<Team> Teams;
        public Assets assets;

        public static readonly string Version =
            $"{Package.Current.Id.Version.Major}." +
            $"{Package.Current.Id.Version.Minor}." +
            $"{Package.Current.Id.Version.Build}";

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderTarget2D renderTarget;
        private readonly List<IController> controllers;
        private readonly Settings settings;

        public Game()
        {
            // TODO: Load settings
            settings = new Settings()
            {
                IsDebug = LaunchParameters.ContainsKey("-d")
            };

            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true
            };

            IsFixedTimeStep = true;
            IsMouseVisible = false;

            Content.RootDirectory = "Content";

            controllers = new List<IController>()
            {
                new KeyboardController(Keys.W, Keys.S, Keys.A, Keys.D, Keys.LeftControl, Keys.LeftShift),
                new KeyboardController(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl, Keys.RightShift),
                new GamepadController(PlayerIndex.One),
                new GamepadController(PlayerIndex.Two),
                new GamepadController(PlayerIndex.Three),
                new GamepadController(PlayerIndex.Four),
            };
        }

        protected override void Initialize()
        {
            assets = Assets.Load(Content, GraphicsDevice);
            MediaPlayer.IsRepeating = true;

            Teams = new List<Team>();
            ScreenManager.NextSong(assets.Sounds, 3);

            ScreenManager.SetScreen(new DemoScreen(Teams, assets, settings, controllers));

            renderTarget = new RenderTarget2D(GraphicsDevice, 640, 400, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (IController controller in controllers)
            {
                if (ScreenManager.ScreenChanged)
                {
                    controller.Reset();
                }

                controller.Update();
            }

            ScreenManager.Update();

            if (MediaPlayer.State == MediaState.Stopped)
            {
                ScreenManager.NextSong(assets.Sounds, 3);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            ScreenManager.Draw(spriteBatch);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            float height = GraphicsDevice.PresentationParameters.Bounds.Height;
            float width = GraphicsDevice.PresentationParameters.Bounds.Width;
            float scale = Math.Min(height / renderTarget.Height, width / renderTarget.Width);
            Matrix matrix = Matrix.CreateScale(scale);
            matrix.Translation = new Vector3((width - renderTarget.Width * scale) / 2,
                (height - renderTarget.Height * scale) / 2, 0);

            var rect = new Rectangle(
                (int)((width - renderTarget.Width * scale) / 2),
                (int)((height - renderTarget.Height * scale) / 2),
                (int)(640 * scale), (int)(400 * scale));

            spriteBatch.Begin(
                SpriteSortMode.Immediate);

            spriteBatch.Draw(renderTarget, rect, Color.White);

            ScreenManager.DrawHighDPI(spriteBatch, rect, scale);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void DrawString(SpriteBatch ctx, int x, int y, string text, AnimatedImage images)
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
