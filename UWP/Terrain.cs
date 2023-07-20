using System;
using System.Collections.Generic;

namespace MrBoom
{
    public class Terrain
    {
        public static Random Random = new Random();

        public readonly int Width;
        public readonly int Height;
        public List<Sprite> Players;
        public Assets assets;
        public List<Monster> Monsters;
        public int TimeLeft;
        public int levelIndex;
        public Sound SoundsToPlay;
        public GameResult Result = GameResult.None;

        public Assets.Level LevelAssets
        {
            get
            {
                return levelAssets;
            }
        }

        public int Winner { get; private set; }

        private byte[] final;
        private int lastApocalypseSound = -1;
        private readonly Cell[] data;
        private int timeToEnd = -1;
        private int time;
        private List<Spawn> spawns;
        private List<PowerUpType> powerUpList;
        private readonly Map map;

        private readonly Assets.Level levelAssets;

        private class Spawn
        {
            public int x;
            public int y;
            public bool busy;
        }

        public Terrain(int levelIndex, Assets assets)
        {
            Monsters = new List<Monster>();

            this.levelIndex = levelIndex;
            this.assets = assets;
            this.levelAssets = assets.levels[levelIndex];
            this.map = Map.Maps[levelIndex];
            this.powerUpList = new List<PowerUpType>();

            foreach (var bonus in map.PowerUps)
            {
                for (int i = 0; i < bonus.Count; i++)
                {
                    this.powerUpList.Add(bonus.Type);
                }
            }
            this.Width = map.Data[0].Length;
            this.Height = map.Data.Length;
            //this.monsters = [];
            this.spawns = new List<Spawn>();
            this.TimeLeft = (map.Time + 31) * 60;
            this.final = map.Final;
            this.Players = new List<Sprite>();

            //this.initialBonus = initial.initialBonus;

            data = new Cell[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    char src = map.Data[y][x];
                    //const bonusStr = "0123456789AB";

                    string bonusStr = "123456789A";
                    if (src == '#')
                    {
                        this.data[y * this.Width + x] = new Cell(TerrainType.PermanentWall);
                    }
                    else if (src == '-')
                    {
                        this.data[y * this.Width + x] = new Cell(TerrainType.TemporaryWall)
                        {
                            Images = levelAssets.Walls
                        };
                    }
                    else if (src == '*')
                    {
                        this.spawns.Add(new Spawn()
                        {
                            x = x,
                            y = y,
                            busy = false
                        });
                        this.data[y * this.Width + x] = new Cell(TerrainType.Free);
                    }
                    else if (src == '%')
                    {
                        this.data[y * this.Width + x] = new Cell(TerrainType.Rubber);
                    }
                    else if (bonusStr.Contains(src.ToString()))
                    {
                        int index = bonusStr.IndexOf(src);
                        this.data[y * this.Width + x] = new Cell(TerrainType.PowerUp)
                        {
                            Images = assets.PowerUps[index],
                            Index = 0,
                            animateDelay = 8,
                            PowerUpType = (PowerUpType)index
                        };
                    }
                    else
                    {
                        this.data[y * this.Width + x] = new Cell(TerrainType.Free);
                    }
                }
            }
        }

        public void LocateSprite(Sprite sprite, int index = -1)
        {
            var spawn = this.spawns[this.generateSpawn(index)];
            sprite.x = spawn.x * 16;
            sprite.y = spawn.y * 16;

            this.Players.Add(sprite);
        }

        public void InitializeMonsters()
        {
            int count = this.spawns.Count - this.Players.Count;

            for (int i = 0; i < count; i++)
            {
                var data = map.Monsters[Random.Next(map.Monsters.Length)];
                Monster monster = new Monster(this, data, assets.Monsters[data.Type], assets.MonsterGhosts[data.Type]);
                int spawn = generateSpawn();
                monster.x = spawns[spawn].x * 16;
                monster.y = spawns[spawn].y * 16;
                this.Monsters.Add(monster);
            }
        }

