using System;
using System.Collections.Generic;
using GameFramework.Debug;

namespace GameFramework.Pool.ReferencePool
{
    public sealed class ReferenceCollection 
    {
        private readonly Queue<IReference> references;
        private int usingReferenceCount;
        private int acquireReferenceCount;
        private int releaseReferenceCount;
        private int addReferenceCount;
        private int removeReferenceCount;
        
        public int UnusedReferenceCount
        {
            get
            {
                return references.Count;
            }
        }

        public int UsingReferenceCount => usingReferenceCount;

        public int AcquireReferenceCount => acquireReferenceCount;

        public int ReleaseReferenceCount => releaseReferenceCount;
       
        public int AddReferenceCount => addReferenceCount;

        public int RemoveReferenceCount => removeReferenceCount;

        public ReferenceCollection()
        {
            references = new Queue<IReference>();
            usingReferenceCount = 0;
            acquireReferenceCount = 0;
            releaseReferenceCount = 0;
            addReferenceCount = 0;
            removeReferenceCount = 0;
        }

        public T Acquire<T>() where T:class,IReference,new()
        {
            usingReferenceCount++;
            acquireReferenceCount++;
            lock (references)
            {
                if (references.Count > 0)
                {
                    return (T)references.Dequeue();
                }
            }

            addReferenceCount++;
            return new T();
        }

        public void Release(IReference reference )
        {
            reference.Reset();
            lock (references)
            {
                if (references.Contains(reference))
                {
                    Debuger.LogError("The reference has been released.");
                }
                references.Enqueue(reference);
            }

            releaseReferenceCount++;
            usingReferenceCount--;
        }
        
        public void Add<T>(int count) where T:class,IReference,new()
        {
            lock (references)
            {
                addReferenceCount += count;
                while (count-- > 0)
                {
                    references.Enqueue(new T());
                }
            }
        }
        
        public void Remove(int count)
        {
            lock (references)
            {
                if (count > references.Count)
                {
                    count = references.Count;
                }

                removeReferenceCount += count;
                while (count-- > 0)
                {
                    references.Dequeue();
                }
            }
        }

        public void RemoveAll()
        {
            lock (references)
            {
                removeReferenceCount += references.Count;
                references.Clear();
            }
        }
    }
}