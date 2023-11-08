// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;

namespace MrBoom
{
    public class NameGenerator
    {
        private readonly Random random;

        private readonly List<string> names = new List<string>
        {
            "gin", "jai", "jay", "lad", "dre", "ash", "zev", "buz", "nox", "oak",
            "coy", "eza", "fil", "kip", "aya", "jem", "roy", "rex", "ryu", "gus",
            "cpp", "sus", "god", "guy", "bob", "jim", "mrb", "max"
        };

        public NameGenerator(Random random)
        {
            this.random = random;
        }

        public string GenerateName()
        {
            int index = random.Next(names.Count);
            names.RemoveAt(index);
            return names[index];
        }
    }
}
