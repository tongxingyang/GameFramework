using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework.UI.UIExtension
{
    public class UIJoystick : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IBeginDragHandler, 
        IDragHandler, 
        IEndDragHandler
    {
        #region Method1

//        #region Files
//
//        public RectTransform Background;
//        public RectTransform Center;
//        
//        public float MaxDistance = 150f;
//        public float DistanceScale = 0.65f;
//        public float VerticalAngleScale = 0.8f;
//        public float CheckTimeMobile = 0.2f;
//        public float CheckTimePC = 0.1f;
//        public Action<Vector2> OnJoystickDown;
//        public Action<Vector2> OnJoystickDrag;
//        public Action<Vector2> OnJoystickUp;
//        public Action OnJoystickSlider;
//
//        Action onClick;
//        Action onPressHold;
//        Action<bool> onChangeState;
//        Action<PointerEventData> onPointUp;
//        Action<PointerEventData> onPointDown;
//        Action<PointerEventData> onDragBegin;
//        Action<PointerEventData> onDrag;
//        Action<PointerEventData> onDragEnd;
//        
//        private bool isUsingJoystick = false;
//        private bool isStarted = false;
//        private Vector2 backgroundFirstPos;
//        private Vector2 centerFirstPos;
//        private Vector2 oldDir = Vector2.zero;
//        private float lastDownTime;
//        private bool cached = false;
//        private bool isDraging = false;
//        private float pressHoldTime = 0.4f;
//        private bool isPressing = false;
//        private bool isPressHoldThisTime = false;
//        private float lastPressTime;
//        #endregion
//
//        void Start()
//        {   
//            Cache();
//            isStarted = true;
//            backgroundFirstPos = Background.anchoredPosition;
//            centerFirstPos = Center.anchoredPosition;
//            AddPointDown(JoystickPointDown);
//            AddDrag(JoystickDrag);
//            AddPointUp(JoystickPointUp);
//        }
//
//        void Cache()
//        {
//            if (cached)
//                return;
//            cached = true;
//            SetState(isDraging,false);
//        }
//        
//        public void SetState(bool state,bool checkSameState=false)
//        {
//            if (checkSameState && isDraging == state && cached)
//                return;
//            isDraging = state;
//            cached = true;
//            onChangeState?.Invoke(isDraging);
//        }
//        
//        public void OnPointerDown(PointerEventData eventData)
//        {
//            isPressing = true;
//            isPressHoldThisTime = false;
//            lastPressTime = Time.unscaledTime;
//
//            if (onPointDown != null)
//            {
//                if(eventData != null)
//                {
//                    eventData.pointerPress = gameObject;
//                }                
//                else
//                {
//                    eventData = new PointerEventData(EventSystem.current) {pointerPress = gameObject};
//                }
//                onPointDown(eventData);
//            }
//        }
//
//        public void OnPointerUp(PointerEventData eventData)
//        {
//            isPressing = false;
//
//            if (onPointUp != null)
//            {
//                if (eventData == null)
//                {
//                    eventData = new PointerEventData(EventSystem.current) {pointerPress = gameObject};
//                }
//                onPointUp(eventData);
//            }
//        }
//
//        public void OnPointerClick(PointerEventData eventData)
//        {
//            if (onPressHold != null &&Time.unscaledTime - lastPressTime > pressHoldTime)
//                return;
//            onClick?.Invoke();
//        }
//
//        public void OnBeginDrag(PointerEventData eventData)
//        {
//            onDragBegin?.Invoke(eventData);
//        }
//
//        public void OnDrag(PointerEventData eventData)
//        {
//            onDrag?.Invoke(eventData);
//        }
//
//        public void OnEndDrag(PointerEventData eventData)
//        {
//            onDragEnd?.Invoke(eventData);
//        }
//
//        public void AddClick(Action cb, bool reset = false)
//        {
//            if (onClick == null || reset)
//            {
//                onClick = cb;
//                return;
//            }
//
//            Delegate[] inlist = onClick.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//            
//            onClick += cb;
//        }
//        
//        public void AddPressHold(Action cb, bool reset = false)
//        {
//            if (onPressHold == null || reset)
//            {
//                onPressHold = cb;
//                return;
//            }
//
//            Delegate[] inlist = onPressHold.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//            
//            onPressHold += cb;
//        }
//        
//        public void AddChangeState(Action<bool> cb, bool reset = false)
//        {
//            if (onChangeState == null || reset)
//            {
//                onChangeState = cb;
//                return;
//            }
//
//            Delegate[] inlist = onChangeState.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//            
//            onChangeState += cb;
//        }
//
//        public void AddPointUp(Action<PointerEventData> cb, bool reset = false)
//        {
//            if (onPointUp == null || reset)
//            {
//                onPointUp = cb;
//                return;
//            }
//            Delegate[] inlist = onPointUp.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//
//            onPointUp += cb;
//        }
//
//        public void AddPointDown(Action<PointerEventData> cb, bool reset = false)
//        {
//            if (onPointDown == null || reset)
//            {
//                onPointDown = cb;
//                return;
//            }
//            Delegate[] inlist = onPointDown.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//
//            onPointDown += cb;
//        }
//
//        public void AddDragBegin(Action<PointerEventData> cb, bool reset = false)
//        {
//            if (onDragBegin == null || reset)
//            {
//                onDragBegin = cb;
//                return;
//            }
//            Delegate[] inlist = onDragBegin.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//
//            onDragBegin += cb;
//        }
//
//        public void AddDrag(Action<PointerEventData> cb, bool reset = false)
//        {
//            if (onDrag == null || reset)
//            {
//                onDrag = cb;
//                return;
//            }
//
//            Delegate[] inlist = onDrag.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//
//            onDrag += cb;
//        }
//
//        public void AddDragEnd(Action<PointerEventData> cb, bool reset = false)
//        {
//            if (onDragEnd == null || reset)
//            {
//                onDragEnd = cb;
//                return;
//            }
//
//            Delegate[] inlist = onDragEnd.GetInvocationList();
//            foreach (Delegate d in inlist)
//                if (d == (Delegate) cb) return;
//
//            onDragEnd += cb;
//        }
//
//        public void ExecuteClick()
//        {
//            onClick?.Invoke();
//        }
//
//        public void ExecuteDown()
//        {
//            ExecuteDown(Vector2.zero);
//        }
//
//        public void ExecuteDown(Vector2 position)
//        {
//            if (onPointDown != null)
//            {
//                var eventData = new PointerEventData(EventSystem.current)
//                {
//                    pointerPress = gameObject,
//                    position = position
//                };
//                onPointDown(eventData);
//            }
//        }
//
//        public void ExecuteUp()
//        {
//            ExecuteUp(Vector2.zero);
//        }
//
//        public void ExecuteUp(Vector2 position)
//        {
//            if (onPointUp != null)
//            {
//                var eventData = new PointerEventData(EventSystem.current)
//                {
//                    pointerPress = gameObject,
//                    position = position
//                };
//                onPointUp(eventData);
//            }
//        }
//
//        public void ExecuteHold()
//        {
//            onPressHold?.Invoke();
//        }
//
//        public void ExecuteBeginDrag()
//        {
//            ExecuteBeginDrag(Vector2.zero);
//        }
//
//        public void ExecuteBeginDrag(Vector2 position)
//        {
//            var eventData = new PointerEventData(EventSystem.current);
//            eventData.pointerPress = gameObject;
//            eventData.pointerDrag = gameObject;
//            eventData.position = position;
//
//            OnBeginDrag(eventData);
//        }
//
//        public void ExecuteDrag()
//        {
//            ExecuteDrag(Vector2.zero);
//        }
//
//        public void ExecuteDrag(Vector2 position)
//        {
//            var eventData = new PointerEventData(EventSystem.current);
//            eventData.pointerPress = gameObject;
//            eventData.pointerDrag = gameObject;
//            eventData.position = position;
//
//            OnDrag(eventData);
//        }
//
//        public void ExecuteEndDrag()
//        {
//            ExecuteEndDrag(Vector2.zero);
//        }
//
//        public void ExecuteEndDrag(Vector2 position)
//        {
//            var eventData = new PointerEventData(EventSystem.current);
//            eventData.pointerPress = gameObject;
//            eventData.pointerDrag = gameObject;
//            eventData.position = position;
//
//            OnEndDrag(eventData);
//        }
//
//        private void JoystickPointDown(PointerEventData data)
//        {
//            var cam = data.pressEventCamera ?? UIMgr.instance.UICamera;
//            Vector2 pos;
//            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Center.parent as RectTransform, data.position, cam, out pos))
//                return;
//            Background.anchoredPosition = pos;
//            Center.anchoredPosition = pos;
//            SetState(true);
//            lastDownTime = Time.time;
//            OnJoystickDown?.Invoke(pos);
//        }
//
//        private void JoystickDrag(PointerEventData data)
//        {
//            var cam = data.pressEventCamera ?? UIMgr.instance.UICamera;
//            Vector2 pos;
//            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Center.parent as RectTransform, data.position, cam, out pos))
//                return;
//            Vector2 link = pos - Background.anchoredPosition;
//            if (link.sqrMagnitude <= MaxDistance * MaxDistance)
//                Center.anchoredPosition = pos;
//            else
//                Center.anchoredPosition = Background.anchoredPosition + link.normalized * MaxDistance;
//
//            OnJoystickDrag?.Invoke(Center.anchoredPosition - Background.anchoredPosition);
//        }
//
//        private void JoystickPointUp(PointerEventData data)
//        {
//            if (!isStarted)
//                return;
//            float sq =(Center.anchoredPosition - Background.anchoredPosition).sqrMagnitude;
//            Background.anchoredPosition = backgroundFirstPos;
//            Center.anchoredPosition = centerFirstPos;
//            SetState(false);
//            OnJoystickUp?.Invoke(data.position);
//#if UNITY_EDITOR || UNITY_STANDALONE
//            if (Time.time - lastDownTime < CheckTimePC && sq > 100 && OnJoystickSlider != null && !isUsingJoystick)
//                OnJoystickSlider();
//#else
//            if (Time.time - lastDownTime < CheckTimeMobile && sq > 100 && OnJoystickSlider != null && !isUsingJoystick)
//                OnJoystickSlider();
//#endif    
//            isUsingJoystick = false;
//        }
//        
//        private Vector2 GetDir()
//        {
//            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
//                return new Vector2(100, 100);
//            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
//                return new Vector2(-100, 100);
//            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
//                return new Vector2(100, -100);
//            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
//                return new Vector2(-100, -100);
//            else if (Input.GetKey(KeyCode.W))
//                return new Vector2(0, 100);
//            else if (Input.GetKey(KeyCode.S))
//                return new Vector2(0, -100);
//            else if (Input.GetKey(KeyCode.A))
//                return new Vector2(-100, 0);
//            else if (Input.GetKey(KeyCode.D))
//                return new Vector2(100, 0);
//            else
//                return Vector2.zero;
//        }
//        
//        private Vector2 GetLeftJoystickDir()
//        {
//
//            if (Input.GetAxis("LeftJoystickVertical") ==1 && Input.GetAxis("LeftJoystickHorizontal")==1)
//                return new Vector2(100, 100);
//            else if (Input.GetAxis("LeftJoystickVertical") == 1 && Input.GetAxis("LeftJoystickHorizontal") == -1)
//                return new Vector2(-100, 100);
//            else if (Input.GetAxis("LeftJoystickVertical") == -1 && Input.GetAxis("LeftJoystickHorizontal") == 1)
//                return new Vector2(100, -100);
//            else if (Input.GetAxis("LeftJoystickVertical") == -1 && Input.GetAxis("LeftJoystickHorizontal") == -1)
//                return new Vector2(-100, -100);
//            else if (Input.GetAxis("LeftJoystickVertical") == 1)
//                return new Vector2(0, 100);
//            else if (Input.GetAxis("LeftJoystickVertical") == -1)
//                return new Vector2(0, -100);
//            else if (Input.GetAxis("LeftJoystickHorizontal")==-1)
//                return new Vector2(-100, 0);
//            else if (Input.GetAxis("LeftJoystickHorizontal") == 1)
//                return new Vector2(100, 0);
//            else
//                return Vector2.zero;
//        }
//        
//        bool IsJoystick()
//        {
//            if (GetLeftJoystickDir() != Vector2.zero)
//            {
//                isUsingJoystick = true;
//                return true;
//            }
//            return false;
//        }
//        
//        public void Update()
//        {
//            Vector2 dir;
//#if UNITY_EDITOR || UNITY_STANDALONE
//            //是否在使用手柄
//            dir = IsJoystick() ? GetLeftJoystickDir() : GetDir();
//#else
//            dir = GetLeftJoystickDir();
//#endif
//            //控制人物移动
//            if (dir != oldDir)
//            {
//                if (oldDir == Vector2.zero && dir != Vector2.zero)
//                {
//                    ExecuteDown(backgroundFirstPos);
//                    ExecuteBeginDrag(backgroundFirstPos);
//                    ExecuteDrag(backgroundFirstPos + dir);
//                }
//                else if (oldDir != Vector2.zero && dir == Vector2.zero)
//                {
//                    ExecuteUp(backgroundFirstPos + dir);
//                    ExecuteEndDrag(backgroundFirstPos + dir);
//                }                
//                else
//                {
//                    ExecuteDrag(backgroundFirstPos + dir);
//                }                
//                oldDir = dir;
//            }
//        }
//        
//        public void LateUpdate()
//        {
//            if(!isPressHoldThisTime &&isPressing&& Time.unscaledTime - lastPressTime > pressHoldTime)
//            {
//                isPressHoldThisTime = true;
//                onPressHold?.Invoke();
//            }
//        }
//        

        #endregion

        #region Method2

        public bool IsJoystickMoveable = true;
        public Vector2 OffectMin;
        public Vector2 OffectMax;
        public float CursorDisplayMaxRadius = 128f;
        public float CursorRespondMinRadius = 15f;
        
        private RectTransform axisRectTransform;
        private RectTransform cursorRectTransform;
        private RectTransform joystickRectTransform;
        private Vector2 originalPosition;
        private Vector2 targetPosition;
        private Vector2 currentPosition;
        private Vector2 axis;
        private Vector2 joystickSize;
        private Vector2 axisAnchorMin;
        private Vector2 axisAnchorMax;
        private Camera camera;
        
        public Vector2 WorldToScreenPoint(Camera camera, Vector3 worldPoint)
        {
            return (camera == null) ? new Vector2(worldPoint.x, worldPoint.y) : (Vector2)camera.WorldToScreenPoint(worldPoint);
        }
        
        public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 screenPoint, float z)
        {
            return (camera == null) ? new Vector3(screenPoint.x, screenPoint.y, z) : camera.ViewportToWorldPoint(new Vector3(screenPoint.x / Screen.width, screenPoint.y / Screen.height, z));
        }

        
        public void Initialize()
        {
            axisRectTransform = gameObject.transform.Find("Axis").GetComponent<RectTransform>();
            if (axisRectTransform != null)
            {
                axisRectTransform.anchoredPosition = Vector2.one;
                originalPosition = WorldToScreenPoint(camera, axisRectTransform.position);
                cursorRectTransform = axisRectTransform.Find("Cursor").GetComponent<RectTransform>();
                if (cursorRectTransform != null)
                {
                    cursorRectTransform.anchoredPosition = Vector2.zero;
                }
                //play fadeout
                axisAnchorMin = axisRectTransform.anchorMin;
                axisAnchorMax = axisRectTransform.anchorMax;
            }
            joystickRectTransform = gameObject.GetComponent<RectTransform>();
            if (joystickRectTransform != null)
            {
                joystickSize = joystickRectTransform.sizeDelta;
            }
        }

        public void OnDestroy()
        {
            axisRectTransform = null;
            cursorRectTransform = null;
        }

        private void Update()
        {
            if (IsJoystickMoveable)
            {
                this.UpdateAxisPosition();
            }
        }

        private void UpdateAxisPosition()
        {
            if (currentPosition != targetPosition)
            {
                Vector2 vector = targetPosition - currentPosition;
                Vector2 dis = (targetPosition - currentPosition) / 3f;
                if (vector.sqrMagnitude <= 1f)
                {
                    currentPosition = targetPosition;
                }
                else
                {
                    currentPosition += dis;
                }
                this.axisRectTransform.position = ScreenToWorldPoint(camera, currentPosition, axisRectTransform.position.z);
            }
        }

        private void MoveAxis(Vector2 position, bool isDown)
        {
            if (isDown || currentPosition == Vector2.zero && targetPosition == Vector2.zero)
            {
                currentPosition = position;
                targetPosition = currentPosition;
                axisRectTransform.position = ScreenToWorldPoint(camera, currentPosition, axisRectTransform.position.z);
            }
            Vector2 vector = position - currentPosition;
            float num = vector.magnitude;
            this.cursorRectTransform.anchoredPosition = ((num > CursorDisplayMaxRadius) ? (vector.normalized * this.CursorDisplayMaxRadius) : vector);
            if (this.IsJoystickMoveable && num > this.CursorDisplayMaxRadius)
            {
                this.targetPosition = this.currentPosition + (position - WorldToScreenPoint(camera, this.cursorRectTransform.position));
            }
            if (num < this.CursorRespondMinRadius)
            {
                this.UpdateAxis(Vector2.zero);
            }
            else
            {
                this.UpdateAxis(vector);
            }
        }
        
        private void UpdateAxis(Vector2 a)
        {
            if (!Equals(this.axis, a))
            {
                this.axis = a;
            }
            if (this.axis == Vector2.zero)
            {
//                this.HideBorder();
            }
            else
            {
//                this.ShowBorder(this.m_axis);
            }
        }
        
        public Vector2 GetAxis()
        {
            return this.axis;
        }
        
        public void ResetAxis()
        {
            this.axisRectTransform.anchoredPosition = Vector2.zero;
            this.cursorRectTransform.anchoredPosition = Vector2.zero;
            this.originalPosition = WorldToScreenPoint(this.m_belongedFormScript.GetCamera(), this.m_axisRectTransform.position);
            this.m_axisCurrentScreenPosition = Vector2.zero;
            this.m_axisTargetScreenPosition = Vector2.zero;
            this.UpdateAxis(Vector2.zero);
            this.AxisFadeout();
        }

        
        public void OnPointerDown(PointerEventData eventData)
        {
            this.MoveAxis(eventData.position, true);
//            this.AxisFadeIn();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            this.ResetAxis();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.MoveAxis(eventData.position, false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            this.MoveAxis(eventData.position, false);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
        }
        
        #endregion

    }
}