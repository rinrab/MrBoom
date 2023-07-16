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
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 400;
            graphics.ApplyChanges();

            assets = Assets.Load(Content);

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

            spriteBatch.Begin(); // Begin drawing

            var bgs = assets.levels[0].Backgrounds;
            bgs[bgTick / 20 % bgs.Length].Draw(spriteBatch, 0, 0);

            assets.Bomb[bgTick / 20 % assets.Bomb.Length].Draw(spriteBatch, 100, 100);

            assets.Players[0][0][0].Draw(spriteBatch, 0, 0);
            assets.PowerUps[0][0].Draw(spriteBatch, 32, 32);
            spriteBatch.End(); // Stop drawing

            base.Draw(gameTime);
        }
    }
}
