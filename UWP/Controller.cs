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
        EndGame
    }

    public interface IController
    {
        bool IsKeyDown(PlayerKeys key);

        bool IsJoined { get; set; }

        void Update();
        bool IsAnyKeyPressed();
    }

    public class KeyboardController : IController
    {
        public bool IsJoined { get; set; } = false;

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
                default:
                    return false;
            }
        }

        public bool IsAnyKeyPressed()
        {
            return keyboardState.GetPressedKeyCount() > 0;
        }
    }

    public class GamepadController : IController
    {
        public bool IsJoined { get; set; } = false;

        private readonly PlayerIndex index;
        private GamePadState state;
        float deadZone = 0.5f;

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
                default:
                    return false;
            }
        }

        public bool IsAnyKeyPressed()
        {
            return !state.IsButtonUp(Buttons.None);
        }
    }
}
