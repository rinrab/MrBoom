using MrBoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MrBoom
{
    public class Terrain
    {
        public readonly int Width;
        public readonly int Height;
        public List<Sprite> Players;

        private readonly Cell[] data;
        private int TimeLeft;
        private Assets.Level LevelAssets;

        public Terrain(Map initial, Assets.Level levelAssets)
        {
            this.LevelAssets = levelAssets;
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
            //this.spawns = [];
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
                        //this.spawns.push({ x: x, y: y });
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

        public void LocateSprite(Sprite sprite)
        {
            sprite.x = 1 * 16;
            sprite.y = 1 * 16;

            this.Players.Add(sprite);
        }

        public void Update()
        {
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
        Rubber
    }

    public class Cell
    {
        public readonly TerrainType Type;
        public Assets.AssetImage[] Images;
        public int Index;

        public Cell(TerrainType type)
        {
            Type = type;
        }
    }
}
