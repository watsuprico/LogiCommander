using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogiGraphics {
    public static class Basics {
        public static byte[,,] PointsToBytes(Point[] points) {
            byte[,,] composite = new byte[3, LogitechGSDK.LOGI_LCD_COLOR_WIDTH, LogitechGSDK.LOGI_LCD_COLOR_HEIGHT];
            foreach (Point p in points) {
                if (p.X <= LogitechGSDK.LOGI_LCD_COLOR_WIDTH && p.Y <= LogitechGSDK.LOGI_LCD_COLOR_HEIGHT) {
                    composite[0, p.X, p.Y] = p.Color.A;
                    composite[0, p.X, p.Y] = p.Color.R;
                    composite[0, p.X, p.Y] = p.Color.G;
                    composite[0, p.X, p.Y] = p.Color.B;
                }
            }
            return composite;
        }

        public static void AddPoint(ref Point[] points, Point point) {
            if (point == null || points == null)
                return;

            //if (points.Contains(point))
            //    return;

            Point[] ogPoints = points;
            points = new Point[ogPoints.Length + 1];
            ogPoints.CopyTo(points, 0);
            points[ogPoints.Length] = point;
            point = null;
        }
        public static void AddPoints(ref Point[] points, Point[] newPoints) {
            if (newPoints == null || points == null)
                return;

            for (int i = 0; i<newPoints.Length; i++) {
                if (!points.Contains(newPoints[i]))
                    AddPoint(ref points, newPoints[i]); // Not already in points, so we'll add it
            }
        }
        public static Point[] AddPoint(Point[] points, Point newPoint) {
            if (newPoint == null)
                return points;

            if (points == null) {
                points = new Point[1];
                points[0] = newPoint;
                return points;
            }

            Point[] pointsClone = new Point[points.Length];
            
            points.CopyTo(pointsClone, 0);
            points = null;

            AddPoint(ref pointsClone, newPoint);
            
            return pointsClone;
        }
        public static Point[] AddPoints(Point[] points, Point[] newPoints) {
            if (newPoints == null)
                return points;

            if (points == null) 
                return newPoints;

            Point[] pointsClone = new Point[points.Length];

            points.CopyTo(pointsClone, 0);
            points = null;

            AddPoints(ref pointsClone, newPoints);

            return pointsClone;
        }

        public static Point[] RenderLine(Point l0, Point l1) {
            Point[] points = new Point[0];
            float x, y, dx, dy, step;

            dx = Math.Abs(l1.X - l0.X);
            dy = Math.Abs(l1.Y - l0.Y);
            if (dx >= dy)
                step = dx;
            else
                step = dy;

            dx /= step;
            dy /= step;

            x = l0.X;
            y = l0.Y;

            bool yFlip = l1.Y < y;
            bool xFlip = l1.X < x;

            for (int i = 1; i < step + 1; i++) {
                Point p = new Point((int)x, (int)y, l0.Color);

                AddPoint(ref points, p);

                if (xFlip)
                    x -= dx;
                else
                    x += dx;

                if (yFlip)
                    y -= dy;
                else
                    y += dy;
            }

            return points;
        }
        public static Point[] RenderLine(Point[] line) {
            return RenderLine(line[0], line[1]);
        }
    }
}
