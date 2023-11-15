// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    // A decorator node, like a composite node, can have a child node.
    // Unlike a composite node, they can specifically only have a single child.
    public abstract class BtDecorator : BtNode
    {
        protected readonly BtNode child;

        public BtDecorator(BtNode child, string name = "BtDecorator") : base(name)
        {
            this.child = child;
        }
    }
}
