// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public enum Directions
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

    public static class DirectionsExtensions
    {
        public static int DeltaX(this Directions direction)
        {
            switch (direction)
            {
                case Directions.Left:
                    return -1;
                case Directions.Right:
                    return 1;
                default: return 0;
            }
        }

        public static int DeltaY(this Directions direction)
        {
            switch (direction)
            {
                case Directions.Up:
                    return -1;
                case Directions.Down:
                    return 1;
                default: return 0;
            }
        }
    }
}
