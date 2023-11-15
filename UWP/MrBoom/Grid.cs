// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class Grid<T>
    {
        private readonly int width;
        private readonly int height;
        private readonly T defaultValue;
        private readonly T[] grid;

        public Grid(int width, int height, T defaultValue = default)
        {
            this.width = width;
            this.height = height;
            this.defaultValue = defaultValue;
            grid = new T[width * height];
        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int CellCount { get { return width * height; } }

        public T this[int x, int y]
        {
            get
            {
                if (x >= 0 && x < width &&
                    y >= 0 && y < height)
                {
                    return grid[GetCellIndex(x, y)];
                }
                else
                {
                    return defaultValue;
                }
            }

            set
            {
                if (x >= 0 && x < width &&
                    y >= 0 && y < height)
                {
                    grid[GetCellIndex(x, y)] = value;
                }
            }
        }

        public T this[int cellIndex]
        {
            get
            {
                return grid[cellIndex];
            }
            set
            {
                grid[cellIndex] = value;
            }
        }

        public int GetCellIndex(int x, int y)
        {
            return y * width + x;
        }

        public int GetCellX(int cellIndex)
        {
            return cellIndex % width;
        }

        public int GetCellY(int cellIndex)
        {
            return cellIndex / width;
        }

        public void Reset()
        {
            Reset(defaultValue);
        }

        public void Reset(T value)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = value;
            }
        }
    }
}
