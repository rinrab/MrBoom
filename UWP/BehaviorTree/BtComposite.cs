// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace MrBoom.BehaviorTree
{
    public abstract class BtComposite : BtNode, IEnumerable<BtNode>
    {
        protected List<BtNode> children;
        protected int index;

        public BtComposite()
        {
            index = 0;
        }

        public void Add(BtNode child)
        {
            children.Add(child);
        }

        public bool HasNoChildren()
        {
            return children.Count == 0;
        }

        public int GetIndex()
        {
            return (index);
        }

        public IEnumerator<BtNode> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return children.GetEnumerator();
        }
    }
}
