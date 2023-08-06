using System;
using System.Collections.Generic;

namespace MrBoom
{
    public class Terrain
    {
        public static Random Random = new Random();

        public readonly int Width;
        public readonly int Height;
        public Assets assets;
        public int TimeLeft;
        public Sound SoundsToPlay;
        public GameResult Result = GameResult.None;
        public int StartMaxFire;
        public int StartMaxBombsCount;
        public int ApocalypseSpeed = 2;
        public int MaxApocalypse;

        public Assets.Level LevelAssets
        {
            get
            {
                return levelAssets;
            }
        }

        public Player Winner { get; private set; }

        private readonly byte[] final;
        private int lastApocalypseSound = -1;
        private readonly Cell[] data;
        private int timeToEnd = -1;
        private int time;
        private readonly List<Spawn> spawns;
        private readonly List<PowerUpType> powerUpList;
        private readonly Map map;
        private readonly List<Player> players;
        private readonly List<Monster> monsters;

        public Feature StartFeatures { get; }

        private readonly Assets.Level levelAssets;

        private class Spawn
        {
            public int x;
            public int y;
            public bool busy;
        }

        public Terrain(int levelIndex, Assets assets)
        {
            monsters = new List<Monster>();

            this.assets = assets;
            this.levelAssets = assets.Levels[levelIndex];
            this.map = Map.Maps[levelIndex];
            this.StartFeatures = Map.Maps[levelIndex].StartFeatures;
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
            this.spawns = new List<Spawn>();
            this.TimeLeft = (map.Time + 31) * 60;
            this.final = map.Final;
            foreach (int fin in final)
            {
                if (fin != 255)
                {
                    MaxApocalypse = Math.Max(fin, MaxApocalypse);
                }
            }
            this.players = new List<Player>();

            this.StartMaxBombsCount = map.StartMaxBombsCount;
            this.StartMaxFire = map.StartMaxFire;

            data = new Cell[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    char src = map.Data[y][x];

                    string bonusStr = "123456789AB";
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

        public void LocateSprite(Player sprite, int index = -1)
        {
            var spawn = this.spawns[this.generateSpawn(index)];
            sprite.x = spawn.x * 16;
            sprite.y = spawn.y * 16;

            this.players.Add(sprite);
        }

        public void InitializeMonsters()
        {
            int count = this.spawns.Count - this.players.Count;

            for (int i = 0; i < count; i++)
            {
                var data = map.Monsters[Random.Next(map.Monsters.Length)];
                Monster monster = new Monster(this, data, assets.Monsters[data.Type]);
                int spawn = generateSpawn();
                monster.x = spawns[spawn].x * 16;
                monster.y = spawns[spawn].y * 16;
                this.monsters.Add(monster);
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
            foreach (Player player in this.players)
            {
                if (!player.IsDie)
                {
                    playersCount++;
                }
            }

            if (TimeLeft < 30 * 60 - 1)
            {
                if (TimeLeft % ApocalypseSpeed == 0)
                {
                    int index = (30 * 60 - TimeLeft) / ApocalypseSpeed;
                    if (index != 255)
                    {
                        for (int i = 0; i < final.Length; i++)
                        {
                            if (index == Math.Min(MaxApocalypse + 5, 255))
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
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (!players[i].IsDie)
                        {
                            Winner = players[i];
                        }
                    }
                    Result = GameResult.Victory;
                }
                else
                {
                    Result = GameResult.Draw;
                }
            }

            if (TimeLeft + ApocalypseSpeed * MaxApocalypse <= 0)
            {
                Result = GameResult.Draw;
            }

            if (playersCount == 1 && players.Count > 1 && this.timeToEnd == -1)
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
                        if (!cell.rcAllowed || !cell.owner.Features.HasFlag(
                            Feature.RemoteControl) || cell.owner.IsDie)
                        {
                            cell.bombTime--;
                        }

                        if (cell.bombTime == 0 || (cell.owner.rcDitonate && cell.rcAllowed))
                        {
                            this.ditonateBomb(x, y);
                            continue;
                        }
                        if (cell.OffsetX == 0 && cell.OffsetY == 0)
                        {
                            var next = this.GetCell(x + cell.DeltaX / 2, y + cell.DeltaY / 2);
                            if (next.Type == TerrainType.Rubber)
                            {
                                cell.DeltaX = -cell.DeltaX;
                                cell.DeltaY = -cell.DeltaY;
                            }
                            else if (next.Type == TerrainType.Bomb && ((cell.DeltaX != 0 && next.DeltaX != 0) || (cell.DeltaY != 0 && next.DeltaY != 0)))
                            {
                                ditonateBomb(x, y);
                                continue;
                            }
                            else if (next.Type != TerrainType.Free)
                            {
                                cell.DeltaY = 0;
                                cell.DeltaX = 0;
                            }
                        }

                        int newX = (x * 16 + cell.OffsetX + cell.DeltaX + 8) / 16;
                        int newY = (y * 16 + cell.OffsetY + cell.DeltaY + 8) / 16;

                        if (newX != x || newY != y)
                        {
                            if (GetCell(newX, newY).Type == TerrainType.Free)
                            {
                                SetCell(x, y, new Cell(TerrainType.Free));
                                SetCell(newX, newY, cell);

                                cell.OffsetX += (x - newX) * 16;
                                cell.OffsetY += (y - newY) * 16;
                            }
                            else
                            {
                                ditonateBomb(x, y);
                                continue;
                            }
                        }

                        cell.OffsetX += cell.DeltaX;
                        cell.OffsetY += cell.DeltaY;
                    }
                }
            }

            foreach (Sprite sprite in GetSprites())
            {
                sprite.Update();
            }

            foreach (Sprite sprite1 in GetSprites())
            {
                foreach (Sprite sprite2 in GetSprites())
                {
                    if (sprite1.CellX == sprite2.CellX &&
                        sprite1.CellY == sprite2.CellY &&
                        sprite1.Skull.HasValue &&
                        !sprite2.Skull.HasValue)
                    {
                        sprite2.SetSkull(sprite1.Skull.Value);
                    }
                }
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

        public bool IsWalkable(int x, int y)
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

        public void RcDitonate(Player player)
        {

        }

        public void ditonateBomb(int bombX, int bombY)
        {
            Cell bombCell = this.GetCell(bombX, bombY); ;
            int maxBoom = bombCell.maxBoom;
            bombCell.owner.BombsPlaced--;

            void burn(int dx, int dy, AnimatedImage image, AnimatedImage imageEnd)
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

        public void PutBomb(int cellX, int cellY, int maxBoom, bool rcAllowed, Player owner)
        {
            SetCell(cellX, cellY, new Cell(TerrainType.Bomb)
            {
                Images = assets.Bomb,
                Index = 0,
                animateDelay = 12,
                bombTime = 210,
                maxBoom = maxBoom,
                rcAllowed = rcAllowed,
                owner = owner
            });
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

        public bool IsTouchingMonster(int cellX, int cellY)
        {
            foreach (Monster m in monsters)
            {
                if (!m.IsDie && (m.x + 8) / 16 == cellX && (m.y + 8) / 16 == cellY)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<Sprite> GetSprites()
        {
            foreach (Sprite sprite in players)
            {
                yield return sprite;
            }

            foreach (Sprite sprite in monsters)
            {
                yield return sprite;
            }
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
        public AnimatedImage Images;
        public int Index;
        public int animateDelay;
        public int bombTime;
        public int maxBoom;
        public bool rcAllowed;
        public Player owner;
        public Cell Next;
        public PowerUpType PowerUpType;
        public int OffsetX;
        public int OffsetY;
        public int DeltaX;
        public int DeltaY;

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

    [Flags]
    public enum Feature
    {
        MultiBomb = 0x01,
        RemoteControl = 0x02,
        Kick = 0x04,
        RollerSkates = 0x08,
    }

    public enum GameResult
    {
        None,
        Victory,
        Draw,
    }
}
