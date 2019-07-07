using com.ootii.Input;
using GameFramework.Base;
using GameFramework.Utility.Singleton;
using UnityEngine;

namespace GameFramework.Input
{
    [DisallowMultipleComponent]
    public class InputComponent : GameFrameworkComponent
    {
        public override int Priority => SingletonMono<GameFramework>.GetInstance().InputPriority;

        public override void OnAwake()
        {
            base.OnAwake();
            InputManager.UseGamepad = false;
            InputManager.Initialize();
        }
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            InputManager.Update();
        }
        public bool IsPressed(int nButtonID)
        {
            return InputManager.IsPressed(nButtonID);
        }

        public bool IsPressed(string strAlias)
        {
            return InputManager.IsPressed(strAlias);
        }

        public float MouseX()
        {
            return InputManager.MouseX;
        }

        public float MouseY()
        {
            return InputManager.MouseY;
        }

        public float MouseXDelta()
        {
            return InputManager.MouseXDelta;
        }

        public float MouseYDelta()
        {
            return InputManager.MouseYDelta;
        }

        public float MouseAxisX()
        {
            return InputManager.MouseAxisX;
        }

        public float MouseAxisY()
        {
            return InputManager.MouseAxisY;
        }

        public float MouseWheel()
        {
            return InputManager.GetValue(EnumInput.MOUSE_WHEEL);
        }

        public bool IsJustPressed(string strAlias)
        {
            return InputManager.IsJustPressed(strAlias);
        }

        public bool IsDoublePressed(string strAlias)
        {
            return InputManager.IsDoublePressed(strAlias);
        }

        public  bool IsJustReleased(string strAlias)
        {
            return InputManager.IsJustReleased(strAlias);
        }

        public  bool IsJustSingleReleased(string strAlias)
        {
            return InputManager.IsJustSingleReleased(strAlias);
        }

        public  bool IsJustDoubleReleased(string strAlias)
        {
            return InputManager.IsJustDoubleReleased(strAlias);
        }
    }
}

