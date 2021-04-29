using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogiGraphics {
    public class Polygon {
        private Point[] _points;

        private float maxX = 0;
        private float maxY = 0;
        private float minX = 0;
        private float minY = 0;

        private byte[,,] _pixels;
        public byte[,,] Pixels {
            get { return _pixels; }
        }
        public Point[] Anchors;

        public Color color;


        //----------
        

        private void UpdateMaxMin() {
            maxX = _points[0].X;
            maxY = _points[0].Y;
            minX = _points[0].X;
            minY = _points[0].Y;

            
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
            Point p = new Point((int)x, (int)y);
            return IsPointWithinBounds(p);
        }

        public Polygon(Point[] points) {
            this.Anchors = points;
            Anchors = points;
        }
        public Polygon(Point[] points, Color color) {
            this.Anchors = points;
            this.color = color;
            Anchors = points;
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

                Basics.AddPoints(ref p, Basics.RenderLine(tmp));

            }
            this._pixels = Basics.PointsToBytes(p);
        }

        public void Fill() {
            // brute force method :))((())
            Outline();
            for (int x = (int)minX; x < maxX; x++) {
                for (int y = (int)minY; y < maxY; y++) {
                    Point p = new Point(x, y, _points[0].Color);

                    if (IsPointWithinBounds(p))
                        Basics.AddPoint(ref _points, p);
                }
            }
        }
    }
}
