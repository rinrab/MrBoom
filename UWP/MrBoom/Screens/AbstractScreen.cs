// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class AbstractScreen : IScreen
    {
        private int currentTick;

        protected int CurrentTick { get => currentTick; }

        public AbstractScreen()
        {
            currentTick = 0;
        }

        protected virtual void OnDraw(SpriteBatch ctx)
        {
        }

        protected virtual void OnDrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnDispose()
        {
        }

        void IScreen.Draw(SpriteBatch ctx)
        {
            OnDraw(ctx);
        }

        void IScreen.DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
            OnDrawHighDPI(ctx, rect, scale, graphicScale);
        }

        void IScreen.Update()
        {
            currentTick++;
            OnUpdate();
        }

        public void Dispose()
        {
            OnDispose();
        }
    }
}
