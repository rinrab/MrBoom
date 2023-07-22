﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MrBoom
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public class Player
        {
            public IController Controller;
            public string Name;
            public int VictoryCount;

            public Player(IController controller)
            {
                Controller = controller;
            }
        }

        public List<Player> Players;
        public Assets assets;
        public SoundAssets sound;
        public List<IController> Controllers;
        public Terrain terrain;
        public Screen state;
        public IScreen menu;

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
            sound = SoundAssets.Load(Content);
            MediaPlayer.IsRepeating = true;

            state = Screen.StartMenu;
            Players = new List<Player>();
            NextSong(3);

            menu = new StartScreen(assets, Players, Controllers);

            renderTarget = new RenderTarget2D(GraphicsDevice, 640, 400, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            base.Initialize();
        }

        public void StartGame()
        {
            terrain = new Terrain(Terrain.Random.Next(Map.Maps.Length), assets);

            NextSong();

            for (int i = 0; i < Players.Count; i++)
            {
                Sprite sprite = new Sprite(terrain, assets.Players[i], assets.BoyGhost, assets.Bomb)
                {
                    Controller = this.Players[i].Controller
                };
                terrain.LocateSprite(sprite);
            }

            terrain.InitializeMonsters();

            state = Screen.Game;
            menu = new GameScreen(terrain, assets);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (state == Screen.Game)
            {
                menu.Update();
                terrain.Update();

                PlaySounds(terrain.SoundsToPlay);

                if (terrain.Result == GameResult.Victory)
                {
                    Player[] players = Players.ToArray();

                    int winner = -1;
                    var winnerSprite = terrain.Players[terrain.Winner];

                    for (int i = 0; i < players.Length; i++)
                    {
                        Game.Player player = players[i];
                        if (winnerSprite.Controller == player.Controller)
                        {
                            player.VictoryCount++;
                            winner = i;
                        }
                    }

                    menu = new ResultScreen(players, winner, assets, Controllers);
                    PlaySounds(menu.SoundsToPlay);
                    state = Screen.Results;
                }
                else if (terrain.Result == GameResult.Draw)
                {
                    menu = new DrawScreen(assets, Controllers);
                    PlaySounds(menu.SoundsToPlay);
                    state = Screen.Draw;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Players = new List<Player>();
                    NextSong(3);
                    menu = new StartScreen(assets, Players, Controllers);
                    PlaySounds(menu.SoundsToPlay);
                    state = Screen.StartMenu;
                }
            }
            else
            {
                menu.Update();
                PlaySounds(menu.SoundsToPlay);
                UpdateNavigation();
            }

            if (MediaPlayer.State == MediaState.Stopped)
            {
                NextSong();
            }

            base.Update(gameTime);
        }

        private void UpdateNavigation()
        {
            if (state != Screen.Game && menu.Next != Screen.None)
            {
                if (menu.Next == Screen.Game)
                {
                    StartGame();
                }
                else if (menu.Next == Screen.StartMenu)
                {
                    state = Screen.StartMenu;
                    Players = new List<Player>();
                    NextSong(3);
                    menu = new StartScreen(assets, Players, Controllers);
                }
                else if (menu.Next == Screen.Victory)
                {
                    state = Screen.Victory;
                    menu = new VictoryScreen(Players.ToArray(), terrain.Winner, assets, Controllers);
                }
                else
                {
                    throw new Exception("Can't navigate to " + menu.Next);
                }

                PlaySounds(menu.SoundsToPlay);
            }
        }

        private void PlaySounds(Sound soundsToPlay)
        {
            if (soundsToPlay.HasFlag(Sound.Bang)) sound.Bang.Play();
            if (soundsToPlay.HasFlag(Sound.PoseBomb)) sound.PoseBomb.Play();
            if (soundsToPlay.HasFlag(Sound.Sac)) sound.Sac.Play();
            if (soundsToPlay.HasFlag(Sound.Pick)) sound.Pick.Play();
            if (soundsToPlay.HasFlag(Sound.PlayerDie)) sound.PlayerDie.Play();
            if (soundsToPlay.HasFlag(Sound.Oioi)) sound.Oioi.Play();
            if (soundsToPlay.HasFlag(Sound.Ai)) sound.Ai.Play();
            if (soundsToPlay.HasFlag(Sound.Addplayer)) sound.Addplayer.Play();
            if (soundsToPlay.HasFlag(Sound.Victory)) sound.Victory.Play();
            if (soundsToPlay.HasFlag(Sound.Draw)) sound.Draw.Play();
            if (soundsToPlay.HasFlag(Sound.Clock)) sound.Clock.Play();
            if (soundsToPlay.HasFlag(Sound.TimeEnd)) sound.TimeEnd.Play();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            menu.Draw(spriteBatch);

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

        public static bool IsAnyKeyPressed(List<IController> controllers)
        {
            if (Keyboard.GetState().GetPressedKeyCount() > 0)
            {
                return true;
            }
            foreach (var controller in controllers)
            {
                controller.Update();
                if (controller.Keys.ContainsValue(true))
                {
                    return true;
                }
            }
            return false;
        }

        public void NextSong(int index = -1)
        {
            if (index == -1)
            {
                index = Terrain.Random.Next(sound.Musics.Length);
            }
            sound.Musics[index].Play();
        }
    }
}
