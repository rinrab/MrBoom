// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    // The Selector composite ticks each child node in order.
    // If a child succeeds or runs, the sequence returns the same status.
    // In the next tick, it will try to run each child in order again.
    // If all children fails, only then does the selector fail.
    public class BtSelector : BtComposite
    {
        public BtSelector(string name = "SelectorNode") : base(name)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            index = 0;
        }

        protected override BtStatus OnUpdate()
        {
            if (HasNoChildren())
            {
                return BtStatus.Success;
            }

            // Keep going until a child behavior says it's running.
            while (true)
            {
                // Hit the end of the array, it didn't end well...
                if (index >= children.Count)
                {
                    return BtStatus.Failure;
                }

                BtNode child = children[index];
                BtStatus status = child.Update();

                // If the child succeeds, or keeps running, do the same.
                if (status != BtStatus.Failure)
                {
                    return status;
                }

                index++;
            }
        }
    }
}
