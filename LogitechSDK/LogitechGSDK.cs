using System;
using System.Runtime.InteropServices;

namespace LogitechSDK {
    /// <summary>
    /// The LogitechGSDK as provided inside their "C# Instructions.pdf" file inside the documentation of their Gaming API.
    /// 
    /// Slight modifications have been made, mainly including documentation notes from their SDK docs. (C# Intellisense)
    /// </summary>
    ///
    public class LogitechGSDK {
        //LCD SDK

        /// <summary>
        /// Color LCD's left arrow button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_LEFT = 0x00000100;

        /// <summary>
        /// Color LCD's right arrow button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_RIGHT = 0x00000200;

        /// <summary>
        /// Color LCD's OK (select) button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_OK = 0x00000400;

        /// <summary>
        /// Color LCD's cancel (back) button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_CANCEL = 0x00000800;

        /// <summary>
        /// Color LCD's up arrow button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_UP = 0x00001000;

        /// <summary>
        /// Color LCD's down arrow button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_DOWN = 0x00002000;

        /// <summary>
        /// Color LCD's menu button.
        /// </summary>
        public const int LOGI_LCD_COLOR_BUTTON_MENU = 0x00004000;

        /// <summary>
        /// Monochrome LCD's left most button.
        /// </summary>
        public const int LOGI_LCD_MONO_BUTTON_0 = 0x00000001;

        /// <summary>
        /// To the right of the left most button for the monochrome LCD.
        /// </summary>
        public const int LOGI_LCD_MONO_BUTTON_1 = 0x00000002;

        /// <summary>
        /// To the left of the right most button for the monochrome LCD.
        /// </summary>
        public const int LOGI_LCD_MONO_BUTTON_2 = 0x00000004;

        /// <summary>
        /// Monochrome LCD's right most button.
        /// </summary>
        public const int LOGI_LCD_MONO_BUTTON_3 = 0x00000008;

        /// <summary>
        /// Width of the monochrome LCD display.
        /// </summary>
        public const int LOGI_LCD_MONO_WIDTH = 160;
        /// <summary>
        /// Height of the monochrome LCD display.
        /// </summary>
        public const int LOGI_LCD_MONO_HEIGHT = 43;
        /// <summary>
        /// Width of the color LCD display.
        /// </summary>
        public const int LOGI_LCD_COLOR_WIDTH = 320;
        /// <summary>
        /// Height of the color LCD display.
        /// </summary>
        public const int LOGI_LCD_COLOR_HEIGHT = 240;
        /// <summary>
        /// Monochrome LCD display.
        /// </summary>
        public const int LOGI_LCD_TYPE_MONO = 0x00000001;
        /// <summary>
        /// Color LCD display.
        /// </summary>
        public const int LOGI_LCD_TYPE_COLOR = 0x00000002;


        /// <summary>
        /// LCD buttons.
        /// </summary>
        [Flags]
        public enum Button {
            /// <inheritdoc cref="LOGI_LCD_MONO_BUTTON_0"/>
            Button0 = LOGI_LCD_MONO_BUTTON_0,
            /// <inheritdoc cref="LOGI_LCD_MONO_BUTTON_1"/>
            Button1 = LOGI_LCD_MONO_BUTTON_1,
            /// <inheritdoc cref="LOGI_LCD_MONO_BUTTON_2"/>
            Button2 = LOGI_LCD_MONO_BUTTON_2,
            /// <inheritdoc cref="LOGI_LCD_MONO_BUTTON_3"/>
            Button3 = LOGI_LCD_MONO_BUTTON_3,

            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_LEFT"/>
            Left = LOGI_LCD_COLOR_BUTTON_LEFT,
            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_RIGHT"/>
            Right = LOGI_LCD_COLOR_BUTTON_RIGHT,
            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_OK"/>
            Okay = LOGI_LCD_COLOR_BUTTON_OK,
            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_CANCEL"/>
            Cancel = LOGI_LCD_COLOR_BUTTON_CANCEL,
            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_UP"/>
            Up = LOGI_LCD_COLOR_BUTTON_UP,
            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_DOWN"/>
            Down = LOGI_LCD_COLOR_BUTTON_DOWN,
            /// <inheritdoc cref="LOGI_LCD_COLOR_BUTTON_MENU"/>
            Menu = LOGI_LCD_COLOR_BUTTON_MENU,
        }


