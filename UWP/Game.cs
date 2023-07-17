using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MrBoom
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Assets assets;

        private int bgTick = 0;

        private Terrain terrain;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            assets = Assets.Load(Content);

            terrain = new Terrain(0, assets);
            Sprite sprite = new Sprite(terrain, assets.Players[0], assets.Bomb)
            {
                Controller = new KeyboardController(Keys.W, Keys.S, Keys.A, Keys.D, Keys.LeftControl, Keys.LeftAlt)
            };
            terrain.LocateSprite(sprite);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            bgTick++;

            terrain.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                samplerState: SamplerState.PointWrap,
                transformMatrix: Matrix.CreateScale(3));

            var bgs = assets.levels[0].Backgrounds;
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

                        image.Draw(spriteBatch, x * 16 + 16 + 8 - image.Width / 2, y * 16 + 32 - image.Height);
                    }
                }
            }

            List<Sprite> spritesToDraw = terrain.Players;
            //spritesToDraw.Sort((a, b) => a.y - b.y);

            foreach (Sprite sprite in spritesToDraw)
            {
                sprite.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
