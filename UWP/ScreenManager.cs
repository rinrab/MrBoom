using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public static class ScreenManager
    {
        static IScreen currentScreen;

        public static void Update()
        {
            if (currentScreen != null)
            {
                currentScreen.Update();
            }
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

        public static Sound SoundsToPlay
        {
            get
            {
                if (currentScreen != null)
                {
                    return currentScreen.SoundsToPlay;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static void SetScreen(IScreen screen)
        {
            currentScreen = screen;
        }
    }
}
