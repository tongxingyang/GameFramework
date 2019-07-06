using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension
{
    public class UIJoystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,  IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Files

        public RectTransform BackgroundRectTransform;
        public RectTransform CursorRectTransform;
        public RectTransform TouchBorderRectTransform;
        public RectTransform InstructionsRectTransform;
        public float MoveMaxDistance = 150f;
        public float CheckTimeMobile = 0.2f;
        public float CheckTimePC = 0.1f;
        public Action OnJoystickClick;
        public Action OnJoystickPressHold;
        public Action OnJoystickSlider;
        public Action<bool> OnJoystickChangeState;
        public Action<Vector2> OnJoystickPointDown;
        public Action<Vector2> OnJoystickPointUp;
        public Action<Vector2> OnJoystickDragBegin;
        public Action<Vector2> OnJoystickDrag;
        public Action<Vector2> OnJoystickDragEnd;
        public Camera UICamera;
        
        private bool isInit = false;
        private Vector2 backgroundFirstPos;
        private Vector2 centerFirstPos;
        private Vector2 instructionsFirstPos;
        private Vector2 oldDir = Vector2.zero;
        private float lastDownTime;
        private float pressHoldTime = 0.4f;
        private bool isPressing = false;
        private bool isPressHoldThisTime = false;
        private float lastPressTime;
        #endregion

        void Start()
        {   
            isInit = true;
            SetStateChange(false);
            backgroundFirstPos = BackgroundRectTransform.anchoredPosition;
            centerFirstPos = CursorRectTransform.anchoredPosition;
            instructionsFirstPos = InstructionsRectTransform.anchoredPosition;
            InstructionsRectTransform.gameObject.SetActive(false);
            AddPointDown(JoystickPointDown);
            AddDrag(JoystickDrag);
            AddPointUp(JoystickPointUp);
            
        }

        private void SetStateChange(bool isMove)
        {
            OnJoystickChangeState?.Invoke(isMove);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            isPressing = true;
            isPressHoldThisTime = false;
            lastPressTime = Time.unscaledTime;
            OnJoystickPointDown?.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
            OnJoystickPointUp?.Invoke(eventData.position);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnJoystickPressHold != null &&Time.unscaledTime - lastPressTime > pressHoldTime)
                return;
            OnJoystickClick?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnJoystickDragBegin?.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnJoystickDrag?.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnJoystickDragEnd?.Invoke(eventData.position);
        }

        public void AddClick(Action cb, bool reset = false)
        {
            if (OnJoystickClick == null || reset)
            {
                OnJoystickClick = cb;
                return;
            }

            Delegate[] inlist = OnJoystickClick.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;
            
            OnJoystickClick = (Action)Delegate.Combine(OnJoystickClick, cb);
        }
        
        public void SubClick(Action cb)
        {
            if (OnJoystickClick == null)
            {
                return;
            }

            Delegate[] inlist = OnJoystickClick.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickClick = (Action)Delegate.Remove(OnJoystickClick, cb);
                }
        }
        
        public void AddPressHold(Action cb, bool reset = false)
        {
            if (OnJoystickPressHold == null || reset)
            {
                OnJoystickPressHold = cb;
                return;
            }

            Delegate[] inlist = OnJoystickPressHold.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;
            
            OnJoystickPressHold = (Action)Delegate.Combine(OnJoystickPressHold, cb);
        }
        
        public void SubPressHold(Action cb)
        {
            if (OnJoystickPressHold == null)
            {
                return;
            }

            Delegate[] inlist = OnJoystickPressHold.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickPressHold = (Action)Delegate.Remove(OnJoystickPressHold, cb);
                }
        }
        
        public void AddChangeState(Action<bool> cb, bool reset = false)
        {
            if (OnJoystickChangeState == null || reset)
            {
                OnJoystickChangeState = cb;
                return;
            }

            Delegate[] inlist = OnJoystickChangeState.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;
            
            OnJoystickChangeState = (Action<bool>)Delegate.Combine(OnJoystickChangeState, cb);
        }
        
        public void SubChangeState(Action<bool> cb)
        {
            if (OnJoystickChangeState == null)
            {
                return;
            }

            Delegate[] inlist = OnJoystickChangeState.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickChangeState = (Action<bool>)Delegate.Remove(OnJoystickChangeState, cb);
                }
            
        }

        public void AddPointUp(Action<Vector2> cb, bool reset = false)
        {
            if (OnJoystickPointUp == null || reset)
            {
                OnJoystickPointUp = cb;
                return;
            }
            Delegate[] inlist = OnJoystickPointUp.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;

            OnJoystickPointUp = (Action<Vector2>)Delegate.Combine(OnJoystickPointUp, cb);
        }
        
        public void SubPointUp(Action<Vector2> cb)
        {
            if (OnJoystickPointUp == null)
            {
                return;
            }
            Delegate[] inlist = OnJoystickPointUp.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickPointUp = (Action<Vector2>)Delegate.Remove(OnJoystickPointUp, cb);
                }
        }

        public void AddPointDown(Action<Vector2> cb, bool reset = false)
        {
            if (OnJoystickPointDown == null || reset)
            {
                OnJoystickPointDown = cb;
                return;
            }
            Delegate[] inlist = OnJoystickPointDown.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;

            OnJoystickPointDown = (Action<Vector2>)Delegate.Combine(OnJoystickPointDown, cb);
        }

        public void SubPointDown(Action<Vector2> cb)
        {
            if (OnJoystickPointDown == null)
            {
                return;
            }
            Delegate[] inlist = OnJoystickPointDown.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickPointDown = (Action<Vector2>)Delegate.Remove(OnJoystickPointDown, cb);
                }
        }
        
        public void AddDragBegin(Action<Vector2> cb, bool reset = false)
        {
            if (OnJoystickDragBegin == null || reset)
            {
                OnJoystickDragBegin = cb;
                return;
            }
            Delegate[] inlist = OnJoystickDragBegin.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;

            OnJoystickDragBegin = (Action<Vector2>)Delegate.Combine(OnJoystickDragBegin, cb);
        }

        public void SubDragBegin(Action<Vector2> cb, bool reset = false)
        {
            if (OnJoystickDragBegin == null)
            {
                return;
            }
            Delegate[] inlist = OnJoystickDragBegin.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickDragBegin = (Action<Vector2>)Delegate.Remove(OnJoystickDragBegin, cb);
                }
        }
        
        public void AddDrag(Action<Vector2> cb, bool reset = false)
        {
            if (OnJoystickDrag == null || reset)
            {
                OnJoystickDrag = cb;
                return;
            }

            Delegate[] inlist = OnJoystickDrag.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;

            OnJoystickDrag = (Action<Vector2>)Delegate.Combine(OnJoystickDrag, cb);
        }
        
        public void SubDrag(Action<Vector2> cb)
        {
            if (OnJoystickDrag == null)
            {
                return;
            }

            Delegate[] inlist = OnJoystickDrag.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickDrag = (Action<Vector2>)Delegate.Remove(OnJoystickDrag, cb);
                }

        }

        public void AddDragEnd(Action<Vector2> cb, bool reset = false)
        {
            if (OnJoystickDragEnd == null || reset)
            {
                OnJoystickDragEnd = cb;
                return;
            }

            Delegate[] inlist = OnJoystickDragEnd.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb) return;

            OnJoystickDragEnd = (Action<Vector2>)Delegate.Combine(OnJoystickDragEnd, cb);
        }
        
        public void SubDragEnd(Action<Vector2> cb)
        {
            if (OnJoystickDragEnd == null)
            {
                return;
            }

            Delegate[] inlist = OnJoystickDragEnd.GetInvocationList();
            foreach (Delegate d in inlist)
                if (d == (Delegate) cb)
                {
                    OnJoystickDragEnd = (Action<Vector2>)Delegate.Remove(OnJoystickDragEnd, cb);
                }
        }

        public void ExecuteClick()
        {
            OnJoystickClick?.Invoke();
        }

        public void ExecuteDown()
        {
            ExecuteDown(Vector2.zero);
        }

        public void ExecuteDown(Vector2 position)
        {
            OnJoystickPointDown?.Invoke(position);
        }

        public void ExecuteUp()
        {
            ExecuteUp(Vector2.zero);
        }

        public void ExecuteUp(Vector2 position)
        {
            OnJoystickPointUp?.Invoke(position);
        }

        public void ExecuteHold()
        {
            OnJoystickPressHold?.Invoke();
        }

        public void ExecuteBeginDrag()
        {
            ExecuteBeginDrag(Vector2.zero);
        }

        public void ExecuteBeginDrag(Vector2 position)
        {
            OnJoystickDragBegin?.Invoke(position);
        }

        public void ExecuteDrag()
        {
            ExecuteDrag(Vector2.zero);
        }

        public void ExecuteDrag(Vector2 position)
        {
            OnJoystickDrag?.Invoke(position);
        }

        public void ExecuteEndDrag()
        {
            ExecuteEndDrag(Vector2.zero);
        }

        public void ExecuteEndDrag(Vector2 position)
        {
            OnJoystickDragEnd?.Invoke(position);
        }

        private void JoystickPointDown(Vector2 data)
        {
            Vector2 pos;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(TouchBorderRectTransform, data, UICamera, out pos))
                return;
            pos.x = pos.x + TouchBorderRectTransform.localPosition.x;
            pos.y = pos.y + TouchBorderRectTransform.localPosition.y;
            BackgroundRectTransform.anchoredPosition = pos;
            CursorRectTransform.anchoredPosition = pos;
            InstructionsRectTransform.anchoredPosition = pos;
            SetStateChange(true);
            lastDownTime = Time.time;
        }

        private void JoystickDrag(Vector2 data)
        {
            Vector2 pos;
            InstructionsRectTransform.gameObject.SetActive(true);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(TouchBorderRectTransform, data, UICamera, out pos))
                return;
            pos.x = pos.x + TouchBorderRectTransform.localPosition.x;
            pos.y = pos.y + TouchBorderRectTransform.localPosition.y;
            Vector2 link = pos - BackgroundRectTransform.anchoredPosition;
            if (link.sqrMagnitude <= MoveMaxDistance * MoveMaxDistance)
                CursorRectTransform.anchoredPosition = pos;
            else
                CursorRectTransform.anchoredPosition = BackgroundRectTransform.anchoredPosition + link.normalized * MoveMaxDistance;

            float angle = Vector2.Angle(Vector2.up, link.normalized);
            angle *= Mathf.Sign(Vector3.Cross(Vector2.up, link.normalized).z);
            InstructionsRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private void JoystickPointUp(Vector2 data)
        {
            if (!isInit)
                return;
            float sq =(CursorRectTransform.anchoredPosition - BackgroundRectTransform.anchoredPosition).sqrMagnitude;
            BackgroundRectTransform.anchoredPosition = backgroundFirstPos;
            CursorRectTransform.anchoredPosition = centerFirstPos;
            SetStateChange(false);
            InstructionsRectTransform.anchoredPosition = instructionsFirstPos;
            InstructionsRectTransform.gameObject.SetActive(false);
            InstructionsRectTransform.localEulerAngles = Vector3.zero;
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Time.time - lastDownTime < CheckTimePC && sq > 100)
                OnJoystickSlider?.Invoke();
