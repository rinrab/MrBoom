// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.BehaviorTree
{
    public abstract class BtNode
    {
        protected BtStatus status;

        public BtNode()
        {
            status = BtStatus.Invalid;
        }

        protected virtual void Initialize()
        {
        }

        protected abstract BtStatus Update();

        protected virtual void Terminate(BtStatus s)
        {
        }

        public BtStatus Tick()
        {
            if (status != BtStatus.Running)
            {
                Initialize();
            }

            status = Update();

            if (status != BtStatus.Running)
            {
                Terminate(status);
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
    }
}
