using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class Monster : MovingSprite
    {
        public bool IsDie = false;

        private readonly Map.MonsterData monsterData;
        private readonly Assets.AssetImage[][] assets;
        private int wait = -1;
        private int tick = 0;

        public Monster(Terrain map, Map.MonsterData monsterData, Assets.AssetImage[][] assets) : base(map)
        {
            this.monsterData = monsterData;
            this.Direction = (Directions)Terrain.Random.Next(1, 5);
            this.assets = assets;
        }

        public override void Update()
        {
            tick++;

            if (!IsDie)
            {
                var cell = terrain.GetCell((x + 8) / 16, (y + 8) / 16);
                if (cell.Type == TerrainType.Fire)
                {
                    IsDie = true;
                    frameIndex = 0;
                    terrain.SetCell((x + 8) / 16, (y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));

                    Game.game.sound.Ai.Play();
                }
                else if (cell.Type == TerrainType.Apocalypse)
                {
                    IsDie = true;
                    frameIndex = 0;
                    Game.game.sound.Ai.Play();
                }
                else
                {
                    int _x = x;
                    int _y = y;
                    if (wait == 0)
                    {
                        wait = -1;

                        for (int i = 0; _x == x && _y == y; i++)
                        {
                            this.Direction = (Directions)Terrain.Random.Next(1, 5);
                            base.Update();
                            if (i > 10)
                            {
                                Direction = Directions.None;
                                break;
                            }
                        }
                    }
                    else if (wait == -1)
                    {
                        if (tick % monsterData.Slow == 0)
                        {
                            if (x % 16 == 0 && y % 16 == 0 && Terrain.Random.Next(16) == 0)
                            {
                                wait = this.monsterData.WaitAfterTurn;
                                frameIndex = 0;
                                Direction = Directions.None;
                            }
                            else
                            {
                                base.Update();
                                if (_x == x && _y == y)
                                {
                                    wait = this.monsterData.WaitAfterTurn;
                                    frameIndex = 0;
                                    Direction = Directions.None;
                                }
                            }
                        }
                    }
                    else
                    {
                        wait--;
                    }
                }
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
                Assets.AssetImage[] animation = this.assets[this.animateIndex];
                Assets.AssetImage img = animation[frameIndex / 8 * monsterData.Slow % animation.Length];
                //if (this.blinking % this.blinkingSpeed * 2 < this.blinkingSpeed)
                //{
                //    img = assets.boyGhost[this.animateIndex * 3 + frames[frameIndex % 4]];
                //}

                int x = this.x + 8 + 8 - img.Width / 2;
                int y = this.y + 16 - img.Height;

                img.Draw(ctx, x, y);
            }
        }
    }
}
