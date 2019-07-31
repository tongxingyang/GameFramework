using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
//using com.ootii.Messages;

namespace com.ootii.Input
{
    /// <summary>
    /// Wraps game input so that we can process it
    /// before applying it to game objects.
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// Max number of keypresses we'll record at once
        /// </summary>
        public const int MAX_KEY_PRESSES = 10;

        /// <summary>
        /// Create the stub at startup and tie it into the Unity update path
        /// </summary>
#pragma warning disable 0414
        //private static InputManagerStub sStub = (new GameObject("InputManagerStub")).AddComponent<InputManagerStub>();
#pragma warning restore 0414

        /// <summary>
        /// Time alloted for a double-press to occur
        /// </summary>
        public static float DoublePressTime = 0.5f;

        /// <summary>
        /// Test if the mouse should be processed
        /// </summary>
        private static bool mUseMouse = true;
        public static bool UseMouse
        {
            get { return mUseMouse; }
            set { mUseMouse = value; }
        }

        /// <summary>
        /// Test if the gamepad should be processed
        /// </summary>
        private static bool mUseGamepad = true;
        public static bool UseGamepad
        {
            get { return mUseGamepad; }
            set { mUseGamepad = value; }
        }

        /// <summary>
        /// Test if the basic keyboard should be processed
        /// </summary>
        private static bool mUseKeyboardBasic = true;
        public static bool UseKeyboardBasic
        {
            get { return mUseKeyboardBasic; }
            set { mUseKeyboardBasic = value; }
        }

        /// <summary>
        /// Test if the keypad should be processed
        /// </summary>
        private static bool mUseKeyboardKeyPad = true;
        public static bool UseKeyboardKeyPad
        {
            get { return mUseKeyboardKeyPad; }
            set { mUseKeyboardKeyPad = value; }
        }

        /// <summary>
        /// Test if the keypad should be processed
        /// </summary>
        private static bool mUseKeyboardFKeys = true;
        public static bool UseKeyboardFKeys
        {
            get { return mUseKeyboardFKeys; }
            set { mUseKeyboardFKeys = value; }
        }

        /// <summary>
        /// Determines if we're inverting the view Y axis in
        /// response to input
        /// </summary>
        private static int mInvertViewY = 1;
        public static bool InvertViewY
        {
            get { return (mInvertViewY == -1); }
            set { mInvertViewY = (value ? -1 : 1); }
        }

        /// <summary>
        /// Determines if we fire of input events as keys are pressed
        /// </summary>
        private static bool mIsEventsEnabled = true;
        public static bool IsEventsEnabled
        {
            get { return mIsEventsEnabled; }
            set { mIsEventsEnabled = value; }
        }

        /// <summary>
        /// Returns the number of keys current pressed
        /// </summary>
        private static int mKeysPressedCount;
        public static int KeysPressedCount
        {
            get { return mKeysPressedCount; }
        }

        /// <summary>
        /// Returns a list of keys that are currently pressed
        /// </summary>
        public static int[] mKeysPressed = new int[InputManager.MAX_KEY_PRESSES];
        public static int[] KeysPressed
        {
            get { return mKeysPressed; }
        }

        /// <summary>
        /// Determines how quickly the mouse view movement updates
        /// </summary>
        private static float mMouseViewSensativity = 2f;
        public static float MouseViewSensativity
        {
            get { return mMouseViewSensativity; }
            set { mMouseViewSensativity = value; }
        }

        /// <summary>
        /// Tracks how long the update process is taking
        /// </summary>
        private static Stopwatch mWatch = new Stopwatch();
        private static long mUpdateElapsedTicks = 0;
        public static long UpdateElapsedTicks
        {
            get { return mUpdateElapsedTicks; }
        }

        /// <summary>
        /// Set by an external object, it tracks the angle of the
        /// user input compared to the camera's forward direction
        /// Note that this info isn't reliable as objects using it 
        /// before it's set it will get float.NaN.
        /// </summary>
        private static float mInputFromCameraAngle = 0f;
        public static float InputFromCameraAngle
        {
            get { return mInputFromCameraAngle; }
            set { mInputFromCameraAngle = value; }
        }

        /// <summary>
        /// Set by an external object, it tracks the angle of the
        /// user input compared to the avatars's forward direction
        /// Note that this info isn't reliable as objects using it 
        /// before it's set it will get float.NaN.
        /// </summary>
        private static float mInputFromAvatarAngle = 0f;
        public static float InputFromAvatarAngle
        {
            get { return mInputFromAvatarAngle; }
            set { mInputFromAvatarAngle = value; }
        }

        /// <summary>
        /// The input states
        /// </summary>
        private static InputState mState;
        private static InputState mPrevState;

        /// <summary>
        /// Smoothing values
        /// </summary>
        private static float mOldViewX = 0f;
        private static float mOldViewY = 0f;

        /// <summary>
        /// Used to help manage the vsync issue. With
        /// vsync off, the mouse values only occur every
        /// 0.016 seconds instead of every frame.
        /// </summary>
        private static float mTargetMouseX = 0f;
        private static float mTargetMouseY = 0f;
        private static float mVSyncTimer = 0;

        /// <summary>
        /// Contains all the alias structures assigned to the alias string
        /// </summary>
        private static Dictionary<string, List<InputAlias>> mAliases;

        /// <summary>
        /// Contains all the alias structures that use the primary control
        /// </summary>
        private static Dictionary<int, List<InputAlias>> mPrimaryAliases;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public static void Initialize()
        {
            mAliases = new Dictionary<string, List<InputAlias>>();
            mPrimaryAliases = new Dictionary<int, List<InputAlias>>();

            AddAlias("_MoveLeftKey", EnumInput.A);
            AddAlias("_MoveRightKey", EnumInput.D);
            AddAlias("_MoveUpKey", EnumInput.W);
            AddAlias("_MoveDownKey", EnumInput.S);
            AddAlias("_MoveHorizontal", EnumInput.GAMEPAD_LEFT_STICK_X);
            AddAlias("_MoveVertical", EnumInput.GAMEPAD_LEFT_STICK_Y);
            AddAlias("_ViewHorizontal", EnumInput.GAMEPAD_RIGHT_STICK_X);
            AddAlias("_ViewVertical", EnumInput.GAMEPAD_RIGHT_STICK_Y);
            AddAlias("_MouseViewEnable", EnumInput.MOUSE_RIGHT_BUTTON);

            mState = new InputState();
            mState.Initialize();

            mPrevState = new InputState();
            mPrevState.Initialize();
        }

        #region Alias Setting

        /// <summary>
        /// Adds an alies to simplify key states. If the alias already
        /// exists, we replace the ID with the new one.
        /// </summary>
        /// <param name="rName">Name of the alias</param>
        /// <param name="rID">Input ID to add</param>
        public static void AddAlias(string rName, int rID)
        {
            AddAlias(rName, rID, 0);
        }

        /// <summary>
        /// Adds an alias to simplify key states. If the alias already
        /// exists, we replace the ID with the new one.
        /// </summary>
        /// <param name="rName">Name of the alias</param>
        /// <param name="rPrimaryID">Input ID</param>
        /// <param name="rSupportID">Required support input ID</param>
        public static void AddAlias(string rName, int rPrimaryID, int rSupportID)
        {
            if (mAliases.ContainsKey(rName))
            {
                List<InputAlias> lAliases = mAliases[rName];
                for (int i = lAliases.Count - 1; i >= 0; i--)
                {
                    if (lAliases[i].PrimaryID == rPrimaryID && lAliases[i].SupportID == rSupportID) { return; }
                }
            }
            else
            {
                List<InputAlias> lValues = new List<InputAlias>();
                mAliases.Add(rName, lValues);
            }

            // At this point, we know we have an alias list for the name and need to add the alias
            InputAlias lAlias = new InputAlias(rName, rPrimaryID, rSupportID);
            mAliases[rName].Add(lAlias);

            // Ensure a primary entry exists for the alias
            AddPrimaryAlias(lAlias);
        }

