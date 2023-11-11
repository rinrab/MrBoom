// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    public class RemotePlayer : AbstractPlayer
    {
        public RemotePlayer(Terrain map, Assets.MovingSpriteAssets animations,
                            int team) : base(map, animations, team)
        {
        }

        public void Recieved(byte[] data)
        {
            if (data[0] == 1)
            {
                int x = data[1] * 256 + data[2];
                int y = data[3] * 256 + data[4];

                if (Math.Abs(x - X) + Math.Abs(y - Y) >= 4)
                {
                    X = x;
                    Y = y;
                }

                if (data[5] == 4)
                {
                    Direction = null;
                }
                else
                {
                    Direction = (Directions)data[5];
                }

                int bombsCount = data[6];
                for (int i = 0; i < bombsCount; i++)
                {
                    int startIndex = 7 + i * 4;
                    int bx = data[startIndex + 0];
                    int by = data[startIndex + 1];
                    int estimateTime = data[startIndex + 2];
                    int maxBoom = data[startIndex + 3];

                    Cell bombCell = terrain.GetCell(bx, by);
                    if (bombCell.Type != TerrainType.Bomb || bombCell.owner != this)
                    {
                        bombCell = terrain.PutBomb(bx, by, maxBoom, false, this);
                    }
                    bombCell.bombCountdown = estimateTime;
                }
            }
        }
    }
}
