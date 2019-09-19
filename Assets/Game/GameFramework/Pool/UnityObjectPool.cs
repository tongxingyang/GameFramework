using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Pool
{
    public class UnityObjectPool
    {
        private readonly Stack<Object> _Stack = new Stack<Object>();

        private Object prefab;
        private int limite;

        public UnityObjectPool(Object prefab, int limite)
        {
            this.prefab = prefab;
            this.limite = limite;
        }

        public Object Get()
        {
            Object element = null;
            if (_Stack.Count == 0)
            {
                element = Object.Instantiate(prefab);
            }
            else
            {
                element = _Stack.Pop();
            }

            return element;
        }

        public void Release(Object element)
        {
            if (_Stack.Count >= limite)
            {
                Object.Destroy(element);
                return;
            }

            _Stack.Push(element);
        }

        public void ReleaseAll()
        {
            while (_Stack.Count > 0)
            {
                Object element = _Stack.Pop();
                Object.Destroy(element);
            }
        }
    }
}