        /// <summary>
        /// Adds an alias to simplify testing for input states. In this this case,
        /// a callback function is used that actually does the test.
        /// </summary>
        /// <param name="rName">Name of the alias</param>
        /// <param name="rCustomTest>Function that will perform the test</param>
        public static void AddAlias(string rName, CustomTestFunction rCustomTest)
        {
            if (mAliases.ContainsKey(rName))
            {
                List<InputAlias> lAliases = mAliases[rName];
                for (int i = lAliases.Count - 1; i >= 0; i--)
                {
                    if (lAliases[i].CustomTest == rCustomTest) { return; }
                }
            }
            else
            {
                List<InputAlias> lValues = new List<InputAlias>();
                mAliases.Add(rName, lValues);
            }

            // At this point, we know we have an alias list for the name and need to add the alias
            InputAlias lAlias = new InputAlias(rName, rCustomTest);
            mAliases[rName].Add(lAlias);
        }

        /// <summary>
        /// Removes the alias.
        /// </summary>
        /// <param name="rName">Alias name</param>
        public static void RemoveAlias(string rName)
        {
            if (mAliases.ContainsKey(rName))
            {
                // Remove the primary aliases matching the aliases
                List<InputAlias> lAliases = mAliases[rName];
                for (int i = lAliases.Count - 1; i >= 0; i--)
                {
                    RemovePrimaryAlias(lAliases[i]);
                }

                // Remove the aliases
                mAliases.Remove(rName);
            }
        }

        /// <summary>
        /// Removes all aliases that match the name and have the
        /// same primary control.
        /// </summary>
        /// <param name="rName">Alias name</param>
        /// <param name="rID">Specific ID to remove</param>
        public static void RemoveAlias(string rName, int rID)
        {
            if (mAliases.ContainsKey(rName))
            {
                List<InputAlias> lAliases = mAliases[rName];
                for (int i = lAliases.Count - 1; i >= 0; i--)
                {
                    if (lAliases[i].PrimaryID == rID)
                    {
                        RemovePrimaryAlias(lAliases[i]);
                        lAliases.RemoveAt(i);
                    }
                }

                if (lAliases.Count == 0)
                {
                    mAliases.Remove(rName);
                }
            }
        }

        /// <summary>
        /// Removes the alias.
        /// </summary>
        /// <param name="rName">Alias name</param>
        /// <param name="rPrimaryID">Specific ID to remove</param>
        /// <param name="rSupportID">Support input ID that must match as well</param>
        public static void RemoveAlias(string rName, int rPrimaryID, int rSupportID)
        {
            if (mAliases.ContainsKey(rName))
            {
                List<InputAlias> lAliases = mAliases[rName];
                for (int i = lAliases.Count - 1; i >= 0; i--)
                {
                    if (lAliases[i].PrimaryID == rPrimaryID && lAliases[i].SupportID == rSupportID)
                    {
                        RemovePrimaryAlias(lAliases[i]);
                        lAliases.RemoveAt(i);
                    }
                }

                if (lAliases.Count == 0)
                {
                    mAliases.Remove(rName);
                }
            }
        }

        /// <summary>
        /// Removes the alias.
        /// </summary>
        /// <param name="rName">Alias name</param>
        /// <param name="rInputFunction">Function used to test the input value</param>
        public static void RemoveAlias(string rName, CustomTestFunction rCustomTest)
        {
            if (mAliases.ContainsKey(rName))
            {
                List<InputAlias> lAliases = mAliases[rName];
                for (int i = lAliases.Count - 1; i >= 0; i--)
                {
                    if (lAliases[i].CustomTest == rCustomTest)
                    {
                        lAliases.RemoveAt(i);
                    }
                }

                if (lAliases.Count == 0)
                {
                    mAliases.Remove(rName);
                }
            }
        }

        /// <summary>
        /// Ensure a matching alias exists for the primary value
        /// </summary>
        /// <param name="lAlias">Alias we're testing for</param>
        private static void AddPrimaryAlias(InputAlias rAlias)
        {
            // Now, we may need to add a list of aliases to the list of primaries
            if (!mPrimaryAliases.ContainsKey(rAlias.PrimaryID))
            {
                List<InputAlias> lValues = new List<InputAlias>();
                mPrimaryAliases.Add(rAlias.PrimaryID, lValues);
            }

            // Test if this alias exists in the list of primaries
            List<InputAlias> lPrimaryAliases = mPrimaryAliases[rAlias.PrimaryID];
            for (int i = lPrimaryAliases.Count - 1; i >= 0; i--)
            {
                if (lPrimaryAliases[i].PrimaryID == rAlias.PrimaryID && lPrimaryAliases[i].SupportID == rAlias.SupportID) { return; }
            }

            // If we got here, we need to add the alias
            lPrimaryAliases.Add(rAlias);
        }

