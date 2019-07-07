using System;

namespace com.ootii.Input
{
    /// <summary>
    /// Stores information about the mouse
    /// </summary>
    public struct InputState
    {
        /// <summary>
        /// Holds the state of all the different input controls
        /// </summary>
        public InputControlState[] Controls;

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Initialize()
        {
            Controls = new InputControlState[EnumInput.MAX + 1];
            for (int i = 0; i < Controls.Length; i++)
            {
                Controls[i] = new InputControlState();
            }
        }
    }
}

