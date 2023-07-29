using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class StartScreen : IScreen
    {
        public Screen Next { get; private set; }

        private int tick = 0;

        private readonly Assets assets;
        private readonly List<PlayerState> players;
        private readonly List<IController> controllers;
        private readonly List<IController> unjoinedControllers;
        private readonly string helpText =
            "welcome to mr.boom v1.2!!!   " +
            "players can join using their drop bomb button   use enter to start game   " +
            "right keyboard controller: use arrows to move ctrl to drop bomb " +
            "and alt to triger it by radio control   " +
            "left keyboard controller: use wasd to move rigth ctrl to drop bomb " +
            "and right alt to triger it by radio control   " +
            "gamepad controller: use d-pad arrows to move a button to drop bomb " +
            "b button to triger it by radio control";

        public StartScreen(Assets assets, List<PlayerState> players, List<IController> controllers)
        {
            this.assets = assets;
            this.players = players;
            this.controllers = controllers;
            this.unjoinedControllers = new List<IController>(controllers);
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.Start.Draw(ctx, 0, 0);

            int ox = 10;
            int oy = 10;

            assets.Controls[0].Draw(ctx, ox, oy);
            Game.DrawString(ctx, ox + 20, oy + 4, "or", assets.Alpha[1]);
            assets.Controls[1].Draw(ctx, ox + 40, oy);
            Game.DrawString(ctx, ox + 70, oy + 4, "join", assets.Alpha[1]);

            assets.Controls[2].Draw(ctx, 320 - ox - 115, oy);
            Game.DrawString(ctx, 320 - ox - 96, oy + 4, "or", assets.Alpha[1]);
            assets.Controls[3].Draw(ctx, 320 - ox - 76, oy - 8);
            Game.DrawString(ctx, 320 - ox - 40, oy + 4, "start", assets.Alpha[1]);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int index = y * 4 + x;
                    AnimatedImage images = assets.Alpha[index / 2 + 2];
                    if (index < players.Count)
                    {
                        PlayerState player = players[index];

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
            tick++;

            List<IController> toRemove = new List<IController>();
            foreach (IController controller in unjoinedControllers)
            {
                if (controller.IsKeyDown(PlayerKeys.Bomb))
                {
                    string[] names = new string[]
                    {
                        "gin", "jai", "jay", "lad", "dre", "ash", "zev", "buz", "nox", "oak",
                        "coy", "eza", "fil", "kip", "aya", "jem", "roy", "rex", "ryu", "gus"
                    };
                    string name = names[Terrain.Random.Next(names.Length)];
                    this.players.Add(new PlayerState(controller) { Name = name });
                    assets.Sounds.Addplayer.Play();

                    toRemove.Add(controller);
                }
            }

            foreach (IController controller in toRemove)
            {
                unjoinedControllers.Remove(controller);
            }

            if (Controller.IsKeyDown(controllers, PlayerKeys.StartGame))
            {
                if (this.players.Count >= 1)
                {
                    Next = Screen.Game;
                }
            }
        }
    }
}
