// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public enum Directions
    {
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
                default:
                    return 0;
            }
        }

        public static int DeltaX(this Directions? direction)
        {
            if (direction.HasValue)
            {
                return direction.Value.DeltaX();
            }
            else
            {
                return 0;
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
                default:
                    return 0;
            }
        }

        public static int DeltaY(this Directions? direction)
        {
            if (direction.HasValue)
            {
                return direction.Value.DeltaY();
            }
            else
            {
                return 0;
            }
        }

        public static Directions Reverse(this Directions direction)
        {
            switch (direction)
            {
                case Directions.Left: return Directions.Right;
                case Directions.Right: return Directions.Left;
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                default: return direction;
            }
        }

        public static Directions? Reverse(this Directions? direction)
        {
            if (direction.HasValue)
            {
                return direction.Value.Reverse();
            }
            else
            {
                return null;
            }
        }
    }
}
