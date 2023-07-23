using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MrBoom
{
    public enum PlayerKeys
    {
        Up,
        Down,
        Left,
        Right,
        Bomb,
        RcDitonate,
        StartGame,
        EndGame,
        Continue
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

    public class KeyboardController : IController
    {
        readonly Keys KeyUp;
        readonly Keys KeyDown;
        readonly Keys KeyLeft;
        readonly Keys KeyRight;
        readonly Keys KeyBomb;
        readonly Keys KeyRcDitonate;
        private KeyboardState keyboardState;

        public KeyboardController(Keys keyUp, Keys keyDown, Keys keyLeft,
            Keys keyRight, Keys keyBomb, Keys keyRcDitonate)
        {
            this.KeyUp = keyUp;
            this.KeyDown = keyDown;
            this.KeyLeft = keyLeft;
            this.KeyRight = keyRight;
            this.KeyBomb = keyBomb;
            this.KeyRcDitonate = keyRcDitonate;
            this.keyboardState = Keyboard.GetState();
        }

        public void Update()
        {
            keyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(PlayerKeys key)
        {
            switch (key)
            {
                case PlayerKeys.Up:
                    return keyboardState.IsKeyDown(KeyUp);
                case PlayerKeys.Down:
                    return keyboardState.IsKeyDown(KeyDown);
                case PlayerKeys.Left:
                    return keyboardState.IsKeyDown(KeyLeft);
                case PlayerKeys.Right:
                    return keyboardState.IsKeyDown(KeyRight);
                case PlayerKeys.Bomb:
                    return keyboardState.IsKeyDown(KeyBomb);
                case PlayerKeys.RcDitonate:
                    return keyboardState.IsKeyDown(KeyRcDitonate);
                case PlayerKeys.StartGame:
                    return keyboardState.IsKeyDown(Keys.Enter);
                case PlayerKeys.EndGame:
                    return keyboardState.IsKeyDown(Keys.Escape);
                case PlayerKeys.Continue:
                    return keyboardState.GetPressedKeyCount() > 0;
                default:
                    return false;
            }
        }
    }

    public class GamepadController : IController
    {
        private readonly PlayerIndex index;
        private GamePadState state;

        readonly float deadZone = 0.5f;

        public GamepadController(PlayerIndex index)
        {
            this.index = index;
            this.state = GamePad.GetState(index);
        }

        public void Update()
        {
            state = GamePad.GetState(this.index);
        }

        public bool IsKeyDown(PlayerKeys key)
        {
            switch (key)
            {
                case PlayerKeys.Up:
                    return state.ThumbSticks.Left.Y > deadZone || state.IsButtonDown(Buttons.DPadUp);
                case PlayerKeys.Down:
                    return state.ThumbSticks.Left.Y < -deadZone || state.IsButtonDown(Buttons.DPadDown);
                case PlayerKeys.Left:
                    return state.ThumbSticks.Left.X < -deadZone || state.IsButtonDown(Buttons.DPadLeft);
                case PlayerKeys.Right:
                    return state.ThumbSticks.Left.X > deadZone || state.IsButtonDown(Buttons.DPadRight);
                case PlayerKeys.Bomb:
                    return state.IsButtonDown(Buttons.A);
                case PlayerKeys.RcDitonate:
                    return state.IsButtonDown(Buttons.B);
                case PlayerKeys.StartGame:
                    return state.IsButtonDown(Buttons.Start);
                case PlayerKeys.EndGame:
                    return false;
                case PlayerKeys.Continue:
                    return !state.IsButtonUp(Buttons.A | Buttons.B | Buttons.X | Buttons.Y);
                default:
                    return false;
            }
        }
    }
}
