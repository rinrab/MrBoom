// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using MrBoom.NetworkProtocol;

namespace MrBoom
{
    public class Terrain
    {
        public Random Random;

        public readonly int Width;
        public readonly int Height;
        public int TimeLeft;
        public Sound SoundsToPlay;
        public GameResult Result = GameResult.None;
        public int ApocalypseSpeed = 2;
        public int MaxApocalypse;

        public int Winner { get; private set; }

        private const int FLAME_ANIMATION_DELAY = 6;
        public const int FLAME_ANIMATION_LENGTH = 4;
        private const int WALL_ANIMATION_DELAY = 4;
        public const int WALL_ANIMATION_LENGTH = 8;
        public const int FIRE_ANIMATION_LENGTH = 6;
        public const int FIRE_ANIMATION_DELAY = 6;

        public int FlameDuration => FLAME_ANIMATION_DELAY * FLAME_ANIMATION_LENGTH;

        private readonly Grid<byte> final;
        private int lastApocalypseSound = -1;
        private readonly Grid<Cell> data;
        private int timeToEnd = -1;
        private readonly List<CellCoord> spawns;
        private readonly List<PowerUpType> powerUpList;
        private readonly Map mapData;
        private readonly List<AbstractPlayer> players;
        private readonly List<AbstractMonster> monsters;

        public int time;

        public readonly Feature StartFeatures;
        public readonly int StartMaxFire;
        public readonly int StartMaxBombsCount;

        private readonly Grid<bool> hasMonsterGrid;
        private readonly Grid<bool> isMonsterComingGrid;
        private readonly Grid<int> killablePlayerGrid;

