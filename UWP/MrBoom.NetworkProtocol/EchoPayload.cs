// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol
{
    internal class EchoPayload
    {
        public readonly long TimeStamp;

        public EchoPayload(long timeStamp)
        {
            this.TimeStamp = timeStamp;
        }

        public static EchoPayload Decode(ReadOnlyByteSpan src)
        {
            using (MemoryStream ms = src.AsStream())
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    return new EchoPayload(reader.ReadInt64());
                }
            }
        }

        public ReadOnlyByteSpan Encode()
        {
            using (MemoryStream ms = new MemoryStream(8))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(TimeStamp);
                }

                return new ReadOnlyByteSpan(ms.ToArray());
            }
        }
    }
}
