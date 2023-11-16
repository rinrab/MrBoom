// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;

namespace MrBoom.NetworkProtocol
{
    internal class NetworkPacket
    {
        public readonly byte Type;
        public readonly ReadOnlyByteSpan Data;

        public NetworkPacket(byte type, ReadOnlyByteSpan data)
        {
            Type = type;
            Data = data;
        }

        public static NetworkPacket Decode(ReadOnlyByteSpan src)
        {
            if (src.Length >= 1)
            {
                return new NetworkPacket(src[0], src.Slice(1));
            }
            else
            {
                throw new Exception("Invalid network packet");
            }
        }

        public ReadOnlyByteSpan Encode()
        {
            using (MemoryStream ms = new MemoryStream(Data.Length + 1))
            {
                ms.WriteByte(Type);
                Data.WriteTo(ms);

                return new ReadOnlyByteSpan(ms.ToArray());
            }
        }
    }
}
