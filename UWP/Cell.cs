// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Cell
    {
        public readonly TerrainType Type;
        public AnimatedImage Images;
        public int Index;
        public int animateDelay;
        public int bombCountdown;
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
}
