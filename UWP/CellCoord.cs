// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public readonly struct CellCoord
    {
        private readonly int x;
        private readonly int y;

        public CellCoord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X => x;
        public int Y => y;
    }
}
