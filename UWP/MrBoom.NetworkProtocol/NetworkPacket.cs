// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;
using System.Text;

namespace MrBoom.NetworkProtocol
{
    internal class NetworkPacket
    {
        public readonly byte Type;
        public readonly UInt32 NetworkId;
        public readonly ReadOnlyByteSpan Data;

        public NetworkPacket(byte type, UInt32 networkId, ReadOnlyByteSpan data)
        {
            Type = type;
            NetworkId = networkId;
            Data = data;
        }

        public static NetworkPacket Decode(ReadOnlyByteSpan src)
        {
            using (MemoryStream ms = src.AsStream())
            {
                byte type;
                UInt32 networkId;

                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8, true))
                {
                    type = reader.ReadByte();
                    networkId = reader.ReadUInt32();
                }

                ReadOnlyByteSpan data = src.Slice((int)ms.Position);

                return new NetworkPacket(type, networkId, data);
            }
        }

        public ReadOnlyByteSpan Encode()
        {
            using (MemoryStream ms = new MemoryStream(Data.Length + 1 + 4))
            {
                using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    writer.Write(Type);
                    writer.Write(NetworkId);
                }

                Data.WriteTo(ms);
                return new ReadOnlyByteSpan(ms.ToArray());
            }
        }
    }
}