        #region General control
        /// <summary>
        /// Makes necessary initializations. You must call this function prior to any other function in the library.
        /// 
        /// (This registers your application as an "applet").
        /// </summary>
        /// <param name="friendlyName">Name of your applet. You cannot change it after initialization.</param>
        /// <param name="lcdType">
        ///     <para>
        ///         Defines the type of your applet LCD target, it can be one of the following:
        ///     </para>
        ///     <para>
        ///         <c>LOGI_LCD_TYPE_MONO</c>, <c>LOGI_LCD_TYPE_COLOR</c>.
        ///     </para>
        ///     <para>
        ///         If you want to initialize your applet for both LCD types just use:
        ///         <br/>
        ///         <c>LOGI_LCD_TYPE_MONO | LOGI_LCD_TYPE_COLOR</c>.
        ///     </para>
        /// </param>
        /// <returns>If the function succeeds, it returns true. Otherwise false.</returns>
        public static bool LogiLcdInit(string friendlyName, int lcdType) {
            return DirectLogitechGSDK.LogiLcdInit(friendlyName, lcdType);
        }

        /// <summary>
        /// Checks if a device of the type specified by the parameter is connected.
        /// </summary>
        /// <remarks>
        /// Requires that <see cref="LogiLcdInit(string, int)"/> be called first.
        /// </remarks>
        /// <param name="lcdType">
        ///     <para>
        ///         Defines the type of your applet LCD target, it can be one of the following:
        ///     </para>
        ///     <para>
        ///         <c>LOGI_LCD_TYPE_MONO</c>, <c>LOGI_LCD_TYPE_COLOR</c>.
        ///     </para>
        ///     <para>
        ///         If you want to initialize your applet for both LCD types just use:
        ///         <br/>
        ///         <c>LOGI_LCD_TYPE_MONO | LOGI_LCD_TYPE_COLOR</c>.
        ///     </para>
        /// </param>
        /// <returns>If a device supporting the lcd type specified is found, it returns true. If the device has not been found or the LogiLcdInit function has not been called before, returns false.</returns>
        public static bool LogiLcdIsConnected(int lcdType) {
            return DirectLogitechGSDK.LogiLcdIsConnected(lcdType);
        }

        /// <summary>
        /// Checks if the button specified by the parameter is being pressed.
        /// </summary>
        /// <param name="button">
        ///     <para>
        ///         Defines the button to check on, it can be one of the following:    
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <term><c>LOGI_LCD_MONO_BUTTON_0</c></term>
        ///             <description>Left most button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_MONO_BUTTON_1</c></term>
        ///             <description>To the right of the left most button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_MONO_BUTTON_2</c></term>
        ///             <description>To the left of the right most button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_MONO_BUTTON_3</c></term>
        ///             <description>Right most button</description>
        ///         </item>
        ///         
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_LEFT</c></term>
        ///             <description>Left arrow button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_RIGHT</c></term>
        ///             <description>Right arrow button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_OK</c></term>
        ///             <description>OK (select) button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_CANCEL</c></term>
        ///             <description>Cancel (back) button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_UP</c></term>
        ///             <description>Up arrow button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_DOWN</c></term>
        ///             <description>Down arrow button</description>
        ///         </item>
        ///         <item>
        ///             <term><c>LOGI_LCD_COLOR_BUTTON_MENU</c></term>
        ///             <description>Menu button</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <returns>If the button specified is being pressed it returns true. Otherwise false.</returns>
        public static bool LogiLcdIsButtonPressed(int button) {
            return DirectLogitechGSDK.LogiLcdIsButtonPressed(button);
        }

        /// <inheritdoc cref="LogiLcdIsButtonPressed(int)"/>
        public static bool LogiLcdIsButtonPressed(Button button) => LogiLcdIsButtonPressed((int)button);

        /// <summary>
        /// Updates the LCD display.
        /// </summary>
        public static void LogiLcdUpdate() {
            DirectLogitechGSDK.LogiLcdUpdate();
        }

        /// <summary>
        /// Kills the applet and frees memory used by the SDK.
        /// </summary>
        public static void LogiLcdShutdown() {
            DirectLogitechGSDK.LogiLcdShutdown();
        }
        #endregion

        #region Monochrome LCD
        /// <summary>
        /// Sets the specified image as background for the monochrome LCD device connected.
        /// </summary>
        /// <param name="monoBitmap">
        ///     <para>
        ///         Array of pixels that define the actual monochrome bitmap.
        ///     </para>
        ///     <para>
        ///         The array of pixels is organized as a rectangular area, 160 bytes wide and 43 bytes high.<br/>
        ///         Despite the display being monochrome, 8 bits per pixel are used here for simple manipulation individual pixels.<br/>
        ///         To learn how to use GDI drawing functions efficiently with such an arrangement, see the sample code.
        ///     </para>
        ///     <remarks>The image size must be 160x43 in order to use this function. The SDK will turn on the pixel on the screen if the value assigned to that byte is &gt;= 128, it will remain off if the value is &lt;128</remarks>
        /// </param>
        /// <returns></returns>
        public static bool LogiLcdMonoSetBackground(byte[] monoBitmap) {
            return DirectLogitechGSDK.LogiLcdMonoSetBackground(monoBitmap);
        }

