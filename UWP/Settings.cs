// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Settings
    {
        public TeamMode TeamMode { get; set; }
    }

    public enum TeamMode
    {
        Off,
        Color,
        Sex
    }
}
