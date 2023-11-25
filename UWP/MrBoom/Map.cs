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

        public abstract class AbstractMonsterParameters
        {
            public int Speed;
            public int Type { get; }
            public int LivesCount { get; }
            public bool IsSlowStart;

            public AbstractMonsterParameters(int type, int livesCount)
            {
                Type = type;
                LivesCount = livesCount;
                Speed = 3;
            }

            public abstract AbstractMonster CreateMonsterSprite(Terrain map, Assets.MovingSpriteAssets animations, int x, int y);
        }

        public class BasicMonsterParameters : AbstractMonsterParameters
        {
            public int WaitAfterTurn { get; }

            public BasicMonsterParameters(int type, int waitAfterTurn, int livesCount) : base(type, livesCount)
            {
                WaitAfterTurn = waitAfterTurn;
            }

            public override AbstractMonster CreateMonsterSprite(Terrain map, Assets.MovingSpriteAssets animations, int x, int y)
            {
                return new BasicMonster(map, this, animations, x, y);
            }
        }

        public string[] Data;
        public int Time;
        public PowerUpData[] PowerUps;
        public AbstractMonsterParameters[] Monsters;
        public byte[] Final;
        public int StartMaxFire = 1;
        public int StartMaxBombsCount = 1;
        public Feature StartFeatures = 0;
        public int Song = -1;
        public bool IsBombApocalypse = false;
    }
}
