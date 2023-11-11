// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom
{
    public static class NetworkParser
    {
        public struct PlayerData
        {
            public int X;
            public int Y;
            public Directions? Direction;
            public BombData[] Bombs;

            public static PlayerData Parse(Stream stream)
            {
                int x = ParseTwoPartInt(stream);
                int y = ParseTwoPartInt(stream);
                Directions? direction = ParseDirection(stream);

                int bombsCount = stream.ReadByte();
                BombData[] bombs = new BombData[bombsCount];
                for (int i = 0; i < bombsCount; i++)
                {
                    bombs[i] = BombData.Parse(stream);
                }

                return new PlayerData
                {
                    X = x,
                    Y = y,
                    Direction = direction,
                    Bombs = bombs
                };
            }
        }

        public struct BombData
        {
            public int X;
            public int Y;
            public int EstimateTime;
            public int MaxFire;

            public static BombData Parse(Stream stream)
            {
                return new BombData
                {
                    X = stream.ReadByte(),
                    Y = stream.ReadByte(),
                    EstimateTime = stream.ReadByte(),
                    MaxFire = stream.ReadByte()
                };
            }
        }

        public struct GameData
        {
            public PlayerData[] Players;

            public static GameData Parse(Stream stream)
            {
                stream.ReadByte(); // Skip type

                PlayerData players = PlayerData.Parse(stream);

                return new GameData
                {
                    Players = new PlayerData[] { players } // TODO:
                };
            }
        }

        public static int ParseTwoPartInt(Stream stream)
        {
            int part1 = stream.ReadByte();
            int part2 = stream.ReadByte();

            return part1 * 256 + part2;
        }

        public static Directions? ParseDirection(Stream stream)
        {
            int direction = stream.ReadByte();

            if (direction == 4)
            {
                return null;
            }
            else
            {
                return (Directions)direction;
            }
        }
    }
}
