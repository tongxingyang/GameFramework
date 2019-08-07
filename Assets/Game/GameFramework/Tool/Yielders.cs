using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Events;

namespace GameFramework.Tool
{
// Usage:
//    yield return new WaitForEndOfFrame();     =>      yield return YieldManager.EndOfFrame;
//    yield return new WaitForFixedUpdate();    =>      yield return YieldManager.FixedUpdate;
//    yield return new WaitForSeconds(1.0f);    =>      yield return YieldManager.GetWaitForSeconds(1.0f);

// http://forum.unity3d.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/
    public static class Yielders
    {
        public static bool Enable = true;
        public static uint internalCounter = 0;// counts how many times the app yields
        static Dictionary<float,WaitForSeconds> waitForSecondses = new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
        static WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { internalCounter++;
                return Enable ? endOfFrame : new WaitForEndOfFrame();
            }
        }
        static WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get { internalCounter++;
                return Enable ? fixedUpdate : new WaitForFixedUpdate();
            }
        }

        public static void ClearWaitForScends()
        {
            waitForSecondses.Clear();
        }

        public static WaitForSeconds GetWaitForSeconds(float time)
        {
            internalCounter++;
            if (!Enable)
            {
                return new WaitForSeconds(time);
            }
            WaitForSeconds wfs;
            if (waitForSecondses.TryGetValue(time, out wfs))
            {
                waitForSecondses.Add(time, wfs = new WaitForSeconds(time));
            }
            return wfs;
        }

        public static IEnumerator DelayCallAction(UnityAction action, float time)
        {
            yield return GetWaitForSeconds(time);
            action?.Invoke();
        }

        public static IEnumerator EndOfFrameCallAction(UnityAction action)
        {
            yield return EndOfFrame;
            action?.Invoke();
        }
    }
}