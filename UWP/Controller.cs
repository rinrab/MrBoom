using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public enum PlayerKeys
    {
        Up,
        Down,
        Left,
        Right,
        Bomb
    }

    public interface IController
    {
        Dictionary<PlayerKeys, bool> Keys { get; }
        void Update();
    }

    public class KeyboardController : IController
    {
        public Dictionary<PlayerKeys, bool> Keys { get; }

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
            //this.playerKeys[PlayerKeys.rcDitonate] = keys[this.keyRcDitonate];
        }
    }
}
