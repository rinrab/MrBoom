// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.Bot
{
    public class TravelCostGrid
    {
        public delegate int GetTravelCostDelegate(int nextX, int nextY);
        public const int CostCantGo = 9999;

        private readonly int width;
        private readonly int height;
        private int[] grid;

        public TravelCostGrid(int width, int height)
        {
            grid = new int[width * height];
            this.width = width;
            this.height = height;
        }

        public void Update(int startX, int startY, GetTravelCostDelegate getCost)
        {
            BitArray visited = new BitArray(grid.Length);

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = CostCantGo;
            }

            Queue<int> queue = new Queue<int>();

            grid[cellIndex(startX, startY)] = 0;
            queue.Enqueue(cellIndex(startX, startY));

            void visitCell(int x, int y, int addX, int addY)
            {
                int nextX = x + addX;
                int nextY = y + addY;
                if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
                {
                    int nextCell = cellIndex(nextX, nextY);
                    if (!visited[nextCell])
                    {
                        int cost = grid[cellIndex(x, y)];
                        int nextCost = cost + getCost(nextX, nextY);
                        if (nextCost < grid[nextCell])
                        {
                            grid[nextCell] = nextCost;
                            queue.Enqueue(nextCell);
                        }
                    }
                }
            }

            while(true)
            {
                int cellIndex;
                if (!queue.TryDequeue(out cellIndex))
                {
                    break;
                }

                if (!visited[cellIndex])
                {
                    visited[cellIndex] = true;

                    int x = cellX(cellIndex);
                    int y = cellY(cellIndex);

                    visitCell(x, y, 0, 1);
                    visitCell(x, y, 0, -1);
                    visitCell(x, y, 1, 0);
                    visitCell(x, y, -1, 0);
                }
            }
        }

        public int GetCost(int x, int y)
        {
            if (x >= 0 && x < width &&
                y >= 0 && y < height)
            {
                return grid[cellIndex(x, y)];
            }
            else
            {
                return CostCantGo;
            }
        }

        public Directions GetBestDirection(int x, int y)
        {
            if (GetCost(x, y) == 0)
            {
                return Directions.None;
            }

            Directions d = Directions.None;
            int minCost = CostCantGo;

            int c;
            c = GetCost(x - 1, y);
            if (c < minCost)
            {
                d = Directions.Left;
                minCost = c;
            }
            c = GetCost(x + 1, y);
            if (c < minCost)
            {
                d = Directions.Right;
                minCost = c;
            }
            c = GetCost(x, y - 1);
            if (c < minCost)
            {
                d = Directions.Up;
                minCost = c;
            }
            c = GetCost(x, y + 1);
            if (c < minCost)
            {
                d = Directions.Down;
                minCost = c;
            }

            return d;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result.AppendFormat("{0,5}", grid[cellIndex(x, y)]);
                }
                result.AppendLine();
            }
            return result.ToString();
        }

        private int cellIndex(int x, int y) { return y * width + x; }
        private int cellX(int cellIndex) { return cellIndex % width; }
        private int cellY(int cellIndex) { return cellIndex / width; }
    }
}
