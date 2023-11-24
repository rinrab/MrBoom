// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    public class Cell
    {
        public readonly TerrainType Type;
        public AnimatedImage Images;
        public CellImageType ImageType;
        public int StartTick;
        public int TimeToNext;
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

        public Cell(TerrainType type, int startTick)
        {
            Type = type;
            StartTick = startTick;
            TimeToNext = -1;
            ImageType = CellImageType.Free;
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
