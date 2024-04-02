using LogitechSDK;
using System;
using System.Windows.Media;

namespace LogiGraphics {
    /// <summary>
    /// Contains the color and monochromatic display matrices
    /// </summary>
    public class DisplayObject {
        public byte[] monoMatrix = new byte[7041];
        public byte[] colorMatrix = new byte[307200];
    }

    /// <summary>
    /// Display controller.
    /// 
    /// API wrapper for LogitechGSDK and drawing to the lcd. Use this to draw LGVs, Images, etc
    /// </summary>
    public class Display {
        public DisplayObject DisplayObject;

        public EventHandler Drawing;

        public int xOffset = 0;
        public int yOffset = 0;

        public Display() {
            DisplayObject = new DisplayObject();
        }

        public void Draw() {
            Drawing?.Invoke(this, EventArgs.Empty);

            LogitechGSDK.LogiLcdMonoSetBackground(DisplayObject.monoMatrix);
            LogitechGSDK.LogiLcdColorSetBackground(DisplayObject.colorMatrix);
            LogitechGSDK.LogiLcdUpdate();
        }
        public void Draw(DisplayObject display) {
            DisplayObject = display;
            Draw();
        }

        #region Pixel Manipulation
        public bool[,] GetMonoScreen() {
            bool[,] scrn = new bool[160, 43];
            for (int y = 0; y < 43; y++) {
                for (int x = 0; x < 160; x++) {
                    scrn[x, y] = DisplayObject.monoMatrix[x + (y * LogitechGSDK.LOGI_LCD_MONO_WIDTH)] >= 0x80;
                }
            }
            return scrn;
        }
        public Color[,] GetColorScreen() {
            Color[,] scrn = new Color[320, 240];
            for (int y = 0; y < 240; y++) {
                for (int x = 0; x < 320; x++) {
                    scrn[x, y] = GetPixel(x, y, false);
                }
            }
            return scrn;
        }

        private int GetBytePosition(int x, int y, bool isMono = false) {
            if (isMono) {
                return x + (y * LogitechGSDK.LOGI_LCD_MONO_WIDTH);
            } else {
                return (x * 4) + (y * LogitechGSDK.LOGI_LCD_COLOR_WIDTH * 4);
            }
        }

        /// <summary>
        /// Gets the color value for a pixel
        /// </summary>
        /// <param name="x">x cord</param>
        /// <param name="y">y cord</param>
        /// <param name="isMono">Referencing the monochromatic screen?</param>
        /// <returns></returns>
        public Color GetPixel(int x, int y, bool isMono = false) {
            x += xOffset;
            y += yOffset;

            if (x < 0 || y < 0 || x > LogitechGSDK.LOGI_LCD_COLOR_WIDTH || y > LogitechGSDK.LOGI_LCD_COLOR_HEIGHT) {
                return Color.FromArgb(0, 0, 0, 0);
            }

            if (x <= LogitechGSDK.LOGI_LCD_MONO_WIDTH && y <= LogitechGSDK.LOGI_LCD_MONO_HEIGHT && isMono) {
                if (DisplayObject.monoMatrix[GetBytePosition(x, y)] >= 0x80) {
                    // Active
                    return Color.FromArgb(255, 1, 1, 1);
                } else {
                    return Color.FromArgb(0, 0, 0, 0);
                }
            } else {
                Color color = new Color();
                int pos = GetBytePosition(x, y);

                color.B = DisplayObject.colorMatrix[pos];
                color.G = DisplayObject.colorMatrix[pos + 1];
                color.R = DisplayObject.colorMatrix[pos + 2];
                color.A = DisplayObject.colorMatrix[pos + 3];
                return color;
            }
        }

