using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogiGraphics {
    public class ButtonPoller {
        public Button Button0 = new Button();
        public Button Button1 = new Button();
        public Button Button2 = new Button();
        public Button Button3 = new Button();


        public Thread PollerThread;


        /// <summary>
        /// How fast the screen and buttons are updated. "Suspended mode" increases the polling speed to 100.
        /// </summary>
        private static int pollingSpeed = 5;
        private readonly int waitTimeBeforeSuspend = 3000; // Time (in ms) to wait before entering "suspend mode"
        private bool pollingModeSuspended = false;

        public ButtonPoller() {
            PollerThread = new Thread(Poller) {
                IsBackground = true,
                Name = "ButtonPoller"
            };
        }

        /// <summary>
        /// This method polls the buttons
        /// </summary>
        private void Poller() {
            bool allInactive;
            int timeInactive = 0;

            int cycleCount = 0;

            while (true) {
                Button0.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0));
                Button1.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1));
                Button2.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_2));
                Button3.Update(LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_3));

                allInactive = (Button0.curState == Button.ButtonState.INACTIVE && Button1.curState == Button.ButtonState.INACTIVE && Button2.curState == Button.ButtonState.INACTIVE && Button3.curState == Button.ButtonState.INACTIVE);

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

                /*if (Button1.curState == BtnState.INACTIVE) {
                    //LogitechGSDK.LogiLcdMonoSetText(2, "inactive!");
                } else if (Button1.curState == BtnState.PRESSED) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "pressed!");
                } else if (Button1.curState == BtnState.RELEASED) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "released!");
                } else if (Button1.curState == BtnState.HELD) {
                    LogitechGSDK.LogiLcdMonoSetText(2, "held!");
                } else if (Button1.curState == BtnState.HOLDING) {
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
    }
}
