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
        }
    }
}
