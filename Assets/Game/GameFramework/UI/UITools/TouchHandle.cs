using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.UI.UITools
{
    public class TouchHandle
    {
        private event OnTouchEventHandle eventHandle;
        private object[] eventParmas;

        public void SetHandle(OnTouchEventHandle handle,params object[] paramsObj)
        {
            ResetHandle();
            eventHandle = handle;
            eventParmas = paramsObj;
        }

        public void ResetHandle()
        {
            if (eventHandle != null)
            {
                eventHandle = null;
                eventParmas = null;
            }
        }

        public void CallHandle(GameObject obj, object args)
        {
            eventHandle?.Invoke(obj, args, eventParmas);
        }
    }
}