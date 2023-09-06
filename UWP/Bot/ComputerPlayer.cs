// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MrBoom.BehaviorTree;

namespace MrBoom.Bot
{
    public class ComputerPlayer : AbstractPlayer
    {
        private BtSelector tree;
        private TravelCostGrid travelCostGrid;
        private readonly TravelCostGrid travelSafeCostGrid;
        private TravelCostGrid findPathCost;
        private Grid<int> bestExplosionGrid;
        private Grid<bool> dangerGrid;
        private int tickCount;

        class ActionNode : BtNode
        {
            public delegate BtStatus ActionlDelegate();

            readonly ActionlDelegate action;
            private readonly bool wait;
            private bool done;

            public ActionNode(ActionlDelegate action, bool wait = false)
            {
                this.action = action;
                this.wait = wait;
            }

            protected override void OnInitialize()
            {
                base.OnInitialize();
                done = false;
            }

            protected override BtStatus OnUpdate()
            {
                BtStatus status = action();
                if (status == BtStatus.Success && wait && !done)
                {
                    done = true;
                    return BtStatus.Running;
                }

                return status;
            }
        }

        public ComputerPlayer(Terrain map, Assets.MovingSpriteAssets animations, int x, int y, int maxBoom, int maxBombs, int team)
            : base(map, animations, x, y, maxBoom, maxBombs, team)
        {
            tree = new BtSelector()
                {
                    new ActionNode(GotoBonusCell),
                    new BtSequence()
                    {
                        new ActionNode(HasBombsLeft),
                        new ActionNode(GotoBestBombCell),
                        new ActionNode(DropBomb, true)
                    },
                    new BtSequence()
                    {
                        new ActionNode(GotoSafeCell),
                        new ActionNode(DitonoteRemoteBomb)
                    }
                };
            travelCostGrid = new TravelCostGrid(map.Width, map.Height);
            travelSafeCostGrid = new TravelCostGrid(map.Width, map.Height);
            findPathCost = new TravelCostGrid(map.Width, map.Height);
            bestExplosionGrid = new Grid<int>(map.Width, map.Height);
            dangerGrid = new Grid<bool>(map.Width, map.Height, false);
        }

        private BtStatus DitonoteRemoteBomb()
        {
            if (Features.HasFlag(Feature.RemoteControl))
            {
                rcDitonateButton = true;
                return BtStatus.Success;
            }
            else
            {
                return BtStatus.Failure;
            }
        }

        private BtStatus GotoBestBombCell()
        {
            return Goto(GetBestBombCell());
        }

        public override void Update()
        {
            int cellX = (X + 8) / 16;
            int cellY = (Y + 8) / 16;

            dangerGrid.Reset();
            for (int i = 0; i < dangerGrid.Width; i++)
            {
                for (int j = 0; j < dangerGrid.Height; j++)
                {
                    Cell cell = terrain.GetCell(i, j);
                    if (cell.Type == TerrainType.Bomb)
                    {
                        foreach (Directions dir in new Directions[] { Directions.Left, Directions.Up, Directions.Right, Directions.Down })
                        {
                            for (int k = 0; k <= cell.maxBoom; k++)
                            {
                                dangerGrid[i + dir.DeltaX() * k, j + dir.DeltaY() * k] = true;
                            }
                        }
                    }
                    else if (terrain.GetCell(i, j).Type == TerrainType.Fire)
                    {
                        dangerGrid[i, j] = true;
                    }
                }
            }

            travelCostGrid.Update(cellX, cellY, CalcTravelCost);
            travelSafeCostGrid.Update(cellX, cellY, CalcSafeTravelCost);

            bestExplosionGrid.Reset();
            for (int i = 0; i < bestExplosionGrid.Width; i++)
            {
                for(int j = 0; j < bestExplosionGrid.Height; j++)
                {
                    int score = 0;
                    if (travelCostGrid.CanWalk(i,j))
                    {
                        score++;
                        Grid<bool> flameGrid = new Grid<bool>(bestExplosionGrid.Width, bestExplosionGrid.Height);

                        // Simple naive flame simulation.
                        // TODO: Improve it.
                        foreach (Directions dir in new Directions[] { Directions.Left, Directions.Up, Directions.Right, Directions.Down })
                        {
                            for (int k = 0; k <= MaxBoom; k++)
                            {
                                if (!flameGrid[i + dir.DeltaX() * k, j + dir.DeltaY() * k])
                                {
                                    Cell cell = terrain.GetCell(i + dir.DeltaX() * k, j + dir.DeltaY() * k);

                                    switch (cell.Type)
                                    {
                                        case TerrainType.TemporaryWall:
                                            score+= 2;
                                            break;
                                        case TerrainType.Bomb:
                                            score+= 1;
                                            break;
                                    }

                                    if (terrain.IsTouchingMonster(i + dir.DeltaX() * k, j + dir.DeltaY() * k))
                                    {
                                        score+= 6;
                                    }

                                    if (cell.Type != TerrainType.Free)
                                    {
                                        break;
                                    }

                                    flameGrid[i + dir.DeltaX() * k, j + dir.DeltaY() * k] = true;
                                }
                            }
                        }

                        // find safe place
                        bool foundSafePlace = false;

                        for (int xx = 0; xx < bestExplosionGrid.Width; xx++)
                        {
                            for (int yy = 0; yy < bestExplosionGrid.Height; yy++)
                            {
                                if (travelCostGrid.CanWalk(xx, yy) && !flameGrid[xx, yy])
                                {
                                    foundSafePlace = true;
                                }
                            }
                        }

                        if (!foundSafePlace)
                        {
                            score = -score;
                        }
                    }

                    bestExplosionGrid[i,j] = score;
                }
            }

            Direction = Directions.None;
            dropBombButton = false;
            rcDitonateButton = false;
            tree.Update();

            tickCount++;

            if (tickCount % (60 * 5) == 0)
            {
                Debug.WriteLine("Travel cost:");
                Debug.Write(travelCostGrid.ToString());
            }

            base.Update();
        }

