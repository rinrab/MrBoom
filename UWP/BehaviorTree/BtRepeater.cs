// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    public class BtRepeater : BtDecorator
    {
        public BtRepeater(BtNode child, string name = "RepeaterNode") : base(child, name)
        {
        }

        protected override void OnInitialize()
        {
        }

        protected override BtStatus OnUpdate()
        {
           child.Update();

            return BtStatus.Running;
        }

        public override string ToString()
        {
            return name + "->" + child.ToString();
        }
    }
}