        /// <summary>
        /// Sets the color value for a pixel
        /// </summary>
        /// <param name="x">x cord</param>
        /// <param name="y">y cord</param>
        /// <param name="color">The color value (a channel value at or above 180 actives the mono pixel)</param>
        public void SetPixel(int x, int y, Color color, bool noOffset = false) {
            if (!noOffset) {
                x += xOffset;
                y += yOffset;
            }

            // Check bounds
            if (x < 0 || y < 0 || x > LogitechGSDK.LOGI_LCD_COLOR_WIDTH || y > LogitechGSDK.LOGI_LCD_COLOR_HEIGHT)
                return; // way too big

            if (color.R < 0)
                color.R = 0;
            if (color.G < 0)
                color.G = 0;
            if (color.B < 0)
                color.B = 0;
            if (color.A < 0)
                color.A = 0;

            if (color.R > 255)
                color.R = 255;
            if (color.G > 255)
                color.G = 255;
            if (color.B > 255)
                color.B = 255;
            if (color.A > 255)
                color.A = 255;

            // Set mono pixel
            if (x <= LogitechGSDK.LOGI_LCD_MONO_WIDTH && y <= LogitechGSDK.LOGI_LCD_MONO_HEIGHT) {
                byte ourGuy = 0x00;
                if ((color.R >= 180 || color.G >= 180 || color.B >= 180) && color.A > 180)
                    ourGuy = 0x80; // 180 = active pixel
                DisplayObject.monoMatrix[GetBytePosition(x, y, true)] = ourGuy;
            }


            // Set color pixel
            /*
             * Colors stored as this (idk bro):
             * | PIXEL 1 | PIXEL 2 |
             * | B,G,R,A | B,G,R,A | ...
             */

            int pos = GetBytePosition(x, y);
            DisplayObject.colorMatrix[pos] = color.B;
            DisplayObject.colorMatrix[pos + 1] = color.G;
            DisplayObject.colorMatrix[pos + 2] = color.R;
            DisplayObject.colorMatrix[pos + 3] = color.A;
        }
        public void SetPixel(int x, int y, int r, int g, int b, int alpha) {
            SetPixel(x, y, Color.FromArgb((byte)alpha, (byte)r, (byte)g, (byte)b));
        }
        public void SetPixel(int x, int y, bool active) {
            int c = active ? 255 : 0;
            SetPixel(x, y, c, c, c, c);
        }

        public void SetPixels(Point[] points) {
            foreach (Point p in points) {
                SetPixel(p.X, p.Y, p.Color);
            }
        }
        public void SetPixels(byte[,,] pixels) {
            int channels = pixels.GetLength(0);
            int layerWidth = pixels.GetLength(1);
            int layerHeight = pixels.GetLength(2);
            int pos;

            for (int y = 0; y < layerHeight; y++) {
                for (int x = 0; x < layerWidth; x++) {
                    //int X = y - Layers[i].xOffset;
                    //int Y = y - Layers[i].yOffset;
                    if (x <= LogitechGSDK.LOGI_LCD_MONO_WIDTH && y <= LogitechGSDK.LOGI_LCD_MONO_HEIGHT)
                        DisplayObject.monoMatrix[GetBytePosition(x, y, true)] = pixels[0, x, y];

                    pos = GetBytePosition(x, y);
                    if (channels == 4) {
                        DisplayObject.colorMatrix[pos] = pixels[3, x, y];     // b
                        DisplayObject.colorMatrix[pos + 1] = pixels[2, x, y]; // g
                        DisplayObject.colorMatrix[pos + 2] = pixels[1, x, y]; // r
                        DisplayObject.colorMatrix[pos + 3] = pixels[0, x, y]; // a
                    }
                }
            }
        }


        public void Clear(Color color) {
            byte active = (byte)(color.A > 0 ? 0x80 : 0x00);
            for (int i = 0; i < DisplayObject.monoMatrix.Length; i++) {
                DisplayObject.monoMatrix[i] = active;
            }

            for (int i = 0; i < DisplayObject.colorMatrix.Length; i += 4) {
                DisplayObject.colorMatrix[i] = color.B;
                DisplayObject.colorMatrix[i + 1] = color.G;
                DisplayObject.colorMatrix[i + 2] = color.R;
                DisplayObject.colorMatrix[i + 3] = color.A;

            }
        }
        public void Clear() {
            Clear(Color.FromArgb(0, 0, 0, 0));
        }

        public void EnablePixel(int x, int y) {
            SetPixel(x, y, 255, 255, 255, 255);
        }
        public void EnablePixel(float x, float y) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), 255, 255, 255, 255);
        }
        public void DisablePixel(int x, int y) {
            SetPixel(x, y, 0, 0, 0, 0);
        }
        public void DisablePixel(float x, float y) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), 0, 0, 0, 0);
        }

        public void LoadImage(Image img) {
            DisplayObject = img.GetDisplayComposite();
        }
        #endregion
    }
}