        public Terrain(int levelIndex, int? seed = null)
        {
            Random = seed.HasValue ? new Random(seed.Value) : new Random();

            monsters = new List<AbstractMonster>();

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

            data = new Grid<Cell>(Width, Height, new Cell(TerrainType.PermanentWall, time));
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    char src = mapData.Data[y][x];

                    string bonusStr = "123456789AB";
                    if (src == '#')
                    {
                        data[x, y] = new Cell(TerrainType.PermanentWall, time);
                    }
                    else if (src == '-')
                    {
                        data[x, y] = new Cell(TerrainType.TemporaryWall, time)
                        {
                            ImageType = CellImageType.Walls,
                        };
                    }
                    else if (src == '*')
                    {
                        spawns.Add(new CellCoord(x, y));
                        data[x, y] = new Cell(TerrainType.Free, time);
                    }
                    else if (src == '%')
                    {
                        data[x, y] = new Cell(TerrainType.Rubber, time);
                    }
                    else if (bonusStr.Contains(src.ToString()))
                    {
                        int index = bonusStr.IndexOf(src);
                        data[x, y] = new Cell(TerrainType.PowerUp, time)
                        {
                            ImageType = powerUps[index], // TODO: Add function
                            animateDelay = 8,
                            PowerUpType = (PowerUpType)index
                        };
                    }
                    else
                    {
                        data[x, y] = new Cell(TerrainType.Free, time);
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

        public void InitializeMonsters(Assets.MovingSpriteAssets[] assets)
        {
            while (true)
            {
                var spawn = GenerateSpawn();
                if (!spawn.HasValue)
                {
                    break;
                }

                var data = Random.NextElement(mapData.Monsters);

                AbstractMonster monster = data.GetMonster(this, assets[data.Type], spawn.Value.X * 16, spawn.Value.Y * 16);

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
                    // Blow cell if final index is 255
                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        data[i] = new Cell(TerrainType.PowerUpFire, time)
                        {
                            ImageType = CellImageType.PowerUpFire,
                            Next = new Cell(TerrainType.Free, time)
                        };
                        PlaySound(Sound.Sac);
                    }
                }
                else if (final[i] == index && final[i] != 255)
                {
                    // Replace cell with apocalypse cell
                    if (cell.Type == TerrainType.Bomb)
                    {
                        cell.owner.BombsPlaced--;
                    }
                    if (cell.Type != TerrainType.PermanentWall)
                    {
                        data[i] = new Cell(TerrainType.Apocalypse, time)
                        {
                            ImageType = CellImageType.Apocalypse,
                            Next = new Cell(TerrainType.PermanentWall, time)
                            {
                                ImageType = CellImageType.Apocalypse,
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

            if (mapData.IsBombApocalypse && index > 0)
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
                    int animateDelay = (cell.animateDelay <= 0) ? 6 : cell.animateDelay;

                    if (cell.TimeToNext != -1)
                    {
                        cell.TimeToNext--;
                        if (cell.TimeToNext == 0)
                        {
                            if (cell.Next == null)
                            {
                                cell.StartTick = time;
                            }
                            else
                            {
                                data[x, y] = cell.Next;
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
                                data[x, y] = new Cell(TerrainType.Free, time);
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

            void burn(int dx, int dy, CellImageType image, CellImageType imageEnd)
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

                        data[x, y] = new Cell(TerrainType.PermanentWall, time)
                        {
                            ImageType = CellImageType.Walls,
                            animateDelay = WALL_ANIMATION_DELAY,
                            TimeToNext = WALL_ANIMATION_LENGTH * WALL_ANIMATION_DELAY,
                            Next = next
                        };
                        break;
                    }
                    else if (cell.Type == TerrainType.PowerUp)
                    {
                        data[x, y] = new Cell(TerrainType.PowerUpFire, time)
                        {
                            ImageType = CellImageType.PowerUpFire,
                            animateDelay = FIRE_ANIMATION_DELAY,
                            TimeToNext = FIRE_ANIMATION_LENGTH * FIRE_ANIMATION_DELAY,
                            Next = new Cell(TerrainType.Free, time)
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
                        data[x, y] = new Cell(TerrainType.Fire, time)
                        {
                            ImageType = i == maxBoom ? imageEnd : image,
                            animateDelay = FLAME_ANIMATION_DELAY,
                            TimeToNext = FlameDuration,
                            Next = new Cell(TerrainType.Free, time)
                        };
                    }
                }
            }

            PlaySound(Sound.Bang);

            data[bombX, bombY] = new Cell(TerrainType.Fire, time)
            {
                ImageType = CellImageType.BoomMid,
                animateDelay = FLAME_ANIMATION_DELAY,
                TimeToNext = FlameDuration,
                Next = new Cell(TerrainType.Free, time)
            };

            burn(1, 0, CellImageType.BoomHor, CellImageType.BoomRightEnd);
            burn(-1, 0, CellImageType.BoomHor, CellImageType.BoomLeftEnd);
            burn(0, 1, CellImageType.BoomVert, CellImageType.BoomBottomEnd);
            burn(0, -1, CellImageType.BoomVert, CellImageType.BoomTopEnd);
        }

        private static readonly CellImageType[] powerUps = new CellImageType[]
        {
            CellImageType.Banana,
            CellImageType.ExtraBomb,
            CellImageType.ExtraFire,
            CellImageType.Skull,
            CellImageType.Shield,
            CellImageType.Life,
            CellImageType.RemoteControl,
            CellImageType.Kick,
            CellImageType.RollerSkate,
            CellImageType.Clock,
            CellImageType.MultiBomb,
        };

        public Cell GeneratePowerUp(PowerUpType powerUpType)
        {
            return new Cell(TerrainType.PowerUp, time)
            {
                ImageType = powerUps[(int)powerUpType], // TODO: Add function
                animateDelay = 8,
                PowerUpType = powerUpType
            };
        }

        public Cell PutBomb(int cellX, int cellY, int maxBoom, bool rcAllowed, AbstractPlayer owner)
        {
            Cell newCell = new Cell(TerrainType.Bomb, time)
            {
                ImageType = CellImageType.Bomb,
                animateDelay = 12,
                bombCountdown = 210,
                maxBoom = maxBoom,
                rcAllowed = rcAllowed,
                owner = owner
            };

            data[cellX, cellY] = newCell;
            return newCell;
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
                return new Cell(TerrainType.Free, time);
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
                    Cell next = generateBonus ? GenerateGiven() : new Cell(TerrainType.Free, time);

                    data[i] = new Cell(TerrainType.PermanentWall, time)
                    {
                        ImageType = CellImageType.Walls,
                        animateDelay = 4,
                        TimeToNext = WALL_ANIMATION_LENGTH * WALL_ANIMATION_DELAY,
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

        public void Recieved(ClientGameStateMessage.GameData data)
        {
            foreach (AbstractPlayer player in players)
            {
                if (player is RemotePlayer remotePlayer)
                {
                    remotePlayer.Recieved(data.Players[0]);
                }
            }
        }

        public ClientGameStateMessage.GameData GetDataToSend()
        {
            List<ClientGameStateMessage.PlayerData> playersData = new List<ClientGameStateMessage.PlayerData>();

            foreach (AbstractPlayer player in players)
            {
                if (player is Human human)
                {
                    playersData.Add(human.GetDataToSend());
                }
            }

            return new ClientGameStateMessage.GameData
            {
                Players = playersData.ToArray(),
            };
        }

        public IEnumerable<Tuple<CellCoord, Cell>> GetMyBombs(AbstractPlayer owner)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = GetCell(x, y);
                    if (cell.Type == TerrainType.Bomb && cell.owner == owner)
                    {
                        yield return new Tuple<CellCoord, Cell>(new CellCoord(x, y), cell);
                    }
                }
            }
        }
    }
}
