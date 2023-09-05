// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Diagnostics;
using MrBoom.BehaviorTree;

namespace MrBoom.Bot
{
    public class ComputerPlayer : AbstractPlayer
    {
        private BtSelector tree;
        private TravelCostGrid travelCost;
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
        }

        private BtStatus GotoBestBombCell()
        {
            return Goto(GetBestBombCell());
        }

        public override void Update()
        {
            base.Update();
            tree.Tick();

            int cellX = (x + 8) / 16;
            int cellY = (y + 8) / 16;

            travelCost.Update(cellX, cellY, (x, y) => terrain.IsWalkable(x, y) ? 1 : TravelCostGrid.CostCantGo);
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

        private BtStatus Goto(int v)
        {
            // TODO:
            return BtStatus.Failure;
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

        private int GetBestBombCell()
        {
            // TODO:
            return -1;
        }

        private int GetSafeCell()
        {
            // TODO:
            return -1;
        }

        private int GetBonusCell()
        {
            // TODO:
            return -1;
        }
    }
}
