// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public static class ScreenManager
    {
        static IScreen currentScreen;
        static bool screenChanged;
        static readonly UnrepeatableRandom SoundRandom = new UnrepeatableRandom();
        static readonly UnrepeatableRandom LevelRandom = new UnrepeatableRandom();

        public static void Update()
        {
            if (currentScreen != null)
            {
                currentScreen.Update();
            }
            screenChanged = false;
        }

        public static void Draw(SpriteBatch ctx)
        {
            if (currentScreen != null)
            {
                currentScreen.Draw(ctx);
            }
        }

        public static void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale)
        {
            if (currentScreen != null)
            {
                currentScreen.DrawHighDPI(ctx, rect, scale, 2);
            }
        }

        public static void NextSong(SoundAssets sounds, int index = -1)
        {
            if (index == -1)
            {
                index = SoundRandom.Next(sounds.Musics.Length);
            }
            sounds.Musics[index].Play();
        }

        public static int GetNextLevel()
        {
#if true
            return LevelRandom.Next(MapData.Data.Length);
#else
            return 0;
#endif
        }

        public static void SetScreen(IScreen screen)
        {
            currentScreen = screen;
            screenChanged = true;
        }

        public static bool ScreenChanged
        {
            get => screenChanged;
        }
    }
}
