// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.BehaviorTree
{
    public abstract class BtComposite : BtNode, IEnumerable<BtNode>
    {
        protected List<BtNode> children;
        protected int index;

        public BtComposite(string name = "CompositeNode") : base(name)
        {
            children = new List<BtNode>();
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(name);

            for (int i = 0; i < children.Count; i++)
            {
                StringBuilder nsb = new StringBuilder(children[i].ToString());

                for (int j = 0; j < nsb.Length; j++)
                {
                    if (nsb[j] == '\n')
                    {
                        nsb.Insert(j + 1, "  ");
                    }
                }

                sb.Append("\n" + ((i == index) ? "- " : "  ") + nsb);
            }

            return sb.ToString();
        }
    }
}
