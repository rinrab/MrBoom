// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    public class BtRepeater : BtNode
    {
        private readonly BtNode child;

        public BtRepeater(BtNode child, string name = "RepeaterNode") : base(name)
        {
            this.child = child;
        }

        protected override void OnInitialize()
        {
        }

        protected override BtStatus OnUpdate()
        {
            status = child.Update();

            return BtStatus.Running;
        }

        public override string ToString()
        {
            return name + "->" + child.ToString();
        }
    }
}
