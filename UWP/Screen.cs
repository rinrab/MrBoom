using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static MrBoom.Game;

namespace MrBoom
{
    public enum Screen
    {
        None,
        StartMenu,
        Game,
        Draw,
        Results,
        Victory
    }

    public interface IScreen
    {
        Screen Next { get; }
        Sound SoundsToPlay { get; }

        void Update();
        void Draw(SpriteBatch ctx);
    }

    public class StartMenu : IScreen
    {
        public Screen Next { get; private set; }
        public Sound SoundsToPlay { get; private set; }

        private int tick = 0;

        private readonly Assets assets;
        private readonly List<Game.Player> players;
        private readonly List<IController> controllers;
        private readonly string helpText =
            "welcome to mr.boom v0.1!!!   " +
            "players can join using their drop bomb button   use enter to start game   " +
            "right keyboard controller: use arrows to move ctrl to drop bomb " +
            "and alt to triger it by radio control   " +
            "left keyboard controller: use wasd to move rigth ctrl to drop bomb " +
            "and right alt to triger it by radio control   " +
            "gamepad controller: use d-pad arrows to move a button to drop bomb " +
            "b button to triger it by radio control";

        public StartMenu(Assets assets, List<Game.Player> players, List<IController> controllers)
        {
            this.assets = assets;
            this.players = players;
            this.controllers = controllers;

            foreach (IController controller in controllers)
            {
                controller.IsJoined = false;
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Start.Draw(ctx, 0, 0);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    Assets.AssetImage[] images = assets.Alpha[index / 2 + 2];
                    if (index < players.Count)
                    {
                        Game.Player player = players[index];

                        Game.DrawString(ctx, 13 + x * 80, 78 + y * 70, "name ?", images);
                        Game.DrawString(ctx, 21 + x * 80, 88 + y * 70, player.Name, images);
                    }
                    else
                    {
                        if (tick / 30 % 4 == 0)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "join", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 88, "us", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                        else if (tick / 30 % 4 == 2)
                        {
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 78, "push", images);
                            Game.DrawString(ctx, x * 80 + 20, y * 70 + 88, "fire", images);
                            Game.DrawString(ctx, x * 80 + 28, y * 70 + 98, "!!", images);
                        }
                    }
                }
            }

            Game.DrawString(ctx, 320 - tick % (helpText.Length * 8 + 320), 192, helpText, assets.Alpha[1]);
        }

        public void Update()
        {
            SoundsToPlay = 0;

            tick++;

            bool isStart = false;

            foreach (var controller in controllers)
            {
                controller.Update();
                if (controller.Keys[PlayerKeys.Bomb] && !controller.IsJoined)
                {
                    controller.IsJoined = true;
                    string[] names = new string[]
                    {
                        "gin", "jai", "jay", "lad", "dre", "ash", "zev", "buz", "nox", "oak",
                        "coy", "eza", "fil", "kip", "aya", "jem", "roy", "rex", "ryu", "gus"
                    };
                    string name = names[Terrain.Random.Next(names.Length)];
                    this.players.Add(new Game.Player(controller) { Name = name });
                    SoundsToPlay |= Sound.Addplayer;
                }
                if (controller.IsStart)
                {
                    isStart = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || isStart)
            {
                if (this.players.Count >= 1)
                {
                    //fade.fadeOut(() =>
                    //{
                    //    isDemo = false;
                    //    music.next();
                    //    map = newMap();
                    //    startGame(this.playerList);
                    //    results = new Results(this.playerList);
                    //});
                    Next = Screen.Game;
                }
            }
        }
    }

    public class ResultScreen : IScreen
    {
        public Screen Next { get; private set; }
        public Sound SoundsToPlay { get; private set; }

        private readonly Game.Player[] players;
        private readonly int winner;
        private readonly List<IController> controllers;
        private readonly Assets assets;

        private int tick;

        public ResultScreen(Game.Player[] players, int winner, Assets assets, List<IController> controllers)
        {
            this.players = players;
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;

            SoundsToPlay |= Sound.Victory;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Med.Draw(ctx, 0, 0);

            Point[] positions = new Point[] {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 0),
                new Point(1, 1),
                new Point(0, 3),
                new Point(0, 4),
                new Point(1, 3),
                new Point(1, 4),
            };

            for (int i = 0; i < players.Length; i++)
            {
                for (int j = 0; j < players[i].VictoryCount; j++)
                {
                    int index = (tick / (8 + j)) % assets.Coin.Length;
                    if (i == this.winner && j == players[i].VictoryCount - 1)
                    {
                        if (tick % 60 < 30)
                        {
                            index = 0;
                        }
                        else
                        {
                            index = -1;
                        }
                    }

                    if (index != -1)
                    {
                        assets.Coin[index].Draw(ctx, positions[i].X * 161 + 44 + j * 23, positions[i].Y * 42 + 27);
                    }
                }
            }


            for (int i = 0; i < positions.Length; i++)
            {
                if (i < players.Length)
                {
                    Game.DrawString(ctx, positions[i].X * 161 + 10, positions[i].Y * 42 + 44,
                        players[i].Name, assets.Alpha[i / 2 + 2]);
                }
            }
        }

        public void Update()
        {
            SoundsToPlay = 0;

            if (this.tick > 120 && Game.IsAnyKeyPressed(controllers))
            {
                if (players[winner].VictoryCount >= 5)
                {
                    Next = Screen.Victory;
                }
                else
                {
                    Next = Screen.Game;
                }
            }

            this.tick++;
        }
    }

    public class DrawScreen : IScreen
    {
        public Screen Next { get; private set; }
        public Sound SoundsToPlay { get; private set; }
        public List<IController> controllers;

        private readonly Assets assets;
        private int tick;

        public DrawScreen(Assets assets, List<IController> controllers)
        {
            this.assets = assets;
            this.controllers = controllers;

            SoundsToPlay |= Sound.Draw;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Draw[tick / 30 % assets.Draw.Length].Draw(ctx, 0, 0);
        }

        public void Update()
        {
            SoundsToPlay = 0;

            tick++;
            if (tick > 120 && Game.IsAnyKeyPressed(controllers))
            {
                Next = Screen.Game;
            }
        }
    }

    public class VictoryScreen : IScreen
    {
        public Screen Next { get; private set; }
        public Sound SoundsToPlay { get; private set; }

        private int tick;

        private readonly Player[] players;
        private readonly int winner;
        private readonly Assets assets;
        private readonly List<IController> controllers;

        public VictoryScreen(Player[] players, int winner, Assets assets, List<IController> controllers)
        {
            this.players = players;
            this.winner = winner;
            this.assets = assets;
            this.controllers = controllers;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Vic[tick / 5 % assets.Vic.Length].Draw(ctx, 0, 0);

            Assets.AssetImage img = assets.Players[winner][0][tick / 20 % assets.Players[winner][0].Length];
            img.Draw(ctx, 320 / 2 - img.Width / 2, 80 - img.Height);
        }

        public void Update()
        {
            SoundsToPlay = 0;

            tick++;
            if (tick > 120 && Game.IsAnyKeyPressed(controllers))
            {
                foreach (Game.Player player in players)
                {
                    player.VictoryCount = 0;
                }
                Next = Screen.StartMenu;
            }
        }
    }
}
