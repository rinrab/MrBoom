// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public abstract class Sprite
    {
        public int CellX { get => (X + 8) / 16; }
        public int CellY { get => (Y + 8) / 16; }
        public int AnimateIndex { get; private set; }

        private int x;
        private int y;
        public Terrain terrain;
        public Directions? Direction { get; protected set; }
        public int frameIndex;
        public int animateIndex;
        private bool isDie = false;
        public Feature Features;
        public SkullType? Skull { get; private set; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public Sound SoundsToPlay {  get; private set; }
        public bool IsDie { get => isDie; }
        public bool IsAlive { get => !isDie; }

        protected int unplugin;

        private int blinking = 0;
        private int skullTimer;
        private readonly Assets.MovingSpriteAssets animations;
        private readonly int DefaultSpeed;

        public Sprite(Terrain terrain, Assets.MovingSpriteAssets assets, int x, int y, int speed)
        {
            this.terrain = terrain;
            animations = assets;
            X = x;
            Y = y;
            DefaultSpeed = speed;
        }

        public virtual void Update()
        {
            SoundsToPlay = 0;

            if (IsDie)
            {
                frameIndex += 4;
                animateIndex = 4;
                skullTimer = 0;
                Skull = null;
                return;
            }

            blinking++;

            int speed = DefaultSpeed;
            if (Features.HasFlag(Feature.RollerSkates))
            {
                speed = 4;
            }
            if (Skull == SkullType.Fast)
            {
                speed = 5;
            }
            if (Skull == SkullType.Slow)
            {
                speed = 1;
            }

            if (skullTimer > 0)
            {
                skullTimer--;
            }
            else
            {
                Skull = null;
            }

            if (unplugin > 0)
            {
                unplugin--;
            }

            Cell cell = terrain.GetCell((X + 8) / 16, (Y + 8) / 16);

            if (cell.Type == TerrainType.Bomb && cell.OffsetX == 0 && cell.OffsetY == 0)
            {
                cell.DeltaX = 0;
                cell.DeltaY = 0;
            }

            void moveY(int delta)
            {
                if (X % 16 == 0)
                {
                    int newY = (delta < 0) ? (Y + delta) / 16 : Y / 16 + 1;
                    int cellX = (X + 8) / 16;
                    int cellY = (Y + 8) / 16;

                    if (terrain.IsWalkable(cellX, newY))
                    {
                        Y += delta;
                    }

                    if (newY == cellY && cell.Type == TerrainType.Bomb)
                    {
                        Y += delta;
                    }
                    else
                    {
                        Cell newCell = terrain.GetCell(cellX, newY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (Features.HasFlag(Feature.Kick))
                            {
                                if (newCell.DeltaX == 0)
                                {
                                    newCell.DeltaY = delta * 2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    XAlign(delta);
                }
            }
            void moveX(int delta)
            {
                if (Y % 16 == 0)
                {
                    int newX = (delta < 0) ? (X + delta) / 16 : X / 16 + 1;
                    int cellX = (X + 8) / 16;
                    int cellY = (Y + 8) / 16;

                    if (terrain.IsWalkable(newX, cellY))
                    {
                        X += delta;
                    }

                    if (newX == cellX && cell.Type == TerrainType.Bomb)
                    {
                        X += delta;
                    }
                    else
                    {
                        Cell newCell = terrain.GetCell(newX, cellY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (Features.HasFlag(Feature.Kick))
                            {
                                if (newCell.DeltaY == 0)
                                {
                                    newCell.DeltaX = delta * 2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    YAlign(delta);
                }
            }

            if (Direction.HasValue)
            {
                frameIndex++;

                int move = 0;
                if (speed == 1)
                    move = (frameIndex % 3 == 0) ? 1 : 0;
                else if (speed == 2)
                    move = (frameIndex % 2 == 0) ? 1 : 0;
                else if (speed == 3)
                    move = 1;
                else if (speed == 4)
                    move = 2;
                else if (speed == 5)
                    move = 4;

                for (int i = 0; i < move; i++)
                {
                    if (Direction == Directions.Up)
                    {
                        animateIndex = 3;
                        moveY(-1);
                    }
                    else if (Direction == Directions.Down)
                    {
                        animateIndex = 0;
                        moveY(1);
                    }
                    else if (Direction == Directions.Left)
                    {
                        moveX(-1);
                        animateIndex = 2;
                    }
                    else if (Direction == Directions.Right)
                    {
                        moveX(1);
                        animateIndex = 1;
                    }
                }
            }
            else
            {
                frameIndex = 0;
            }
        }

        void XAlign(int deltaY)
        {
            if (terrain.IsWalkable((X - 1) / 16, (Y + 8) / 16 + deltaY))
            {
                X -= 1;
                AnimateIndex = 2;
            }
            else if (terrain.IsWalkable((X + 16) / 16, (Y + 8) / 16 + deltaY))
            {
                X += 1;
                AnimateIndex = 1;
            }
        }

        void YAlign(int deltaX)
        {
            if (terrain.IsWalkable((X + 8) / 16 + deltaX, (Y - 1) / 16))
            {
                Y -= 1;
                AnimateIndex = 3;
            }
            else if (terrain.IsWalkable((X + 8) / 16 + deltaX, (Y + 16) / 16))
            {
                Y += 1;
                AnimateIndex = 0;
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            if (frameIndex != -1)
            {
                Color color = Color.White;

                AnimatedImage animation = animations.Normal[animateIndex];
                if (unplugin > 0 && blinking % 30 < 15)
                {
                    animation = animations.Ghost[animateIndex];
                }
                if (skullTimer > 0 && blinking % 30 > 15)
                {
                    animation = animations.Red[animateIndex];
                }

                Image img = animation[frameIndex / 20];

                int x = X + 8 + 8 - img.Width / 2;
                int y = Y + 16 - img.Height;

                if (animateIndex != 4 || frameIndex / 20 < animations.Normal[4].Length)
                {
                    img.Draw(ctx, x, y, color);
                }
            }
        }

        public void SetSkull(SkullType skullType)
        {
            PlaySound(Sound.Skull);

            skullTimer = 600;
            Skull = skullType;
        }

        protected void Kill()
        {
            isDie = true;
            Direction = null;
            frameIndex = 0;
            unplugin = 0;
        }

        protected void PlaySound(Sound sound)
        {
            SoundsToPlay |= sound;
        }

        public virtual string GetCellDebugInfo(int cellX, int cellY)
        {
            return string.Empty;
        }

        public virtual string GetDebugInfo()
        {
            return string.Empty;
        }
    }
}