        /// <summary>
        /// Remove the alias with the values from the list
        /// </summary>
        /// <param name="rAlias"></param>
        private static void RemovePrimaryAlias(InputAlias rAlias)
        {
            List<InputAlias> lPrimaryAliases = mPrimaryAliases[rAlias.PrimaryID];
            for (int i = lPrimaryAliases.Count - 1; i >= 0; i--)
            {
                if (lPrimaryAliases[i].Name == rAlias.Name &&
                    lPrimaryAliases[i].PrimaryID == rAlias.PrimaryID && 
                    lPrimaryAliases[i].SupportID == rAlias.SupportID) 
                {
                    lPrimaryAliases.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Custom state retrieval

        /// <summary>
        /// Speed of movement in the range of -1 (full backwards) to 1 (full forward)
        /// </summary>
        /// <value>The speed in the range of -1 to 1</value>
        public static float MovementMagnitude
        {
            get
            {
                float lMovementX = MovementX;
                float lMovementY = MovementY;
                return Mathf.Sqrt((lMovementX * lMovementX) + (lMovementY * lMovementY));
            }
        }

        /// <summary>
        /// Horizontal movement in the range of -1 (left) to 1 (right)
        /// </summary>
        /// <value>The movement in the range of -1 to 1</value>
        public static float MovementX
        {
            get
            {
                float lMovement = GetValue("_MoveHorizontal");
                if (lMovement != 0f) { return lMovement; }

                float lMin = GetValue("_MoveLeftKey");
                float lMax = GetValue("_MoveRightKey");
                return (lMin * -1) + lMax;
            }
        }

        /// <summary>
        /// Vertical movement in the range of -1 (down) to 1 (up)
        /// </summary>
        /// <value>The movement in the range of -1 to 1</value>
        public static float MovementY
        {
            get
            {
                float lMovement = GetValue("_MoveVertical");
                if (lMovement != 0f) { return lMovement; }

                float lMin = GetValue("_MoveUpKey");
                float lMax = GetValue("_MoveDownKey");
                return lMin + (lMax * -1);
            }
        }

        /// <summary>
        /// Horizontal view change in the range of -1 (left) to 1 (right)
        /// </summary>
        /// <value>The view in the range of -1 to 1</value>
        public static float ViewX
        {
            get
            {
                float lView = GetValue("_ViewHorizontal");
                if (lView != 0f) { return lView; }

                bool lEnabled = (!mAliases.ContainsKey("_MouseViewEnable") || IsPressed("_MouseViewEnable"));
                if (lEnabled)
                {
                    lView = mState.Controls[EnumInput.MOUSE_AXIS_X].Value * mMouseViewSensativity;
                    if (lView < -mMouseViewSensativity) { lView = -mMouseViewSensativity; }
                    else if (lView > mMouseViewSensativity) { lView = mMouseViewSensativity; }

                    lView = Mathf.Lerp(mOldViewX, lView, 0.3f);
                    mOldViewX = lView;
                }

                return lView;
            }
        }

        /// <summary>
        /// Vertical view change in the range of 1 (top) to -1 (bottom)
        /// </summary>
        /// <value>The view in the range of -1 to 1</value>
        public static float ViewY
        {
            get
            {
                float lView = GetValue("_ViewVertical");
                if (lView != 0f) { return lView; }

                bool lEnabled = (!mAliases.ContainsKey("_MouseViewEnable") || IsPressed("_MouseViewEnable"));
                if (lEnabled)
                {
                    lView = mState.Controls[EnumInput.MOUSE_AXIS_Y].Value * mMouseViewSensativity;
                    if (lView < -mMouseViewSensativity) { lView = -mMouseViewSensativity; }
                    else if (lView > mMouseViewSensativity) { lView = mMouseViewSensativity; }

                    lView = Mathf.Lerp(mOldViewY, lView, 0.3f);
                    mOldViewY = lView;
                }

                return lView * mInvertViewY;
            }
        }

        #endregion

        #region Mouse state retrieval

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        /// <value>The mouse x.</value>
        public static float MouseX
        {
            get { return mState.Controls[EnumInput.MOUSE_X].Value; }
        }

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        /// <value>The mouse y.</value>
        public static float MouseY
        {
            get { return mState.Controls[EnumInput.MOUSE_Y].Value; }
        }

        /// <summary>
        /// Change in position of the mouse
        /// </summary>
        /// <value>The mouse x.</value>
        public static float MouseXDelta
        {
            get { return mState.Controls[EnumInput.MOUSE_X].Value - mPrevState.Controls[EnumInput.MOUSE_X].Value; }
        }

        /// <summary>
        /// Change in position of the mouse
        /// </summary>
        /// <value>The mouse y.</value>
        public static float MouseYDelta
        {
            get { return mState.Controls[EnumInput.MOUSE_Y].Value - mPrevState.Controls[EnumInput.MOUSE_Y].Value; }
        }

        /// <summary>
        /// Input value of the mouse by unity when treated as an axis
        /// </summary>
        /// <value>The mouse x.</value>
        public static float MouseAxisX
        {
            get { return mState.Controls[EnumInput.MOUSE_AXIS_X].Value; }
        }

        /// <summary>
        /// Input value of the mouse by unity when treated as an axis
        /// </summary>
        /// <value>The mouse y.</value>
        public static float MouseAxisY
        {
            get { return mState.Controls[EnumInput.MOUSE_AXIS_Y].Value; }
        }

        #endregion

        #region Gamepad state retrieval

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        /// <value>The mouse x.</value>
        public static float LeftStickX
        {
            get { return mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_X].Value; }
        }

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        /// <value>The mouse y.</value>
        public static float LeftStickY
        {
            get { return mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_Y].Value; }
        }

        /// <summary>
        /// Determines if the stick is currently being moved
        /// </summary>
        /// <value><c>true</c> if is L stick is moved; otherwise, <c>false</c>.</value>
        public static bool IsLeftStickActive
        {
            get { return mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_X].IsPressed || mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_Y].IsPressed; }
        }

        /// <summary>
        /// Gets the L stick magnitude.
        /// </summary>
        /// <value>The L stick magnitude.</value>
        public static float LeftStickMagnitude
        {
            get { return Mathf.Sqrt((mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_X].Value * mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_X].Value) + (mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_Y].Value * mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_Y].Value)); }
        }

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        /// <value>The mouse x.</value>
        public static float RightStickX
        {
            get { return mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_X].Value; }
        }

        /// <summary>
        /// Current position of the mouse
        /// </summary>
        /// <value>The mouse y.</value>
        public static float RightStickY
        {
            get { return mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_Y].Value; }
        }

        /// <summary>
        /// Determines if the stick is currently being moved
        /// </summary>
        /// <value><c>true</c> if is R stick is moved; otherwise, <c>false</c>.</value>
        public static bool IsRightStickActive
        {
            get { return mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_X].IsPressed || mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_Y].IsPressed; }
        }

        /// <summary>
        /// Gets the L stick magnitude.
        /// </summary>
        /// <value>The L stick magnitude.</value>
        public static float RightStickMagnitude
        {
            get { return Mathf.Sqrt((mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_X].Value * mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_X].Value) + (mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_Y].Value * mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_Y].Value)); }
        }

        /// <summary>
        /// Get the value of the left trigger
        /// </summary>
        /// <value>The left trigger value.</value>
        public static float LeftTrigger
        {
            get { return mState.Controls[EnumInput.GAMEPAD_LEFT_TRIGGER].Value; }
        }

        /// <summary>
        /// Get the value of the right trigger
        /// </summary>
        /// <value>The right trigger value.</value>
        public static float RightTrigger
        {
            get { return mState.Controls[EnumInput.GAMEPAD_RIGHT_TRIGGER].Value; }
        }

        #endregion

        #region Control state retrieval

        /// <summary>
        /// Value of the control
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="rButtonID">Button ID to test</param>
        public static float GetValue(int rButtonID)
        {
            return mState.Controls[rButtonID].Value;
        }

        /// <summary>
        /// Value of the control
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="rAlias">Button ID to test</param>
        public static float GetValue(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return 0; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                int lPrimaryID = 0;
                float lPrimaryValue = 0;

                if (lInputAlias[i].CustomTest != null)
                {
                    lPrimaryValue = lInputAlias[i].CustomTest();
                    if (lPrimaryValue != 0) { return lPrimaryValue; }
                }
                else
                {
                    lPrimaryID = lInputAlias[i].PrimaryID;
                    lPrimaryValue = mState.Controls[lPrimaryID].Value;

                    // If there is a support associated with the alias, check it.
                    // If it is active and the primary is active, we're done
                    if (lInputAlias[i].SupportID > 0)
                    {
                        if (mState.Controls[lInputAlias[i].SupportID].Value != 0)
                        {
                            if (lPrimaryValue != 0) { return lPrimaryValue; };
                        }
                    }
                    // If we get here, we have a lone primary
                    else if (lPrimaryValue != 0)
                    {
                        // If the lone primary is active, we need to see if there are 
                        // any other aliases that use this primary where the secondary
                        // is active. In this way, 'X' won't fire if 'X+Shift' is active.
                        List<InputAlias> lPrimaryAliases = mPrimaryAliases[lPrimaryID];
                        for (int j = lPrimaryAliases.Count - 1; j >= 0; j--)
                        {
                            // We know this primary is active, test the secondary
                            if (lPrimaryAliases[j].SupportID > 0)
                            {
                                // If the secondary for this other alias is active, our lone primary can't be
                                if (mState.Controls[lPrimaryAliases[j].SupportID].Value != 0)
                                {
                                    return 0;
                                }
                            }
                        }

                        // Since no other alias has a support active for this primary, we're good
                        return lPrimaryValue;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Determines if the specified button/key is pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to test</param>
        public static bool IsPressed(int rButtonID)
        {
            return mState.Controls[rButtonID].IsPressed;
        }

        /// <summary>
        /// Determines if the specified button/key is pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static bool IsPressed(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                int lPrimaryID = 0;
                bool lPrimaryValue = false;

                if (lInputAlias[i].CustomTest != null)
                {
                    lPrimaryValue = (lInputAlias[i].CustomTest() == 1);
                    if (lPrimaryValue) { return lPrimaryValue; }
                }
                else
                {
                    lPrimaryID = lInputAlias[i].PrimaryID;
                    lPrimaryValue = mState.Controls[lPrimaryID].IsPressed;
                    UnityEngine.Debug.Log("IsPressed PrimaryID:" + lPrimaryID + " Value:" + mState.Controls[lPrimaryID].Value + " Pressed:" + mState.Controls[lPrimaryID].IsPressed);

                    // If there is a support associated with the alias, check it.
                    // If it is active and the primary is active, we're done
                    if (lInputAlias[i].SupportID > 0)
                    {
                        if (mState.Controls[lInputAlias[i].SupportID].IsPressed)
                        {
                            if (lPrimaryValue) { return true; };
                        }
                    }
                    // If we get here, we have a lone primary
                    else if (lPrimaryValue)
                    {
                        // If the lone primary is active, we need to see if there are 
                        // any other aliases that use this primary where the secondary
                        // is active. In this way, 'X' won't fire if 'X+Shift' is active.
                        List<InputAlias> lPrimaryAliases = mPrimaryAliases[lPrimaryID];
                        for (int j = lPrimaryAliases.Count - 1; j >= 0; j--)
                        {
                            // We know this primary is active, test the secondary
                            if (lPrimaryAliases[j].SupportID > 0)
                            {
                                // If the secondary for this other alias is active, our lone primary can't be
                                if (mState.Controls[lPrimaryAliases[j].SupportID].IsPressed)
                                {
                                    return false;
                                }
                            }
                        }

                        // Since no other alias has a support active for this primary, we're good
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if the button is double pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static bool IsDoublePressed(int rButtonID)
        {
            return mState.Controls[rButtonID].IsDoublePressed;
        }

        /// <summary>
        /// Tests if the button is double pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static bool IsDoublePressed(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                int lPrimaryID = lInputAlias[i].PrimaryID;
                bool lPrimaryValue = mState.Controls[lPrimaryID].IsDoublePressed;

                // If there is a support associated with the alias, check it.
                // If it is active and the primary is active, we're done
                if (lInputAlias[i].SupportID > 0)
                {
                    if (mState.Controls[lInputAlias[i].SupportID].IsPressed)
                    {
                        if (lPrimaryValue) { return true; };
                    }
                }
                // If we get here, we have a lone primary
                else if (lPrimaryValue)
                {
                    // If the lone primary is active, we need to see if there are 
                    // any other aliases that use this primary where the secondary
                    // is active. In this way, 'X' won't fire if 'X+Shift' is active.
                    List<InputAlias> lPrimaryAliases = mPrimaryAliases[lPrimaryID];
                    for (int j = lPrimaryAliases.Count - 1; j >= 0; j--)
                    {
                        // We know this primary is active, test the secondary
                        if (lPrimaryAliases[j].SupportID > 0)
                        {
                            // If the secondary for this other alias is active, our lone primary can't be
                            if (mState.Controls[lPrimaryAliases[j].SupportID].IsPressed)
                            {
                                return false;
                            }
                        }
                    }

                    // Since no other alias has a support active for this primary, we're good
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if the button was double pressed and is just now released on the second press. This is
        /// what should be used when testing for different click actions (ie IsJustSingleReleased, IsJustDoubleReleased)
        /// </summary>
        /// <returns><c>true</c> if the specified rButtonID; is released after a double press otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static bool IsJustDoubleReleased(int rButtonID)
        {
            return (mPrevState.Controls[rButtonID].IsDoublePressed && !mState.Controls[rButtonID].IsPressed);
        }

        /// <summary>
        /// Tests if the button was double pressed and is just now released on the second press. This is
        /// what should be used when testing for different click actions (ie IsJustSingleReleased, IsJustDoubleReleased)
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static bool IsJustDoubleReleased(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                int lPrimaryID = lInputAlias[i].PrimaryID;
                int lSupportID = lInputAlias[i].SupportID;

                bool lCurrentPrimaryValue = mState.Controls[lPrimaryID].IsPressed;
                bool lCurrentSupportValue = (lSupportID == 0 ? false : mState.Controls[lSupportID].IsPressed);

                bool lPrevPrimaryValue = mPrevState.Controls[lPrimaryID].IsDoublePressed;
                bool lPrevSupportValue = (lSupportID == 0 ? true : mPrevState.Controls[lSupportID].IsPressed);

                // We can only be on a DoubleReleased if we come from a DoublePressed and are currently released
                if ((lPrevPrimaryValue && lPrevSupportValue) && (!lCurrentPrimaryValue && !lCurrentSupportValue))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if the button is currently toggled. The toggle flips each time the
        /// button is released. It starts off as 'off', switches to 'on' on the
        /// first press, switches to 'off' on the second press, etc.
        /// </summary>
        /// <returns><c>true</c> if the specified rButtonID is toggled. Otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static bool IsToggled(int rButtonID)
        {
            return mState.Controls[rButtonID].IsToggled;
        }

        /// <summary>
        /// Tests if the button is currently toggled. The toggle flips each time the
        /// button is released. It starts off as 'off', switches to 'on' on the
        /// first press, switches to 'off' on the second press, etc.
        /// </summary>
        /// <returns><c>true</c> if the specified alias is toggled. Otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Alias to be tested.</param>
        public static bool IsToggled(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                if (lInputAlias[i].PrimaryID > 0)
                {
                    // Check the primary
                    if (mState.Controls[lInputAlias[i].PrimaryID].IsToggled)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified button/key is pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static bool IsJustPressed(int rButtonID)
        {
            return mState.Controls[rButtonID].IsPressed && !mPrevState.Controls[rButtonID].IsPressed;
        }

        /// <summary>
        /// Determines if the specified button/key is pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static bool IsJustPressed(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                bool lWasPressed = mPrevState.Controls[lInputAlias[i].PrimaryID].IsPressed;
                if (lWasPressed && lInputAlias[i].SupportID > 0) { lWasPressed = mPrevState.Controls[lInputAlias[i].SupportID].IsPressed; }

                // Get out early if we can
                if (lWasPressed) { continue; }

                // Test the current pressed state
                int lPrimaryID = lInputAlias[i].PrimaryID;
                bool lPrimaryValue = mState.Controls[lPrimaryID].IsPressed;

                // If there is a support associated with the alias, check it.
                // If it is active and the primary is active, we're done
                if (lInputAlias[i].SupportID > 0)
                {
                    if (mState.Controls[lInputAlias[i].SupportID].IsPressed)
                    {
                        if (lPrimaryValue) { return true; };
                    }
                }
                // If we get here, we have a lone primary
                else if (lPrimaryValue)
                {
                    // If the lone primary is active, we need to see if there are 
                    // any other aliases that use this primary where the secondary
                    // is active. In this way, 'X' won't fire if 'X+Shift' is active.
                    List<InputAlias> lPrimaryAliases = mPrimaryAliases[lPrimaryID];
                    for (int j = lPrimaryAliases.Count - 1; j >= 0; j--)
                    {
                        // We know this primary is active, test the secondary
                        if (lPrimaryAliases[j].SupportID > 0)
                        {
                            // If the secondary for this other alias is active, our lone primary can't be
                            if (mState.Controls[lPrimaryAliases[j].SupportID].IsPressed)
                            {
                                return false;
                            }
                        }
                    }

                    // Since no other alias has a support active for this primary, we're good
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the specified button/key is pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static bool IsJustReleased(int rButtonID)
        {
            return !mState.Controls[rButtonID].IsPressed && mPrevState.Controls[rButtonID].IsPressed;
        }

        /// <summary>
        /// Determines if the specified button/key is pressed
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static bool IsJustReleased(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                bool lWasPressed = mPrevState.Controls[lInputAlias[i].PrimaryID].IsPressed;
                if (lWasPressed && lInputAlias[i].SupportID > 0) { lWasPressed = mPrevState.Controls[lInputAlias[i].SupportID].IsPressed; }

                // Get out early if we can
                if (!lWasPressed) { continue; }

                // If there is a support associated with the alias, check it
                // first. If it is not active, the primary can't be active.
                if (lInputAlias[i].SupportID > 0)
                {
                    if (!mState.Controls[lInputAlias[i].SupportID].IsPressed) 
                    { 
                        return true; 
                    }
                }

                // Check the primary
                if (!mState.Controls[lInputAlias[i].PrimaryID].IsPressed)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if the button was pressed and is just now released. However, there is a delay to ensure we're not
        /// in the middle of a double-press. This is what should be used when testing for different click types
        /// (ie IsJustSingleReleased, IsJustDoubleReleased)
        /// </summary>
        /// <returns><c>true</c> if the specified rButtonID; is released after a double press otherwise, <c>false</c>.</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static bool IsJustSingleReleased(int rButtonID)
        {
            if (mState.Controls[rButtonID].TimePressed == 0) { return false; }
            if (mState.Controls[rButtonID].IsPressed) { return false; }
            if (mState.Controls[rButtonID].IsDoubleReleased) { return false; }
            if (mPrevState.Controls[rButtonID].IsPressed) { return false; }

            // Now we want to check the duration of the release. When it crosses over the DoublePressTime, we can flag the release
            float lLastFrameReleasedDuration = (Time.time - Time.deltaTime) - mState.Controls[rButtonID].TimeReleased;
            float lThisFrameReleasedDuration = Time.time - mState.Controls[rButtonID].TimeReleased;
            if ((lLastFrameReleasedDuration <= DoublePressTime) && (lThisFrameReleasedDuration >= DoublePressTime))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tests if the button was pressed and is just now released. However, there is a delay to ensure we're not
        /// in the middle of a double-press. This is what should be used when testing for different click types
        /// (ie IsJustSingleReleased, IsJustDoubleReleased)
        /// </summary>
        /// <returns><c>true</c> if is pressed the specified rButtonID; otherwise, <c>false</c>.</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static bool IsJustSingleReleased(string rAlias)
        {
            if (!mAliases.ContainsKey(rAlias)) { return false; }

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                int lPrimaryID = lInputAlias[i].PrimaryID;
                int lSupportID = lInputAlias[i].SupportID;

                bool lPrevPrimaryValue = mPrevState.Controls[lPrimaryID].IsPressed;
                bool lPrevSupportValue = (lSupportID == 0 ? false : mPrevState.Controls[lSupportID].IsPressed);

                bool lCurrentPrimaryValue = mState.Controls[lPrimaryID].IsPressed;
                bool lCurrentSupportValue = (lSupportID == 0 ? false : mState.Controls[lSupportID].IsPressed);

                if (mState.Controls[lPrimaryID].TimePressed == 0) { continue; }
                if (mState.Controls[lPrimaryID].IsDoubleReleased) { continue; }
                if (lCurrentPrimaryValue && lCurrentSupportValue) { continue; }
                if (lPrevPrimaryValue && lPrevSupportValue) { continue; }

                // Now we want to check the duration of the release. When it crosses over the DoublePressTime, we can flag the release
                float lLastFrameReleasedDuration = (Time.time - Time.deltaTime) - mState.Controls[lPrimaryID].TimeReleased;
                float lThisFrameReleasedDuration = Time.time - mState.Controls[lPrimaryID].TimeReleased;
                if ((lLastFrameReleasedDuration <= DoublePressTime) && (lThisFrameReleasedDuration >= DoublePressTime))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves the time (in game play seconds) when the
        /// button/key was last pressed
        /// </summary>
        /// <returns>Time the button was pressed</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static float PressedTime(int rButtonID)
        {
            if (!mState.Controls[rButtonID].IsPressed) { return 0; }
            return mState.Controls[rButtonID].TimePressed;
        }

        /// <summary>
        /// Retrieves the time (in game play seconds) when the
        /// button/key was last pressed
        /// </summary>
        /// <returns>Time the button was pressed</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static float PressedTime(string rAlias)
        {
            float lMinTime = float.MaxValue;

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                int lPrimaryID = lInputAlias[i].PrimaryID;

                if (mState.Controls[lPrimaryID].IsPressed)
                {
                    bool lIsPressed = true;

                    // If we're dealing with a lone primary, we need to ensure no other alias is active
                    if (lInputAlias[i].SupportID == 0)
                    {
                        // If the lone primary is active, we need to see if there are 
                        // any other aliases that use this primary where the secondary
                        // is active. In this way, 'X' won't fire if 'X+Shift' is active.
                        List<InputAlias> lPrimaryAliases = mPrimaryAliases[lPrimaryID];
                        for (int j = lPrimaryAliases.Count - 1; j >= 0; j--)
                        {
                            // We know this primary is active, test the secondary
                            if (lPrimaryAliases[j].SupportID > 0)
                            {
                                // If the secondary for this other alias is active, our lone primary can't be
                                if (mState.Controls[lPrimaryAliases[j].SupportID].IsPressed)
                                {
                                    lIsPressed = false;
                                    continue;
                                }
                            }
                        }
                    }
                    // If we have a support, the support must be active
                    else if (!mState.Controls[lInputAlias[i].SupportID].IsPressed)
                    {
                        lIsPressed = false;
                    }

                    // Grab the lowest time
                    if (lIsPressed)
                    {
                        float lTime = mState.Controls[lPrimaryID].TimePressed;
                        if (lTime < lMinTime) { lMinTime = lTime; }
                    }
                }
            }

            if (lMinTime == float.MaxValue) { lMinTime = 0; }
            return lMinTime;
        }

        /// <summary>
        /// Retrieves the time (in seconds) the button/key has been pressed for
        /// </summary>
        /// <returns>Time the button has been pressed for</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static float PressedElapsedTime(int rButtonID)
        {
            if (!mState.Controls[rButtonID].IsPressed) { return 0; }
            return Time.time - mState.Controls[rButtonID].TimePressed;
        }

        /// <summary>
        /// Retrieves the time (in seconds) the button/key has been pressed for
        /// </summary>
        /// <returns>Time the button has been pressed for</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static float PressedElapsedTime(string rAlias)
        {
            float lMinTime = float.MaxValue;

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                int lPrimaryID = lInputAlias[i].PrimaryID;

                if (mState.Controls[lPrimaryID].IsPressed)
                {
                    bool lIsPressed = true;

                    // If we're dealing with a lone primary, we need to ensure no other alias is active
                    if (lInputAlias[i].SupportID == 0)
                    {
                        // If the lone primary is active, we need to see if there are 
                        // any other aliases that use this primary where the secondary
                        // is active. In this way, 'X' won't fire if 'X+Shift' is active.
                        List<InputAlias> lPrimaryAliases = mPrimaryAliases[lPrimaryID];
                        for (int j = lPrimaryAliases.Count - 1; j >= 0; j--)
                        {
                            // We know this primary is active, test the secondary
                            if (lPrimaryAliases[j].SupportID > 0)
                            {
                                // If the secondary for this other alias is active, our lone primary can't be
                                if (mState.Controls[lPrimaryAliases[j].SupportID].IsPressed)
                                {
                                    lIsPressed = false;
                                    continue;
                                }
                            }
                        }
                    }
                    // If we have a support, the support must be active
                    else if (!mState.Controls[lInputAlias[i].SupportID].IsPressed)
                    {
                        lIsPressed = false;
                    }

                    // Grab the lowest time
                    if (lIsPressed)
                    {
                        float lTime = mState.Controls[lPrimaryID].TimePressed;
                        if (lTime < lMinTime) { lMinTime = lTime; }
                    }
                }
            }

            if (lMinTime == float.MaxValue)
            {
                return 0;
            }
            else
            {
                return Time.time - lMinTime;
            }
        }

        /// <summary>
        /// Retrieves the time (in game play seconds) when the
        /// button/key was last released
        /// </summary>
        /// <returns>Time the button was pressed</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static float ReleasedTime(int rButtonID)
        {
            if (mState.Controls[rButtonID].IsPressed) { return 0; }
            return mState.Controls[rButtonID].TimeReleased;
        }

        /// <summary>
        /// Retrieves the time (in game play seconds) when the
        /// button/key was last released
        /// </summary>
        /// <returns>Time the button was pressed</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static float ReleasedTime(string rAlias)
        {
            float lMaxTime = float.MinValue;

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                if (lInputAlias[i].SupportID <= 0 && mState.Controls[lInputAlias[i].PrimaryID].IsPressed) { return 0; }
                if (mState.Controls[lInputAlias[i].PrimaryID].IsPressed && mState.Controls[lInputAlias[i].SupportID].IsPressed) { return 0; }

                float lTime = mState.Controls[lInputAlias[i].PrimaryID].TimeReleased;
                if (lTime > lMaxTime) { lMaxTime = lTime; }
            }

            return lMaxTime;
        }

        /// <summary>
        /// Retrieves the time (in seconds) the button/key has been released for
        /// </summary>
        /// <returns>Time the button has been released for</returns>
        /// <param name="rButtonID">Button ID to be tested.</param>
        public static float ReleasedElapsedTime(int rButtonID)
        {
            if (mState.Controls[rButtonID].IsPressed) { return 0; }
            return Time.time - mState.Controls[rButtonID].TimeReleased;
        }

        /// <summary>
        /// Retrieves the time (in seconds) the button/key has been released for
        /// </summary>
        /// <returns>Time the button has been released for</returns>
        /// <param name="rAlias">Button ID to be tested.</param>
        public static float ReleasedElapsedTime(string rAlias)
        {
            float lMaxTime = float.MinValue;

            List<InputAlias> lInputAlias = mAliases[rAlias];
            for (int i = lInputAlias.Count - 1; i >= 0; i--)
            {
                if (lInputAlias[i].CustomTest != null) { continue; }

                if (lInputAlias[i].SupportID <= 0 && mState.Controls[lInputAlias[i].PrimaryID].IsPressed) { return 0; }
                if (mState.Controls[lInputAlias[i].PrimaryID].IsPressed && mState.Controls[lInputAlias[i].SupportID].IsPressed) { return 0; }

                float lTime = mState.Controls[lInputAlias[i].PrimaryID].TimeReleased;
                if (lTime > lMaxTime) { lMaxTime = lTime; }
            }

            return Time.time - lMaxTime;
        }

        #endregion

        /// <summary>
        /// Grab and process information from the input in one place. This
        /// allows us to calculated changes over time too.
        /// </summary>
        public static void Update()
        {
            // Start tracking
            mWatch.Reset();
            mWatch.Start();

            // Swap the states
            InputState lState = mPrevState;
            mPrevState = mState;
            mState = lState;

            // Handle the mouse
            if (mUseMouse)
            {
                mState.Controls[EnumInput.MOUSE_LEFT_BUTTON].Value = (UnityEngine.Input.GetMouseButton(0) ? 1 : 0);
                mState.Controls[EnumInput.MOUSE_RIGHT_BUTTON].Value = (UnityEngine.Input.GetMouseButton(1) ? 1 : 0);
                mState.Controls[EnumInput.MOUSE_MIDDLE_BUTTON].Value = (UnityEngine.Input.GetMouseButton(2) ? 1 : 0);
                mState.Controls[EnumInput.MOUSE_X].Value = UnityEngine.Input.mousePosition.x;
                mState.Controls[EnumInput.MOUSE_Y].Value = UnityEngine.Input.mousePosition.y;
                
                float lMouseX = UnityEngine.Input.GetAxis("Mouse X");                
                float lMouseY = UnityEngine.Input.GetAxis("Mouse Y");

                // With VSync off, we don't get input information every frame
                // like we'd expect. So, we need to compensate for it here
                if (UnityEngine.QualitySettings.vSyncCount == 0)
                {
                    if (lMouseX != 0f)
                    {
                        mTargetMouseX = lMouseX * 2f; // Multiplier is to compensate for speed of no-vsync
                        mVSyncTimer = 0f;
                    }

                    if (lMouseY != 0f)
                    {
                        mTargetMouseY = lMouseY * 2f; // Multiplier is to compensate for speed of no-vsync
                        mVSyncTimer = 0f;
                    }

                    // If vsync is off, and we get no mouse input, we 
                    // run the timer that will eventually set the view to 0.
                    if (lMouseX == 0f && lMouseY == 0f)
                    {
                        mVSyncTimer += Time.deltaTime;
                        if (mVSyncTimer > Time.fixedDeltaTime)
                        {
                            mTargetMouseX = 0f;
                            mTargetMouseY = 0f;
                            mVSyncTimer = 0f;
                        }
                    }

                    // Set the actual view values
                    mState.Controls[EnumInput.MOUSE_AXIS_X].Value = Mathf.Lerp(mState.Controls[EnumInput.MOUSE_AXIS_X].Value, mTargetMouseX, 0.9f);
                    mState.Controls[EnumInput.MOUSE_AXIS_Y].Value = Mathf.Lerp(mState.Controls[EnumInput.MOUSE_AXIS_Y].Value, mTargetMouseY, 0.9f);
                }
                // With VSync on, we're fine
                else
                {
                    mState.Controls[EnumInput.MOUSE_AXIS_X].Value = lMouseX;
                    mState.Controls[EnumInput.MOUSE_AXIS_Y].Value = lMouseY;
                }

                mState.Controls[EnumInput.MOUSE_WHEEL].Value = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            }

            // Clear out the key list
            mKeysPressedCount = 0;

            // Shortcut the keyboard check if no keys are pressed. This helps
            // performance since typically keys are not continuously pressed.
            if (!UnityEngine.Input.anyKey)
            {
                // Cycle through all the keys and flag them as off
                for (int i = EnumInput.KEYBOARD_MIN; i <= EnumInput.KEYBOARD_MAX; i++)
                {
                    mState.Controls[i].Value = 0;
                }
            }
            // Since a key (or mouse button) is pressed, find which one. We do track all of the possible
            // inputs which is a bit of overkill. However, we want to know when the buttons swap from
            // pressed to released. This information can then be used to message any listeners.
            else
            {
                // Handle the keyboard
                if (mUseKeyboardBasic)
                {
                    mState.Controls[EnumInput.BACKSPACE].Value = (UnityEngine.Input.GetKey(KeyCode.Backspace) ? 1 : 0);
                    mState.Controls[EnumInput.TAB].Value = (UnityEngine.Input.GetKey(KeyCode.Tab) ? 1 : 0);

                    mState.Controls[EnumInput.ENTER].Value = (UnityEngine.Input.GetKey(KeyCode.Return) ? 1 : 0);

                    mState.Controls[EnumInput.ESCAPE].Value = (UnityEngine.Input.GetKey(KeyCode.Escape) ? 1 : 0);

                    mState.Controls[EnumInput.SPACE].Value = (UnityEngine.Input.GetKey(KeyCode.Space) ? 1 : 0);

                    mState.Controls[EnumInput.EXCLAMATION].Value = (UnityEngine.Input.GetKey(KeyCode.Exclaim) ? 1 : 0);
                    mState.Controls[EnumInput.DOUBLE_QUOTE].Value = (UnityEngine.Input.GetKey(KeyCode.DoubleQuote) ? 1 : 0);
                    mState.Controls[EnumInput.HASH].Value = (UnityEngine.Input.GetKey(KeyCode.Hash) ? 1 : 0);
                    mState.Controls[EnumInput.DOLLAR].Value = (UnityEngine.Input.GetKey(KeyCode.Dollar) ? 1 : 0);
                    //mState.Controls[EnumInput.PERCENT].Value = (UnityEngine.Input.GetKey(KeyCode.Alpha5) ? 1 : 0);              // Unity doesn't have a percent?

                    // ASCII Symbols
                    for (int i = 38; i <= 64; i++)
                    {
                        mState.Controls[i].Value = (UnityEngine.Input.GetKey((KeyCode)i) ? 1 : 0);
                    }

                    // ASCII Letters
                    for (int i = 97; i <= 122; i++)
                    {
                        mState.Controls[i].Value = (UnityEngine.Input.GetKey((KeyCode)i) ? 1 : 0);
                    }
                    UnityEngine.Debug.Log("UnityEngine.InputGetKey:" + UnityEngine.Input.GetKey(KeyCode.A) + " mState.Controls[A]:" + mState.Controls[EnumInput.A].Value);

                    mState.Controls[EnumInput.DELETE].Value = (UnityEngine.Input.GetKey(KeyCode.Delete) ? 1 : 0);             // Unity doesn't have a percent?

                    mState.Controls[EnumInput.CAPS_LOCK].Value = (UnityEngine.Input.GetKey(KeyCode.CapsLock) ? 1 : 0);
                    mState.Controls[EnumInput.SCROLL_LOCK].Value = (UnityEngine.Input.GetKey(KeyCode.ScrollLock) ? 1 : 0);
                    mState.Controls[EnumInput.LEFT_SHIFT].Value = (UnityEngine.Input.GetKey(KeyCode.LeftShift) ? 1 : 0);
                    mState.Controls[EnumInput.RIGHT_SHIFT].Value = (UnityEngine.Input.GetKey(KeyCode.RightShift) ? 1 : 0);
                    mState.Controls[EnumInput.LEFT_CONTROL].Value = (UnityEngine.Input.GetKey(KeyCode.LeftControl) ? 1 : 0);
                    mState.Controls[EnumInput.RIGHT_CONTROL].Value = (UnityEngine.Input.GetKey(KeyCode.RightControl) ? 1 : 0);
                    mState.Controls[EnumInput.LEFT_ALT].Value = (UnityEngine.Input.GetKey(KeyCode.LeftAlt) ? 1 : 0);
                    mState.Controls[EnumInput.RIGHT_ALT].Value = (UnityEngine.Input.GetKey(KeyCode.RightAlt) ? 1 : 0);
                    mState.Controls[EnumInput.LEFT_BRACKET].Value = (UnityEngine.Input.GetKey(KeyCode.LeftBracket) ? 1 : 0);
                    mState.Controls[EnumInput.RIGHT_BRACKET].Value = (UnityEngine.Input.GetKey(KeyCode.RightBracket) ? 1 : 0);

                    mState.Controls[EnumInput.UP_ARROW].Value = (UnityEngine.Input.GetKey(KeyCode.UpArrow) ? 1 : 0);
                    mState.Controls[EnumInput.DOWN_ARROW].Value = (UnityEngine.Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
                    mState.Controls[EnumInput.RIGHT_ARROW].Value = (UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
                    mState.Controls[EnumInput.LEFT_ARROW].Value = (UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
                    mState.Controls[EnumInput.INSERT].Value = (UnityEngine.Input.GetKey(KeyCode.Insert) ? 1 : 0);
                    mState.Controls[EnumInput.HOME].Value = (UnityEngine.Input.GetKey(KeyCode.Home) ? 1 : 0);
                    mState.Controls[EnumInput.END].Value = (UnityEngine.Input.GetKey(KeyCode.End) ? 1 : 0);
                    mState.Controls[EnumInput.PAGE_UP].Value = (UnityEngine.Input.GetKey(KeyCode.PageUp) ? 1 : 0);
                    mState.Controls[EnumInput.PAGE_DOWN].Value = (UnityEngine.Input.GetKey(KeyCode.PageDown) ? 1 : 0);

                    mState.Controls[EnumInput.BACK_SLASH].Value = (UnityEngine.Input.GetKey(KeyCode.Backslash) ? 1 : 0);
                    mState.Controls[EnumInput.BACK_QUOTE].Value = (UnityEngine.Input.GetKey(KeyCode.BackQuote) ? 1 : 0);
                }

                // Handle the keypad
                if (mUseKeyboardKeyPad)
                {
                    mState.Controls[EnumInput.NUM_LOCK].Value = (UnityEngine.Input.GetKey(KeyCode.Numlock) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_0].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad0) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_1].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad1) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_2].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad2) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_3].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad3) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_4].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad4) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_5].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad5) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_6].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad6) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_7].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad7) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_8].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad8) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_9].Value = (UnityEngine.Input.GetKey(KeyCode.Keypad9) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_PERIOD].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadPeriod) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_DIVIDE].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadDivide) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_MULTIPLY].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadMultiply) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_MINUS].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadMinus) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_PLUS].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadPlus) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_ENTER].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadEnter) ? 1 : 0);
                    mState.Controls[EnumInput.KEYPAD_EQUALS].Value = (UnityEngine.Input.GetKey(KeyCode.KeypadEquals) ? 1 : 0);
                }

                // Handle the function keys
                if (mUseKeyboardFKeys)
                {
                    mState.Controls[EnumInput.F1].Value = (UnityEngine.Input.GetKey(KeyCode.F1) ? 1 : 0);
                    mState.Controls[EnumInput.F2].Value = (UnityEngine.Input.GetKey(KeyCode.F2) ? 1 : 0);
                    mState.Controls[EnumInput.F3].Value = (UnityEngine.Input.GetKey(KeyCode.F3) ? 1 : 0);
                    mState.Controls[EnumInput.F4].Value = (UnityEngine.Input.GetKey(KeyCode.F4) ? 1 : 0);
                    mState.Controls[EnumInput.F5].Value = (UnityEngine.Input.GetKey(KeyCode.F5) ? 1 : 0);
                    mState.Controls[EnumInput.F6].Value = (UnityEngine.Input.GetKey(KeyCode.F6) ? 1 : 0);
                    mState.Controls[EnumInput.F7].Value = (UnityEngine.Input.GetKey(KeyCode.F7) ? 1 : 0);
                    mState.Controls[EnumInput.F8].Value = (UnityEngine.Input.GetKey(KeyCode.F8) ? 1 : 0);
                    mState.Controls[EnumInput.F9].Value = (UnityEngine.Input.GetKey(KeyCode.F9) ? 1 : 0);
                    mState.Controls[EnumInput.F10].Value = (UnityEngine.Input.GetKey(KeyCode.F10) ? 1 : 0);
                    mState.Controls[EnumInput.F11].Value = (UnityEngine.Input.GetKey(KeyCode.F11) ? 1 : 0);
                    mState.Controls[EnumInput.F12].Value = (UnityEngine.Input.GetKey(KeyCode.F12) ? 1 : 0);
                    mState.Controls[EnumInput.F13].Value = (UnityEngine.Input.GetKey(KeyCode.F13) ? 1 : 0);
                    mState.Controls[EnumInput.F14].Value = (UnityEngine.Input.GetKey(KeyCode.F14) ? 1 : 0);
                    mState.Controls[EnumInput.F15].Value = (UnityEngine.Input.GetKey(KeyCode.F15) ? 1 : 0);
                }
            }

            // Handle the gamepad
            if (mUseGamepad)
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor)
                {
                    mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_X].Value = UnityEngine.Input.GetAxis("WXLeftStickX");
                    mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_Y].Value = UnityEngine.Input.GetAxis("WXLeftStickY");
                    mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_BUTTON].Value = (UnityEngine.Input.GetButton("WXLeftStickButton") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_X].Value = UnityEngine.Input.GetAxis("WXRightStickX");
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_Y].Value = UnityEngine.Input.GetAxis("WXRightStickY");
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_BUTTON].Value = (UnityEngine.Input.GetButton("WXRightStickButton") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_DPAD_X].Value = UnityEngine.Input.GetAxis("WXDPadX");
                    mState.Controls[EnumInput.GAMEPAD_DPAD_Y].Value = UnityEngine.Input.GetAxis("WXDPadY");
                    mState.Controls[EnumInput.GAMEPAD_0_BUTTON].Value = (UnityEngine.Input.GetButton("WXButton0") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_1_BUTTON].Value = (UnityEngine.Input.GetButton("WXButton1") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_2_BUTTON].Value = (UnityEngine.Input.GetButton("WXButton2") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_3_BUTTON].Value = (UnityEngine.Input.GetButton("WXButton3") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_BACK_BUTTON].Value = (UnityEngine.Input.GetButton("WXBack") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_START_BUTTON].Value = (UnityEngine.Input.GetButton("WXStart") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_LEFT_TRIGGER].Value = UnityEngine.Input.GetAxis("WXLeftTrigger");
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_TRIGGER].Value = UnityEngine.Input.GetAxis("WXRightTrigger");
                    mState.Controls[EnumInput.GAMEPAD_LEFT_BUMPER].Value = (UnityEngine.Input.GetButton("WXLeftBumper") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_BUMPER].Value = (UnityEngine.Input.GetButton("WXRightBumper") ? 1 : 0);
                }
                else if (Application.platform == RuntimePlatform.OSXPlayer ||
                         Application.platform == RuntimePlatform.OSXEditor)
                {
                    mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_X].Value = UnityEngine.Input.GetAxis("MXLeftStickX");
                    mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_Y].Value = UnityEngine.Input.GetAxis("MXLeftStickY");
                    mState.Controls[EnumInput.GAMEPAD_LEFT_STICK_BUTTON].Value = (UnityEngine.Input.GetButton("MXLeftStickButton") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_X].Value = UnityEngine.Input.GetAxis("MXRightStickX");
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_Y].Value = UnityEngine.Input.GetAxis("MXRightStickY");
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_STICK_BUTTON].Value = (UnityEngine.Input.GetButton("MXRightStickButton") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_DPAD_X].Value = UnityEngine.Input.GetAxis("MXDPadX");
                    mState.Controls[EnumInput.GAMEPAD_DPAD_Y].Value = UnityEngine.Input.GetAxis("MXDPadY");
                    mState.Controls[EnumInput.GAMEPAD_0_BUTTON].Value = (UnityEngine.Input.GetButton("MXButton0") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_1_BUTTON].Value = (UnityEngine.Input.GetButton("MXButton1") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_2_BUTTON].Value = (UnityEngine.Input.GetButton("MXButton2") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_3_BUTTON].Value = (UnityEngine.Input.GetButton("MXButton3") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_BACK_BUTTON].Value = (UnityEngine.Input.GetButton("MXBack") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_START_BUTTON].Value = (UnityEngine.Input.GetButton("MXStart") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_LEFT_TRIGGER].Value = UnityEngine.Input.GetAxis("MXLeftTrigger");
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_TRIGGER].Value = UnityEngine.Input.GetAxis("MXRightTrigger");
                    mState.Controls[EnumInput.GAMEPAD_LEFT_BUMPER].Value = (UnityEngine.Input.GetButton("MXLeftBumper") ? 1 : 0);
                    mState.Controls[EnumInput.GAMEPAD_RIGHT_BUMPER].Value = (UnityEngine.Input.GetButton("MXRightBumper") ? 1 : 0);
                }
            }

            // Process the input to determine which ones have just started and
            // which ones have just ended
            for (int i = 0; i <= EnumInput.MAX; i++)
            {
                if (!mUseMouse && i <= EnumInput.MOUSE_MAX) { continue; }
                if (!mUseKeyboardBasic && (i >= EnumInput.KEYBOARD_BASIC_MIN && i <= EnumInput.KEYBOARD_BASIC_MAX)) { continue; }
                if (!mUseKeyboardKeyPad && (i >= EnumInput.KEYBOARD_KEYPAD_MIN && i <= EnumInput.KEYBOARD_KEYPAD_MAX)) { continue; }
                if (!mUseKeyboardFKeys && (i >= EnumInput.KEYBOARD_FKEYS_MIN && i <= EnumInput.KEYBOARD_FKEYS_MAX)) { continue; }
                if (!mUseGamepad && (i >= EnumInput.GAMEPAD_MIN && i <= EnumInput.GAMEPAD_MAX)) { continue; }

                InputControlState lControlState = mState.Controls[i];
                InputControlState lControlPrevState = mPrevState.Controls[i];

                lControlState.IsPressed = (lControlState.Value != 0);

                lControlState.IsToggled = lControlPrevState.IsToggled;
                lControlState.IsDoublePressed = lControlPrevState.IsDoublePressed;
                lControlState.IsDoubleReleased = lControlPrevState.IsDoubleReleased;
                lControlState.TimePressed = lControlPrevState.TimePressed;
                lControlState.TimeReleased = lControlPrevState.TimeReleased;

                if (lControlState.IsPressed)
                {
                    lControlState.IsDoubleReleased = false; 

                    if (!lControlPrevState.IsPressed)
                    {
                        lControlState.IsDoublePressed = (lControlState.TimePressed + DoublePressTime >= Time.time);
                        lControlState.TimePressed = Time.time;
                    }
                }

                if (!lControlState.IsPressed && lControlPrevState.IsPressed)
                {
                    lControlState.IsToggled = !lControlState.IsToggled;
                    lControlState.IsDoublePressed = false;
                    lControlState.TimeReleased = Time.time;
                }

                if (!lControlState.IsPressed && lControlPrevState.IsDoublePressed)
                {
                    lControlState.IsDoubleReleased = true;
                }

                mState.Controls[i] = lControlState;

                // Stack up the key presses
                if (i >= EnumInput.KEYBOARD_MIN && i <= EnumInput.KEYBOARD_MAX && lControlState.IsPressed)
                {
                    if (mKeysPressedCount < InputManager.MAX_KEY_PRESSES)
                    {
                        mKeysPressed[mKeysPressedCount] = i;
                        mKeysPressedCount++;
                    }
                }
            }

            // Send input events using the messagner (if it's enabled)
            if (mIsEventsEnabled) { SendEvents(); }

            // Stop tracking
            mWatch.Stop();
            mUpdateElapsedTicks = mWatch.ElapsedTicks;
        }

        /// <summary>
        /// Use the message dispatcher to send input events		
        /// </summary>
        public static void SendEvents()
        {
            // ******************************************************************************
            // Use the Event System Dispatcher to push input updates to objects that
            // are listening for them. If you have the Event System Dispatcher, simply
            // uncomment the 'using' statement at the top and the code below.
            //
            // https://www.assetstore.unity3d.com/#/content/12715
            // ******************************************************************************

            //// Cycle through all the controls to see if one is pressed
            //for (int i = 0; i <= EnumInput.MAX; i++)
            //{
            //    if (!mUseMouse && i <= EnumInput.MOUSE_MAX) { continue; }
            //    if (!mUseKeyboardBasic && (i >= EnumInput.KEYBOARD_BASIC_MIN && i <= EnumInput.KEYBOARD_BASIC_MAX)) { continue; }
            //    if (!mUseKeyboardKeyPad && (i >= EnumInput.KEYBOARD_KEYPAD_MIN && i <= EnumInput.KEYBOARD_KEYPAD_MAX)) { continue; }
            //    if (!mUseKeyboardFKeys && (i >= EnumInput.KEYBOARD_FKEYS_MIN && i <= EnumInput.KEYBOARD_FKEYS_MAX)) { continue; }
            //    if (!mUseGamepad && (i >= EnumInput.GAMEPAD_MIN && i <= EnumInput.GAMEPAD_MAX)) { continue; }

            //    if (IsJustPressed(i))
            //    {
            //        MessageDispatcher.SendMessage(null, EnumInputMessageType.INPUT_JUST_PRESSED, i.ToString(), mState.Controls[i], 0f);
            //    }

            //    if (IsJustReleased(i))
            //    {
            //        MessageDispatcher.SendMessage(null, EnumInputMessageType.INPUT_JUST_RELEASED, i.ToString(), mState.Controls[i], 0f);
            //    }
            //}
        }

        /// <summary>
        /// Writes out the input information
        /// </summary>
        /// <returns></returns>
        public static string DebugString()
        {
            return string.Format("dt:{0:f5} mx:{1:f3} my:{2:f3} vx:{3:f3} vy:{4:f3} lmb:{5} rmb:{6}", Time.deltaTime, MovementX, MovementY, ViewX, ViewY, IsPressed(EnumInput.MOUSE_LEFT_BUTTON), IsPressed(EnumInput.MOUSE_RIGHT_BUTTON));
        }
    }

    /// <summary>
    /// Used by the InputManager to hook into the unity update process. This allows us
    /// to update the input and track old values
    /// </summary>
    public sealed class InputManagerStub : MonoBehaviour
    {
        /// <summary>
        /// Raised first when the object comes into existance. Called
        /// even if script is not enabled.
        /// </summary>
        void Awake()
        {
            // Don't destroyed automatically when loading a new scene
            DontDestroyOnLoad(gameObject);

            // Initialize the manager
            InputManager.Initialize();
        }

        /// <summary>
        /// Called after the Awake() and before any update is called.
        /// </summary>
        public IEnumerator Start()
        {
            // Create the coroutine here so we don't re-create over and over
            WaitForEndOfFrame lWaitForEndOfFrame = new WaitForEndOfFrame();

            // Loop endlessly so we can process the input
            // at the end of each frame, preparing for the next
            while (true)
            {
                yield return lWaitForEndOfFrame;
                InputManager.Update();
            }
        }

        /// <summary>
        /// Called when the InputManager is disabled. We use this to
        /// clean up objects that were created.
        /// </summary>
        public void OnDisable()
        {
        }
    }
}

