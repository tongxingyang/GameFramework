using System;
using GameFramework.Debug;

namespace GameFramework.Utility
{
    public delegate void MonoUpdateEvent();
    public class MonoHelper
    {
        private event MonoUpdateEvent updateEvent = null;
        private event MonoUpdateEvent laterUpdateEvent = null;
        private event MonoUpdateEvent fixedUpdateEvent = null;

        public void AddUpdateListener(MonoUpdateEvent listener)
        {
            if(listener!=null)
                updateEvent += listener;
        }
        
        public void AddFixedUpdateListener(MonoUpdateEvent listener)
        {
            if(listener!=null)
                fixedUpdateEvent += listener;
        }
        
        public void AddLaterUpdateListener(MonoUpdateEvent listener)
        {
            if(listener!=null)
                laterUpdateEvent += listener;
        }
        
        public void RemoveUpdateListener(MonoUpdateEvent listener)
        {
            if(listener!=null)
                updateEvent -= listener;
        }
        
        public void RemoveFixedUpdateListener(MonoUpdateEvent listener)
        {
            if(listener!=null)
                fixedUpdateEvent -= listener;
        }
        
        public void RemoveLaterUpdateListener(MonoUpdateEvent listener)
        {
            if(listener!=null)
                laterUpdateEvent -= listener;
        }

        public void OnUpdate()
        {
            if (updateEvent != null)
            {
                try
                {
                    updateEvent();
                }
                catch (Exception e)
                {
                    Debuger.LogError("MonoHelper Update() Error:{0}\n{1}",LogColor.Red, e.Message, e.StackTrace);
                }
            }
        }

        public void OnFixedUpdate()
        {
            if (fixedUpdateEvent != null)
            {
                try
                {
                    fixedUpdateEvent();
                }
                catch (Exception e)
                {
                    Debuger.LogError("MonoHelper FixedUpdate() Error:{0}\n{1}", LogColor.Red,e.Message, e.StackTrace);
                }
            }
        }
        public void OnLaterUpdate()
        {
            if (laterUpdateEvent != null)
            {
                try
                {
                    laterUpdateEvent();
                }
                catch (Exception e)
                {
                    Debuger.LogError("MonoHelper LaterUpdate() Error:{0}\n{1}", LogColor.Red,e.Message, e.StackTrace);
                }
            }
        }
        
    }
}