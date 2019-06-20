using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Animation
{
    [DisallowMultipleComponent]
    public class AnimationManager : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().AnimationPriority;
        
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        #region Animator

        public static int GetIDParamByName(string name)
        {
            return UnityEngine.Animator.StringToHash(name);
        }

        #region Bool

        public static bool GetBool(Animator animator, string name)
        {
            if (animator == null) return false;
            return animator.GetBool(name);
        }
        
        public static bool GetBool(Animator animator, int id)
        {
            if (animator == null) return false;
            return animator.GetBool(id);
        }

        public static void Animator(Animator animator, string name, bool state)
        {
            if (animator == null) return;
            animator.SetBool(name, state);
        }

        public static void Animator(Animator animator, int id, bool state)
        {
            if (animator == null) return;
            animator.SetBool(id, state);
        }
        
        public static void Animator(GameObject obj, string name, bool state)
        {
            Animator animator = obj.GetComponent<Animator>();
            Animator(animator, name, state);
        }
        
        public static void Animator(GameObject obj, int id, bool state)
        {
            Animator animator = obj.GetComponent<Animator>();
            Animator(animator, id, state);
        }

        public static bool GetBool(GameObject obj, string name)
        {
            Animator animator = obj.GetComponent<Animator>();
            return GetBool(animator, name);
        }
        
        public static bool GetBool(GameObject obj, int id)
        {
            Animator animator = obj.GetComponent<Animator>();
            return GetBool(animator, id);
        }

        #endregion
        
        #region Float

        public static float GetFloat(Animator animator, string name)
        {
            if (animator == null) return -1;
            return animator.GetFloat(name);
        }
        
        public static float GetFloat(Animator animator, int id)
        {
            if (animator == null) return -1;
            return animator.GetFloat(id);
        }

        public static void Animator(Animator animator, string name, float value)
        {
            if (animator == null) return;
            animator.SetFloat(name, value);
        }

        public static void Animator(Animator animator, int id, float value)
        {
            if (animator == null) return;
            animator.SetFloat(id, value);
        }
        
        public static void Animator(GameObject obj, string name, float value)
        {
            Animator animator = obj.GetComponent<Animator>();
            Animator(animator, name, value);
        }
        
        public static void Animator(GameObject obj, int id, float value)
        {
            Animator animator = obj.GetComponent<Animator>();
            Animator(animator, id, value);
        }

        public static float GetFloat(GameObject obj, string name)
        {
            Animator animator = obj.GetComponent<Animator>();
            return GetFloat(animator, name);
        }
        
        public static float GetFloat(GameObject obj, int id)
        {
            Animator animator = obj.GetComponent<Animator>();
            return GetFloat(animator, id);
        }

        #endregion
        
        #region Integer

        public static int GetInteger(Animator animator, string name)
        {
            if (animator == null) return -1;
            return animator.GetInteger(name);
        }
        
        public static int GetInteger(Animator animator, int id)
        {
            if (animator == null) return -1;
            return animator.GetInteger(id);
        }

        public static void Animator(Animator animator, string name, int value)
        {
            if (animator == null) return;
            animator.SetInteger(name, value);
        }

        public static void Animator(Animator animator, int id, int value)
        {
            if (animator == null) return;
            animator.SetInteger(id, value);
        }
        
        public static void Animator(GameObject obj, string name, int value)
        {
            Animator animator = obj.GetComponent<Animator>();
            Animator(animator, name, value);
        }
        
        public static void Animator(GameObject obj, int id, int value)
        {
            Animator animator = obj.GetComponent<Animator>();
            Animator(animator, id, value);
        }

        public static float GetInteger(GameObject obj, string name)
        {
            Animator animator = obj.GetComponent<Animator>();
            return GetInteger(animator, name);
        }
        
        public static float GetInteger(GameObject obj, int id)
        {
            Animator animator = obj.GetComponent<Animator>();
            return GetInteger(animator, id);
        }

        #endregion
        
        #region Trigger

        public static void SetTrigger(Animator animator, string name)
        {
            if (animator == null) return;
            animator.SetTrigger(name);
        }
        
        public static void SetTrigger(Animator animator, int id)
        {
            if (animator == null) return;
            animator.SetTrigger(id);
        }
        
        public static void ResetTrigger(Animator animator, string name)
        {
            if (animator == null) return;
            animator.ResetTrigger(name);
        }
        
        public static void ResetTrigger(Animator animator, int id)
        {
            if (animator == null) return;
            animator.ResetTrigger(id);
        }

        public static void SetTrigger(GameObject obj, string name)
        {
            Animator animator = obj.GetComponent<Animator>();
            SetTrigger(animator,name);
        }
        
        public static void SetTrigger(GameObject obj, int id)
        {
            Animator animator = obj.GetComponent<Animator>();
            SetTrigger(animator,id);
        }
        
        public static void ResetTrigger(GameObject obj, string name)
        {
            Animator animator = obj.GetComponent<Animator>();
            ResetTrigger(animator,name);
        }
        
        public static void ResetTrigger(GameObject obj, int id)
        {
            Animator animator = obj.GetComponent<Animator>();
            ResetTrigger(animator,id);
        }
      
        #endregion
        
        #endregion
       
    }
}