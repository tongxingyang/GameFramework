using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ootii.Input
{
    /// <summary>
    /// Provides a simple reference for the types of messages that
    /// can be sent from the InputManager using the Dispatcher
    /// </summary>
    public class EnumInputMessageType
    {
        public const string INPUT_JUST_PRESSED = "INPUT_JUST_PRESSED";

        public const string INPUT_JUST_RELEASED = "INPUT_JUST_RELEASED";
    }
}