        /// <summary>
        /// Sets the specified text in the requested line on the monochrome lcd device connected.
        /// </summary>
        /// <param name="lineNumber">The line on the screen you want the text to appear. The monochrome lcd display has 4 lines, so this parameter can be any number from 0 to 3.</param>
        /// <param name="text">Text you want to display.</param>
        /// <returns></returns>
        public static bool LogiLcdMonoSetText(int lineNumber, string text) {
            return DirectLogitechGSDK.LogiLcdMonoSetText(lineNumber, text);
        }
        #endregion

        #region Color LCD
        /// <summary>
        /// Sets the specified image as background for the color lcd device connected.
        /// </summary>
        /// <param name="colorBitmap">
        ///     <para>
        ///         The array of pixels that define the actual color bitmap.<br/>
        ///         <c>colorBitmap[0]</c> = Blue @(0,0)<br/>
        ///         <c>colorBitmap[1]</c> = Green @(0,0)<br/>
        ///         <c>colorBitmap[2]</c> =  Red @(0,0)<br/>
        ///         <c>colorBitmap[2]</c> = Alpha channel @(0,0)<br/>
        ///         
        ///         If you offset the index by (x*4) you can address the pixel @(x, 0)<br/>
        ///         If you offset the index by (y*320*4) you can address the pixel @(0, y)<br/>
        ///         Use this formula to calculate an appropriate offset [(x*4) + (y*320*4)] for pixel @(x, y)
        ///     </para>
        ///     <para>
        ///         The array of pixels is organized as a rectangular area, 320 bytes wide and 240 bytes high.<br/>
        ///         Since the color lcd can display the full RGB gamma, 32 bits per pixel(4 bytes) are used.<br/>
        ///         The size of the colorBitmap array has to be 320x240x4 = 307200 therefore.<br/>
        ///         To learn how to use GDI drawing functions efficiently with such an arrangement, see the sample code.
        ///     </para>
        ///     <para>
        ///         32 bit values are stored in 4 consecutive bytes that represent the RGB color values for that pixel.<br/>
        ///         These values use the same top left to bottom right raster style transform to the flat character array with the exception that each pixel value is specified using 4 consecutive bytes.<br/>
        ///     </para>
        ///     <para>
        ///         Each of the bytes in the RGB quad specify the intensity of the given color. The value ranges from 0 (the darkest color value) to 255 (brightest color value).
        ///     </para>
        /// </param>
        /// <returns>True if it succeeds, false otherwise.</returns>
        public static bool LogiLcdColorSetBackground(byte[] colorBitmap) {
            return DirectLogitechGSDK.LogiLcdColorSetBackground(colorBitmap);
        }

        /// <summary>
        /// sets the specified text in the first line on the color lcd device connected.
        /// The font size that will be displayed is bigger than the one used in the other lines, so you can use this function to set the title of your applet/page.
        /// </summary>
        /// <param name="text">Text you want to display as title</param>
        /// <param name="red">Red value for the text</param>
        /// <param name="green">Green value for the text</param>
        /// <param name="blue">Blue value for the text</param>
        /// <returns></returns>
        public static bool LogiLcdColorSetTitle(string text, int red = 255, int green = 255, int blue = 255) {
            return DirectLogitechGSDK.LogiLcdColorSetTitle(text, red, green, blue);
        }

        /// <summary>
        /// Sets the specified text in the requested line on the color lcd device connected.
        /// </summary>
        /// <param name="lineNumber">Line on the screen you want the text to appear. The color lcd display has 8 lines for standard text, so this parameter can be any number from 0 to 7.</param>
        /// <param name="text">Text to display</param>
        /// <param name="red">Red value for the text</param>
        /// <param name="green">Green value for the text</param>
        /// <param name="blue">Blue value for the text</param>
        /// <returns></returns>
        public static bool LogiLcdColorSetText(int lineNumber, string text, int red = 255, int green = 255, int blue = 255) {
            return DirectLogitechGSDK.LogiLcdColorSetText(lineNumber, text, red, green, blue);
        }
        #endregion




