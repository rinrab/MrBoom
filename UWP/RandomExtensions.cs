// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;

namespace MrBoom
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this Random random, IList<T> items)
        {
            int count = items.Count;

            for (int i = 0; i < count - 1; i++)
            {
                int j = random.Next(i, count);

                if (j != i)
                {
                    T temp = items[i];
                    items[i] = items[j];
                    items[j] = temp;
                }
            }
        }
    }
}