        private int CalcTravelCost(int x, int y)
        {
            if (!terrain.IsWalkable(x, y))
            {
                return TravelCostGrid.CostCantGo;
            }

            if (terrain.GetCell(x, y).Type == TerrainType.Fire)
            {
                return TravelCostGrid.CostCantGo;
            }

            if (terrain.IsTouchingMonster(x, y) || terrain.IsMonsterComing(x, y))
            {
                return TravelCostGrid.CostCantGo;
            }

            return 1;
        }

        private int CalcSafeTravelCost(int x, int y)
        {
            if (dangerGrid[x, y])
                return TravelCostGrid.CostCantGo;

            return CalcTravelCost(x, y);
        }

        private BtStatus GotoSafeCell()
        {
            return Goto(GetSafeCell());
        }

        private Directions CalcPathDirection(CellCoord target)
        {
            findPathCost.Update(target.X, target.Y,
                (x, y) => (x == CellX && y == CellY) ? 1 : CalcSafeTravelCost(x, y));

            var result = findPathCost.GetBestDirection(CellX, CellY);
            if (result == Directions.None)
            {
                findPathCost.Update(target.X, target.Y,
                    (x, y) => (x == CellX && y == CellY) ? 1 : CalcTravelCost(x, y));

                result = findPathCost.GetBestDirection(CellX, CellY);
            }

            return result;
        }

        private BtStatus Goto(CellCoord? target)
        {
            if (target.HasValue)
            {
                int cellX = (X + 8) / 16;
                int cellY = (Y + 8) / 16;

                if (target.Value.X == cellX &&
                    target.Value.Y == cellY)
                {
                    const int MAX_PIXELS_PER_FRAME = 8;

                    int targetX = target.Value.X * 16;
                    int targetY = target.Value.Y * 16;

                    if (Math.Abs(targetX - X) < MAX_PIXELS_PER_FRAME / 2 && Math.Abs(targetY - Y) < MAX_PIXELS_PER_FRAME / 2)
                    {
                        Direction = Directions.None;
                        return BtStatus.Success;
                    }

                    if (X > targetX)
                    {
                        Direction = Directions.Left;
                        return BtStatus.Running;
                    }
                    else if (X < targetX)
                    {
                        Direction = Directions.Right;
                        return BtStatus.Running;
                    }
                    else if (Y > targetY)
                    {
                        Direction = Directions.Up;
                        return BtStatus.Running;
                    }
                    else if (Y < targetY)
                    {
                        Direction = Directions.Down;
                        return BtStatus.Running;
                    }
                    else
                    {
                        Direction = Directions.None;
                        return BtStatus.Success;
                    }
                }
                else
                {
                    Direction = CalcPathDirection(target.Value);
                    if (Direction == Directions.None)
                    {
                        return BtStatus.Failure;
                    }
                    else
                    {
                        return BtStatus.Running;
                    }
                }
            }
            else
            {
                // TODO:
                Direction = Directions.None;

                return BtStatus.Failure;
            }
        }

