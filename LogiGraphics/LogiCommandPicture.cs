using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogiCommand {
    class LCP {
        class Color {
            public int r;
            public int g;
            public int b;
        }
        class Polygon {
            private Point[] _points;

            private float maxX = 0;
            private float maxY = 0;
            private float minX = 0;
            private float minY = 0;

            public Point[] Points {
                get { return _points; }
                set {
                    _points = value;
                    UpdateMaxMin();
                }
            }
            public Color color;

            private void UpdateMaxMin() {
                maxX = _points[0].X;
                maxY = _points[0].Y;
                minX = _points[0].X;
                minY = _points[0].Y;

                foreach (Point p in Points) {
                    if (p.X > maxX)
                        maxX = p.X;
                    if (p.X < minX)
                        minX = p.X;

                    if (p.Y > maxY)
                        maxY = p.Y;
                    if (p.Y < minY)
                        minY = p.Y;
                }
            }

            public bool IsPointWithinBounds(Point p) {
                if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY) {
                    return false;
                }
                bool inside = false;
                for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++) {
                    if ((_points[i].Y > p.Y) != (_points[j].Y > p.Y) && p.X < (_points[j].X - _points[i].X) * (p.Y - _points[i].Y) / (_points[j].Y - _points[i].Y) + _points[i].X) {
                        inside = !inside;
                    }
                }
                return inside;
            }
            public bool IsPointWithinBounds(float x, float y) {
                Point p = new Point();
                p.x = x;
                p.y = y;
                return IsPointWithinBounds(p);
            }

            public Polygon(Point[] points) {
                this.Points = points;
            }
            public Polygon(Point[] points, Color color) {
                this.Points = points;
                this.color = color;
            }

            public void Outline() {
                Point[] p = new Point[0];
                // all we do is draw lines :^)
                for (int i = 0; i < Points.Length; i++) {
                    Point[] tmp = new Point[2];
                    tmp[0] = Points[i];
                    if (i + 1 >= Points.Length)
                        tmp[1] = Points[0];
                    else
                        tmp[1] = Points[i + 1];

                    Point.Combine(ref p, RenderLine(tmp));

                }
                this.Points = p;
            }

            public void Fill() {
                // brute force method :))((())
                Outline();
                for (float x = minX; x < maxX; x++) {
                    for (float y = minY; y < maxY; y++) {
                        Point p = new Point();
                        p.x = x;
                        p.y = y;
                        p.active = active;
                        if (IsPointWithinBounds(p))
                            _points = AddPoint(Points, p);
                    }
                }
            }
        }

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

        public LCP() {}
        public LCP(string img) {
            
        }


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
            SetPixel(x, y, active? 1 : 0, 0, 0, 255, wrapPixel);
        }
        public void SetPixel(float x, float y, bool active = true, bool wrapPixel = false) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), active ? 1 : 0, 0, 0, 255, wrapPixel);
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


        private static Point[] RenderLine(Point l0, Point l1) {
            Point[] points = new Point[0];
            float x, y, dx, dy, step;

            dx = Math.Abs(l1.X - l0.X);
            dy = Math.Abs(l1.Y - l0.Y);
            if (dx >= dy)
                step = dx;
            else
                step = dy;

            dx = dx / step;
            dy = dy / step;

            x = l0.X;
            y = l0.Y;

            bool yFlip = l1.Y < y;
            bool xFlip = l1.X < x;

            for (int i = 1; i < step + 1; i++) {
                Point p = new();
                p.X = (int)x;
                p.Y = (int)y;
                p.r = l0.r;
                p.g = l0.g;
                p.b = l0.b;

                p.Add(ref points);

                if (xFlip)
                    x = x - dx;
                else
                    x = x + dx;

                if (yFlip)
                    y = y - dy;
                else
                    y = y + dy;
            }

            return points;
        }
        private Point[] RenderLine(Point[] line) {
            return RenderLine(line[0], line[1]);
        }


    }
}
