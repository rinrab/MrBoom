// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    // The Sequence composite ticks each child node in order.
    // If a child fails or runs, the sequence returns the same status.
    // In the next tick, it will try to run each child in order again.
    // If all children succeeds, only then does the sequence succeed.
    public class BtSequence : BtComposite
    {
        public BtSequence(string name = "SequenceNode") : base(name)
        {
        }

        protected override void OnInitialize()
        {
            index = 0;
        }

        protected override BtStatus OnUpdate()
        {
            if (HasNoChildren())
            {
                return (BtStatus.Success);
            }

            // Keep going until a child behavior says it's running.
            while (true)
            {
                // Hit the end of the array, job done!
                if (index >= children.Count)
                {
                    return BtStatus.Success;
                }

                BtNode child = children[index];
                BtStatus status = child.Update();

                // If the child fails, or keeps running, do the same.
                if (status != BtStatus.Success)
                {
                    return status;
                }

                index++;
            }
        }
    }
}
