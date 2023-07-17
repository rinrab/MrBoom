using MrBoom;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MrBoom
{
    public class Terrain
    {
        public static Random Random = new Random();

        public readonly int Width;
        public readonly int Height;
        public List<Sprite> Players;

        private readonly Cell[] data;
        private int TimeLeft;
        private int time;
        private Assets assets;
        private int levelIndex;
        private List<Spawn> spawns;

        private Assets.Level LevelAssets
        {
            get
            {
                return this.assets.levels[levelIndex];
            }
        }

        private class Spawn
        {
            public int x;
            public int y;
            public bool busy;
        }

        public Terrain(int levelIndex, Assets assets)
        {
            var initial = Map.Maps[levelIndex];
            this.levelIndex = levelIndex;
            this.assets = assets;
            //this.powerUpList = [];
            //for (let bonus of initial.powerUps)
            //{
            //    for (let i = 0; i < bonus.count; i++)
            //    {
            //        this.powerUpList.push(bonus.type);
            //    }
            //}
            this.Width = initial.Data[0].Length;
            this.Height = initial.Data.Length;
            //this.monsters = [];
            this.spawns = new List<Spawn>();
            this.TimeLeft = initial.Time * 60 + 31;
            //this.fin = [];
            //for (let fin of initial.fin)
            //{
            //    const finNum = parseInt(fin);
            //    this.fin.push(finNum);
            //    if (finNum != 255 && finNum > this.maxFin)
            //    {
            //        this.maxFin = finNum;
            //    }
            //}

            this.Players = new List<Sprite>();

            //this.initialBonus = initial.initialBonus;

            data = new Cell[this.Width * this.Height];
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    char src = initial.Data[y][x];
                    //const bonusStr = "0123456789AB";

                    if (src == '#')
                    {
                        this.data[y * this.Width + x] = new Cell(TerrainType.PermanentWall);
                    }
                    else if (src == '-')
                    {
                        this.data[y * this.Width + x] = new Cell(TerrainType.TemporaryWall)
                        {
                            Images = LevelAssets.Walls
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
                    //else if (bonusStr.includes(src))
                    //{
                    //    const index = bonusStr.charAt(src);
                    //    this.data[y * this.width + x] = {
                    //    type: TerrainType.PowerUp,
                    //    image: assets.powerups[index],
                    //    imageIdx: 0,
                    //    animateDelay: 8,
                    //    powerUpType: index
                    //                    }
                    //}
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

        public void Update()
        {
            this.time++;

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
                        //if (!cell.rcAllowed || !cell.owner.rcAllowed || cell.owner.isDie)
                        {
                            cell.bombTime--;
                        }

                        if (cell.bombTime == 0)
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
                        Cell next = new Cell(TerrainType.Free);
                        //this.generateGiven();

                        this.SetCell(x, y, new Cell(TerrainType.PermanentWall)
                        {
                            Images = LevelAssets.Walls,
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
                        //this.playSound("sac");
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

            //this.playSound("bang");

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

        public Cell(TerrainType type)
        {
            Type = type;
            Index = -1;
        }
    }
}
