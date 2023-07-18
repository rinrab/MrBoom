using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MrBoom
{
    public enum State
    {
        StartMenu,
        Game,
        Draw,
        Victory
    }

    public interface IState
    {
        void Update();
        void Draw(SpriteBatch ctx);
    }

    public class StartMenu : IState
    {
        private int tick = 0;

        private readonly Game game;
        private readonly string helpText =
            "welcome to mr.boom v0.1!!!   " +
            "players can join using their drop bomb button   use enter to start game   " +
            "right keyboard controller: use arrows to move ctrl to drop bomb " +
            "and alt to triger it by radio control   " +
            "left keyboard controller: use wasd to move rigth ctrl to drop bomb " +
            "and right alt to triger it by radio control   " +
            "gamepad controller: use d-pad arrows to move a button to drop bomb " +
            "b button to triger it by radio control";

        public StartMenu(Game game)
        {
            this.game = game;
            game.NextSong(3);
        }

        public void Draw(SpriteBatch ctx)
        {
            game.assets.Start.Draw(ctx, 0, 0);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    Assets.AssetImage[] images = game.assets.Alpha[index / 2 + 2];
                    if (index < game.Players.Count)
                    {
                        Game.Player player = game.Players[index];

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

            Game.DrawString(ctx, 320 - tick % (helpText.Length * 8 + 320), 192, helpText, game.assets.Alpha[1]);
        }

        public void Update()
        {
            tick++;

            bool isStart = false;

            foreach (var controller in game.Controllers)
            {
                controller.Update();
                if (controller.Keys[PlayerKeys.Bomb] && !controller.IsJoined)
                {
                    controller.IsJoined = true;
                    var names = new string[]
                    {
                        "gin", "jai", "jay", "lad", "dre", "ash", "zev", "buz", "nox", "oak",
                        "coy", "eza", "fil", "kip", "aya", "jem", "roy", "rex", "ryu", "gus"
                    };
                    string name = names[Terrain.Random.Next(names.Length)];
                    this.game.Players.Add(new Game.Player(controller) { Name = name });
                    game.sound.Addplayer.Play();
                }
                if (controller.IsStart)
                {
                    isStart = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || isStart)
            {
                if (this.game.Players.Count >= 1)
                {
                    //fade.fadeOut(() =>
                    //{
                    //    isDemo = false;
                    //    music.next();
                    //    map = newMap();
                    //    startGame(this.playerList);
                    //    results = new Results(this.playerList);
                    //});
                    this.game.StartGame();
                }
            }
        }
    }

    public class Results : IState
    {
        private readonly Game game;
        private readonly Game.Player[] players;
        private readonly int winner;
        private int tick;

        public Results(Game.Player[] players, int winner, Game game)
        {
            this.game = game;
            this.players = players;
            this.winner = winner;
            game.sound.Victory.Play();
        }

        public void Draw(SpriteBatch ctx)
        {
            game.assets.Med.Draw(ctx, 0, 0);
            
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
                for (int j = 0; j < players[i].VictoryCount; j++) {
                    int index = (tick / (8 + j)) % game.assets.Coin.Length;
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
                        game.assets.Coin[index].Draw(ctx, positions[i].X * 161 + 44 + j * 23, positions[i].Y * 42 + 27);
                    }
                }
            }


            for (int i = 0; i < positions.Length; i++)
            {
                if (i < players.Length)
                {
                    Game.DrawString(ctx, positions[i].X * 161 + 10, positions[i].Y * 42 + 44,
                        players[i].Name, game.assets.Alpha[i / 2 + 2]);
                }
            }
        }

        public void Update()
        {
            if (this.tick > 120 && game.IsAnyKeyPressed())
            {
                //if (this.next == "game")
                game.StartGame();
                //else
                //{
                //    victory = new Victory(this.next);
                //    state = States.victory;
                //}
            }

            this.tick++;
        }
    }

    public class DrawMenu : IState
    {
        private readonly Game game;
        private int tick;

        public DrawMenu(Game game)
        {
            this.game = game;
        }

        public void Draw(SpriteBatch ctx)
        {
            game.assets.Draw[tick / 30 % game.assets.Draw.Length].Draw(ctx, 0, 0);
        }

        public void Update()
        {
            tick++;
            if (tick > 120 && game.IsAnyKeyPressed())
            {
                game.StartGame();
            }
        }
    }

    public class Victory : IState
    {
        private int tick;
        private readonly int winner;
        private readonly Game game;

        public Victory(Game game, int winner)
        {
            this.game = game;
            this.winner = winner;
            game.sound.Victory.Play();
        }

        public void Draw(SpriteBatch ctx)
        {
            game.assets.Vic[tick / 5 % game.assets.Vic.Length].Draw(ctx, 0, 0);

            Assets.AssetImage img = game.assets.Players[winner][0][tick / 20 % game.assets.Players[winner][0].Length];
            img.Draw(ctx, 320 / 2 - img.Width / 2, 80 - img.Height);
        }

        public void Update()
        {
            tick++;
            if (tick > 120 && game.IsAnyKeyPressed())
            {
                foreach (Game.Player player in game.Players)
                {
                    player.VictoryCount = 0;
                }
                game.menu = new StartMenu(game);
                game.state = State.StartMenu;
            }
        }
    }
}
