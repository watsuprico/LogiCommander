using System;
using System.Runtime.InteropServices;
/// <summary>
/// The LogitechGSDK as provided inside their "C# Instructions.pdf" file inside the documentation of their Gaming API.
/// </summary>
namespace LogitechSDK {
    public class DirectLogitechGSDK {
        //LCD	SDK
        public const int LOGI_LCD_COLOR_BUTTON_LEFT = (0x00000100);
        public const int LOGI_LCD_COLOR_BUTTON_RIGHT = (0x00000200);
        public const int LOGI_LCD_COLOR_BUTTON_OK = (0x00000400);
        public const int LOGI_LCD_COLOR_BUTTON_CANCEL = (0x00000800);
        public const int LOGI_LCD_COLOR_BUTTON_UP = (0x00001000);
        public const int LOGI_LCD_COLOR_BUTTON_DOWN = (0x00002000);
        public const int LOGI_LCD_COLOR_BUTTON_MENU = (0x00004000);
        public const int LOGI_LCD_MONO_BUTTON_0 = (0x00000001);
        public const int LOGI_LCD_MONO_BUTTON_1 = (0x00000002);
        public const int LOGI_LCD_MONO_BUTTON_2 = (0x00000004);
        public const int LOGI_LCD_MONO_BUTTON_3 = (0x00000008);
        public const int LOGI_LCD_MONO_WIDTH = 160;
        public const int LOGI_LCD_MONO_HEIGHT = 43;
        public const int LOGI_LCD_COLOR_WIDTH = 320;
        public const int LOGI_LCD_COLOR_HEIGHT = 240;
        public const int LOGI_LCD_TYPE_MONO = (0x00000001);
        public const int LOGI_LCD_TYPE_COLOR = (0x00000002);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdInit(String friendlyName, int lcdType);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdIsConnected(int lcdType);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdIsButtonPressed(int button);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLcdUpdate();
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLcdShutdown();
        //	Monochrome	LCD	functions
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdMonoSetBackground(byte[] monoBitmap);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdMonoSetText(int lineNumber, String text);
        //	Color	LCD	functions
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetBackground(byte[] colorBitmap);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetTitle(String text, int red, int green,
        int blue);
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetText(int lineNumber, String text, int red,
        int green, int blue);


        /*
         * 
         *  === BEGIN ===
         * 
         *    G-KEY SDK
         * 
         */


        //G-KEY	SDK
        public const int LOGITECH_MAX_MOUSE_BUTTONS = 20;
        public const int LOGITECH_MAX_GKEYS = 29;
        public const int LOGITECH_MAX_M_STATES = 3;
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GkeyCode {
            public ushort complete;
            //	index	of	the	G	key	or	mouse	button,	for	example,	6	for	G6	or	Button	6
            public int keyIdx {
                get {
                    return complete & 255;
                }
            }
            //	key	up	or	down,	1	is	down,	0	is	up
            public int keyDown {
                get {
                    return (complete >> 8) & 1;
                }
            }
            //	mState	(1,	2	or	3	for	M1,	M2	and	M3)
            public int mState {
                get {
                    return (complete >> 9) & 3;
                }
            }

            //	indicate	if	the	Event	comes	from	a	mouse,	1	is	yes,	0	is	no.
            public int Mouse {
                get {
                    return (complete >> 11) & 15;
                }
            }
            //	reserved1
            public int Reserved1 {
                get {
                    return (complete >> 15) & 1;
                }
            }
            //	reserved2
            public int Reserved2 {
                get {
                    return (complete >> 16) & 131071;
                }
            }
        }
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void logiGkeyCB(GkeyCode gkeyCode,
        [MarshalAs(UnmanagedType.LPWStr)] String gkeyOrButtonString, IntPtr context);   //	??
        [DllImport("LogitechGkeyEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]

        public static extern int LogiGkeyInitWithoutCallback();
        [DllImport("LogitechGkeyEnginesWrapper	", CharSet = CharSet.Unicode,
            CallingConvention = CallingConvention.Cdecl)]

        public static extern int LogiGkeyInitWithoutContext(logiGkeyCB gkeyCB);
        [DllImport("LogitechGkeyEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]

        public static extern int LogiGkeyIsMouseButtonPressed(int buttonNumber);
        [DllImport("LogitechGkeyEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LogiGkeyGetMouseButtonString(int buttonNumber);
        public static String LogiGkeyGetMouseButtonStr(int buttonNumber) {
            String str =
            Marshal.PtrToStringUni(LogiGkeyGetMouseButtonString(buttonNumber));
            return str;
        }
        [DllImport("LogitechGkeyEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern int LogiGkeyIsKeyboardGkeyPressed(int gkeyNumber, int
        modeNumber);
        [DllImport("LogitechGkeyEnginesWrapper")]
        public static extern IntPtr LogiGkeyGetKeyboardGkeyString(int gkeyNumber, int
        modeNumber);
        public static String LogiGkeyGetKeyboardGkeyStr(int gkeyNumber, int modeNumber) {
            String str = Marshal.PtrToStringUni(LogiGkeyGetKeyboardGkeyString(gkeyNumber, modeNumber));
            return str;
        }
        [DllImport("LogitechGkeyEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiGkeyShutdown();
    }
}
