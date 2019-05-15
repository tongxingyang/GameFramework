using UnityEngine;

namespace GameFramework.Base
{
    public abstract class GameFrameworkComponent:MonoBehaviour
    {
        public virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        public virtual void OnAwake()
        {
            
        }

        public virtual void OnStart()
        {
            
        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        
        public virtual void OnLateUpdate()
        {
            
        }
        
        public virtual void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }
        
        public virtual void Shutdown()
        {
            
        }
    }
}