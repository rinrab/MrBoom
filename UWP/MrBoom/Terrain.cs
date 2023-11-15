// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

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

        public int FlameDuration
        {
            get
            {
                return assets.BoomMid.Length * FLAME_ANIMATION_DELAY;
            }
        }
        private readonly Grid<byte> final;
        private int lastApocalypseSound = -1;
        private readonly Grid<Cell> data;
        private int timeToEnd = -1;
        private int time;
        private const int FLAME_ANIMATION_DELAY = 6;
        private readonly List<CellCoord> spawns;
        private readonly List<PowerUpType> powerUpList;
        private readonly Map mapData;
        private readonly List<AbstractPlayer> players;
        private readonly List<AbstractMonster> monsters;

        public readonly Feature StartFeatures;
        public readonly int StartMaxFire;
        public readonly int StartMaxBombsCount;

        private readonly Assets.Level levelAssets;
        private readonly Grid<bool> hasMonsterGrid;
        private readonly Grid<bool> isMonsterComingGrid;
        private readonly Grid<int> killablePlayerGrid;

        public Terrain(int levelIndex, Assets assets)
        {
            monsters = new List<AbstractMonster>();

            this.assets = assets;
            levelAssets = assets.Levels[levelIndex];
            mapData = MapData.Data[levelIndex];
            StartFeatures = mapData.StartFeatures;
            powerUpList = new List<PowerUpType>();

            foreach (var bonus in mapData.PowerUps)
            {
                for (int i = 0; i < bonus.Count; i++)
                {
                    powerUpList.Add(bonus.Type);
                }
            }
            Width = mapData.Data[0].Length;
            Height = mapData.Data.Length;
            spawns = new List<CellCoord>();
            TimeLeft = (mapData.Time + 31) * 60;
            final = new Grid<byte>(Width, Height, 255);
            for (int i = 0; i < final.CellCount; i++)
            {
                byte fin = mapData.Final[i];
                final[i] = fin;
                if (fin != 255)
                {
                    MaxApocalypse = Math.Max(fin, MaxApocalypse);
                }
            }
            players = new List<AbstractPlayer>();

            StartMaxBombsCount = mapData.StartMaxBombsCount;
            StartMaxFire = mapData.StartMaxFire;

            data = new Grid<Cell>(Width, Height, new Cell(TerrainType.PermanentWall));
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char src = mapData.Data[y][x];

                    string bonusStr = "123456789AB";
                    if (src == '#')
                    {
                        data[x, y] = new Cell(TerrainType.PermanentWall);
                    }
                    else if (src == '-')
                    {
                        data[x, y] = new Cell(TerrainType.TemporaryWall)
                        {
                            Images = levelAssets.Walls
                        };
                    }
                    else if (src == '*')
                    {
                        spawns.Add(new CellCoord(x, y));
                        data[x, y] = new Cell(TerrainType.Free);
                    }
                    else if (src == '%')
                    {
                        data[x, y] = new Cell(TerrainType.Rubber);
                    }
                    else if (bonusStr.Contains(src.ToString()))
                    {
                        int index = bonusStr.IndexOf(src);
                        data[x, y] = new Cell(TerrainType.PowerUp)
                        {
                            Images = assets.PowerUps[index],
                            Index = 0,
                            animateDelay = 8,
                            PowerUpType = (PowerUpType)index
                        };
                    }
                    else
                    {
                        data[x, y] = new Cell(TerrainType.Free);
                    }
                }
            }

            hasMonsterGrid = new Grid<bool>(Width, Height, false);
            isMonsterComingGrid = new Grid<bool>(Width, Height, false);
            killablePlayerGrid = new Grid<int>(Width, Height, 0);
            Random.Shuffle(spawns);
        }

        public void AddPlayer(AbstractPlayer player)
        {
            CellCoord spawn = GenerateSpawn().Value;

            player.X = spawn.X * 16;
            player.Y = spawn.Y * 16;

            players.Add(player);
        }

        public void InitializeMonsters()
        {
            while (true)
            {
                var spawn = GenerateSpawn();
                if (!spawn.HasValue)
                {
                    break;
                }

                var data = Random.NextElement(mapData.Monsters);

                AbstractMonster monster = data.GetMonster(this, assets.Monsters[data.Type], spawn.Value.X * 16, spawn.Value.Y * 16);

                monsters.Add(monster);
            }
        }

        public int GetCellApocalypseRemainingTime(int cellX, int cellY)
        {
            byte apocalypseIndex = final[cellX, cellY];
            if (apocalypseIndex == 255)
            {
                return int.MaxValue;
            }
            else
            {
                return apocalypseIndex * ApocalypseSpeed + TimeLeft - 30 * 60;
            }
        }

        public void Update()
        {
            SoundsToPlay = 0;
            time++;
            TimeLeft--;

            if (timeToEnd != -1)
            {
                timeToEnd--;
            }

            int index = (30 * 60 - TimeLeft) / ApocalypseSpeed;
            for (int i = 0; i < final.CellCount; i++)
            {
                Cell cell = data[i];
                if (index == MaxApocalypse + 5)
                {
                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        data[i] = new Cell(TerrainType.PowerUpFire)
                        {
                            Images = assets.Fire,
                            Index = 0,
                            Next = new Cell(TerrainType.Free)
                        };
                        PlaySound(Sound.Sac);
                    }
                }
                else if (final[i] == index && final[i] != 255)
                {
                    if (cell.Type == TerrainType.Bomb)
                    {
                        cell.owner.BombsPlaced--;
                    }
                    if (cell.Type != TerrainType.PermanentWall)
                    {
                        data[i] = new Cell(TerrainType.Apocalypse)
                        {
                            Images = levelAssets.PermanentWalls,
                            Index = 0,
                            Next = new Cell(TerrainType.PermanentWall)
                            {
                                Images = levelAssets.PermanentWalls,
                            }
                        };
                        if (Math.Abs(lastApocalypseSound - TimeLeft) > 60)
                        {
                            PlaySound(Sound.Sac);
                            lastApocalypseSound = TimeLeft;
                        }
                    }
                }
            }

            if (mapData.BombApocalypse && TimeLeft < 30 * 60 - ApocalypseSpeed * MaxApocalypse)
            {
                if (TimeLeft % 16 == 0)
                {
                    Directions dir = Random.NextElement(new Directions[] { Directions.Left, Directions.Right });
                    int x = (dir == Directions.Right) ? 1 : Width - 2;
                    int y = (Random.Next(0, Height / 2)) * 2 + 1;

                    PutBomb(x, y, 4, false, null);
                    data[x, y].DeltaX = dir.DeltaX() * 2;
                }
            }

            int playersCount = 0;
            foreach (AbstractPlayer player in players)
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

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell cell = data[x, y];
                    if (cell.Index != -1)
                    {
                        int animateDelay = (cell.animateDelay <= 0) ? 6 : cell.animateDelay;
                        if (time % animateDelay == 0)
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
                                    data[x, y] = cell.Next;
                                }
                            }
                        }
                    }

                    if (cell.Type == TerrainType.Bomb)
                    {
                        if (!cell.rcAllowed || !cell.owner.Features.HasFlag(
                            Feature.RemoteControl) || cell.owner.IsDie)
                        {
                            cell.bombCountdown--;
                        }

                        if (cell.bombCountdown == 0 || (cell.owner != null && cell.owner.rcDitonate && cell.rcAllowed))
                        {
                            DitonateBomb(x, y);
                            continue;
                        }
                        if (cell.OffsetX == 0 && cell.OffsetY == 0)
                        {
                            var next = data[x + cell.DeltaX / 2, y + cell.DeltaY / 2];
                            if (next.Type == TerrainType.Rubber)
                            {
                                cell.DeltaX = -cell.DeltaX;
                                cell.DeltaY = -cell.DeltaY;
                            }
                            else if (next.Type == TerrainType.Bomb && ((cell.DeltaX != 0 && next.DeltaX != 0) || (cell.DeltaY != 0 && next.DeltaY != 0)))
                            {
                                DitonateBomb(x, y);
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
                            if (data[newX, newY].Type == TerrainType.Free)
                            {
                                data[x, y] = new Cell(TerrainType.Free);
                                data[newX, newY] = cell;

                                cell.OffsetX += (x - newX) * 16;
                                cell.OffsetY += (y - newY) * 16;
                            }
                            else
                            {
                                DitonateBomb(x, y);
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
            foreach (AbstractMonster m in monsters)
            {
                if (m.IsAlive)
                {
                    hasMonsterGrid[m.CellX, m.CellY] = true;
                    isMonsterComingGrid[m.CellX + m.Direction.DeltaX(), m.CellY + m.Direction.DeltaY()] = true;
                }
            }

            killablePlayerGrid.Reset(0);
            foreach (AbstractPlayer player in players)
            {
                // TODO: Check for unplugin.
                killablePlayerGrid[player.CellX, player.CellY] |= (1 << player.Team);
            }

            foreach (Sprite sprite in GetSprites())
            {
                sprite.Update();
                PlaySound(sprite.SoundsToPlay);
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
            return data[x, y];
        }

        public void SetCell(int x, int y, Cell cell)
        {
            data[x, y] = cell;
        }

        public bool IsWalkable(int x, int y)
        {
            Cell cell = data[x, y];

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

        public void DitonateBomb(int bombX, int bombY)
        {
            Cell bombCell = data[bombX, bombY];
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
                    Cell cell = data[x, y];

                    if (cell.Type == TerrainType.PermanentWall ||
                        cell.Type == TerrainType.Apocalypse ||
                        cell.Type == TerrainType.Rubber)
                    {
                        break;
                    };

                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        Cell next = GenerateGiven();

                        data[x, y] = new Cell(TerrainType.PermanentWall)
                        {
                            Images = levelAssets.Walls,
                            Index = 0,
                            animateDelay = 4,
                            Next = next
                        };
                        break;
                    }
                    else if (cell.Type == TerrainType.PowerUp)
                    {
                        data[x, y] = new Cell(TerrainType.PowerUpFire)
                        {
                            Images = assets.Fire,
                            Index = 0,
                            animateDelay = 6,
                            Next = new Cell(TerrainType.Free)
                        };
                        PlaySound(Sound.Sac);
                        break;
                    }
                    else if (cell.Type == TerrainType.Bomb)
                    {
                        DitonateBomb(x, y);
                        break;
                    }
                    else if (cell.Type == TerrainType.Fire ||
                             cell.Type == TerrainType.PowerUpFire)
                    {
                    }
                    else
                    {
                        data[x, y] = new Cell(TerrainType.Fire)
                        {
                            Images = i == maxBoom ? imageEnd : image,
                            Index = 0,
                            animateDelay = FLAME_ANIMATION_DELAY,
                            Next = new Cell(TerrainType.Free)
                        };
                    }
                }
            }

            PlaySound(Sound.Bang);

            data[bombX, bombY] = new Cell(TerrainType.Fire)
            {
                Images = assets.BoomMid,
                Index = 0,
                animateDelay = FLAME_ANIMATION_DELAY,
                Next = new Cell(TerrainType.Free)
            };

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
            data[cellX, cellY] = new Cell(TerrainType.Bomb)
            {
                Images = assets.Bomb,
                Index = 0,
                animateDelay = 12,
                bombCountdown = 210,
                maxBoom = maxBoom,
                rcAllowed = rcAllowed,
                owner = owner
            };
        }

        Cell GenerateGiven()
        {
            int rnd = Random.Next(int.MaxValue);
            if (rnd < int.MaxValue / 2)
            {
                var powerUpType = Random.NextElement(powerUpList);

                return GeneratePowerUp(powerUpType);
            }
            else
            {
                return new Cell(TerrainType.Free);
            }
        }

        CellCoord? GenerateSpawn()
        {
            if (spawns.Count <= 0)
            {
                return null;
            }

            int spawnIndex = Random.Next(spawns.Count);

            var spawn = spawns[spawnIndex];
            spawns.RemoveAt(spawnIndex);
            return spawn;
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

        public int GetKillablePlayers(int cellX, int cellY)
        {
            return killablePlayerGrid[cellX, cellY];
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
            List<string> list = new List<string>();

            foreach (Sprite sprite in players)
            {
                if (sprite.IsAlive)
                {
                    string debugInfo = sprite.GetCellDebugInfo(cellX, cellY);

                    if (!string.IsNullOrEmpty(debugInfo) && !list.Contains(debugInfo))
                    {
                        list.Add(debugInfo);
                    }
                }
            }

            return string.Join('\n', list);
        }

        public string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"DEBUG INFO");
            sb.AppendLine($"Version: {Game.Version}");

            foreach (Sprite sprite in GetSprites())
            {
                sb.AppendLine(sprite.GetDebugInfo());
            }

            sb.AppendLine($"F1 - detonate all");
            sb.AppendLine($"F2 - clear all");
            sb.AppendLine($"F3 - apocalypse");
            sb.AppendLine($"F4 - toggle debug info");
            sb.AppendLine($"F5 - give all");

            return sb.ToString();
        }

        public void DetonateAll(bool generateBonus)
        {
            for (int i = 0; i < data.CellCount; i++)
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
}
