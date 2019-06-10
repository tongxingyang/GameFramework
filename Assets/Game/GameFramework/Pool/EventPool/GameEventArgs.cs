using System;
using GameFramework.Base;
using GameFramework.Pool.ReferencePool;

namespace GameFramework.Pool.EventPool
{
    public abstract class GameEventArgs : EventArgs, IReference
    {
        public abstract enEventID EventID { get; }
        public abstract void Reset();
    }
}