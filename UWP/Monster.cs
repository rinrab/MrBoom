// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;

namespace MrBoom
{
    public class Monster : Sprite
    {
        private readonly Map.MonsterData monsterData;
        private int wait = -1;
        private int livesCount;

        public Monster(Terrain map, Map.MonsterData monsterData,
            Assets.MovingSpriteAssets animations, int x, int y) :
            base(map, animations, x, y, monsterData.Speed)
        {
            this.monsterData = monsterData;
            this.livesCount = monsterData.LivesCount - 1;

            if (monsterData.IsSlowStart)
            {
                wait = 120;
                unplugin = 120;
            }
            else
            {
                this.Direction = Terrain.Random.NextEnum<Directions>();
            }
        }

        public override void Update()
        {
            bool isWalkable(int dx, int dy)
            {
                switch (terrain.GetCell((X + dx * 8 + 8 + dx) / 16, (Y + dy * 8 + 8 + dy) / 16).Type)
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

            if (IsAlive)
            {
                var cell = terrain.GetCell((X + 8) / 16, (Y + 8) / 16);
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
                        Kill();
                        frameIndex = 0;
                        terrain.SetCell((X + 8) / 16, (Y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));
                    }
                }
                if (cell.Type == TerrainType.Apocalypse)
                {
                    Kill();
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
                            Directions dir = Terrain.Random.NextEnum<Directions>();

                            if (isWalkable(dir.DeltaX(), dir.DeltaY()))
                            {
                                Direction = dir;
                                break;
                            }
                            if (i >= 32)
                            {
                                Direction = null;
                                wait = monsterData.WaitAfterTurn;
                                break;
                            }
                        }
                    }
                    else if (wait == -1)
                    {
                        if (X % 16 == 0 && Y % 16 == 0 && Terrain.Random.Next(16) == 0)
                        {
                            wait = this.monsterData.WaitAfterTurn;
                            frameIndex = 0;
                            Direction = null;
                        }
                        else
                        {
                            if (!isWalkable(delta[(int)Direction].X, delta[(int)Direction].Y))
                            {
                                wait = this.monsterData.WaitAfterTurn;
                                frameIndex = 0;
                                Direction = null;
                            }
                        }
                    }
                    else
                    {
                        wait--;
                    }
                }
            }
            base.Update();
        }
    }
}
