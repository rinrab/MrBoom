// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Diagnostics;
using MrBoom.BehaviorTree;

namespace MrBoom.Bot
{
    public class ComputerPlayer : AbstractPlayer
    {
        private BtSelector tree;
        private TravelCostGrid travelCost;
        private TravelCostGrid findPathCost;
        private int tickCount;

        class ActionNode : BtNode
        {
            public delegate BtStatus ActionlDelegate();

            readonly ActionlDelegate action;

            public ActionNode(ActionlDelegate action)
            {
                this.action = action;
            }

            protected override BtStatus OnUpdate()
            {
                return action();
            }
        }

        public ComputerPlayer(Terrain map, Assets.MovingSpriteAssets animations, int maxBoom, int maxBombs, int team)
            : base(map, animations, maxBoom, maxBombs, team)
        {
            tree = new BtSelector()
                {
                    new ActionNode(GotoBonusCell),
                    new BtSequence()
                    {
                        new ActionNode(HasBombsLeft),
                        new ActionNode(GotoBestBombCell),
                        new ActionNode(DropBomb)
                    },
                    new ActionNode(GotoSafeCell)
                };
            travelCost = new TravelCostGrid(map.Width, map.Height);
            findPathCost = new TravelCostGrid(map.Width, map.Height);
        }

        private BtStatus GotoBestBombCell()
        {
            return Goto(GetBestBombCell());
        }

        public override void Update()
        {
            base.Update();

            int cellX = (x + 8) / 16;
            int cellY = (y + 8) / 16;

            travelCost.Update(cellX, cellY, (x, y) => terrain.IsWalkable(x, y) ? 1 : TravelCostGrid.CostCantGo);

            Direction = Directions.None;
            tree.Update();

            tickCount++;

            if (tickCount % (60 * 5) == 0)
            {
                Debug.WriteLine("Travel cost:");
                Debug.Write(travelCost.ToString());
            }
        }

        private BtStatus GotoSafeCell()
        {
            return Goto(GetSafeCell());
        }

        private BtStatus Goto(CellCoord? target)
        {
            if (target.HasValue)
            {
                int cellX = (x + 8) / 16;
                int cellY = (y + 8) / 16;

                if (target.Value.X == cellX &&
                    target.Value.Y == cellY)
                {
                    return BtStatus.Success;
                }
                else
                {
                    findPathCost.Update(target.Value.X, target.Value.Y, (x, y) => terrain.IsWalkable(x, y) ? 1 : TravelCostGrid.CostCantGo);

                    Direction = findPathCost.GetBestDirection(cellX, cellY);
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
                return BtStatus.Failure;
            }
        }

        private BtStatus GotoBonusCell()
        {
            return Goto(GetBonusCell());
        }

        private BtStatus DropBomb()
        {
            return BtStatus.Success;
        }

        private BtStatus HasBombsLeft()
        {
            return BtStatus.Success;
        }

        private CellCoord? GetBestBombCell()
        {
            // TODO:
            return null;
        }

        private CellCoord? GetSafeCell()
        {
            // TODO:
            return new CellCoord(1, 1);
        }

        private CellCoord? GetBonusCell()
        {
            // TODO:
            return null;
        }
    }
}
