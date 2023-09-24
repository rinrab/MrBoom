// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MrBoom
{
    public class GameScreen : IScreen
    {
        private Terrain terrain;
        private readonly Assets assets;
        private readonly Game game;
        private readonly bool isDemo;

        private int bgTick = 0;
        private bool isPause = false;
        private Menu pauseWindow;
        private readonly bool isDebug;

        private bool isF4Toggle = false;
        private bool f4Mask;

        private Menu demoMenu;
        private bool demoMenuShow = true;

        public Screen Next { get; private set; }

        public GameScreen(List<Team> teams, Assets assets, Game game, bool isDemo)
        {
            isDebug = game.LaunchParameters.ContainsKey("-d");

            this.assets = assets;
            this.game = game;
            this.isDemo = isDemo;

#if true
            int levelIndex = game.LevelRandom.Next(Map.Maps.Length);
#else
            int levelIndex = 0;
#endif
            terrain = new Terrain(levelIndex, assets);

            game.NextSong(Map.Maps[levelIndex].Song);

            if (isDemo)
            {
                for (int i = 0; i < 4; i++)
                {
                    terrain.AddComputer(assets.Players[i], i);
                }

                demoMenu = new Menu(new IMenuItem[]
                {
                    new TextMenuItem("PLAY"),
                    new SelectMenuItem("TEAM", new string[]
                    {
                        "OFF",
                        "COLOR",
                        "SEX",
                    }),
                    new TextMenuItem("QUIT"),
                }, assets, game.Controllers, 160);
            }
            else
            {
                int playersCount = 0;
                for (int i = 0; i < teams.Count; i++)
                {
                    for (int j = 0; j < teams[i].Players.Count; j++)
                    {
                        var player = teams[i].Players[j];
                        if (player.Type == PlayerState.PlayerType.Human)
                        {
                            terrain.AddPlayer(assets.Players[player.Index], player.Controller, i);
                        }
                        else
                        {
                            terrain.AddComputer(assets.Players[player.Index], i);
                        }
                        playersCount++;
                    }
                }
            }

            terrain.InitializeMonsters();
        }

        public void Update()
        {
            bgTick++;

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.F4))
            {
                if (!f4Mask)
                {
                    isF4Toggle = !isF4Toggle;
                }
                f4Mask = true;
            }
            else
            {
                f4Mask = false;
            }

            if (isPause)
            {
                pauseWindow.Update();

                if (Controller.IsKeyDown(game.Controllers, PlayerKeys.Menu) ||
                    Controller.IsKeyDown(game.Controllers, PlayerKeys.Back))
                {
                    isPause = false;
                    Controller.Reset(game.Controllers);
                }

                if (pauseWindow.Action == 0)
                {
                    isPause = false;
                }
                else if (pauseWindow.Action == 1)
                {
                    game.NextSong(3);
                    ScreenManager.SetScreen(new MultiplayerStartScreen(assets, game.Teams, game.Controllers));
                }
                else if (pauseWindow.Action == 2)
                {
                    game.Exit();
                }
            }
            else
            {
                terrain.Update();

                if (isDebug)
                {
                    if (state.IsKeyDown(Keys.F1))
                    {
                        terrain.DetonateAll(true);
                    }
                    if (state.IsKeyDown(Keys.F2))
                    {
                        terrain.DetonateAll(false);
                    }
                    if (state.IsKeyDown(Keys.F3))
                    {
                        terrain.StartApocalypse();
                    }
                    if (state.IsKeyDown(Keys.F5))
                    {
                        terrain.GiveAll();
                    }
                }

                PlaySounds(terrain.SoundsToPlay);

                if (isDemo)
                {
                    if (terrain.Result == GameResult.Victory || terrain.Result == GameResult.Draw)
                    {
                        int levelIndex = game.LevelRandom.Next(Map.Maps.Length);

                        terrain = new Terrain(levelIndex, assets);

                        game.NextSong(Map.Maps[levelIndex].Song);

                        for (int i = 0; i < 4; i++)
                        {
                            terrain.AddComputer(assets.Players[i], i);
                        }

                        terrain.InitializeMonsters();
                    }

                    if (demoMenuShow)
                    {
                        demoMenu.Update();

                        if (demoMenu.Action == 0)
                        {
                            Next = Screen.StartMenu;
                        }
                        else if (demoMenu.Action == 2)
                        {
                            game.Exit();
                        }
                        Team.Mode = ((SelectMenuItem)demoMenu.Items[1]).SelectionIndex;
                    }

                    if (Controller.IsKeyDown(game.Controllers, PlayerKeys.Continue))
                    {
                        if (!demoMenuShow)
                        {
                            Controller.Reset(game.Controllers);
                            demoMenuShow = true;
                        }
                    }
                }
                else
                {
                    if (terrain.Result == GameResult.Victory)
                    {
                        int winner = terrain.Winner;

                        game.Teams[winner].VictoryCount++;

                        ScreenManager.SetScreen(new ResultScreen(game.Teams.ToArray(), winner, assets, game.Controllers, Team.Mode));
                    }
                    else if (terrain.Result == GameResult.Draw)
                    {
                        ScreenManager.SetScreen(new DrawScreen(assets, game.Controllers));
                    }

                    if (Controller.IsKeyDown(game.Controllers, PlayerKeys.Menu))
                    {
                        var options = new IMenuItem[]
                        {
                            new TextMenuItem("RESUME"),
                            new TextMenuItem("MAIN MENU"),
                            new TextMenuItem("QUIT")
                        };

                        pauseWindow = new Menu(options, assets, game.Controllers, -1);
                        Controller.Reset(game.Controllers);
                        isPause = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            if (terrain.LevelAssets.MovingBackground != null)
            {
                Image img = terrain.LevelAssets.MovingBackground;
                int xCount = 320 / img.Width + 2;

                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        img.Draw(ctx, img.Width * xCount - (bgTick / 2 + x * img.Width +
                            y * img.Height / 2) % (img.Width * xCount) - img.Width, y * img.Height);
                    }
                }
            }

            var bgs = terrain.LevelAssets.Backgrounds;
            bgs[bgTick / 20].Draw(ctx, 0, 0);
            var bgSprites = terrain.LevelAssets.BackgroundSprites;
            if (bgSprites != null)
            {
                foreach (var overlay in bgSprites)
                {
                    overlay.Images[bgTick / overlay.AnimationDelay].Draw(ctx, overlay.x, overlay.y);
                }
            }

            for (int y = 0; y < terrain.Height; y++)
            {
                for (int x = 0; x < terrain.Width; x++)
                {
                    Cell cell = terrain.GetCell(x, y);
                    if (cell.Images != null)
                    {
                        int index = (cell.Index == -1) ? 0 : cell.Index;
                        var image = cell.Images[index];

                        image.Draw(ctx, x * 16 + 8 + 8 - image.Width / 2 + cell.OffsetX, y * 16 + 16 - image.Height + cell.OffsetY);
                    }
                }
            }

            List<Sprite> spritesToDraw = new List<Sprite>(terrain.GetSprites());

            spritesToDraw.Sort((a, b) => a.Y - b.Y);

            foreach (Sprite sprite in spritesToDraw)
            {
                sprite.Draw(ctx);
            }

            var overlays = terrain.LevelAssets.Overlays;
            if (overlays != null)
            {
                foreach (var overlay in overlays)
                {
                    overlay.Images[bgTick / overlay.AnimationDelay].Draw(ctx, overlay.x, overlay.y);
                }
            }

            int drawInStart = 60 * 30 - terrain.ApocalypseSpeed * (terrain.MaxApocalypse + 5);
            if (terrain.TimeLeft > 30 * 60)
            {
                int time = (terrain.TimeLeft - 30 * 60) / 60;

                int min = time / 60;
                int sec = time % 60;

                string str = min.ToString() + ":" + ((sec < 10) ? 0 + sec.ToString() : sec.ToString());
                int x = 270;
                foreach (char c in str)
                {
                    string alpha = "0123456789:";
                    int index = alpha.IndexOf(c);
                    assets.BigDigits[index].Draw(ctx, x, 182);
                    if (index == 10)
                    {
                        x += 9;
                    }
                    else
                    {
                        x += 14;
                    }
                }
            }
            else if (terrain.TimeLeft < drawInStart)
            {
                int x = 30;
                int y = 20;
                if (terrain.TimeLeft > drawInStart - 20 - assets.DrawGameIn.Height)
                {
                    y = drawInStart - terrain.TimeLeft - assets.DrawGameIn.Height;
                }

                assets.DrawGameIn.Draw(ctx, x, y);

                int timeLeft = (terrain.TimeLeft + terrain.ApocalypseSpeed * terrain.MaxApocalypse) / 60;
                int firstNumber = timeLeft / 10;
                int secondNumber = timeLeft % 10;

                assets.DrawGameInNumbers[firstNumber].Draw(ctx, x + 42, y + 15);
                assets.DrawGameInNumbers[secondNumber].Draw(ctx, x + 8 + 42, y + 15);
            }

            if (isPause)
            {
                pauseWindow.Draw(ctx);
            }
            if (demoMenu != null && demoMenuShow)
            {
                demoMenu.Draw(ctx);
            }
        }

        private void PlaySounds(Sound soundsToPlay)
        {
            var soundAssets = assets.Sounds;
            if (soundsToPlay.HasFlag(Sound.Bang)) soundAssets.Bang.Play();
            if (soundsToPlay.HasFlag(Sound.PoseBomb)) soundAssets.PoseBomb.Play();
            if (soundsToPlay.HasFlag(Sound.Sac)) soundAssets.Sac.Play();
            if (soundsToPlay.HasFlag(Sound.Pick)) soundAssets.Pick.Play();
            if (soundsToPlay.HasFlag(Sound.PlayerDie)) soundAssets.PlayerDie.Play();
            if (soundsToPlay.HasFlag(Sound.Oioi)) soundAssets.Oioi.Play();
            if (soundsToPlay.HasFlag(Sound.Ai)) soundAssets.Ai.Play();
            if (soundsToPlay.HasFlag(Sound.Addplayer)) soundAssets.Addplayer.Play();
            if (soundsToPlay.HasFlag(Sound.Victory)) soundAssets.Victory.Play();
            if (soundsToPlay.HasFlag(Sound.Draw)) soundAssets.Draw.Play();
            if (soundsToPlay.HasFlag(Sound.Clock)) soundAssets.Clock.Play();
            if (soundsToPlay.HasFlag(Sound.TimeEnd)) soundAssets.TimeEnd.Play();
            if (soundsToPlay.HasFlag(Sound.Skull)) soundAssets.Skull.Play();
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            if (isPause)
            {
                pauseWindow.DrawHighDPI(ctx, rect, scale, graphicScale);
            }

            if (demoMenu != null && demoMenuShow)
            {
                demoMenu.DrawHighDPI(ctx, rect, scale, graphicScale);
            }

            if (isDebug && isF4Toggle)
            {
                for (int y = 1; y < terrain.Height - 1; y++)
                {
                    for (int x = 1; x < terrain.Width - 1; x++)
                    {
                        string debugInfo = terrain.GetCellDebugInfo(x, y);

                        Vector2 size = assets.DebugFont.MeasureString(debugInfo) / 6 / graphicScale;

                        Vector2 position =
                            (new Vector2(x, y) * 16 + new Vector2(8 + 8, 0 + 8) - size / 2) *
                            graphicScale * scale + new Vector2(rect.X, rect.Y);

                        ctx.DrawString(assets.DebugFont,
                                       debugInfo,
                                       position,
                                       Color.White,
                                       0,
                                       Vector2.One / 2,
                                       scale / 6,
                                       SpriteEffects.None,
                                       0);
                    }
                }

                string text = terrain.GetDebugInfo();

                Vector2 debugInfoSize = (assets.DebugFont.MeasureString(text) + new Vector2(16)) / 6 * scale;
                Rectangle area = new Rectangle(0, 0, (int)debugInfoSize.X, (int)debugInfoSize.Y);
                ctx.Draw(assets.BlackPixel, area, Color.White * 0.7f);

                ctx.DrawString(assets.DebugFont,
                               text,
                               Vector2.Zero,
                               Color.White,
                               0,
                               Vector2.Zero,
                               scale / 6,
                               SpriteEffects.None,
                               0);
            }
        }
    }
}