#else
            if (Time.time - lastDownTime < CheckTimeMobile && sq > 100)
                OnJoystickSlider?.Invoke();
#endif    
        }
        
        private Vector2 GetDir()
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
                return new Vector2(100, 100);
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                return new Vector2(-100, 100);
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
                return new Vector2(100, -100);
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
                return new Vector2(-100, -100);
            else if (Input.GetKey(KeyCode.W))
                return new Vector2(0, 100);
            else if (Input.GetKey(KeyCode.S))
                return new Vector2(0, -100);
            else if (Input.GetKey(KeyCode.A))
                return new Vector2(-100, 0);
            else if (Input.GetKey(KeyCode.D))
                return new Vector2(100, 0);
            else
                return Vector2.zero;
        }
      
        public void Update()
        {
            Vector2 dir;
#if UNITY_EDITOR || UNITY_STANDALONE
            dir = GetDir();
#endif
            if (dir != oldDir)
            {
                if (oldDir == Vector2.zero && dir != Vector2.zero)
                {
                    ExecuteDown(backgroundFirstPos);
                    ExecuteBeginDrag(backgroundFirstPos);
                    ExecuteDrag(backgroundFirstPos + dir);
                }
                else if (oldDir != Vector2.zero && dir == Vector2.zero)
                {
                    ExecuteUp(backgroundFirstPos + dir);
                    ExecuteEndDrag(backgroundFirstPos + dir);
                }                
                else
                {
                    ExecuteDrag(backgroundFirstPos + dir);
                }                
                oldDir = dir;
            }
        }
        
        public void LateUpdate()
        {
            if(!isPressHoldThisTime &&isPressing&& Time.unscaledTime - lastPressTime > pressHoldTime)
            {
                isPressHoldThisTime = true;
                OnJoystickPressHold?.Invoke();
            }
        }
        
    }
}