        private BtStatus GotoBonusCell()
        {
            return Goto(GetBonusCell());
        }

        private BtStatus DropBomb()
        {
            dropBombButton = true;
            return BtStatus.Success;
        }

        private BtStatus HasBombsLeft()
        {
            if (BombsRemaining > 0)
            {
                return BtStatus.Success;
            }
            else
            {
                return BtStatus.Failure;
            }
        }

        private CellCoord? GetBestBombCell()
        {
            CellCoord? bestCell = null;
            int bestScore = 0;

            for (int x = 0; x < bestExplosionGrid.Width; x++)
            {
                for (int y = 0; y < bestExplosionGrid.Height; y++)
                {
                    if (terrain.GetCell(x, y).Type != TerrainType.Bomb)
                    {
                        int score = bestExplosionGrid[x, y] * 128;
                        if (score < 0)
                            score = 0;
                        int travelCost = 1 + travelCostGrid.GetCost(x, y) / 2;
                        if (score > travelCost)
                        {
                            score = score / travelCost;
                        }

                        if (score > bestScore)
                        {
                            bestCell = new CellCoord(x, y);
                            bestScore = score;
                        }
                    }
                }
            }

            return bestCell;
        }

        private CellCoord? GetSafeCell()
        {
            CellCoord? bestCell = null;
            int bestScore = int.MinValue;

            for (int x = 0; x < dangerGrid.Width; x++)
            {
                for (int y = 0; y < dangerGrid.Height; y++)
                {
                    if (!dangerGrid[x, y] && travelCostGrid.CanWalk(x, y))
                    {
                        int score = -travelCostGrid.GetCost(x, y);
                        if (score > bestScore)
                        {
                            bestCell = new CellCoord(x, y);
                            bestScore = score;
                        }
                    }
                }
            }

            return bestCell;
        }

        private bool IsInterestingBonus(PowerUpType bonusType)
        {
            switch (bonusType)
            {
                case PowerUpType.Banana:
                    return true;
                case PowerUpType.ExtraBomb:
                    return true;
                case PowerUpType.ExtraFire:
                    return true;
                case PowerUpType.Skull:
                    return false;
                case PowerUpType.Shield:
                    return true;
                case PowerUpType.Life:
                    return true;
                case PowerUpType.RemoteControl:
                    return !Features.HasFlag(Feature.RemoteControl);
                case PowerUpType.Kick:
                    return !Features.HasFlag(Feature.Kick);
                case PowerUpType.RollerSkate:
                    return !Features.HasFlag(Feature.RollerSkates);
                case PowerUpType.Clock:
                    return false;
                case PowerUpType.MultiBomb:
                    return !Features.HasFlag(Feature.MultiBomb);
                default:
                    return false;
            }
        }

        private int CalcBonusScore(PowerUpType bonusType, int distance)
        {
            if (distance == TravelCostGrid.CostCantGo)
                return 0;

            if (!IsInterestingBonus(bonusType))
                return 0;

            switch(bonusType)
            {
                case PowerUpType.Kick:
                case PowerUpType.RemoteControl:
                case PowerUpType.Shield:
                    distance /= 4;
                    break;

                case PowerUpType.Life:
                case PowerUpType.RollerSkate:
                    distance /= 8;
                    break;
            }

            return TravelCostGrid.CostCantGo - distance;
        }

        private CellCoord? GetBonusCell()
        {
            CellCoord? bestCell = null;
            int bestScore = 0;

            for (int x = 0; x < terrain.Width; x++)
            {
                for (int y = 0; y < terrain.Height; y++)
                {
                    Cell cell = terrain.GetCell(x, y);
                    if (cell.Type == TerrainType.PowerUp)
                    {
                        int distance = travelSafeCostGrid.GetCost(x, y);
                        int score = CalcBonusScore(cell.PowerUpType, distance);
                        if (score > bestScore)
                        {
                            bestCell = new CellCoord(x, y);
                            bestScore = score;
                        }
                    }
                }
            }

            return bestCell;
        }
    }
}
