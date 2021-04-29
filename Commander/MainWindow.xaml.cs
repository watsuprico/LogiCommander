using LogiGraphics;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace LogiCommand {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        /*
         * [x,y,active][x1,y1,active1]...[xN,yN,activeN]
         * 
         * {command,[x,y,active][x1,y1,active1]...[xN,yN,activeN]}
         * 
         * /ignore me/
         * 
         * 
         */

        [DllImport("user32.dll")]
        public static extern void Keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;// code to jump to next track
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;// code to play or pause a song
        public const int VK_MEDIA_PREV_TRACK = 0xB1;// code to jump to prev track


        private bool bExit = false;
        private bool nowRunning = false;
        private bool isMono = false;
        /// <summary>
        /// How fast the screen and buttons are updated. "Suspended mode" increases the polling speed to 100.
        /// </summary>
        private static int pollingSpeed = 5;
        private readonly int waitTimeBeforeSuspend = 3000; // Time (in ms) to wait before entering "suspend mode"
        private bool pollingModeSuspended = false;


        // The display "background" (the display)
        private byte[] displayMatrix;
        private bool matrixChanged = true;
        private LGV MainImage;


        private Thread pollThread;

        // Amount of time needed to pass from a button being pressed to being held
        private readonly static int pressedToHeldTime = 500;
        enum BtnState {
            /// <summary>
            /// Not pressed/held, waiting
            /// </summary>
            INACTIVE,
            /// <summary>
            /// Button has just been released
            /// </summary>
            RELEASED,
            /// <summary>
            /// Button just pressed
            /// </summary>
            PRESSED,
            /// <summary>
            /// Has just been held for pressedToHeldTime
            /// </summary>
            HELD,
            /// <summary>
            /// Being constantly held
            /// </summary>
            HOLDING,
        }
        class LogiButton {
            private int _pressedTime = 0; // Time _held has been true (ms)
            private bool _held = false; // internal 'held'

            public bool IsInactive {
                get { return curState == BtnState.INACTIVE; }
            }
            public bool IsPressed {
                get { return curState == BtnState.PRESSED; }
            }
            public bool IsReleased {
                get { return curState == BtnState.RELEASED; }
            }
            public bool IsHeld {
                get { return curState == BtnState.HELD; }
            }
            public bool IsHolding {
                get { return curState == BtnState.HOLDING; }
            }

            public event EventHandler Inactive;
            public event EventHandler Pressed;
            public event EventHandler Released;
            public event EventHandler Held;
            public event EventHandler Holding;

            public BtnState curState = BtnState.INACTIVE;

            public LogiButton Update(bool btnPressed) {
                if (btnPressed) {
                    if (curState == BtnState.INACTIVE && !_held) {
                        curState = BtnState.PRESSED;

                        if (Pressed != null)
                            Pressed.Invoke(this, EventArgs.Empty);
                    } else if (_held)
                        _pressedTime += pollingSpeed;

                    else if (curState == BtnState.PRESSED) {
                        _pressedTime += pollingSpeed;
                        _held = true;
                        curState = BtnState.INACTIVE;

                        if (Inactive != null)
                            Inactive.Invoke(this, EventArgs.Empty);

                    } else if (curState == BtnState.HELD) {
                        curState = BtnState.HOLDING;

                        if (Holding != null)
                            Holding.Invoke(this, EventArgs.Empty);

                    }

                } else {
                    if (_held)
                        curState = BtnState.RELEASED;

                    else if (curState != BtnState.INACTIVE) {
                        if (curState == BtnState.RELEASED) {
                            curState = BtnState.INACTIVE;

                            if (Inactive != null)
                                Inactive.Invoke(this, EventArgs.Empty);

                        } else {
                            curState = BtnState.RELEASED;

                            if (Released != null)
                                Released.Invoke(this, EventArgs.Empty);

                        }
                    }

                    _held = false;
                    _pressedTime = 0;

                    return this;
                }

                if (_pressedTime > pressedToHeldTime) {
                    _pressedTime = 0;
                    curState = BtnState.HELD;

                    if (Held != null)
                        Held.Invoke(this, EventArgs.Empty);

                    _held = false;
                }

                return this;
            }
        }
        private LogiButton btn0 = new();
        private LogiButton btn1 = new();
        private LogiButton btn2 = new();
        private LogiButton btn3 = new();


        #region Window events
        public MainWindow() {
            InitializeComponent();

            pollThread = new Thread(ButtonPoller) {
                IsBackground = true,
                Name = "BtnPoller"
            };

            Editor editor = new Editor();
            editor.Show();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (bExit) {
                LogitechGSDK.LogiLcdShutdown();
            } else {
                e.Cancel = true;
            }
        }
        #endregion


        #region Screen stuff
        /// <summary>
        /// This method polls the buttons
        /// </summary>
        private void ButtonPoller() {
            bool allInactive = false;
            int timeInactive = 0;

            int cycleCount = 0;

            while (!bExit) {
                btn0.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0));
                btn1.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1));
                btn2.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_2));
                btn3.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_3));

                allInactive = (btn0.curState == BtnState.INACTIVE && btn1.curState == BtnState.INACTIVE && btn2.curState == BtnState.INACTIVE && btn3.curState == BtnState.INACTIVE);

                if (allInactive)
                    timeInactive += pollingSpeed;
                else {
                    timeInactive = 0;
                    if (pollingModeSuspended) {
                        pollingSpeed = 5;
                        pollingModeSuspended = false;
                    }
                }

                if (timeInactive >= waitTimeBeforeSuspend) {
                    pollingSpeed = 100;
                    pollingModeSuspended = true;
                }

                /*if (btn1.curState == BtnState.INACTIVE) {
                    //LogitechGSDK.LogiLcdMonoSetText(2, "inactive!");
                } else if (btn1.curState == BtnState.PRESSED) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "pressed!");
                } else if (btn1.curState == BtnState.RELEASED) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "released!");
                } else if (btn1.curState == BtnState.HELD) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "held!");
                } else if (btn1.curState == BtnState.HOLDING) {
                    //LogitechGSDK.LogiLcdMonoSetText(2, "holding!");
                }*/

                //LogitechGSDK.LogiLcdMonoSetText(0, (pollingModeSuspended) ? "suspended" : "active");


                cycleCount++;


                if (cycleCount == 20) {
                    cycleCount = 0;

                    LogitechGSDK.LogiLcdUpdate(); // Update display
                }




                Thread.Sleep(pollingSpeed); // Limit polling a little bit, we don't want to destroy performance
            }
        } 

        /// <summary>
        /// Draws an outline using multiple points (connect the dots)
        /// </summary>
        /// <param name="sqr">The points we're connecting</param>
        /// <returns></returns>
        private LogiGraphics.Point[] RenderOutline(LogiGraphics.Point[] sqr) {
            if (sqr.Length < 3)
                return new LogiGraphics.Point[0];

            LogiGraphics.Point[] points = new LogiGraphics.Point[0];
            // all we do is draw 4 lines :^)
            for (int i = 0; i < sqr.Length; i++) {
                LogiGraphics.Point[] tmp = new LogiGraphics.Point[2];
                tmp[0] = sqr[i];
                if (i + 1 >= sqr.Length)
                    tmp[1] = sqr[0];
                else
                    tmp[1] = sqr[i + 1];

                //points = AddPoints(points, RenderLine(tmp));

            }

            return points;
        }

        public void DrawImage(string img) {
            new LGV(img);
        }
        #endregion


        #region UI Logic


        private void btnBegin_Click(object sender, RoutedEventArgs e) {
            if (nowRunning)
                return;


            if (!LogitechGSDK.LogiLcdInit("Spotlet", LogitechGSDK.LOGI_LCD_TYPE_MONO | LogitechGSDK.LOGI_LCD_TYPE_COLOR))
                MessageBox.Show("Applet failed to start!", "Failure.", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            nowRunning = true;

            //LogitechGSDK.LogiLcdMonoSetText(0, "...");

            if (LogitechGSDK.LogiLcdIsConnected(LogitechGSDK.LOGI_LCD_TYPE_MONO)) {
                displayMatrix = new byte[LogitechGSDK.LOGI_LCD_MONO_WIDTH * LogitechGSDK.LOGI_LCD_MONO_HEIGHT * 4];
                isMono = true;
            } else {
                displayMatrix = new byte[LogitechGSDK.LOGI_LCD_COLOR_WIDTH * LogitechGSDK.LOGI_LCD_COLOR_HEIGHT * 4];
            }

            pollThread.Start();

            // load images
            String splash = File.ReadAllText("../../../splash.lcp");
            MainImage = new LGV(splash);

            btn0.Pressed += UpdateImage;
            btn3.Pressed += ClearImage;
        }

        private void ClearImage(object sender, EventArgs e) {
            for (int x = 0; x < LogitechGSDK.LOGI_LCD_MONO_WIDTH; x++) {
                for (int y = 0; y < LogitechGSDK.LOGI_LCD_MONO_HEIGHT; y++) {
                    MainImage.DisablePixel(x, y);
                }
            }
            MainImage.Draw();
        }

        private void UpdateImage(object sender, EventArgs e) {
            String splash = File.ReadAllText("../../../splash.lcp");
            MainImage.Draw(splash);
        }

        private void bntExit_Click(object sender, RoutedEventArgs e) {
            bExit = true;
            this.Close();
        }


        #endregion

        private void Button_Click(object sender, RoutedEventArgs e) {
            PixelEditor pEditor = new PixelEditor();
            pEditor.PixelWidth = LogitechGSDK.LOGI_LCD_MONO_WIDTH;
            pEditor.PixelHeight = LogitechGSDK.LOGI_LCD_MONO_HEIGHT;

            Editor editor = new Editor();
            editor.Show();
        }
    }
}
