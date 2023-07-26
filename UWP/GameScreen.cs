using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class GameScreen : IScreen
    {
        private readonly Terrain terrain;
        private readonly Assets assets;
        private readonly Game game;
        private int bgTick = 0;

        public Screen Next { get; private set; }

        public GameScreen(List<Player> players, Assets assets, Game game)
        {
            this.assets = assets;
            this.game = game;

            terrain = new Terrain(Terrain.Random.Next(Map.Maps.Length), assets);

            game.NextSong();

            for (int i = 0; i < players.Count; i++)
            {
                var ghosts = (i % 2) == 0 ? assets.BoyGhost : assets.GirlGhost;
                Sprite sprite = new Sprite(terrain, assets.Players[i], ghosts, assets.Bomb)
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

            PlaySounds(terrain.SoundsToPlay);

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

            bool endGame = false;
            foreach (var controller in game.Controllers)
            {
                if (controller.IsKeyDown(PlayerKeys.EndGame))
                {
                    endGame = true;
                }
            }

            if (endGame)
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
            bgs[bgTick / 20].Draw(spriteBatch, 0, 0);

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
                    overlay.Images[bgTick / overlay.AnimationDelay].Draw(spriteBatch, overlay.x, overlay.y);
                }
            }

            int drawInStart = 60 * 30 - terrain.ApocalypseSpeed * (terrain.MaxApocalypse + 5);
            if (terrain.TimeLeft > 30 * 60)
            {
                int time = (terrain.TimeLeft - 30 * 60) / 60;
                if (bgTick <= 180)
                {
                    time = (terrain.TimeLeft + bgTick - 181 - 30 * 60) / 60;
                }

                int min = time / 60;
                int sec = time % 60;

                string str = min.ToString() + ":" + ((sec < 10) ? 0 + sec.ToString() : sec.ToString());
                int x = 270;
                foreach (char c in str)
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
            else if (terrain.TimeLeft < drawInStart)
            {
                int x = 30;
                int y = 20;
                if (terrain.TimeLeft > drawInStart - 20 - assets.DrawGameIn.Height)
                {
                    y = drawInStart - terrain.TimeLeft - assets.DrawGameIn.Height;
                }

                assets.DrawGameIn.Draw(spriteBatch, x, y);

                int timeLeft = (terrain.TimeLeft + terrain.ApocalypseSpeed * terrain.MaxApocalypse) / 60;
                int firstNumber = timeLeft / 10;
                int secondNumber = timeLeft % 10;

                assets.DrawGameInNumbers[firstNumber].Draw(spriteBatch, x + 42, y + 15);
                assets.DrawGameInNumbers[secondNumber].Draw(spriteBatch, x + 8 + 42, y + 15);
            }

            if (bgTick <= 180)
            {
                int number = (180 - bgTick) / 60 + 1;
                Assets.AssetImage img = assets.BigDigits[number];
                img.Draw(spriteBatch, 320 / 2 - img.Width / 2, 200 / 2 - img.Height / 2);
                if (bgTick % 60 == 0)
                {
                    PlaySounds(Sound.Clock);
                }
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
        }
    }
}
