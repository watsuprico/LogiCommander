﻿using System.Windows.Media;

namespace LogiGraphics {
    /// <summary>
    /// Used for creating a point from a string
    /// </summary>
    public class PointConstructor {
        public int X;
        public int Y;

        public string _X {
            get { return X.ToString(); }
            set {
                int val;
                if (int.TryParse(value, out val))
                    X = val;
            }
        }
        public string _Y {
            get { return Y.ToString(); }
            set {
                int val;
                if (int.TryParse(value, out val))
                    Y = val;
            }
        }

        public Color Color;



        public string _R {
            get { return Color.R.ToString(); }
            set {
                int val;
                if (int.TryParse(value, out val))
                    Color.R = (byte)val;
            }
        }
        public string _G {
            get { return Color.G.ToString(); }
            set {
                int val;
                if (int.TryParse(value, out val))
                    Color.G = (byte)val;
            }
        }
        public string _B {
            get { return Color.B.ToString(); }
            set {
                int val;
                if (int.TryParse(value, out val))
                    Color.B = (byte)val;
            }
        }
        public string _A {
            get { return Color.A.ToString(); }
            set {
                int val;
                if (int.TryParse(value, out val))
                    Color.A = (byte)val;
            }
        }

        public PointConstructor() { }

        public Point ToPoint() {
            return new Point(X, Y, Color);
        }
    }
}
