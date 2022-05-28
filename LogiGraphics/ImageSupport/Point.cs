using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogiGraphics {
    public class Point {
        public int X;
        public int Y;

        public Color Color;

        public Point() {
        }

        public Point(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }
        public Point(int X, int Y, Color Color) {
            this.X = X;
            this.Y = Y;
            this.Color = Color;
        }
    }
}