        #region G-Key SDK
        /// <summary>
        /// Maximum support mouse buttons
        /// </summary>
        public const int LOGITECH_MAX_MOUSE_BUTTONS = 20;
        /// <summary>
        /// Maximum support G-Key buttons
        /// </summary>
        public const int LOGITECH_MAX_GKEYS = 29;
        /// <summary>
        /// Maximum supported keyboard "modes"
        /// </summary>
        public const int LOGITECH_MAX_M_STATES = 3;
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GkeyCode {
            public ushort complete;
            /// <summary>
            /// Index of the G-key or mouse button. For example, 6 for G6 or Button 6
            /// </summary>
            public readonly int keyIdx => complete & 255;

            /// <summary>
            /// Key up or down. 1 is down, 0 is up.
            /// </summary>
            public readonly int keyDown => (complete >> 8) & 1;

            /// <summary>
            /// MState (mode) (1, 2 or 3 for Mode 1, Mode 2, and Mode 3)
            /// </summary>
            public readonly int mState => (complete >> 9) & 3;

            /// <summary>
            /// Indicate if the event comes from a mouse (1) or not (0).
            /// </summary>
            public readonly int Mouse => (complete >> 11) & 15;
            //	reserved1

            /// <summary>
            /// Reserved1 (unknown).
            /// </summary>
            public readonly int Reserved1 => (complete >> 15) & 1;

            /// <summary>
            /// Reserved2 (unknown).
            /// </summary>
            public readonly int Reserved2 => (complete >> 16) & 131071;
        }


        /// <summary>
        /// The LogiGkeyInit() function initializes the G-key SDK. It must be called before your application can see G-key/button events.
        /// </summary>
        /// <returns>If the function succeeds, it returns TRUE. Otherwise FALSE.</returns>
        public static int LogiGkeyInitWithoutCallback() {
            return DirectLogitechGSDK.LogiGkeyInitWithoutCallback();
        }

        /// <summary>
        /// Undocumented? Check page 17 boss. Says this should be used for Unity, but gives no documentation for the parameter.
        /// </summary>
        /// <param name="gkeyCB">Your guess is as good as mine bud</param>
        /// <returns></returns>
        public static int LogiGkeyInitWithoutContext(DirectLogitechGSDK.logiGkeyCB gkeyCB) {
            return DirectLogitechGSDK.LogiGkeyInitWithoutContext(gkeyCB);
        }

        /// <summary>
        /// Indicates whether a mouse button is currently being pressed
        /// </summary>
        /// <param name="buttonNumber">Number of the button to check (for example between 6 and 20 for G600).</param>
        /// <returns></returns>
        public static int LogiGkeyIsMouseButtonPressed(int buttonNumber) {
            return DirectLogitechGSDK.LogiGkeyIsMouseButtonPressed(buttonNumber);
        }

        private static nint LogiGkeyGetMouseButtonString(int buttonNumber) {
            return DirectLogitechGSDK.LogiGkeyGetMouseButtonString(buttonNumber);
        }
        /// <summary>
        /// Returns a button-specific friendly string. (Button number -> string)
        /// </summary>
        /// <param name="buttonNumber">Number of the button to check (for example between 6 and 20 for G600).</param>
        /// <returns>Friendly string for specified button number. For example "Mouse Btn 8".</returns>
        public static string LogiGkeyGetMouseButtonStr(int buttonNumber) {
            string str =
            Marshal.PtrToStringUni(LogiGkeyGetMouseButtonString(buttonNumber));
            return str;
        }

        /// <summary>
        /// Indicates whether a keyboard G-key is currently being pressed
        /// </summary>
        /// <param name="gkeyNumber">Number of the G-key to check (for example between 1 and 6 for G710).</param>
        /// <param name="modeNumber">Number of the mode currently selected (1, 2 or 3)</param>
        /// <returns>TRUE if the specified G-key for the specified Mode is currently being pressed, FALSE otherwise.</returns>
        public static int LogiGkeyIsKeyboardGkeyPressed(int gkeyNumber, int modeNumber) {
            return DirectLogitechGSDK.LogiGkeyIsKeyboardGkeyPressed(gkeyNumber, modeNumber);
        }


        private static nint LogiGkeyGetKeyboardGkeyString(int gkeyNumber, int modeNumber) {
            return DirectLogitechGSDK.LogiGkeyGetKeyboardGkeyString(gkeyNumber, modeNumber);
        }
        /// <summary>
        /// Returns a G-key-specific friendly string.
        /// </summary>
        /// <param name="gkeyNumber">Number of the G-key to check (for example between 1 and 6 for G710).</param>
        /// <param name="modeNumber">Number of the mode currently selected (1, 2 or 3)</param>
        /// <returns>Friendly string for specified G-key and Mode number. For example "G5/M1".</returns>
        public static string LogiGkeyGetKeyboardGkeyStr(int gkeyNumber, int modeNumber) {
            string str = Marshal.PtrToStringUni(LogiGkeyGetKeyboardGkeyString(gkeyNumber, modeNumber));
            return str;
        }

        /// <summary>
        /// Unloads the corresponding DLL and frees up any allocated resources.
        /// </summary>
        public static void LogiGkeyShutdown() {
            DirectLogitechGSDK.LogiGkeyShutdown();
        }
        #endregion
    }
}