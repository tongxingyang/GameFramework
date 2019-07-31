using System;
using System.Collections.Generic;
using GameFramework.DataNode;
using GameFramework.DataTable;
using GameFramework.Debug;
using GameFramework.Download;
using GameFramework.FSM;
using GameFramework.Pool.ReferencePool;
using GameFramework.Res;
using GameFramework.Setting;
using GameFramework.Sound;
using GameFramework.Timer;
using GameFramework.Utility.Singleton;
using GameFramework.Web;
using UnityEngine;

namespace GameFramework.Base
{
    public class GameEntry : Singleton<GameEntry>
    {
        public DataNodeComponent DataNodeComponent;
        public DataTableComponent DataTableComponent;
        public DownloadComponent DownloadComponent;
        public FSMComponent FsmComponent;
        public ResourceComponent ResourceComponent;
        public SettingComponent SettingComponent;
        public SoundComponent SoundComponent;
        public TimerComponent TimerComponent;
        public WebComponent WebComponent;

        private readonly LinkedList<GameFrameworkComponent> GameFrameworkComponents =
            new LinkedList<GameFrameworkComponent>();

        public Transform ComponentRoot = null;

        public void InitComponent(Transform componentRoot)
        {
            this.ComponentRoot = componentRoot;
            DataNodeComponent = FindAndRegisterComponent<DataNodeComponent>();
            DataTableComponent = FindAndRegisterComponent<DataTableComponent>();
            DownloadComponent = FindAndRegisterComponent<DownloadComponent>();
            FsmComponent = FindAndRegisterComponent<FSMComponent>();
            ResourceComponent = FindAndRegisterComponent<ResourceComponent>();
            SettingComponent = FindAndRegisterComponent<SettingComponent>();
            SoundComponent = FindAndRegisterComponent<SoundComponent>();
            TimerComponent = FindAndRegisterComponent<TimerComponent>();
            WebComponent = FindAndRegisterComponent<WebComponent>();
        }

        private T FindAndRegisterComponent<T>() where T : GameFrameworkComponent
        {
            T t = this.ComponentRoot.GetComponentInChildren<T>();
            if (t == null)
            {
                UnityEngine.Debug.LogError("不能获取Component : "+typeof(T));
            }
            RegisterComponent(t);
            return t;
        }
        
        public T GetComponent<T>() where T : GameFrameworkComponent
        {
            var component = (T) GetComponent(typeof(T));
            if (component == null)
            {
                component = AddComponent<T>();
                if (component == null)
                    Debuger.LogError(Utility.StringUtility.Format("can not find the component '{0}'",
                        typeof(T).FullName));
            }

            return component;
        }

        private T AddComponent<T>() where T : GameFrameworkComponent
        {
            Type type = typeof(T);
            GameObject newComponent = new GameObject(typeof(T).FullName);
            GameFrameworkComponent component = newComponent.AddComponent<T>();
            if (component == null)
            {
                Debuger.LogError(Utility.StringUtility.Format("can not add the component '{0}' .", type.FullName));
                return null;
            }
            newComponent.transform.parent = ComponentRoot;
            newComponent.transform.position = Vector3.zero;
            RegisterComponent(component);
            return (T) component;
        }

        public void RemoveComponent(Type type)
        {
            var component = GetComponent(type);
            if (component == null)
            {
                return;
            }
            GameFrameworkComponents.Remove(component);
            UnityEngine.Object.Destroy(component.gameObject);
        }
        
        public GameFrameworkComponent GetComponent(Type type)
        {
            LinkedListNode<GameFrameworkComponent> current = GameFrameworkComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        public void Shutdown()
        {
            for (LinkedListNode<GameFrameworkComponent> current = GameFrameworkComponents.Last;
                current != null;
                current = current.Previous)
            {
                current.Value.Shutdown();
            }
            ReferencePool.ClearAllPools();
            GameFrameworkComponents.Clear();
        }

        public bool IsHaveComponent(GameFrameworkComponent gameFrameworkComponent)
        {
            Type type = gameFrameworkComponent.GetType();

            LinkedListNode<GameFrameworkComponent> current = GameFrameworkComponents.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    Debuger.LogError("Game Framework component type '{0}' is already exist.", LogColor.Red,
                        type.FullName);
                    return true;
                }

                current = current.Next;
            }
            return false;
        }

        public void RegisterComponent(GameFrameworkComponent gameFrameworkComponent)
        {
            if (gameFrameworkComponent == null)
            {
                return;
            }
            if (IsHaveComponent(gameFrameworkComponent))
            {
                Debuger.LogError("error Repeat addition component  type = " +
                                 gameFrameworkComponent.GetType().FullName);
                return;
            }
            LinkedListNode<GameFrameworkComponent> current = GameFrameworkComponents.First;
            while (current != null)
            {
                if (gameFrameworkComponent.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                GameFrameworkComponents.AddBefore(current, gameFrameworkComponent);
            }
            else
            {
                GameFrameworkComponents.AddLast(gameFrameworkComponent);
            }
        }

        public void OnAwake()
        {
            foreach (GameFrameworkComponent module in GameFrameworkComponents)
            {
                module.OnAwake();
            }
        }

        public void OnStart()
        {
            foreach (GameFrameworkComponent module in GameFrameworkComponents)
            {
                module.OnStart();
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (GameFrameworkComponent module in GameFrameworkComponents)
            {
                module.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        public void OnLateUpdate()
        {
            foreach (GameFrameworkComponent module in GameFrameworkComponents)
            {
                module.OnLateUpdate();
            }
        }

        public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (GameFrameworkComponent module in GameFrameworkComponents)
            {
                module.OnFixedUpdate(elapseSeconds, realElapseSeconds);
            }
        }
    }
}