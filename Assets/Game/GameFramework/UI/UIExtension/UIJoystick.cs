using System;
using UnityEngine;

namespace GameFramework.UI.UIExtension
{
    public class UIJoystick : MonoBehaviour
    {
        #region Files

        public RectTransform background;
        public RectTransform center;
        public float maxDistance = 150f;
        public float distanceScale = 0.65f;
        public float verticalAngleScale = 0.8f;
        public float checkTimeMobile = 0.2f;
        public float checkTimePC = 0.1f;
        public Action<Vector2> onJoystickDown;
        public Action<Vector2> onJoystickDrag;
        public Action<Vector2> onJoystickUp;
        public Action onJoystickSlider;

        private bool isUsingJoystick = false;
        private bool isStarted = false;
        private Vector2 backgroundFirstPos;
        private Vector2 centerFirstPos;

        #endregion
    }
}