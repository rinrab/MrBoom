// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using MrBoom.Bot;
using static System.Net.Mime.MediaTypeNames;

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
        public int ApocalypseSpeed = 2;
        public int MaxApocalypse;

        public Assets.Level LevelAssets => levelAssets;
        public int Winner { get; private set; }

        private readonly byte[] final;
        private int lastApocalypseSound = -1;
        private readonly Cell[] data;
        private int timeToEnd = -1;
        private int time;
        private readonly List<Spawn> spawns;
        private readonly List<PowerUpType> powerUpList;
        private readonly Map map;
        private readonly List<AbstractPlayer> players;
        private readonly List<Monster> monsters;
        private readonly int startMaxFire;
        private readonly int startMaxBombsCount;

        public Feature StartFeatures { get; }

        private readonly Assets.Level levelAssets;
        private readonly Grid<bool> hasMonsterGrid;
        private readonly Grid<bool> isMonsterComingGrid;

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
            this.players = new List<AbstractPlayer>();

            this.startMaxBombsCount = map.StartMaxBombsCount;
            this.startMaxFire = map.StartMaxFire;

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

            hasMonsterGrid = new Grid<bool>(Width, Height, false);
            isMonsterComingGrid = new Grid<bool>(Width, Height, false);
        }

        public void AddPlayer(Assets.MovingSpriteAssets movingSpriteAssets, IController controller, int team)
        {
            var spawn = this.spawns[this.generateSpawn()];

            AbstractPlayer sprite = new Human(
                this, movingSpriteAssets,
                spawn.x * 16, spawn.y * 16,
                controller, startMaxFire, startMaxBombsCount, team);

            this.players.Add(sprite);
        }

        public void AddComputer(Assets.MovingSpriteAssets movingSpriteAssets, int team)
        {
            var spawn = this.spawns[this.generateSpawn()];

            AbstractPlayer sprite = new ComputerPlayer(
                this, movingSpriteAssets,
                spawn.x * 16, spawn.y * 16,
                startMaxFire, startMaxBombsCount, team);

            this.players.Add(sprite);
        }

        public void InitializeMonsters()
        {
            int count = this.spawns.Count - this.players.Count;

            for (int i = 0; i < count; i++)
            {
                var data = map.Monsters[Random.Next(map.Monsters.Length)];
                var spawn = spawns[generateSpawn()];
                Monster monster = new Monster(
                    this, data, assets.Monsters[data.Type],
                    spawn.x * 16, spawn.y * 16);
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
                                if (cell.Type == TerrainType.Bomb)
                                {
                                    cell.owner.BombsPlaced--;
                                }
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

            if (map.BombApocalypse && TimeLeft < 30 * 60 - ApocalypseSpeed * MaxApocalypse)
            {
                if (TimeLeft % 16 == 0)
                {
                    int rndX = Random.Next(2);
                    int x = (rndX == 0) ? 1 : Width - 2;
                    int y = (Random.Next(0, Height / 2)) * 2 + 1;

                    PutBomb(x, y, 4, false, null);
                    GetCell(x, y).DeltaX = (rndX == 0) ? 2 : -2;
                }
            }

            int playersCount = 0;
            foreach (AbstractPlayer player in this.players)
            {
                if (player.IsAlive)
                {
                    playersCount++;
                }
            }

            if (timeToEnd == -1)
            {
                if (playersCount == 0)
                {
                    timeToEnd = 60 * 3;
                }
                else if (players.Count != 1)
                {
                    List<int> live = new List<int>();
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].IsAlive)
                        {
                            live.Add(players[i].Team);
                        }
                    }

                    if (Array.TrueForAll(live.ToArray(), val => live[0] == val))
                    {
                        timeToEnd = 60 * 3;
                    }
                }
            }

            if (timeToEnd == 0)
            {
                if (playersCount >= 1)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].IsAlive)
                        {
                            Winner = players[i].Team;
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

                        if (cell.bombTime == 0 || (cell.owner != null && cell.owner.rcDitonate && cell.rcAllowed))
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

            hasMonsterGrid.Reset(false);
            isMonsterComingGrid.Reset(false);
            foreach (Monster m in monsters)
            {
                if (m.IsAlive)
                {
                    hasMonsterGrid[m.CellX, m.CellY] = true;
                    isMonsterComingGrid[m.CellX + m.Direction.DeltaX(), m.CellY + m.Direction.DeltaY()] = true;
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
                        if (sprite1.IsAlive && sprite2.IsAlive)
                        {
                            sprite2.SetSkull(sprite1.Skull.Value);
                        }
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

        public void RcDitonate(AbstractPlayer player)
        {

        }

        public void ditonateBomb(int bombX, int bombY)
        {
            Cell bombCell = this.GetCell(bombX, bombY); ;
            int maxBoom = bombCell.maxBoom;

            if (bombCell.owner != null)
            {
                bombCell.owner.BombsPlaced--;
            }

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

        public void PutBomb(int cellX, int cellY, int maxBoom, bool rcAllowed, AbstractPlayer owner)
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
            return hasMonsterGrid[cellX, cellY];
        }

        public bool IsMonsterComing(int cellX, int cellY)
        {
            return isMonsterComingGrid[cellX, cellY];
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

        public string GetCellDebugInfo(int cellX, int cellY)
        {
            List<string> cellDebugInfo = new List<string>();

            foreach (Sprite sprite in players)
            {
                if (sprite.IsAlive)
                {
                    string debugInfo = sprite.GetCellDebugInfo(cellX, cellY);

                    if (!string.IsNullOrEmpty(debugInfo))
                    {
                        cellDebugInfo.Add(debugInfo);
                    }
                }
            }

            return string.Join('\n', cellDebugInfo);
        }

        public string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (AbstractPlayer player in players)
            {
                sb.Append(player.GetDebugInfo() + "\n");
            }

            return
                $"DEBUG INFO\n" +
                $"Version: {Game.Version}\n" +
                sb.ToString() +
                $"F1 - detonate all\n" +
                $"F2 - clear all\n" +
                $"F3 - apocalypse\n" +
                $"F4 - toggle debug info\n" +
                $"F5 - give all";
        }

        public void DetonateAll(bool generateBonus)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Type == TerrainType.TemporaryWall)
                {
                    Cell next = generateBonus ? GenerateGiven() : new Cell(TerrainType.Free);

                    data[i] = new Cell(TerrainType.PermanentWall)
                    {
                        Images = levelAssets.Walls,
                        Index = 0,
                        animateDelay = 4,
                        Next = next
                    };
                }
            }
        }

        public void StartApocalypse()
        {
            const int timeToApocalypse = (30 + 2) * 60;

            if (TimeLeft > timeToApocalypse)
            {
                TimeLeft = timeToApocalypse;
            }
        }

        public void GiveAll()
        {
            foreach (AbstractPlayer player in players)
            {
                player.GiveAll();
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
        public AbstractPlayer owner;
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
