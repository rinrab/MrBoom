﻿using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public static class ScreenManager
    {
        static IScreen currentScreen;
        static bool screenChanged;

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

        public static Screen Next
        {
            get
            {
                if (currentScreen != null)
                {
                    return currentScreen.Next;
                }
                else
                {
                    return Screen.None;
                }
            }
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