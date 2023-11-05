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

        private readonly Grid<int> grid;

        public TravelCostGrid(int width, int height)
        {
            grid = new Grid<int>(width, height, CostCantGo);
        }

        public void Update(int startX, int startY, GetTravelCostDelegate getCost)
        {
            BitArray visited = new BitArray(grid.Size);

            grid.Reset(CostCantGo);

            Queue<int> queue = new Queue<int>();

            grid[startX, startY] = 0;
            queue.Enqueue(grid.GetCellIndex(startX, startY));

            void visitCell(int x, int y, int addX, int addY)
            {
                int nextX = x + addX;
                int nextY = y + addY;
                if (nextX >= 0 && nextX < grid.Width && nextY >= 0 && nextY < grid.Height)
                {
                    int nextCell = grid.GetCellIndex(nextX, nextY);
                    if (!visited[nextCell])
                    {
                        int cost = grid[x, y];
                        int nextCost = cost + getCost(nextX, nextY);
                        if (nextCost < grid[nextCell])
                        {
                            grid[nextCell] = nextCost;
                            queue.Enqueue(nextCell);
                        }
                    }
                }
            }

            while (true)
            {
                int cellIndex;
                if (!queue.TryDequeue(out cellIndex))
                {
                    break;
                }

                if (!visited[cellIndex])
                {
                    visited[cellIndex] = true;

                    int x = grid.GetCellX(cellIndex);
                    int y = grid.GetCellY(cellIndex);

                    visitCell(x, y, 0, 1);
                    visitCell(x, y, 0, -1);
                    visitCell(x, y, 1, 0);
                    visitCell(x, y, -1, 0);
                }
            }
        }

        public int GetCost(int x, int y)
        {
            return grid[x, y];
        }

        public bool CanWalk(int x, int y)
        {
            return grid[x, y] < CostCantGo;
        }

        public Directions? GetBestDirection(int x, int y, Directions[] directions)
        {
            if (GetCost(x, y) == 0)
            {
                return null;
            }

            Directions? bestDir = null;
            int bestCost = CostCantGo;

            foreach (Directions dir in directions)
            {
                int c = GetCost(x + dir.DeltaX(), y + dir.DeltaY());

                if (c < bestCost)
                {
                    bestDir = dir;
                    bestCost = c;
                }
            }

            return bestDir;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    result.AppendFormat("{0,5}", grid[x, y]);
                }
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
