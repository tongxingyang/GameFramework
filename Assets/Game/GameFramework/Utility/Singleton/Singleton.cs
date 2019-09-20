using System;
using System.Reflection;
using GameFramework.Debug;
using UnityEngine;

namespace GameFramework.Utility.Singleton
{
    public abstract class Singleton<T> : IHandleMessage where T : class, new() 
    {
        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    CreateInstance();
                }
                return m_instance;
            }
        }

        private static void CreateInstance()
        {
            if (m_instance == null)
            {
                m_instance = Activator.CreateInstance<T>();
                if (m_instance != null)
                {
                    (m_instance as Singleton<T>).Init();
                }
                else
                {
                    Debuger.LogError("Create Singletion Onject Error Type ："+typeof(T));
                }
            }
        }

        public static void DestoryInstance()
        {
            if (m_instance != null)
            {
                (m_instance as Singleton<T>).UnInit();
                m_instance = null;
            }
        }
        public static T GetInstance()
        {
            if (m_instance == null)
            {
                CreateInstance();
            }
            return m_instance;
        }

        public virtual void Init()
        {

        }

        public virtual void UnInit()
        {

        }

        public void HandleMessage(string msg, object[] args)
        {
            var mi = this.GetType()
                .GetMethod(msg, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
            {
                mi.Invoke(this, BindingFlags.NonPublic, null, args, null);
            }
        }

        public object HandleMessageRetValue(string msg, object[] args)
        {
            var mi = this.GetType()
                .GetMethod(msg, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(this, BindingFlags.NonPublic, null, args, null);
            }
            return null;
        }
    }
}
