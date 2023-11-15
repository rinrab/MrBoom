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
                    (items[j], items[i]) = (items[i], items[j]);
                }
            }
        }

        public static T NextEnum<T>(this Random random) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));

            return (T)values.GetValue(random.Next(values.Length));
        }

        public static T NextElement<T>(this Random random, IList<T> items)
        {
            int index = random.Next(items.Count);

            return items[index];
        }
    }
}
