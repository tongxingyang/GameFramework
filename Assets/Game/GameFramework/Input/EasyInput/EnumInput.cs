using System;
using System.Collections.Generic;

namespace com.ootii.Input
{
    /// <summary>
    /// Provides an enumeration for all the device controls
    /// in the game. We use our own instead of Unity's KeyCode
    /// since they waste space. We want to compress the values
    /// as much as possible for performance.
    /// 
    /// Using consts are replaced at compile time. This makes them
    /// faster than using lookups or enums at runtime.
    /// </summary>
    public class EnumInput
    {
        public const int MAX = 187;
        public const int MOUSE_MIN = 0;
        public const int MOUSE_MAX = 7;
        public const int KEYBOARD_MIN = 8;
        public const int KEYBOARD_MAX = 164;
        public const int KEYBOARD_BASIC_MIN = 8;
        public const int KEYBOARD_BASIC_MAX = 127;
        public const int KEYBOARD_KEYPAD_MIN = 128;
        public const int KEYBOARD_KEYPAD_MAX = 144;
        public const int KEYBOARD_FKEYS_MIN = 150;
        public const int KEYBOARD_FKEYS_MAX = 164;
        public const int GAMEPAD_MIN = 170;
        public const int GAMEPAD_MAX = 187;

        // mUseMouse
        public const int MOUSE_LEFT_BUTTON = 0;
        public const int MOUSE_RIGHT_BUTTON = 1;
        public const int MOUSE_MIDDLE_BUTTON = 2;
        public const int MOUSE_X = 3;
        public const int MOUSE_Y = 4;
        public const int MOUSE_AXIS_X = 5;
        public const int MOUSE_AXIS_Y = 6;
        public const int MOUSE_WHEEL = 7;

        // mUseKeyboardBasic
        public const int BACKSPACE = 8;
        public const int TAB = 9;

        public const int ENTER = 13;

        public const int ESCAPE = 27;

        public const int SPACE = 32;

        public const int EXCLAMATION = 33;
        public const int DOUBLE_QUOTE = 34;
        public const int HASH = 35;
        public const int DOLLAR = 36;
        //public const int PERCENT = 37;
        public const int AMPERSAND = 38;
        public const int QUOTE = 39;
        public const int LEFT_PARENTHESIS = 40;
        public const int RIGHT_PARENTHESIS = 41;
        public const int ASTERIK = 42;
        public const int PLUS = 43;
        public const int COMMA = 44;
        public const int MINUS = 45;
        public const int PERIOD = 46;
        public const int SLASH = 47;

        public const int ALPHA_0 = 48;
        public const int ALPHA_1 = 49;
        public const int ALPHA_2 = 50;
        public const int ALPHA_3 = 51;
        public const int ALPHA_4 = 52;
        public const int ALPHA_5 = 53;
        public const int ALPHA_6 = 54;
        public const int ALPHA_7 = 55;
        public const int ALPHA_8 = 56;
        public const int ALPHA_9 = 57;

        public const int COLON = 58;
        public const int SEMICOLON = 59;
        public const int LESS = 60;
        public const int EQUALS = 61;
        public const int GREATER = 62;
        public const int QUESTION = 63;
        public const int AT_SIGN = 64;

        public const int CAPS_LOCK = 66;        //301;
        public const int SCROLL_LOCK = 67;      //302;
        public const int RIGHT_SHIFT = 68;      //303;
        public const int LEFT_SHIFT = 69;       //304;
        public const int RIGHT_CONTROL = 70;    //305;
        public const int LEFT_CONTROL = 71;     //306;
        public const int RIGHT_ALT = 72;        //307;
        public const int LEFT_ALT = 73;         //308;

        public const int UP_ARROW = 74;         //273;
        public const int DOWN_ARROW = 75;       //274;
        public const int RIGHT_ARROW = 76;      //275;
        public const int LEFT_ARROW = 77;       //276;
        public const int INSERT = 78;           //277;
        public const int HOME = 79;             //278;
        public const int END = 80;              //279;
        public const int PAGE_UP = 81;          //280;
        public const int PAGE_DOWN = 82;        //281;

