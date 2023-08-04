using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public abstract class Sprite
    {
        public int x;
        public int y;
        public int speed;
        public Terrain terrain;
        private readonly Assets.MovingSpriteAssets animations;
        public Directions Direction;
        public int frameIndex;
        public int animateIndex;
        public bool IsDie = false;
        public Feature Features;

        public Sprite(Terrain map, Assets.MovingSpriteAssets animations)
        {
            this.terrain = map;
            this.animations = animations;
        }

        public int AnimateIndex { get; private set; }

        public int unplugin;
        public int skullTimer;
        public SkullType SkullType;

        public virtual void Update()
        {
            if (IsDie)
            {
                frameIndex += 4;
                animateIndex = 4;
                return;
            }

            if (Features.HasFlag(Feature.RollerSkates))
            {
                speed = 4;
            }
            if (SkullType == SkullType.Fast)
            {
                speed = 5;
            }
            if (SkullType == SkullType.Slow)
            {
                speed = 1;
            }

            if (skullTimer > 0)
            {
                skullTimer--;
            }
            else
            {
                SkullType = SkullType.None;
            }

            Cell cell = terrain.GetCell((x + 8) / 16, (y + 8) / 16);

            if (cell.Type == TerrainType.Bomb && cell.OffsetX == 0 && cell.OffsetY == 0)
            {
                cell.DeltaX = 0;
                cell.DeltaY = 0;
            }

            void moveY(int delta)
            {
                if (this.x % 16 == 0)
                {
                    int newY = (delta < 0) ? (this.y + delta) / 16 : this.y / 16 + 1;
                    int cellX = (this.x + 8) / 16;
                    int cellY = (this.y + 8) / 16;

                    if (terrain.IsWalkable(cellX, newY))
                    {
                        this.y += delta;
                    }

                    if (newY == cellY && cell.Type == TerrainType.Bomb)
                    {
                        this.y += delta;
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
                    this.XAlign(delta);
                }
            }
            void moveX(int delta)
            {
                if (this.y % 16 == 0)
                {
                    int newX = (delta < 0) ? (this.x + delta) / 16 : this.x / 16 + 1;
                    int cellX = (this.x + 8) / 16;
                    int cellY = (this.y + 8) / 16;

                    if (terrain.IsWalkable(newX, cellY))
                    {
                        this.x += delta;
                    }

                    if (newX == cellX && cell.Type == TerrainType.Bomb)
                    {
                        this.x += delta;
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
                    this.YAlign(delta);
                }
            }

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
                if (this.Direction == Directions.Up)
                {
                    this.animateIndex = 3;
                    moveY(-1);
                }
                else if (this.Direction == Directions.Down)
                {
                    this.animateIndex = 0;
                    moveY(1);
                }
                else if (this.Direction == Directions.Left)
                {
                    moveX(-1);
                    this.animateIndex = 2;
                }
                else if (this.Direction == Directions.Right)
                {
                    moveX(1);
                    this.animateIndex = 1;
                }
            }

            if (Direction == Directions.None)
            {
                frameIndex = 0;
            }
            else
            {
                frameIndex++;
            }
        }

        void XAlign(int deltaY)
        {
            if (terrain.IsWalkable((this.x - 1) / 16, (this.y + 8) / 16 + deltaY))
            {
                this.x -= 1;

                this.AnimateIndex = 2;
            }
            else if (terrain.IsWalkable((this.x + 16) / 16, (this.y + 8) / 16 + deltaY))
            {
                this.x += 1;

                this.AnimateIndex = 1;
            }
        }

        void YAlign(int deltaX)
        {
            if (terrain.IsWalkable((this.x + 8) / 16 + deltaX, (this.y - 1) / 16))
            {
                this.y -= 1;
                this.AnimateIndex = 3;
            }
            else if (terrain.IsWalkable((this.x + 8) / 16 + deltaX, (this.y + 16) / 16))
            {
                this.y += 1;

                this.AnimateIndex = 0;
            }
        }

        public enum Directions
        {
            None,
            Up,
            Down,
            Left,
            Right,
        }

        public void Draw(SpriteBatch ctx)
        {
            if (frameIndex != -1)
            {
                AnimatedImage animation;
                if (unplugin == 0 || unplugin % 30 < 15)
                {
                    animation = this.animations.Normal[this.animateIndex];
                }
                else
                {
                    animation = this.animations.Ghost[animateIndex];
                }

                Image img = animation[frameIndex / 20];

                int x = this.x + 8 + 8 - img.Width / 2;
                int y = this.y + 16 - img.Height;

                Color color = Color.White;

                if (skullTimer % 30 > 15)
                {
                    color = new Color(255, 0, 0);
                }

                if (animateIndex != 4 || frameIndex / 20 < animations.Normal[4].Length)
                {
                    img.Draw(ctx, x, y, color);
                }
            }
        }

        public void SetSkull(SkullType skullType)
        {
            terrain.PlaySound(Sound.Skull);

            skullTimer = 600;
            SkullType = skullType;
        }
    }
}
