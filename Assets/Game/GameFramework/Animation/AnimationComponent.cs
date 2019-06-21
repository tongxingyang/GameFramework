using System.Collections;
using System.Collections.Generic;
using GameFramework.Animation.Base;
using GameFramework.Base;
using GameFramework.Debug;
using GameFramework.Tool;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace GameFramework.Animation
{
    [DisallowMultipleComponent]
    public class AnimationComponent : GameFrameworkComponent
    {
        private static AnimationComponent self;

        public static AnimationComponent Instance
        {
            get { return self; }
        }
        
        private Dictionary<GameObject,AnimationItem> animationItems = new Dictionary<GameObject, AnimationItem>();
        private Dictionary<GameObject,Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();
        private List<GameObject> removeList = new List<GameObject>();
        private List<UnityAction> actions = new List<UnityAction>();
        
        public override int Priority => SingletonMono<GameFramework>.GetInstance().AnimationPriority;
        
        public override void OnAwake()
        {
            self = this;
            base.OnAwake();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            foreach (AnimationItem item in this.animationItems.Values)
            {
                this.UpdateAnimation(item);
            }
            foreach (GameObject item in this.removeList)
            {
                this.animationItems.Remove(item);
            }
            foreach (UnityAction action in this.actions)
            {
                try
                {
                    action.Invoke();
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogErrorFormat(
                        "AnimationManager action error: {0}",
                        e
                    );
                }
            }
            this.removeList.Clear();
            this.actions.Clear();
        }

        private void UpdateAnimation(AnimationItem item)
        {
            bool isDone = true;
            if (item.parameter.isColor)
            {
                isDone &= (item.obj == null || AnimationColor.Color(item));
            }
            if (item.parameter.isFading)
            {
                isDone &= (item.obj == null || AnimationFade.Fade(item));
            }
            if (item.parameter.isResizing)
            {
                isDone &= (item.obj == null || AnimationSize.Resize(item));
            }
            if (item.parameter.isRotating)
            {
                isDone &= (item.obj == null || AnimationRotate.Rotate(item));
            }
            if (item.parameter.isScaling)
            {
                isDone &= (item.obj == null || AnimationScale.Scale(item));
            }
            if (item.parameter.isMoving)
            {
                isDone &= (item.obj == null || AnimationMove.Move(item));
            }
            item.frameCallback?.Invoke();
            if (isDone)
            {
                if (item.callback != null)
                {
                    if (!actions.Contains(item.callback))
                    {
                        actions.Add(item.callback);
                    }
                }
                removeList.Add(item.obj);
            }
            else
            {
                item.time += Time.unscaledDeltaTime;
            }
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
            foreach (KeyValuePair<GameObject,Coroutine> keyValuePair in coroutines)
            {
                StopCoroutine(keyValuePair.Value);
            }
            coroutines.Clear();
            animationItems.Clear();
            removeList.Clear();
            actions.Clear();
            
        }

        private IEnumerator AnimationDelay(AnimationItem item,float delayTime)
        {
            yield return Yielders.GetWaitForSeconds(delayTime);
            coroutines.Remove(item.obj);
            if (!animationItems.ContainsKey(item.obj))
            {
                this.animationItems.Add(item.obj,item);
            }
        }

        public AnimationParam GetAnimationParam(GameObject obj, string name)
        {
            AnimationCombo combo = obj.GetComponent<AnimationCombo>();
            if (combo == null || combo.GetAnimation(name) == null)
            {
                return null;
            }
            return combo.GetAnimation(name);
        }
        
        public static void PlayAnimation(GameObject obj,string name,UnityAction finishCallback = null,UnityAction frameCallback = null,bool loop = false, float delay = 0)
        {
            AnimationParam param = Instance.GetAnimationParam(obj, name);
            if (obj == null || param == null)
            {
                return;
            }
            param.loop = param.loop ? param.loop : loop;
            param.delay = delay != 0 ? delay : param.delay;
            AnimationItem item = new AnimationItem
            {
                obj = obj,
                time = param.startTime,
                parameter = param,
                callback = finishCallback,
                frameCallback = frameCallback
            };
            if (param.delay != 0)
            {
                if (Instance.coroutines.ContainsKey(obj))
                {
                    Instance.StopCoroutine(Instance.coroutines[obj]);
                    Instance.coroutines[obj] = Instance.StartCoroutine(Instance.AnimationDelay(item, param.delay));
                }
                else
                {
                    Instance.coroutines.Add(obj, Instance.StartCoroutine(Instance.AnimationDelay(item, param.delay)));
                }
            }
            else
            {
                if (Instance.animationItems.ContainsKey(obj))
                {
                    Instance.animationItems[obj] = item;
                }
                else
                {
                    Instance.animationItems.Add(obj,item);
                }
            }
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

        public static bool HasParameterOfType(Animator animator, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name)) { return false; }
            AnimatorControllerParameter[] parameters = animator.parameters;
            foreach (AnimatorControllerParameter currParam in parameters) 
            {
                if (currParam.type == type && currParam.name == name) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}