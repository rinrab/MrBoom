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

        public static void SetScreen(IScreen screen)
        {
            currentScreen = screen;
        }
    }
}
