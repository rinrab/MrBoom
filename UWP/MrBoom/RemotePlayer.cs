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
            int x = data[0] * 256 + data[1];
            int y = data[2] * 256 + data[3];

            if (Math.Abs(x - X) + Math.Abs(y - Y) >= 4)
            {
                X = x;
                Y = y;
            }

            if (data[4] == 4)
            {
                Direction = null;
            }
            else
            {
                Direction = (Directions)data[4];
            }

            int bombsCount = data[5];
            for (int i = 0; i < bombsCount; i++)
            {
                int startIndex = 6 + i * 4;
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
