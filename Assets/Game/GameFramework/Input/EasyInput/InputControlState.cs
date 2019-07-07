using System;

namespace com.ootii.Input
{
    /// <summary>
    /// Tracks the state of a single button
    /// </summary>
    public struct InputControlState
    {
        /// <summary>
        /// Current value of the control
        /// </summary>
        public float Value;

        /// <summary>
        /// Determines if it's pressed
        /// </summary>
        public bool IsPressed;

        /// <summary>
        /// Determines if it's double-clicked
        /// </summary>
        public bool IsDoublePressed;

        /// <summary>
        /// Determines if it was double-clicked
        /// </summary>
        public bool IsDoubleReleased;

        /// <summary>
        /// Determines if the input is toggled
        /// </summary>
        public bool IsToggled;

        /// <summary>
        /// Determines how long it's been pressed for
        /// </summary>
        public float TimePressed;

        /// <summary>
        /// Determines how long it's been released for
        /// </summary>
        public float TimeReleased;
    }
}

