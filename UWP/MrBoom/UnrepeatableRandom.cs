// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    public class UnrepeatableRandom
    {
        private readonly Random random;

        private int? last = null;

        public UnrepeatableRandom()
        {
            random = new Random();
        }

        public int Next(int max)
        {
            if (max <= 1)
            {
                return 0;
            }
            else
            {
                while (true)
                {
                    int val = random.Next(max);
                    if (val != last)
                    {
                        last = val;
                        return val;
                    }
                }
            }
        }
    }
}
