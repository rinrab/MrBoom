using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MrBoom
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D startGameSplash;

        private Assets assets;

        private Texture2D[] levelTextures;
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

            terrain = new Terrain(Map.Maps[0], assets.levels[0]);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startGameSplash = Content.Load<Texture2D>("MICRO");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            bgTick++;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Begin drawing
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
                    var cell = terrain.GetCell(x, y);
                    if (cell.Images != null)
                    {
                        var image = cell.Images[cell.Index];

                        image.Draw(spriteBatch, x * 16 + 16 + 8 - image.Width / 2, y * 16 + 32 - image.Height);
                    }
                }
            }

            assets.Bomb[bgTick / 20 % assets.Bomb.Length].Draw(spriteBatch, 100, 100);

            assets.Players[0][0][0].Draw(spriteBatch, 0, 0);
            assets.PowerUps[0][0].Draw(spriteBatch, 32, 32);

            spriteBatch.End(); // Stop drawing

            base.Draw(gameTime);
        }
    }
}
