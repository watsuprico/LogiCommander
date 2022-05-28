using LogiGraphics.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogiGraphics.Buttons {
    /// <summary>
    /// Used to listen to button events (presses, releases, holds)
    /// </summary>
    public class ButtonPoller {
        /*
         * The Logitech SDK is a bit wonky, I don't blame them, but, when a button is pressed one keyboard and a button then gets pressed on another keyboard the first keyboard's buttons drop.
         * 
         * 
         * Weird things happen when two buttons are pressed
         * 
         * 
         */


        public Button Button0 = new Button();
        public Button Button1 = new Button();
        public Button Button2 = new Button();
        public Button Button3 = new Button();

        public Button Left = new Button();
        public Button Right = new Button();
        public Button Up = new Button();
        public Button Down = new Button();
        public Button Ok = new Button();
        public Button Cancel = new Button();
        public Button Menu = new Button();


        /// <summary>
        /// Button polling background thread
        /// </summary>
        public Thread PollerThread;


        
        /// <summary>
        /// PollingRate when not suspended
        /// </summary>
        public static short ActiveStatePollingRate = 5;

        /// <summary>
        /// PollingRate when suspended
        /// </summary>
        public static short SuspendedPollingRate = 100;

        /// <summary>
        /// Time to wait before "suspending"
        /// </summary>
        public int TimeUntilSuspending = 3000; // Time (in ms) to wait before entering "suspend mode"

        /// <summary>
        /// How fast the screen and buttons are updated. "Suspended mode" increases the polling speed to 100.
        /// </summary>
        public int PollingRate = 1;

        /// <summary>
        /// Whether or not polling is running at full speed or "suspended" (I.E. background)
        /// </summary>
        private bool PollingSuspended = false;

        public ButtonPoller() {
            PollerThread = new Thread(Poller) {
                IsBackground = true,
                Name = "ButtonPoller"
            };
        }

        ~ButtonPoller() {
            if (PollerThread != null) {
                PollerThread.Abort();
            }
        }

        /// <summary>
        /// This method polls the buttons
        /// </summary>
        private void Poller() {
            bool allInactive;
            int timeInactive = 0;

            int cycleCount = 0;

            while (true) {
                Button0.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0), PollingRate);
                Button1.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1), PollingRate);
                Button2.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_2), PollingRate);
                Button3.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_3), PollingRate);

                Left.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_LEFT), PollingRate);
                Right.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_RIGHT), PollingRate);
                Up.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_UP), PollingRate);
                Down.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_DOWN), PollingRate);
                Ok.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_OK), PollingRate);
                Cancel.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_CANCEL), PollingRate);
                Menu.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_COLOR_BUTTON_MENU), PollingRate);


                allInactive = Button0.CurrentState == 0 && Button1.CurrentState == 0 && Button2.CurrentState == 0 && Button3.CurrentState == 0
                    && Left.CurrentState == 0 && Right.CurrentState == 0 && Up.CurrentState == 0 && Down.CurrentState == 0
                    && Ok.CurrentState == 0 && Cancel.CurrentState == 0 && Menu.CurrentState == 0;

                if (allInactive)
                    timeInactive += PollingRate;
                else {
                    timeInactive = 0;
                    if (PollingSuspended) {
                        PollingRate = ActiveStatePollingRate;
                        PollingSuspended = false;
                    }
                }

                if (timeInactive >= TimeUntilSuspending) {
                    PollingRate = SuspendedPollingRate;
                    PollingSuspended = true;
                }

                /*
                if (Button1.CurrentState == ButtonStates.INACTIVE) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "inactive!");
                } else if (Button1.CurrentState == ButtonStates.PRESSED) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "pressed!");
                } else if (Button1.CurrentState == ButtonStates.RELEASED) {
                    LogitechGSDK.LogiLcdMonoSetText(1, "released!");
                    LogitechGSDK.LogiLcdMonoSetText(2, "");
                } else if (Button1.CurrentState == ButtonStates.HELD) {
                    LogitechGSDK.LogiLcdMonoSetText(1, "held!");
                } else if (Button1.CurrentState == ButtonStates.HOLDING) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "holding!");
                } else if (Button1.CurrentState == ButtonStates.RELEASING) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "releasing!");
                }
                LogitechGSDK.LogiLcdMonoSetText(0, PollingSuspended ? "suspended" : "active");
                LogitechGSDK.LogiLcdUpdate();
                */
                


                cycleCount++;

                Thread.Sleep(PollingRate); // Limit polling a little bit, we don't want to destroy performance
            }
        }
    }
}
