// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    public class DelayNode : BtNode
    {
        private readonly int delay;
        private int ticks;

        public DelayNode(int delay, string name = "DelayNode") : base(name)
        {
            this.delay = delay;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ticks = 0;
        }

        protected override BtStatus OnUpdate()
        {
            if (ticks > delay)
            {
                return BtStatus.Success;
            }
            else
            {
                ticks++;
                return BtStatus.Running;
            }
        }

        public override string ToString()
        {
            if (status == BtStatus.Running)
            {
                return name + " " + (delay - ticks + 1);
            }
            else
            {
                return name + " " + delay;
            }
        }
    }
}
