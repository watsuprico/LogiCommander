using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace LogiGraphics {
    /// <summary>
    /// LogiGraphics View. This is 
    /// </summary>
    public class LGV {
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }
        public bool isMono = true;
        public byte[] Matrix { get { return _displayMatrix; } }

        /// <summary>
        /// Event raised each time a pixel is updated in the matrix
        /// </summary>
        public event EventHandler MatrixChanged;
        /// <summary>
        /// Event raised after a major draw method concludes
        /// </summary>
        public event EventHandler Updated;

        private int _width = LogitechGSDK.LOGI_LCD_MONO_WIDTH;
        private int _height = LogitechGSDK.LOGI_LCD_MONO_HEIGHT;
        private byte[] _displayMatrix;


        public void Initializer() {
            if (LogitechGSDK.LogiLcdIsConnected(LogitechGSDK.LOGI_LCD_TYPE_MONO)) {
                _displayMatrix = new byte[LogitechGSDK.LOGI_LCD_MONO_WIDTH * LogitechGSDK.LOGI_LCD_MONO_HEIGHT * 4];
                isMono = true;
            } else {
                _displayMatrix = new byte[LogitechGSDK.LOGI_LCD_COLOR_WIDTH * LogitechGSDK.LOGI_LCD_COLOR_HEIGHT * 4];
            }
        }


        public LGV() {
            Initializer();
        }
        public LGV(string img) {
            Initializer();
            
            Draw(img);
        }

        public void Draw() {
            if (isMono)
                LogitechGSDK.LogiLcdMonoSetBackground(_displayMatrix);
            else
                LogitechGSDK.LogiLcdColorSetBackground(_displayMatrix);

            LogitechGSDK.LogiLcdUpdate();

            if (Updated != null)
                Updated.Invoke(this, EventArgs.Empty); // technically correct, but we already updated the screen
        }
        public void Draw(string img) {
            Point[] points = RenderPoints(img);
            foreach (Point p in points) {
                SetPixel(p.X, p.Y, p.Color);
            }
            Draw();
        }

        private void SetPixel(float x, float y, Color color) {
            SetPixel(x, y, color.R, color.G, color.B, color.A);
        }


        #region Pixel Manipulation
        private int GetBytePosition(int x, int y) {
            if (isMono) {
                return (x + (y * LogitechGSDK.LOGI_LCD_MONO_WIDTH));
            } else {
                return (x + (y * LogitechGSDK.LOGI_LCD_COLOR_WIDTH));
            }
        }
        public void SetPixel(int x, int y, int r, int g, int b, int alpha, bool wrapPixel = false) {
            if (!wrapPixel && (x < 0 || y < 0 || x > Width || y > Height))
                return;

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            if (isMono) {
                byte ourGuy = 0x00;
                if (r > 0)
                    ourGuy = 0x80;

                _displayMatrix[GetBytePosition(x, y)] = ourGuy;
            } else {
                //idk
            }

            if (MatrixChanged != null)
                MatrixChanged.Invoke(this, EventArgs.Empty);
        }
        public void SetPixel(float x, float y, int r, int g, int b, int alpha, bool wrapPixel = false) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), r, g, b, alpha, wrapPixel);
        }
        public void SetPixel(int x, int y, bool active = true, bool wrapPixel = false) {
            SetPixel(x, y, active ? 1 : 0, 0, 0, 255, wrapPixel);
        }
        public void SetPixel(float x, float y, bool active = true, bool wrapPixel = false) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), active ? 1 : 0, 0, 0, 255, wrapPixel);
        }

        public Color GetColor(int x, int y) {
            if (x < 0 || y < 0 || x > Width || y > Height)
                return Color.FromArgb(0, 0, 0, 0);

            if (isMono) {
                if (_displayMatrix[GetBytePosition(x, y)]>=0x80) {
                    // Active
                    return Color.FromArgb(255,1,1,1);
                }
            }
            return Color.FromArgb(0, 0, 0, 0);
        }

        public void Clear(int r = 0, int g = 0, int b = 0, int a = 0) {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (isMono) {
                        if (r > 0 || g > 0 || b > 0 || a > 0) {
                            SetPixel(x, y, true);
                        } else {
                            SetPixel(x, y, false);
                        }
                    } else {
                        SetPixel(x, y, r, g, b, a);
                    }
                }
            }

            Draw();
        }

        public void EnablePixel(int x, int y, bool wrapPixel = false) {
            SetPixel(x, y, 255, 255, 255, 255, wrapPixel);
        }
        public void EnablePixel(float x, float y, bool wrapPixel = false) {
            SetPixel(x, y, 255, 255, 255, 255, wrapPixel);
        }
        public void DisablePixel(int x, int y, bool wrapPixel = false) {
            SetPixel(x, y, 0, 0, 0, 255, wrapPixel);
        }
        public void DisablePixel(float x, float y, bool wrapPixel = false) {
            SetPixel(x, y, 0, 0, 0, 255, wrapPixel);
        }
        #endregion


        private static Point[] AddPoint(Point[] points, Point newPoint) {
            Point[] newPoints = new Point[points.Length + 1];
            points.CopyTo(newPoints, 0);
            //for (int i = 0; i<points.Length; i++) {
            //    newPoints[i] = points[i];
            //}
            newPoints[points.Length] = newPoint;

            return newPoints;
        }
        private static Point[] AddPoints(Point[] ogPoints, Point[] newPoints) {
            Point[] nP = new Point[ogPoints.Length + newPoints.Length];
            ogPoints.CopyTo(nP, 0);
            int a = ogPoints.Length;
            for (int i = 0; i < newPoints.Length; i++) {
                nP[a] = newPoints[i];
                a++;
            }

            return nP;
        }


        

        public static Point[] RenderPoints(string img) {
            try {
                Point[] points = new Point[0];
                string command = "";
                Point[] commandPoints = new Point[0];

                bool inCommand = false;
                bool inPoint = false;
                bool inIgnore = false;
                RenderPosition position = RenderPosition.NONE;

                PointConstructor activePoint = new PointConstructor();

                foreach (char c in img) {
                    if (!inIgnore)
                        if (inPoint) {
                            if (c == ',') { // Move position
                                if (position == RenderPosition.X)
                                    position = RenderPosition.Y;
                                else if (position == RenderPosition.Y)
                                    position = RenderPosition.R;
                                else if (position == RenderPosition.R)
                                    position = RenderPosition.G;
                                else if (position == RenderPosition.G)
                                    position = RenderPosition.B;
                                else if (position == RenderPosition.B)
                                    position = RenderPosition.A;
                                else if (position == RenderPosition.A)
                                    position = RenderPosition.NONE;

                                else {
                                    // what? invalid, but whatever ??? just kinda skip it
                                    position = RenderPosition.NONE;
                                }
                            } else if (c == ')' || c == ']') { // End of point

                                if (inCommand)
                                    commandPoints = AddPoint(commandPoints, activePoint.ToPoint());
                                else
                                    points = AddPoint(points, activePoint.ToPoint());

                                inPoint = false;
                                position = RenderPosition.NONE;

                            } else if (c == '(') { // What?

                            } else { // Render a value
                                if (position == RenderPosition.X)
                                    activePoint._X += c; // Add next character to previous character
                                else if (position == RenderPosition.Y)
                                    activePoint._Y += c;
                                else if (position == RenderPosition.R)
                                    activePoint._R += c;
                                else if (position == RenderPosition.G)
                                    activePoint._G += c;
                                else if (position == RenderPosition.B)
                                    activePoint._B += c;
                                else if (position == RenderPosition.A)
                                    activePoint._A += c;
                            }

                        } else if (inCommand) {
                            if (c == '}') {
                                inCommand = false;
                                // do command.
                                if (command == "line") {
                                    points = AddPoints(points, Basics.RenderLine(commandPoints)); // Generate the line

                                } else if (command == "fill") {
                                    Polygon poly = new Polygon(commandPoints);
                                    poly.Fill();
                                    //points = AddPoints(points, poly.Pixels);

                                } else if (command == "outline") {
                                    Polygon poly = new Polygon(commandPoints);
                                    poly.Outline();
                                    //points = AddPoints(points, poly.Pixels);

                                }

                            } else if (c == '(') {
                                // Do point
                                inPoint = true;
                                activePoint = new PointConstructor();
                                position = RenderPosition.X;

                            } else if (c != ',' && c != '{') {
                                // probably a part of the command string 
                                command += c;
                            }

                            // } command
                        } else { // Looking for command/point
                            if (c == '(' || c == '[') {
                                inPoint = true;
                                activePoint = new PointConstructor();
                                position = RenderPosition.X;

                            } else if (c == '{') {
                                inCommand = true;
                                command = "";
                                commandPoints = new Point[0];
                            } else if (c == '/') {
                                inIgnore = true;
                            }
                        }
                    else {
                        if (c == '\\')
                            inIgnore = false;
                    }
                }

                return points;
            } catch (Exception e) {
                MessageBox.Show("Failed to render image.");
                return new Point[0];
            }
        }
        public string ToString(bool mono = false, bool giveExact = false) {
            String str = "/LCP1\\";

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Color pixel = GetColor(x, y);
                    if (giveExact) {
                        str += "(" + x + "," + y + "," + pixel.R + "," + pixel.G + "," + pixel.B + "," + pixel.A + ")";
                    } else {
                        if (pixel.R > 0 || pixel.G > 0 || pixel.B > 0 || pixel.A > 0) {
                            if (mono)
                                str += "(" + x + "," + y + ",1)"; // we don't need to color data :^)
                            else
                                str += "(" + x + "," + y + "," + pixel.R + "," + pixel.G + "," + pixel.B + "," + pixel.A + ")";
                        }
                        
                    }
                }
            }

            return str;
        }
    }




    enum RenderPosition {
        NONE,
        X,
        Y,
        R,
        G,
        B,
        A,
    }
}
