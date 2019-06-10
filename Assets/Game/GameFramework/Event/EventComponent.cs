using System;
using GameFramework.Base;
using GameFramework.Pool.EventPool;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Event
{
    [DisallowMultipleComponent]
    public class EventComponent : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().EventPriority;
        public int EventHandlerCount => eventPool.EventHandlerCount;
        public int EventCount => eventPool.EventCount;
        private EventPool<GameEventArgs> eventPool;

        public override void OnAwake()
        {
            base.OnAwake();
            eventPool = new EventPool<GameEventArgs>(enEventPoolMode.AllowNoHandler | enEventPoolMode.AllowMultiHandler);
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            eventPool.Update(elapseSeconds,realElapseSeconds);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            eventPool.Shutdown();
        }
        
        public int Count(enEventID id)
        {
            return eventPool.Count(id);
        }
        
        public bool Check(enEventID id, EventHandler<GameEventArgs> handler)
        {
            return eventPool.Check(id, handler);
        }
        
        public void Subscribe(enEventID id, EventHandler<GameEventArgs> handler)
        {
            eventPool.Subscribe(id, handler);
        }
        
        public void Unsubscribe(enEventID id, EventHandler<GameEventArgs> handler)
        {
            eventPool.Unsubscribe(id, handler);
        }
        
        public void SetDefaultHandler(EventHandler<GameEventArgs> handler)
        {
            eventPool.SetDefaultHandler(handler);
        }
        
        public void Fire(object sender, GameEventArgs e)
        {
            eventPool.Fire(sender, e);
        }

        public void FireNow(object sender, GameEventArgs e)
        {
            eventPool.FireNow(sender, e);
        }
    }
}