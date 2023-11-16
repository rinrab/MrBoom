// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;

namespace MrBoom
{
    public class ReadOnlyByteSpan
    {
        byte[] buffer;
        int start;
        int length;

        public int Length
        {
            get
            {
                return length;
            }
        }

        public byte this[int idx]
        {
            get
            {
                return buffer[start + idx];
            }
        }

        public ReadOnlyByteSpan(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        {
        }

        public ReadOnlyByteSpan(byte[] buffer, int start)
            : this(buffer, start, buffer.Length - start)
        {
        }

        public ReadOnlyByteSpan(byte[] buffer, int start, int length)
        {
            if (start + length > buffer.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.buffer = buffer;
            this.start = start;
            this.length = length;
        }

        public byte[] AsArray()
        {
            if (start == 0 && length == buffer.Length)
            {
                return buffer;
            }
            else
            {
                byte[] result = new byte[length];

                Array.Copy(buffer, start, result, 0, length);

                return result;
            }
        }

        public MemoryStream AsStream()
        {
            return new MemoryStream(buffer, start, length);
        }

        public ReadOnlyByteSpan Slice(int start, int length)
        {
            return new ReadOnlyByteSpan(buffer, this.start + start, length);
        }

        public ReadOnlyByteSpan Slice(int start)
        {
            return Slice(start, length - start);
        }

        public void WriteTo(Stream stream)
        {
            stream.Write(buffer, start, length);
        }
    }

    public static class ReadOnlyByteSpanExtensions
    {
        public static ReadOnlyByteSpan AsByteSpan(this byte[] bytes)
        {
            return new ReadOnlyByteSpan(bytes);
        }
    }
}
