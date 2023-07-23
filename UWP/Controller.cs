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
        RcDitonate
    }

    public interface IController
    {
        bool IsKeyDown(PlayerKeys key);

        bool IsJoined { get; set; }

        bool IsStart { get; }

        void Update();
        bool IsAnyKeyPressed();
    }

    public class KeyboardController : IController
    {
        public bool IsJoined { get; set; } = false;

        public bool IsStart => false;

        private readonly Dictionary<PlayerKeys, bool> keys;

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

            this.keys = new Dictionary<PlayerKeys, bool>();
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            this.keys[PlayerKeys.Up] = keyboardState.IsKeyDown(KeyUp);
            this.keys[PlayerKeys.Down] = keyboardState.IsKeyDown(KeyDown);
            this.keys[PlayerKeys.Left] = keyboardState.IsKeyDown(KeyLeft);
            this.keys[PlayerKeys.Right] = keyboardState.IsKeyDown(KeyRight);
            this.keys[PlayerKeys.Bomb] = keyboardState.IsKeyDown(KeyBomb);
            this.keys[PlayerKeys.RcDitonate] = keyboardState.IsKeyDown(KeyRcDitonate);
        }

        public bool IsKeyDown(PlayerKeys key)
        {
            return this.keys[key];
        }

        public bool IsAnyKeyPressed()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            return keyboardState.GetPressedKeyCount() > 0;
        }
    }

    public class GamepadController : IController
    {
        public bool IsJoined { get; set; } = false;

        public bool IsStart => GamePad.GetState(index).IsButtonDown(Buttons.Start);

        private readonly PlayerIndex index;
        private readonly Dictionary<PlayerKeys, bool> keys;

        public GamepadController(PlayerIndex index)
        {
            keys = new Dictionary<PlayerKeys, bool>();
            this.index = index;
        }

        public void Update()
        {
            var state = GamePad.GetState(this.index);

            float deadZone = 0.5f;

            keys[PlayerKeys.Up] = state.ThumbSticks.Left.Y > deadZone || state.IsButtonDown(Buttons.DPadUp);
            keys[PlayerKeys.Down] = state.ThumbSticks.Left.Y < -deadZone || state.IsButtonDown(Buttons.DPadDown);
            keys[PlayerKeys.Left] = state.ThumbSticks.Left.X < -deadZone || state.IsButtonDown(Buttons.DPadLeft);
            keys[PlayerKeys.Right] = state.ThumbSticks.Left.X > deadZone || state.IsButtonDown(Buttons.DPadRight);
            keys[PlayerKeys.Bomb] = state.IsButtonDown(Buttons.A);
            keys[PlayerKeys.RcDitonate] = state.IsButtonDown(Buttons.B);
        }

        public bool IsKeyDown(PlayerKeys key)
        {
            return this.keys[key];
        }

        public bool IsAnyKeyPressed()
        {
            return this.keys.ContainsValue(true);
        }
    }
}
