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
        Dictionary<PlayerKeys, bool> Keys { get; }

        bool IsJoined { get; set; }

        bool IsStart { get; }

        void Update();
    }

    public class KeyboardController : IController
    {
        public Dictionary<PlayerKeys, bool> Keys { get; }
        public bool IsJoined { get; set; } = false;

        public bool IsStart => false;

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

            this.Keys = new Dictionary<PlayerKeys, bool>();
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            this.Keys[PlayerKeys.Up] = keyboardState.IsKeyDown(KeyUp);
            this.Keys[PlayerKeys.Down] = keyboardState.IsKeyDown(KeyDown);
            this.Keys[PlayerKeys.Left] = keyboardState.IsKeyDown(KeyLeft);
            this.Keys[PlayerKeys.Right] = keyboardState.IsKeyDown(KeyRight);
            this.Keys[PlayerKeys.Bomb] = keyboardState.IsKeyDown(KeyBomb);
            this.Keys[PlayerKeys.RcDitonate] = keyboardState.IsKeyDown(KeyRcDitonate);
        }
    }

    public class GamepadController : IController
    {
        public bool IsJoined { get; set; } = false;
        public Dictionary<PlayerKeys, bool> Keys { get; }

        public bool IsStart => GamePad.GetState(index).IsButtonDown(Buttons.Start);

        private readonly PlayerIndex index;

        public GamepadController(PlayerIndex index)
        {
            Keys = new Dictionary<PlayerKeys, bool>();
            this.index = index;
        }

        public void Update()
        {
            var state = GamePad.GetState(this.index);

            float deadZone = 0.5f;

            Keys[PlayerKeys.Up] = state.ThumbSticks.Left.Y > deadZone || state.IsButtonDown(Buttons.DPadUp);
            Keys[PlayerKeys.Down] = state.ThumbSticks.Left.Y < -deadZone || state.IsButtonDown(Buttons.DPadDown);
            Keys[PlayerKeys.Left] = state.ThumbSticks.Left.X < -deadZone || state.IsButtonDown(Buttons.DPadLeft);
            Keys[PlayerKeys.Right] = state.ThumbSticks.Left.X > deadZone || state.IsButtonDown(Buttons.DPadRight);
            Keys[PlayerKeys.Bomb] = state.IsButtonDown(Buttons.A);
            Keys[PlayerKeys.RcDitonate] = state.IsButtonDown(Buttons.B);
        }
    }
}
