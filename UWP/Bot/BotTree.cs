// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.BehaviorTree;

namespace MrBoom.Bot
{
    public class BotTree
    {
        class ActionNode : BtNode
        {
            public delegate BtStatus ActionlDelegate();

            readonly ActionlDelegate action;

            public ActionNode(ActionlDelegate action)
            {
                this.action = action;
            }

            protected override BtStatus Update()
            {
                return action();
            }
        }

        public BotTree()
        {
            var root = new BtSelector()
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
        }

        private BtStatus GotoBestBombCell()
        {
            return Goto(GetBestBombCell());
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
