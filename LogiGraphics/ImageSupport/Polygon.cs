using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogiGraphics {

    /// <summary>
    /// Trash Polygon drawing API
    /// </summary>
    public class Polygon {
        public Point[] _points = new Point[0];

        public int maxX = 0;
        public int maxY = 0;
        public int minX = 0;
        public int minY = 0;

        private byte[,,] _pixels;
        public byte[,,] Pixels {
            get { return _pixels; }
        }
        public Point[] Anchors;

        public Color Color;


        //----------
        


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
        public bool IsPointWithinBounds(int x, int y) {
            Point p = new Point(x, y);
            return IsPointWithinBounds(p);
        }

        public Polygon(Point[] points) {
            Anchors = new Point[points.Length];
            for (int i = 0; i < points.Length; i++) {
                Anchors[i] = new Point(){
                    X = points[i].X,
                    Y = points[i].Y,
                    Color = points[i].Color
                };
            }
            Color = points[0].Color;
        }
        public Polygon(Point[] points, Color color) {
            Anchors = points;
            Color = color;
        }

        public void FindMaxMin() {
            if (Anchors.Length < 0)
                return;

            maxX = minX = Anchors[0].X;
            maxY = minY = Anchors[0].Y;

            foreach (Point p in Anchors) {
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

        public void Outline() {
            Point[] p = new Point[0];
            // all we do is draw lines :^)
            for (int i = 0; i < Anchors.Length; i++) {
                Point[] tmp = new Point[2];
                tmp[0] = Anchors[i];
                if (i + 1 >= Anchors.Length)
                    tmp[1] = Anchors[0];
                else
                    tmp[1] = Anchors[i + 1];

                tmp[0].Color = Color;
                tmp[1].Color = Color;

                Basics.AddPoints(ref p, Basics.RenderLine(tmp));

            }
            _points = p;
            _pixels = Basics.PointsToBytes(_points);
        }

        public void Fill() {
            // brute force method :))((())
            FindMaxMin();
            Outline();
            for (int x = (int)minX; x < maxX; x++) {
                for (int y = (int)minY; y < maxY; y++) {
                    if (IsPointWithinBounds(x, y)) {
                        Point p = new Point(x, y, _points[0].Color);
                        Basics.AddPoint(ref _points, p);
                    }
                }
            }
            _pixels = Basics.PointsToBytes(_points);
        }
    }
}
