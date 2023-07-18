using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MrBoom
{
    public enum State
    {
        StartMenu,
        Game
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

            bool isGamepadStart = false;

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
                    //soundManager.playSound("addplayer");
                }
                //if (controller.gamepad)
                //{
                //    const buttons = controller.gamepad.buttons;
                //    if (buttons[6].pressed && buttons[7].pressed)
                //    {
                //        isGamepadStart = true;
                //    }
                //}
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) || isGamepadStart)
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
}
