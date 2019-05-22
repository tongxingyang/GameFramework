using GameFramework.Utility.Extension;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework.UI.UITools
{
    public delegate void OnTouchEventHandle(GameObject listener, object args, params object[] param);

    public class UIEventListener : 
        MonoBehaviour,
        IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler,
        IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler,
        IScrollHandler,IUpdateSelectedHandler,ISelectHandler,IDeselectHandler,
        IMoveHandler,ISubmitHandler,ICancelHandler
    {

        private float lastClickTime = 0f;
        
        public TouchHandle onClick;
        public TouchHandle onDoubleClick;
        public TouchHandle onDown;
        public TouchHandle onEnter;
        public TouchHandle onExit;
        public TouchHandle onUp;
        public TouchHandle onSelect;
        public TouchHandle onUpdateSelect;
        public TouchHandle onDeSelect;
        public TouchHandle onDrag;
        public TouchHandle onEndDrag;
        public TouchHandle onDrop;
        public TouchHandle onScroll;
        public TouchHandle onMove;
        public TouchHandle onBeginDrag;
        public TouchHandle onSubmit;
        public TouchHandle onCancel;

        public static UIEventListener Get(GameObject o)
        {
            return o.GetOrAddComponent<UIEventListener>();
        }

        public void SetUIEventListener(enTouchEventType type,OnTouchEventHandle handle,params object[] paramsObj)
        {
            switch (type)
            {
                case enTouchEventType.OnClick:
                    if (null == onClick)
                    {
                        onClick = new TouchHandle();
                    }
                    onClick.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnDoubleClick:
                    if (null == onDoubleClick)
                    {
                        onDoubleClick = new TouchHandle();
                    }
                    onDoubleClick.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnDown:
                    if (onDown == null)
                    {
                        onDown = new TouchHandle();
                    }
                    onDown.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnUp:
                    if (onUp == null)
                    {
                        onUp = new TouchHandle();
                    }
                    onUp.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnEnter:
                    if (onEnter == null)
                    {
                        onEnter = new TouchHandle();
                    }
                    onEnter.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnExit:
                    if (onExit == null)
                    {
                        onExit = new TouchHandle();
                    }
                    onExit.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnDrag:
                    if (onDrag == null)
                    {
                        onDrag = new TouchHandle();
                    }
                    onDrag.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnDrop:
                    if (onDrop == null)
                    {
                        onDrop = new TouchHandle();
                    }
                    onDrop.SetHandle(handle, paramsObj);
                    break;

                case enTouchEventType.OnEndDrag:
                    if (onEndDrag == null)
                    {
                        onEndDrag = new TouchHandle();
                    }
                    onEndDrag.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnSelect:
                    if (onSelect == null)
                    {
                        onSelect = new TouchHandle();
                    }
                    onSelect.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnUpdateSelect:
                    if (onUpdateSelect == null)
                    {
                        onUpdateSelect = new TouchHandle();
                    }
                    onUpdateSelect.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnDeSelect:
                    if (onDeSelect == null)
                    {
                        onDeSelect = new TouchHandle();
                    }
                    onDeSelect.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnScroll:
                    if (onScroll == null)
                    {
                        onScroll = new TouchHandle();
                    }
                    onScroll.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnMove:
                    if (onMove == null)
                    {
                        onMove = new TouchHandle();
                    }
                    onMove.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnBeginDrag:
                    if (onBeginDrag == null)
                    {
                        onBeginDrag = new TouchHandle();
                    }
                    onBeginDrag.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnSubmit:
                    if (onSubmit == null)
                    {
                        onSubmit = new TouchHandle();
                    }
                    onSubmit.SetHandle(handle, paramsObj);
                    break;
                case enTouchEventType.OnCancel:
                    if (onCancel == null)
                    {
                        onCancel = new TouchHandle();
                    }
                    onCancel.SetHandle(handle, paramsObj);
                    break;
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickTime - lastClickTime < 0.3f)
            {
                onDoubleClick?.CallHandle(this.gameObject,eventData);
                lastClickTime = 0;
            }
            else
            {
                onClick?.CallHandle(this.gameObject,eventData);
                lastClickTime = eventData.clickTime;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onDown?.CallHandle(this.gameObject,eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onUp.CallHandle(this.gameObject,eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnter?.CallHandle(this.gameObject,eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit?.CallHandle(this.gameObject,eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            onSelect?.CallHandle(this.gameObject,eventData);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelect?.CallHandle(this.gameObject,eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            onDeSelect?.CallHandle(this.gameObject,eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.CallHandle(this.gameObject,eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.CallHandle(this.gameObject,eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            onDrop?.CallHandle(this.gameObject,eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            onScroll?.CallHandle(this.gameObject,eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            onMove?.CallHandle(this.gameObject,eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.CallHandle(this.gameObject,eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            onSubmit?.CallHandle(this.gameObject,eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            onCancel?.CallHandle(this.gameObject,eventData);
        }

        void OnDestroy()
        {
            onClick?.ResetHandle();
            onClick = null;
            onDoubleClick?.ResetHandle();
            onDoubleClick = null;
            onDown?.ResetHandle();
            onDown = null;
            onEnter?.ResetHandle();
            onEnter = null;
            onExit?.ResetHandle();
            onExit = null;
            onUp?.ResetHandle();
            onUp = null;
            onSelect?.ResetHandle();
            onSelect = null;
            onUpdateSelect?.ResetHandle();
            onUpdateSelect = null;
            onDeSelect?.ResetHandle();
            onDeSelect = null;
            onDrag?.ResetHandle();
            onDrag = null;
            onEndDrag?.ResetHandle();
            onEndDrag = null;
            onDrop?.ResetHandle();
            onDrop = null;
            onScroll?.ResetHandle();
            onScroll = null;
            onMove?.ResetHandle();
            onMove = null;
        }

      
    }
}