// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    class ActionNode : BtNode
    {
        public delegate BtStatus ActionlDelegate();

        readonly ActionlDelegate action;
        private readonly bool wait;
        private bool done;

        public ActionNode(ActionlDelegate action, string name = "ActionNode", bool wait = false) : base(name)
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
}