        public const int LEFT_BRACKET = 91;
        public const int BACK_SLASH = 92;
        public const int RIGHT_BRACKET = 93;
        public const int CARET = 94;
        public const int UNDERSCORE = 95;
        public const int BACK_QUOTE = 96;

        public const int A = 97;
        public const int B = 98;
        public const int C = 99;
        public const int D = 100;
        public const int E = 101;
        public const int F = 102;
        public const int G = 103;
        public const int H = 104;
        public const int I = 105;
        public const int J = 106;
        public const int K = 107;
        public const int L = 108;
        public const int M = 109;
        public const int N = 110;
        public const int O = 111;
        public const int P = 112;
        public const int Q = 113;
        public const int R = 114;
        public const int S = 115;
        public const int T = 116;
        public const int U = 117;
        public const int V = 118;
        public const int W = 119;
        public const int X = 120;
        public const int Y = 121;
        public const int Z = 122;

        //public const int LEFT_CURLY_BRACKET = 123;
        //public const int PIPE = 124;
        //public const int RIGHT_CURLY_BRACKET = 125;
        //public const int TILDA = 126;
        public const int DELETE = 127;

        // mUseKeyboardKeyPad
        public const int NUM_LOCK = 65;         //300;

        public const int KEYPAD_0 = 128;        //256;
        public const int KEYPAD_1 = 129;        //257;
        public const int KEYPAD_2 = 130;        //258;
        public const int KEYPAD_3 = 131;        //259;
        public const int KEYPAD_4 = 132;        //260;
        public const int KEYPAD_5 = 133;        //261;
        public const int KEYPAD_6 = 134;        //262;
        public const int KEYPAD_7 = 135;        //263;
        public const int KEYPAD_8 = 136;        //264;
        public const int KEYPAD_9 = 137;        //265;
        public const int KEYPAD_PERIOD = 138;   //266;
        public const int KEYPAD_DIVIDE = 139;   //267;
        public const int KEYPAD_MULTIPLY = 140; //268;
        public const int KEYPAD_MINUS = 141;    //269;
        public const int KEYPAD_PLUS = 142;     //270;
        public const int KEYPAD_ENTER = 143;    //271;
        public const int KEYPAD_EQUALS = 144;   //272;

        // mUseKeyboardFKeys
        public const int F1 = 150;              //282;
        public const int F2 = 151;              //283;
        public const int F3 = 152;              //284;
        public const int F4 = 153;              //285;
        public const int F5 = 154;              //286;
        public const int F6 = 155;              //287;
        public const int F7 = 156;              //288;
        public const int F8 = 157;              //289;
        public const int F9 = 158;              //290;
        public const int F10 = 159;             //291;
        public const int F11 = 160;             //292;
        public const int F12 = 161;             //293;
        public const int F13 = 162;             //294;
        public const int F14 = 163;             //295;
        public const int F15 = 164;             //296;

        // mUseGamepad
        public const int GAMEPAD_LEFT_STICK_X = 170;
        public const int GAMEPAD_LEFT_STICK_Y = 171;
        public const int GAMEPAD_LEFT_STICK_BUTTON = 172;
        public const int GAMEPAD_RIGHT_STICK_X = 173;
        public const int GAMEPAD_RIGHT_STICK_Y = 174;
        public const int GAMEPAD_RIGHT_STICK_BUTTON = 175;
        public const int GAMEPAD_LEFT_TRIGGER = 176;
        public const int GAMEPAD_RIGHT_TRIGGER = 177;
        public const int GAMEPAD_0_BUTTON = 178;
        public const int GAMEPAD_1_BUTTON = 179;
        public const int GAMEPAD_2_BUTTON = 180;
        public const int GAMEPAD_3_BUTTON = 181;
        public const int GAMEPAD_BACK_BUTTON = 182;
        public const int GAMEPAD_START_BUTTON = 183;
        public const int GAMEPAD_LEFT_BUMPER = 184;
        public const int GAMEPAD_RIGHT_BUMPER = 185;
        public const int GAMEPAD_DPAD_X = 186;
        public const int GAMEPAD_DPAD_Y = 187;

