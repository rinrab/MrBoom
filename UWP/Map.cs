// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Map
    {
        public class PowerUpData
        {
            public PowerUpType Type;
            public int Count;

            public PowerUpData(PowerUpType type, int count)
            {
                Type = type;
                Count = count;
            }
        }

        public class MonsterData
        {
            public int Speed;
            public int WaitAfterTurn { get; }
            public int Type { get; }
            public int LivesCount { get; }
            public bool IsSlowStart;

            public MonsterData(int type, int waitAfterTurn, int livesCount)
            {
                WaitAfterTurn = waitAfterTurn;
                Type = type;
                LivesCount = livesCount;

                Speed = 3;
            }
        }

        public string[] Data;
        public int Time;
        public PowerUpData[] PowerUps;
        public MonsterData[] Monsters;
        public byte[] Final;
        public int StartMaxFire = 1;
        public int StartMaxBombsCount = 1;
        public Feature StartFeatures = 0;
        public int Song = -1;
        public bool BombApocalypse = false;
    }
}
