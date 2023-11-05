// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    public abstract class BtNode
    {
        protected BtStatus status;
        protected readonly string name;

        public BtNode(string name = "BtNode")
        {
            status = BtStatus.Invalid;
            this.name = name;
        }

        protected virtual void OnInitialize()
        {
        }

        protected abstract BtStatus OnUpdate();

        protected virtual void OnTerminate(BtStatus s)
        {
        }

        public BtStatus Update()
        {
            if (status != BtStatus.Running)
            {
                OnInitialize();
            }

            status = OnUpdate();

            if (status != BtStatus.Running)
            {
                OnTerminate(status);
            }

            return status;
        }

        public bool IsSuccess
        {
            get { return status == BtStatus.Success; }
        }

        public bool IsFailure
        {
            get { return status == BtStatus.Failure; }
        }

        public bool IsRunning
        {
            get { return status == BtStatus.Running; }
        }

        public bool IsTerminated
        {
            get { return IsSuccess || IsFailure; }
        }

        public void Reset()
        {
            status = BtStatus.Invalid;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