        /// <summary>
        /// Contains a mapping from ID to names
        /// </summary>
        public static Dictionary<int, string> EnumNames = new Dictionary<int, string>();

        /// <summary>
        /// Constructor. We use this to initialize the EnumNames dictionary
        /// for key-to-string lookups.
        /// </summary>
        static EnumInput()
        {
            // mUseMouse
            EnumNames.Add(EnumInput.MOUSE_LEFT_BUTTON, "MOUSE_LEFT_BUTTON"); //0;    
            EnumNames.Add(EnumInput.MOUSE_RIGHT_BUTTON, "MOUSE_RIGHT_BUTTON"); //1;  
            EnumNames.Add(EnumInput.MOUSE_MIDDLE_BUTTON, "MOUSE_MIDDLE_BUTTON"); //2;
            EnumNames.Add(EnumInput.MOUSE_X, "MOUSE_X"); //3;          
            EnumNames.Add(EnumInput.MOUSE_Y, "MOUSE_Y"); //4;          
            EnumNames.Add(EnumInput.MOUSE_AXIS_X, "MOUSE_AXIS_X"); //5;
            EnumNames.Add(EnumInput.MOUSE_AXIS_Y, "MOUSE_AXIS_Y"); //6;
            EnumNames.Add(EnumInput.MOUSE_WHEEL, "MOUSE_WHEEL"); //7; 

            // mUseKeyboardBasic
            EnumNames.Add(EnumInput.BACKSPACE, "BACKSPACE"); //8;
            EnumNames.Add(EnumInput.TAB, "TAB"); //9;

            EnumNames.Add(EnumInput.ENTER, "ENTER"); //13;

            EnumNames.Add(EnumInput.ESCAPE, "ESCAPE"); //27;

            EnumNames.Add(EnumInput.SPACE, "SPACE"); //32;

            EnumNames.Add(EnumInput.EXCLAMATION, "EXCLAMATION"); //33;
            EnumNames.Add(EnumInput.DOUBLE_QUOTE, "DOUBLE_QUOTE"); //34;
            EnumNames.Add(EnumInput.HASH, "HASH"); //35;
            EnumNames.Add(EnumInput.DOLLAR, "DOLLAR"); //36;
            //EnumNames.Add(EnumInput, "PERCENT"); //37;
            EnumNames.Add(EnumInput.AMPERSAND, "AMPERSAND"); //38;
            EnumNames.Add(EnumInput.QUOTE, "QUOTE"); //39;
            EnumNames.Add(EnumInput.LEFT_PARENTHESIS, "LEFT_PARENTHESIS"); //40;
            EnumNames.Add(EnumInput.RIGHT_PARENTHESIS, "RIGHT_PARENTHESIS"); //41;
            EnumNames.Add(EnumInput.ASTERIK, "ASTERIK"); //42;
            EnumNames.Add(EnumInput.PLUS, "PLUS"); //43;
            EnumNames.Add(EnumInput.COMMA, "COMMA"); //44;
            EnumNames.Add(EnumInput.MINUS, "MINUS"); //45;
            EnumNames.Add(EnumInput.PERIOD, "PERIOD"); //46;
            EnumNames.Add(EnumInput.SLASH, "SLASH"); //47;

            EnumNames.Add(EnumInput.ALPHA_0, "ALPHA_0"); //48;
            EnumNames.Add(EnumInput.ALPHA_1, "ALPHA_1"); //49;
            EnumNames.Add(EnumInput.ALPHA_2, "ALPHA_2"); //50;
            EnumNames.Add(EnumInput.ALPHA_3, "ALPHA_3"); //51;
            EnumNames.Add(EnumInput.ALPHA_4, "ALPHA_4"); //52;
            EnumNames.Add(EnumInput.ALPHA_5, "ALPHA_5"); //53;
            EnumNames.Add(EnumInput.ALPHA_6, "ALPHA_6"); //54;
            EnumNames.Add(EnumInput.ALPHA_7, "ALPHA_7"); //55;
            EnumNames.Add(EnumInput.ALPHA_8, "ALPHA_8"); //56;
            EnumNames.Add(EnumInput.ALPHA_9, "ALPHA_9"); //57;

            EnumNames.Add(EnumInput.COLON, "COLON"); //58;
            EnumNames.Add(EnumInput.SEMICOLON, "SEMICOLON"); //59;
            EnumNames.Add(EnumInput.LESS, "LESS"); //60;
            EnumNames.Add(EnumInput.EQUALS, "EQUALS"); //61;
            EnumNames.Add(EnumInput.GREATER, "GREATER"); //62;
            EnumNames.Add(EnumInput.QUESTION, "QUESTION"); //63;
            EnumNames.Add(EnumInput.AT_SIGN, "AT_SIGN"); //64;

            EnumNames.Add(EnumInput.CAPS_LOCK, "CAPS_LOCK"); //66;        //301;
            EnumNames.Add(EnumInput.SCROLL_LOCK, "SCROLL_LOCK"); //67;      //302;
            EnumNames.Add(EnumInput.RIGHT_SHIFT, "RIGHT_SHIFT"); //68;      //303;
            EnumNames.Add(EnumInput.LEFT_SHIFT, "LEFT_SHIFT"); //69;       //304;
            EnumNames.Add(EnumInput.RIGHT_CONTROL, "RIGHT_CONTROL"); //70;    //305;
            EnumNames.Add(EnumInput.LEFT_CONTROL, "LEFT_CONTROL"); //71;     //306;
            EnumNames.Add(EnumInput.RIGHT_ALT, "RIGHT_ALT"); //72;        //307;
            EnumNames.Add(EnumInput.LEFT_ALT, "LEFT_ALT"); //73;         //308;

            EnumNames.Add(EnumInput.UP_ARROW, "UP_ARROW"); //74;         //273;
            EnumNames.Add(EnumInput.DOWN_ARROW, "DOWN_ARROW"); //75;       //274;
            EnumNames.Add(EnumInput.RIGHT_ARROW, "RIGHT_ARROW"); //76;      //275;
            EnumNames.Add(EnumInput.LEFT_ARROW, "LEFT_ARROW"); //77;       //276;
            EnumNames.Add(EnumInput.INSERT, "INSERT"); //78;           //277;
            EnumNames.Add(EnumInput.HOME, "HOME"); //79;             //278;
            EnumNames.Add(EnumInput.END, "END"); //80;              //279;
            EnumNames.Add(EnumInput.PAGE_UP, "PAGE_UP"); //81;          //280;
            EnumNames.Add(EnumInput.PAGE_DOWN, "PAGE_DOWN"); //82;        //281;

            EnumNames.Add(EnumInput.LEFT_BRACKET, "LEFT_BRACKET"); //91;
            EnumNames.Add(EnumInput.BACK_SLASH, "BACK_SLASH"); //92;
            EnumNames.Add(EnumInput.RIGHT_BRACKET, "RIGHT_BRACKET"); //93;
            EnumNames.Add(EnumInput.CARET, "CARET"); //94;
            EnumNames.Add(EnumInput.UNDERSCORE, "UNDERSCORE"); //95;
            EnumNames.Add(EnumInput.BACK_QUOTE, "BACK_QUOTE"); //96;

            EnumNames.Add(EnumInput.A, "A"); //97;
            EnumNames.Add(EnumInput.B, "B"); //98;
            EnumNames.Add(EnumInput.C, "C"); //99;
            EnumNames.Add(EnumInput.D, "D"); //100;
            EnumNames.Add(EnumInput.E, "E"); //101;
            EnumNames.Add(EnumInput.F, "F"); //102;
            EnumNames.Add(EnumInput.G, "G"); //103;
            EnumNames.Add(EnumInput.H, "H"); //104;
            EnumNames.Add(EnumInput.I, "I"); //105;
            EnumNames.Add(EnumInput.J, "J"); //106;
            EnumNames.Add(EnumInput.K, "K"); //107;
            EnumNames.Add(EnumInput.L, "L"); //108;
            EnumNames.Add(EnumInput.M, "M"); //109;
            EnumNames.Add(EnumInput.N, "N"); //110;
            EnumNames.Add(EnumInput.O, "O"); //111;
            EnumNames.Add(EnumInput.P, "P"); //112;
            EnumNames.Add(EnumInput.Q, "Q"); //113;
            EnumNames.Add(EnumInput.R, "R"); //114;
            EnumNames.Add(EnumInput.S, "S"); //115;
            EnumNames.Add(EnumInput.T, "T"); //116;
            EnumNames.Add(EnumInput.U, "U"); //117;
            EnumNames.Add(EnumInput.V, "V"); //118;
            EnumNames.Add(EnumInput.W, "W"); //119;
            EnumNames.Add(EnumInput.X, "X"); //120;
            EnumNames.Add(EnumInput.Y, "Y"); //121;
            EnumNames.Add(EnumInput.Z, "Z"); //122;

            //EnumNames.Add(EnumInput, "LEFT_CURLY_BRACKET"); //123;
            //EnumNames.Add(EnumInput, "PIPE"); //124;
            //EnumNames.Add(EnumInput, "RIGHT_CURLY_BRACKET"); //125;
            //EnumNames.Add(EnumInput, "TILDA"); //126;
            EnumNames.Add(EnumInput.DELETE, "DELETE"); //127;

            // mUseKeyboardKeyPad
            EnumNames.Add(EnumInput.NUM_LOCK, "NUM_LOCK"); //65;         //300;

            EnumNames.Add(EnumInput.KEYPAD_0, "KEYPAD_0"); //128;        //256;
            EnumNames.Add(EnumInput.KEYPAD_1, "KEYPAD_1"); //129;        //257;
            EnumNames.Add(EnumInput.KEYPAD_2, "KEYPAD_2"); //130;        //258;
            EnumNames.Add(EnumInput.KEYPAD_3, "KEYPAD_3"); //131;        //259;
            EnumNames.Add(EnumInput.KEYPAD_4, "KEYPAD_4"); //132;        //260;
            EnumNames.Add(EnumInput.KEYPAD_5, "KEYPAD_5"); //133;        //261;
            EnumNames.Add(EnumInput.KEYPAD_6, "KEYPAD_6"); //134;        //262;
            EnumNames.Add(EnumInput.KEYPAD_7, "KEYPAD_7"); //135;        //263;
            EnumNames.Add(EnumInput.KEYPAD_8, "KEYPAD_8"); //136;        //264;
            EnumNames.Add(EnumInput.KEYPAD_9, "KEYPAD_9"); //137;        //265;
            EnumNames.Add(EnumInput.KEYPAD_PERIOD, "KEYPAD_PERIOD"); //138;   //266;
            EnumNames.Add(EnumInput.KEYPAD_DIVIDE, "KEYPAD_DIVIDE"); //139;   //267;
            EnumNames.Add(EnumInput.KEYPAD_MULTIPLY, "KEYPAD_MULTIPLY"); //140; //268;
            EnumNames.Add(EnumInput.KEYPAD_MINUS, "KEYPAD_MINUS"); //141;    //269;
            EnumNames.Add(EnumInput.KEYPAD_PLUS, "KEYPAD_PLUS"); //142;     //270;
            EnumNames.Add(EnumInput.KEYPAD_ENTER, "KEYPAD_ENTER"); //143;    //271;
            EnumNames.Add(EnumInput.KEYPAD_EQUALS, "KEYPAD_EQUALS"); //144;   //272;

            // mUseKeyboardFKeys
            EnumNames.Add(EnumInput.F1, "F1"); //150;              //282;
            EnumNames.Add(EnumInput.F2, "F2"); //151;              //283;
            EnumNames.Add(EnumInput.F3, "F3"); //152;              //284;
            EnumNames.Add(EnumInput.F4, "F4"); //153;              //285;
            EnumNames.Add(EnumInput.F5, "F5"); //154;              //286;
            EnumNames.Add(EnumInput.F6, "F6"); //155;              //287;
            EnumNames.Add(EnumInput.F7, "F7"); //156;              //288;
            EnumNames.Add(EnumInput.F8, "F8"); //157;              //289;
            EnumNames.Add(EnumInput.F9, "F9"); //158;              //290;
            EnumNames.Add(EnumInput.F10, "F10"); //159;             //291;
            EnumNames.Add(EnumInput.F11, "F11"); //160;             //292;
            EnumNames.Add(EnumInput.F12, "F12"); //161;             //293;
            EnumNames.Add(EnumInput.F13, "F13"); //162;             //294;
            EnumNames.Add(EnumInput.F14, "F14"); //163;             //295;
            EnumNames.Add(EnumInput.F15, "F15"); //164;             //296;

            // mUseGamepad
            EnumNames.Add(EnumInput.GAMEPAD_LEFT_STICK_X, "GAMEPAD_LEFT_STICK_X"); //170;
            EnumNames.Add(EnumInput.GAMEPAD_LEFT_STICK_Y, "GAMEPAD_LEFT_STICK_Y"); //171;
            EnumNames.Add(EnumInput.GAMEPAD_LEFT_STICK_BUTTON, "GAMEPAD_LEFT_STICK_BUTTON"); //172;
            EnumNames.Add(EnumInput.GAMEPAD_RIGHT_STICK_X, "GAMEPAD_RIGHT_STICK_X"); //173;
            EnumNames.Add(EnumInput.GAMEPAD_RIGHT_STICK_Y, "GAMEPAD_RIGHT_STICK_Y"); //174;
            EnumNames.Add(EnumInput.GAMEPAD_RIGHT_STICK_BUTTON, "GAMEPAD_RIGHT_STICK_BUTTON"); //175;
            EnumNames.Add(EnumInput.GAMEPAD_LEFT_TRIGGER, "GAMEPAD_LEFT_TRIGGER"); //176;
            EnumNames.Add(EnumInput.GAMEPAD_RIGHT_TRIGGER, "GAMEPAD_RIGHT_TRIGGER"); //177;
            EnumNames.Add(EnumInput.GAMEPAD_0_BUTTON, "GAMEPAD_0_BUTTON"); //178;
            EnumNames.Add(EnumInput.GAMEPAD_1_BUTTON, "GAMEPAD_1_BUTTON"); //179;
            EnumNames.Add(EnumInput.GAMEPAD_2_BUTTON, "GAMEPAD_2_BUTTON"); //180;
            EnumNames.Add(EnumInput.GAMEPAD_3_BUTTON, "GAMEPAD_3_BUTTON"); //181;
            EnumNames.Add(EnumInput.GAMEPAD_BACK_BUTTON, "GAMEPAD_BACK_BUTTON"); //182;
            EnumNames.Add(EnumInput.GAMEPAD_START_BUTTON, "GAMEPAD_START_BUTTON"); //183;
            EnumNames.Add(EnumInput.GAMEPAD_LEFT_BUMPER, "GAMEPAD_LEFT_BUMPER"); //184;
            EnumNames.Add(EnumInput.GAMEPAD_RIGHT_BUMPER, "GAMEPAD_RIGHT_BUMPER"); //185;
            EnumNames.Add(EnumInput.GAMEPAD_DPAD_X, "GAMEPAD_DPAD_X"); //186;
            EnumNames.Add(EnumInput.GAMEPAD_DPAD_Y, "GAMEPAD_DPAD_Y"); //187;           
        }
    }
}

