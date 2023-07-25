using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MrBoom
{
    [Flags]
    public enum PlayerKeys
    {
        Up = 0x001,
        Down = 0x002,
        Left = 0x004,
        Right = 0x008,
        Bomb = 0x010,
        RcDitonate = 0x020,
        StartGame = 0x040,
        EndGame = 0x080,
        Continue = 0x100
    }

    public static class Controller
    {
        public static bool IsKeyDown(IEnumerable<IController> controllers, PlayerKeys key)
        {
            foreach (IController controller in controllers)
            {
                if (controller.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public interface IController
    {
        bool IsKeyDown(PlayerKeys key);

        void Update();
    }

    public abstract class AbstractController : IController
    {
        private PlayerKeys state;

        public bool IsKeyDown(PlayerKeys key)
        {
            return state.HasFlag(key);
        }

        public void Update()
        {
            state = GetState();
        }

        protected abstract PlayerKeys GetState();
    }

    public class KeyboardController : AbstractController
    {
        readonly Keys KeyUp;
        readonly Keys KeyDown;
        readonly Keys KeyLeft;
        readonly Keys KeyRight;
        readonly Keys KeyBomb;
        readonly Keys KeyRcDitonate;

        public KeyboardController(Keys keyUp, Keys keyDown, Keys keyLeft,
            Keys keyRight, Keys keyBomb, Keys keyRcDitonate)
        {
            this.KeyUp = keyUp;
            this.KeyDown = keyDown;
            this.KeyLeft = keyLeft;
            this.KeyRight = keyRight;
            this.KeyBomb = keyBomb;
            this.KeyRcDitonate = keyRcDitonate;
        }

        protected override PlayerKeys GetState()
        {
            PlayerKeys result = 0;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(KeyUp)) result |= PlayerKeys.Up;
            if (keyboardState.IsKeyDown(KeyDown)) result |= PlayerKeys.Down;
            if (keyboardState.IsKeyDown(KeyLeft)) result |= PlayerKeys.Left;
            if (keyboardState.IsKeyDown(KeyRight)) result |= PlayerKeys.Right;
            if (keyboardState.IsKeyDown(KeyBomb)) result |= PlayerKeys.Bomb;
            if (keyboardState.IsKeyDown(KeyRcDitonate)) result |= PlayerKeys.RcDitonate;
            if (keyboardState.IsKeyDown(Keys.Enter)) result |= PlayerKeys.StartGame;
            if (keyboardState.IsKeyDown(Keys.Escape)) result |= PlayerKeys.EndGame;
            if (keyboardState.GetPressedKeyCount() > 0) result |= PlayerKeys.Continue;

            return result;
        }
    }

    public class GamepadController : AbstractController
    {
        private readonly PlayerIndex index;

        readonly float deadZone = 0.5f;

        public GamepadController(PlayerIndex index)
        {
            this.index = index;
        }

        protected override PlayerKeys GetState()
        {
            PlayerKeys result = 0;

            var state = GamePad.GetState(this.index);

            if (state.ThumbSticks.Left.Y > deadZone || state.IsButtonDown(Buttons.DPadUp))
                result |= PlayerKeys.Up;

            if (state.ThumbSticks.Left.Y < -deadZone || state.IsButtonDown(Buttons.DPadDown))
                result |= PlayerKeys.Down;

            if (state.ThumbSticks.Left.X < -deadZone || state.IsButtonDown(Buttons.DPadLeft))
                result |= PlayerKeys.Left;

            if (state.ThumbSticks.Left.X > deadZone || state.IsButtonDown(Buttons.DPadRight))
                result |= PlayerKeys.Right;

            if (state.IsButtonDown(Buttons.A))
                result |= PlayerKeys.Bomb;

            if (state.IsButtonDown(Buttons.B))
                result |= PlayerKeys.RcDitonate;

            if (state.IsButtonDown(Buttons.Start))
                result |= PlayerKeys.StartGame;

            if (!state.IsButtonUp(Buttons.A | Buttons.B | Buttons.X | Buttons.Y))
                result |= PlayerKeys.Continue;

            return result;
        }
    }
}
