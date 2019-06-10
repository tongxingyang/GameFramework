using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Pool.ReferencePool
{
    public static class ReferencePool
    {
        private static readonly Dictionary<string, ReferenceCollection> referenceCollections = new Dictionary<string, ReferenceCollection>();
        public static int ReferencePoolCount => referenceCollections.Count;

        public static void ClearAllPools()
        {
            lock (referenceCollections)
            {
                foreach (var referenceCollection in referenceCollections)
                {
                    referenceCollection.Value.RemoveAll();
                }
                referenceCollections.Clear();
            }
        }
        
        public static T Acquire<T>() where T:class,IReference,new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        public static void Release<T>(T reference) where T : class, IReference
        {
            if(reference == null) return;
            GetReferenceCollection(typeof(T)).Release(reference);
        }
        
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            if (count > 0)
            {
                GetReferenceCollection(typeof(T)).Add<T>(count);
            }
        }
        
        public static void Remove<T>(int count) where T : class, IReference
        {
            if (count > 0)
            {
                GetReferenceCollection(typeof(T)).Remove(count);
            }
        }
        
        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }
        private static ReferenceCollection GetReferenceCollection(Type type)
        {
            lock (referenceCollections)
            {
                string fullName = type.FullName;
                if (!referenceCollections.TryGetValue(fullName, out var referenceCollection))
                {
                    referenceCollection = new ReferenceCollection();
                    referenceCollections.Add(fullName, referenceCollection);
                }
                return referenceCollection;
            }
        }
    }
}