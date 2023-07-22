using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static MrBoom.Game;

namespace MrBoom
{
    public class GameScreen : IScreen
    {
        private readonly Terrain terrain;
        private readonly Assets assets;
        private readonly Game game;
        private int bgTick = 0;

        public Screen Next { get; private set; }

        public Sound SoundsToPlay { get => terrain.SoundsToPlay; }

        public GameScreen(List<Player> players, Assets assets, Game game)
        {
            this.assets = assets;
            this.game = game;

            terrain = new Terrain(Terrain.Random.Next(Map.Maps.Length), assets);

            game.NextSong();

            for (int i = 0; i < players.Count; i++)
            {
                Sprite sprite = new Sprite(terrain, assets.Players[i], assets.BoyGhost, assets.Bomb)
                {
                    Controller = players[i].Controller
                };
                terrain.LocateSprite(sprite);
            }

            terrain.InitializeMonsters();
        }

        public void Update()
        {
            bgTick++;

            terrain.Update();

            if (terrain.Result == GameResult.Victory)
            {
                Player[] players = game.Players.ToArray();

                int winner = -1;
                var winnerSprite = terrain.Players[terrain.Winner];

                for (int i = 0; i < players.Length; i++)
                {
                    Player player = players[i];
                    if (winnerSprite.Controller == player.Controller)
                    {
                        player.VictoryCount++;
                        winner = i;
                    }
                }

                ScreenManager.SetScreen(new ResultScreen(players, winner, assets, game.Controllers));
            }
            else if (terrain.Result == GameResult.Draw)
            {
                ScreenManager.SetScreen(new DrawScreen(assets, game.Controllers));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                game.Players = new List<Player>();
                game.NextSong(3);
                ScreenManager.SetScreen(new StartScreen(assets, game.Players, game.Controllers));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (terrain.levelIndex == 3)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        assets.Sky.Draw(spriteBatch, 48 * 8 - (bgTick / 2 + x * 48 + y * 24) % (48 * 8) - 48, y * 44);
                    }
                }
            }

            var bgs = terrain.LevelAssets.Backgrounds;
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

                        image.Draw(spriteBatch, x * 16 + 8 + 8 - image.Width / 2 + cell.OffsetX, y * 16 + 16 - image.Height + cell.OffsetY);
                    }
                }
            }

            List<MovingSprite> spritesToDraw = new List<MovingSprite>(terrain.Players);
            foreach (var monster in terrain.Monsters)
            {
                spritesToDraw.Add(monster);
            }

            spritesToDraw.Sort((a, b) => a.y - b.y);

            foreach (MovingSprite sprite in spritesToDraw)
            {
                sprite.Draw(spriteBatch);
            }

            var overlays = terrain.LevelAssets.Overlays;
            if (overlays != null)
            {
                foreach (var overlay in overlays)
                {
                    overlay.Images[bgTick / overlay.AnimationDelay % overlay.Images.Length].Draw(spriteBatch, overlay.x, overlay.y);
                }
            }

            if (terrain.TimeLeft > 30 * 60)
            {
                int min = (terrain.TimeLeft - 30 * 60) / 60 / 60;
                int sec = (terrain.TimeLeft - 30 * 60) / 60 % 60;

                string time = min.ToString() + ":" + ((sec < 10) ? 0 + sec.ToString() : sec.ToString());
                int x = 270;
                foreach (char c in time)
                {
                    string alpha = "0123456789:";
                    int index = alpha.IndexOf(c);
                    assets.BigDigits[index].Draw(spriteBatch, x, 182);
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
            else if (terrain.TimeLeft < 60 * 30 - terrain.ApocalypseSpeed * (terrain.MaxApocalypse + 5))
            {
                int x = 320 / 2 - assets.DrawGameIn.Width / 2;
                int y = 20;
                assets.DrawGameIn.Draw(spriteBatch, x, y);

                int firstNumber = terrain.TimeLeft / 60 / 10;
                int secondNumber = terrain.TimeLeft / 60 % 10;

                assets.DrawGameInNumbers[firstNumber].Draw(spriteBatch, x + 42, y + 15);
                assets.DrawGameInNumbers[secondNumber].Draw(spriteBatch, x + 8 + 42, y + 15);
            }
        }

    }
}