        public void Update()
        {
            SoundsToPlay = 0;
            this.time++;
            TimeLeft--;

            if (this.timeToEnd != -1)
            {
                this.timeToEnd--;
            }

            int playersCount = 0;
            foreach (Sprite player in this.Players)
            {
                if (!player.isDie)
                {
                    playersCount++;
                }
            }

            int speed = 2;

            int maxFin = 0;
            foreach (byte fin in final)
            {
                if (fin != 255)
                {
                    maxFin = Math.Max(maxFin, fin);
                }
            }

            if (TimeLeft < 30 * 60 - 1)
            {
                if (TimeLeft % speed == 0)
                {
                    int index = (30 * 60 - TimeLeft) / speed;
                    if (index != 255)
                    {
                        for (int i = 0; i < final.Length; i++)
                        {
                            if (index == Math.Min(maxFin + 5, 255))
                            {
                                var cell = GetCell(i % Width, i / Width);
                                if (cell.Type == TerrainType.TemporaryWall)
                                {
                                    SetCell(i % Width, i / Width, new Cell(TerrainType.PowerUpFire)
                                    {
                                        Images = assets.Fire,
                                        Index = 0,
                                        Next = new Cell(TerrainType.Free)
                                    });
                                    PlaySound(Sound.Sac);
                                }
                            }
                            else if (final[i] == index)
                            {
                                var cell = GetCell(i % Width, i / Width);
                                if (cell.Type != TerrainType.PermanentWall)
                                {
                                    SetCell(i % Width, i / Width, new Cell(TerrainType.Apocalypse)
                                    {
                                        Images = levelAssets.PermanentWalls,
                                        Index = 0,
                                        Next = new Cell(TerrainType.PermanentWall)
                                        {
                                            Images = levelAssets.PermanentWalls,
                                        }
                                    });
                                    if (Math.Abs(lastApocalypseSound - TimeLeft) > 60)
                                    {
                                        PlaySound(Sound.Sac);
                                        lastApocalypseSound = TimeLeft;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.timeToEnd == 0)
            {
                if (playersCount == 1)
                {
                    for (int i = 0; i < Players.Count; i++)
                    {
                        if (!Players[i].isDie)
                        {
                            Winner = i;
                        }
                    }
                    Result = GameResult.Victory;
                }
                else
                {
                    Result = GameResult.Draw;
                }
            }

            if (playersCount == 1 && Players.Count > 1 && this.timeToEnd == -1)
            {
                this.timeToEnd = 60 * 3;
            }
            if (playersCount == 0 && this.timeToEnd == -1)
            {
                this.timeToEnd = 60 * 3;
            }


            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    Cell cell = this.GetCell(x, y);
                    if (cell.Index != -1)
                    {
                        int animateDelay = (cell.animateDelay <= 0) ? 6 : cell.animateDelay;
                        if (this.time % animateDelay == 0)
                        {
                            cell.Index++;
                            if (cell.Index >= cell.Images.Length)
                            {
                                if (cell.Next == null)
                                {
                                    cell.Index = 0;
                                }
                                else
                                {
                                    this.SetCell(x, y, cell.Next);
                                }
                            }
                        }
                    }

                    if (cell.Type == TerrainType.Bomb)
                    {
                        if (!cell.rcAllowed || !cell.owner.rcAllowed || cell.owner.isDie)
                        {
                            cell.bombTime--;
                        }

                        if (cell.bombTime == 0 | (cell.owner.rcDitonate && cell.rcAllowed))
                        {
                            this.ditonateBomb(x, y);
                            continue;
                        }
                        //if (cell.offsetX == 0 && cell.offsetY == 0)
                        //{
                        //    const next = this.getCell(x + getSign(cell.dx), y + getSign(cell.dy)).type;
                        //    if (next == TerrainType.Rubber)
                        //    {
                        //        cell.dx = -cell.dx;
                        //        cell.dy = -cell.dy;
                        //    }
                        //    else if (next != TerrainType.Free)
                        //    {
                        //        cell.dy = 0;
                        //        cell.dx = 0;
                        //    }
                        //}

                        //const newX = Int.divRound(x * 16 + cell.offsetX + cell.dx, 16);
                        //const newY = Int.divRound(y * 16 + cell.offsetY + cell.dy, 16);

                        //cell.offsetX += cell.dx;
                        //cell.offsetY += cell.dy;

                        //this.setCell(x, y, {
                        //type: TerrainType.Free
                        //});

                        //    this.setCell(newX, newY, cell);

                        //    cell.offsetX += (x - newX) * 16;
                        //    cell.offsetY += (y - newY) * 16;
                        //}
                    }
                }
            }

            foreach (Sprite player in this.Players)
            {
                player.Update();
            }

            foreach (Monster monster in this.Monsters)
            {
                monster.Update();
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (x >= 0 && x < this.Width && y >= 0 && y < this.Height)
            {
                return this.data[y * this.Width + x];
            }
            else
            {
                return new Cell(TerrainType.PermanentWall);
            }
        }

        public void SetCell(int x, int y, Cell cell)
        {
            this.data[y * this.Width + x] = cell;
        }

        public bool isWalkable(int x, int y)
        {
            Cell cell = this.GetCell(x, y);

            switch (cell.Type)
            {
                case TerrainType.Free:
                case TerrainType.PowerUpFire:
                    return true;

                case TerrainType.PermanentWall:
                case TerrainType.Rubber:
                case TerrainType.Apocalypse:
                    return false;

                case TerrainType.TemporaryWall:
                case TerrainType.Bomb:
                    return false; // cheats.noClip

                default:
                    return true;
            }
        }

        public void ditonateBomb(int bombX, int bombY)
        {
            Cell bombCell = this.GetCell(bombX, bombY); ;
            int maxBoom = bombCell.maxBoom;
            bombCell.owner.BombsPlaced--;

            void burn(int dx, int dy, Assets.AssetImage[] image, Assets.AssetImage[] imageEnd)
            {
                for (int i = 1; i <= maxBoom; i++)
                {
                    int x = bombX + i * dx;
                    int y = bombY + i * dy;
                    Cell cell = this.GetCell(x, y);

                    if (cell.Type == TerrainType.PermanentWall || cell.Type == TerrainType.Apocalypse ||
                        cell.Type == TerrainType.Rubber)
                    {
                        break;
                    };

                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        Cell next = this.GenerateGiven();

                        this.SetCell(x, y, new Cell(TerrainType.PermanentWall)
                        {
                            Images = levelAssets.Walls,
                            Index = 0,
                            animateDelay = 4,
                            Next = next
                        });
                        break;
                    }
                    else if (cell.Type == TerrainType.PowerUp)
                    {
                        this.SetCell(x, y, new Cell(TerrainType.PowerUpFire)
                        {
                            Images = assets.Fire,
                            Index = 0,
                            animateDelay = 6,
                            Next = new Cell(TerrainType.Free)
                        });
                        this.PlaySound(Sound.Sac);
                        break;
                    }
                    else if (cell.Type == TerrainType.Bomb)
                    {
                        this.ditonateBomb(x, y);
                        break;
                    }
                    else if (cell.Type == TerrainType.Fire || cell.Type == TerrainType.PowerUpFire)
                    {
                    }
                    else
                    {
                        this.SetCell(x, y, new Cell(TerrainType.Fire)
                        {
                            Images = i == maxBoom ? imageEnd : image,
                            Index = 0,
                            Next = new Cell(TerrainType.Free)
                        });
                    }
                }
            }

            PlaySound(Sound.Bang);

            this.SetCell(bombX, bombY, new Cell(TerrainType.Fire)
            {
                Images = assets.BoomMid,
                Index = 0,
                Next = new Cell(TerrainType.Free)
            });

            burn(1, 0, assets.BoomHor, assets.BoomRightEnd);
            burn(-1, 0, assets.BoomHor, assets.BoomLeftEnd);
            burn(0, 1, assets.BoomVert, assets.BoomBottomEnd);
            burn(0, -1, assets.BoomVert, assets.BoomTopEnd);
        }

        public Cell GeneratePowerUp(PowerUpType powerUpType)
        {
            return new Cell(TerrainType.PowerUp)
            {
                Images = assets.PowerUps[(int)powerUpType],
                Index = 0,
                animateDelay = 8,
                PowerUpType = powerUpType
            };
        }

        Cell GenerateGiven()
        {
            int rnd = Random.Next(int.MaxValue);
            if (rnd < int.MaxValue / 2)
            {
                var powerUpIndex = Random.Next(this.powerUpList.Count);
                var powerUpType = this.powerUpList[powerUpIndex];

                return GeneratePowerUp(powerUpType);
            }
            else
            {
                return new Cell(TerrainType.Free);
            }
        }

        int generateSpawn(int spawnIndex = -1)
        {
            if (spawnIndex == -1)
            {
                var indexList = new List<int>();
                for (int i = 0; i < this.spawns.Count; i++)
                {
                    if (!this.spawns[i].busy)
                    {
                        indexList.Add(i);
                    }
                }
                spawnIndex = indexList[Random.Next(indexList.Count)];
            }
            this.spawns[spawnIndex].busy = true;
            return spawnIndex;
        }

        public void PlaySound(Sound sound)
        {
            SoundsToPlay |= sound;
        }
    }

    public enum TerrainType
    {
        Free,
        PermanentWall,
        TemporaryWall,
        Bomb,
        PowerUp,
        PowerUpFire,
        Apocalypse,
        Rubber,
        Fire
    }

    public class Cell
    {
        public readonly TerrainType Type;
        public Assets.AssetImage[] Images;
        public int Index;
        public int animateDelay;
        public int bombTime;
        public int maxBoom;
        public bool rcAllowed;
        public Sprite owner;
        public Cell Next;
        public PowerUpType PowerUpType;

        public Cell(TerrainType type)
        {
            Type = type;
            Index = -1;
        }
    }

    public enum PowerUpType
    {
        Banana,
        ExtraBomb,
        ExtraFire,
        Skull,
        Shield,
        Life,
        RemoteControl,
        Kick,
        RollerSkate,
        Clock,
        MultiBomb,
    }

    public enum GameResult
    {
        None,
        Victory,
        Draw,
    }
}
