using Microsoft.Xna.Framework;
namespace MrBoom
{
    public class Monster : MovingSprite
    {
        public bool IsDie = false;

        private readonly Map.MonsterData monsterData;
        private int wait = -1;
        private int livesCount;

        public Monster(Terrain map, Map.MonsterData monsterData,
            Assets.MovingSpriteAssets animations) : base(map, animations)
        {
            this.monsterData = monsterData;
            Slow = monsterData.Slow;
            this.livesCount = monsterData.LivesCount - 1;
            this.Direction = (Directions)Terrain.Random.Next(1, 5);
        }

        public override void Update()
        {
            speed = 1;
            Slow = monsterData.Slow;

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
                    terrain.PlaySound(Sound.Ai);
                    if (livesCount > 0)
                    {
                        livesCount--;
                        this.unplugin = 165;
                    }
                    else
                    {
                        IsDie = true;
                        frameIndex = 0;
                        animateIndex = 4;
                        terrain.SetCell((x + 8) / 16, (y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));
                    }
                }
                if (cell.Type == TerrainType.Apocalypse)
                {
                    IsDie = true;
                    frameIndex = 0;
                    animateIndex = 4;
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
                base.Update();
            }
            else
            {
                frameIndex += 4;
                animateIndex = 4;
            }
        }
    }
}
