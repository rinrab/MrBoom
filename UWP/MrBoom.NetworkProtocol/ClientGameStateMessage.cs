// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;
using System;

namespace MrBoom.NetworkProtocol
{
    public static class ClientGameStateMessage
    {
        public struct PlayerData
        {
            public int X;
            public int Y;
            public byte Direction;
            public BombData[] Bombs;

            public static PlayerData Parse(BinaryReader reader)
            {
                int x = reader.ReadInt16();
                int y = reader.ReadInt16();
                byte direction = reader.ReadByte();

                int bombsCount = reader.ReadByte();
                BombData[] bombs = new BombData[bombsCount];
                for (int i = 0; i < bombsCount; i++)
                {
                    bombs[i] = BombData.Parse(reader);
                }

                return new PlayerData
                {
                    X = x,
                    Y = y,
                    Direction = direction,
                    Bombs = bombs
                };
            }

            public void WriteTo(BinaryWriter writer)
            {
                writer.Write((Int16)X);
                writer.Write((Int16)Y);
                writer.Write((byte)Direction);
                writer.Write((byte)Bombs.Length);

                foreach (var bombData in Bombs)
                {
                    bombData.WriteTo(writer);
                }
            }
        }

        public struct BombData
        {
            public int X;
            public int Y;
            public int EstimateTime;
            public int MaxFire;

            public static BombData Parse(BinaryReader reader)
            {
                return new BombData
                {
                    X = reader.ReadByte(),
                    Y = reader.ReadByte(),
                    EstimateTime = reader.ReadByte(),
                    MaxFire = reader.ReadByte()
                };
            }

            public void WriteTo(BinaryWriter writer)
            {
                writer.Write((byte)X);
                writer.Write((byte)Y);
                writer.Write((byte)EstimateTime);
                writer.Write((byte)MaxFire);
            }
        }

        public class GameData
        {
            public PlayerData[] Players;

            public static GameData Decode(ReadOnlyByteSpan span)
            {
                using (MemoryStream stream = span.AsStream())
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        return Parse(reader);
                    }
                }
            }

            public static GameData Parse(BinaryReader reader)
            {
                reader.ReadByte(); // Skip type

                PlayerData player = PlayerData.Parse(reader);

                return new GameData
                {
                    Players = new PlayerData[] { player } // TODO:
                };
            }

            public ReadOnlyByteSpan Encode()
            {
                // TODO: Add size.
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        writer.Write(GameMessageType.ClientGameState);
                        foreach (PlayerData player in Players)
                        {
                            player.WriteTo(writer);
                        }
                    }

                    return new ReadOnlyByteSpan(ms.ToArray());
                }
            }
        }
    }
}
