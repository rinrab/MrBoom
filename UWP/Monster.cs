using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static MrBoom.Assets;

namespace MrBoom
{
    public class Monster : MovingSprite
    {
        public bool IsDie = false;

        private readonly Map.MonsterData monsterData;
        private readonly AnimatedImage[] assets;
        private readonly AnimatedImage ghosts;
        private int wait = -1;
        private int tick = 0;
        private int livesCount;
        private int unplugin = 180;
        private int freeze = 180;

        public Monster(Terrain map, Map.MonsterData monsterData, AnimatedImage[] assets, AnimatedImage ghosts) : base(map)
        {
            this.monsterData = monsterData;
            this.livesCount = monsterData.LivesCount - 1;
            this.Direction = (Directions)Terrain.Random.Next(1, 5);
            this.assets = assets;
            this.ghosts = ghosts;
        }

        public override void Update()
        {
            tick++;

            if (freeze > 0)
            {
                unplugin--;
                freeze--;
                return;
            }

            bool isWalkable(int dx, int dy)
            {
                switch (terrain.GetCell((x + dx * 8 + 8 + dx) / 16, (y + dy * 8 + 8 + dy) / 16).Type)
                {
                    case TerrainType.Free:
                    case TerrainType.PowerUpFire:
                    case TerrainType.PowerUp:
                        return true;

                    case TerrainType.PermanentWall:
                    case TerrainType.TemporaryWall:
                    case TerrainType.Bomb:
                    case TerrainType.Fire:
                    case TerrainType.Apocalypse:
                    case TerrainType.Rubber:
                        return false;

                    default: return true;
                }
            }

            if (!IsDie)
            {
                if (unplugin != 0)
                {
                    unplugin--;
                }

                var cell = terrain.GetCell((x + 8) / 16, (y + 8) / 16);
                if (cell.Type == TerrainType.Fire && unplugin == 0)
                {
                    if (livesCount > 0)
                    {
                        terrain.PlaySound(Sound.Ai);
                        livesCount--;
                        this.unplugin = 165;
                    }
                    else
                    {
                        IsDie = true;
                        frameIndex = 0;
                        terrain.SetCell((x + 8) / 16, (y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));
                    }
                }
                else if (cell.Type == TerrainType.Apocalypse)
                {
                    IsDie = true;
                    frameIndex = 0;
                    terrain.PlaySound(Sound.Ai);
                }
                else
                {
                    Point[] delta = new Point[]
                    {
                        new Point(0, -1),
                        new Point(0, 1),
                        new Point(-1, 0),
                        new Point(1, 0),
                    };

                    if (wait == 0)
                    {
                        wait = -1;

                        for (int i = 0; ; i++)
                        {
                            int index = Terrain.Random.Next(4);

                            if (isWalkable(delta[index].X, delta[index].Y))
                            {
                                Direction = (Directions)index + 1;
                                break;
                            }
                            if (i >= 32)
                            {
                                Direction = Directions.None;
                                wait = monsterData.WaitAfterTurn;
                                break;
                            }
                        }
                    }
                    else if (wait == -1)
                    {
                        if (x % 16 == 0 && y % 16 == 0 && Terrain.Random.Next(16) == 0)
                        {
                            wait = this.monsterData.WaitAfterTurn;
                            frameIndex = 0;
                            Direction = Directions.None;
                        }
                        else
                        {
                            if (!isWalkable(delta[(int)Direction - 1].X, delta[(int)Direction - 1].Y))
                            {
                                wait = this.monsterData.WaitAfterTurn;
                                frameIndex = 0;
                                Direction = Directions.None;
                            }
                        }
                    }
                    else
                    {
                        wait--;
                    }
                }
                base.Update(tick % monsterData.Slow == 0);
            }
            else
            {
                if (frameIndex != -1 && (frameIndex + 1) / 8 + 1 < assets[4].Length)
                {
                    animateIndex = 4;
                    frameIndex++;
                }
                else
                {
                    frameIndex = -1;
                }
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            if (frameIndex != -1)
            {
                AnimatedImage animation = this.assets[this.animateIndex];
                Image img = animation[frameIndex / 8 * monsterData.Slow];
                //if (this.blinking % this.blinkingSpeed * 2 < this.blinkingSpeed)
                //{
                //    img = assets.boyGhost[this.animateIndex * 3 + frames[frameIndex % 4]];
                //}

                int x = this.x + 8 + 8 - img.Width / 2;
                int y = this.y + 16 - img.Height;

                if (unplugin == 0 || unplugin % 30 < 15)
                {
                    img.Draw(ctx, x, y);
                }
                else
                {
                    if (ghosts.Length / 4 == 2)
                    {
                        ghosts[animateIndex * 2 + frameIndex / 20 % 2].Draw(ctx, x, y);
                    }
                    else
                    {
                        int[] index = new int[] { 0, 1, 0, 2 };
                        ghosts[animateIndex * 3 + index[frameIndex / 20 % 4]].Draw(ctx, x, y);
                    }
                }
            }
        }
    }
}
