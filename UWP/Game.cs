using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MrBoom
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public List<Player> Players;
        public Assets assets;
        public List<IController> Controllers;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderTarget2D renderTarget;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            graphics.ToggleFullScreen();

            IsFixedTimeStep = true;

            Controllers = new List<IController>()
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
#if DEBUG
            graphics.IsFullScreen = false;
#else
            graphics.IsFullScreen = true;
#endif
            graphics.ApplyChanges();

            assets = Assets.Load(Content, GraphicsDevice);
            MediaPlayer.IsRepeating = true;

            Players = new List<Player>();
            NextSong(3);

            ScreenManager.SetScreen(new SplashScreen(assets, Players, Controllers));

            renderTarget = new RenderTarget2D(GraphicsDevice, 640, 400, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            base.Initialize();
        }

        public void StartGame()
        {
            ScreenManager.SetScreen(new GameScreen(Players, assets, this));
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (IController controller in Controllers)
            {
                if (ScreenManager.ScreenChanged)
                {
                    controller.Reset();
                }

                controller.Update();
            }

            ScreenManager.Update();

            UpdateNavigation();

            if (MediaPlayer.State == MediaState.Stopped)
            {
                NextSong();
            }

            base.Update(gameTime);
        }

        private void UpdateNavigation()
        {
            if (ScreenManager.Next != Screen.None)
            {
                if (ScreenManager.Next == Screen.Game)
                {
                    StartGame();
                }
                else if (ScreenManager.Next == Screen.StartMenu)
                {
                    Players = new List<Player>();
                    NextSong(3);
                    ScreenManager.SetScreen(new StartScreen(assets, Players, Controllers));
                }
                else
                {
                    throw new Exception("Can't navigate to " + ScreenManager.Next);
                }
            }
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
            matrix.Translation = new Vector3((width - renderTarget.Width * scale) / 2, (height - renderTarget.Height * scale) / 2, 0);

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                samplerState: SamplerState.PointWrap,
                transformMatrix: matrix);

            spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);

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

        public void NextSong(int index = -1)
        {
            if (index == -1)
            {
                index = Terrain.Random.Next(assets.Sounds.Musics.Length);
            }
            assets.Sounds.Musics[index].Play();
        }
    }
}
