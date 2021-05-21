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
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;// code to jump to next track
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;// code to play or pause a song
        public const int VK_MEDIA_PREV_TRACK = 0xB1;// code to jump to prev track


        private bool bExit = false;
        private bool nowRunning = false;


        // The display "background" (the display)
        private LGV MainImage;


        private ButtonPoller buttonPoller;


        #region Window events
        public MainWindow() {
            InitializeComponent();

            

            Editor editor = new();
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

            // load images
            String splash = File.ReadAllText("../../../splash.lcp");
            MainImage = new LGV(splash);
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

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            buttonPoller = new ButtonPoller();
            buttonPoller.PollerThread.Start();

            buttonPoller.Button1.Pressed += (object sender, EventArgs e) => {
                keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
            };
            buttonPoller.Button2.Pressed += (object sender, EventArgs e) => {
                keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
            };
            buttonPoller.Button3.Pressed += (object sender, EventArgs e) => {
                keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);
            };
        }
    }
}
