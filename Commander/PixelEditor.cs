using LogiGraphics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogiCommand {
    public class PixelEditor : FrameworkElement {
        public Surface surface;
        public Display display;
        private readonly Visual _gridLines;

        public int PixelWidth = LogitechGSDK.LOGI_LCD_COLOR_WIDTH;
        public int PixelHeight = LogitechGSDK.LOGI_LCD_COLOR_HEIGHT;

        public int Magnification { get; } = 7;

        public Tool tool = Tool.PEN;
        public Color toolColor = Colors.DodgerBlue;

        public bool autoUpdate = false;

        public PixelEditor() {
            display = new Display();
            surface = new Surface(this, display);
            _gridLines = CreateGridLines();

            Cursor = Cursors.Pen;

            AddVisualChild(surface);
            AddVisualChild(_gridLines);

            surface.Updated += (CanvasUpdated e) => {
                if (Updated != null)
                    Updated.Invoke(e);
            };
        }

        public delegate void UpdatedHandler(CanvasUpdated e);
        public event UpdatedHandler Updated;

        protected override int VisualChildrenCount => 2;

        protected override Visual GetVisualChild(int index) {
            return index == 0 ? surface : _gridLines;
        }

        private void FloodFill(int x, int y) {
            Color currentColor = surface.GetColor(x, y);
            if (currentColor.Equals(toolColor))
                return;

            Stack<System.Windows.Point> pixels = new Stack<System.Windows.Point>();

            pixels.Push(new System.Windows.Point(x, y));
            while (pixels.Count != 0) {
                System.Windows.Point temp = pixels.Pop();
                int y1 = (int)temp.Y;
                while (y1 >= 0 && surface._bitmap.GetPixel((int)temp.X, y1) == currentColor) {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < PixelHeight && surface.GetColor((int)temp.X, y1) == currentColor) {
                    surface._bitmap.SetPixel((int)temp.X, y1, toolColor);

                    if (!spanLeft && temp.X > 0 && surface._bitmap.GetPixel((int)temp.X - 1, y1) == currentColor) {
                        pixels.Push(new System.Windows.Point(temp.X - 1, y1));
                        spanLeft = true;
                    } else if (spanLeft && temp.X - 1 == 0 && surface._bitmap.GetPixel((int)temp.X - 1, y1) != currentColor) {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.X < PixelWidth - 1 && surface._bitmap.GetPixel((int)temp.X + 1, y1) == currentColor) {
                        pixels.Push(new System.Windows.Point(temp.X + 1, y1));
                        spanRight = true;
                    } else if (spanRight && temp.X < PixelWidth - 1 && surface._bitmap.GetPixel((int)temp.X + 1, y1) != currentColor) {
                        spanRight = false;
                    }
                    y1++;
                }

            }

            surface.UpdateImage();
            surface.image.Draw();
        }

        private void Draw(bool remove = false) {
            var p = Mouse.GetPosition(surface);
            var magnification = Magnification;
            var surfaceWidth = PixelWidth * magnification;
            var surfaceHeight = PixelHeight * magnification;

            if (p.X < 0 || p.X >= surfaceWidth || p.Y < 0 || p.Y >= surfaceHeight)
                return;

            int x = (int)(p.X / magnification);
            int y = (int)(p.Y / magnification);

            if (remove || tool == Tool.ERASER)
                surface.EraseColor(x, y);
            else if (tool == Tool.BUCKET) {
                FloodFill(x, y);
                surface.SetColor(x, y, toolColor);
            } else
                surface.SetColor(x, y, toolColor);

            surface.InvalidateVisual();
            //surface._display.Draw();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
                Draw();
            else if (e.RightButton == MouseButtonState.Pressed && IsMouseCaptured)
                Draw(true);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
            base.OnMouseRightButtonDown(e);
            CaptureMouse();
            Draw(true);
        }
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
            base.OnMouseRightButtonUp(e);
            ReleaseMouseCapture();
            if (Updated != null)
                Updated.Invoke(new CanvasUpdated(surface.image));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            CaptureMouse();
            Draw();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            if (Updated != null)
                Updated.Invoke(new CanvasUpdated(surface.image));
        }


        protected override Size MeasureOverride(Size availableSize) {
            var magnification = Magnification;
            var size = new Size(PixelWidth * magnification, PixelHeight * magnification);

            surface.Measure(size);

            return size;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            surface.Arrange(new Rect(finalSize));
            return finalSize;
        }

        private Visual CreateGridLines() {
            var dv = new DrawingVisual();
            var dc = dv.RenderOpen();

            var w = PixelWidth;
            var h = PixelHeight;
            var m = Magnification;
            var d = -0.5d; // snap grid lines to device pixels

            var pen = new Pen(new SolidColorBrush(Color.FromArgb(63, 63, 63, 63)), 1d);

            pen.Freeze();

            for (var x = 1; x < w; x++)
                dc.DrawLine(pen, new System.Windows.Point(x * m + d, 0), new System.Windows.Point(x * m + d, h * m));

            for (var y = 1; y < h; y++)
                dc.DrawLine(pen, new System.Windows.Point(0, y * m + d), new System.Windows.Point(w * m, y * m + d));

            dc.Close();

            return dv;
        }

        public sealed class Surface : FrameworkElement {
            private readonly PixelEditor _owner;
            public readonly Display _display;
            public WriteableBitmap _bitmap;

            public delegate void UpdatedHandler(CanvasUpdated e);
            public event UpdatedHandler Updated;

            public Image image;

            public Surface(PixelEditor owner, Display display) {
                _owner = owner;
                _display = display;
                _bitmap = BitmapFactory.New(owner.PixelWidth, owner.PixelHeight);
                _bitmap.Clear(Colors.Transparent);
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
                image = new Image(display);
            }

            public void UpdateImage() {
                for (int x = 0; x<_owner.PixelWidth; x++) {
                    for (int y = 0; y<_owner.PixelHeight; y++) {
                        Color pixel = GetColor(x, y);
                        image.Layers[0].SetPixel(x, y, pixel.R, pixel.G, pixel.B, pixel.A);
                    }
                }
                _display.Draw();
            }

            protected override void OnRender(DrawingContext dc) {
                base.OnRender(dc);

                var magnification = _owner.Magnification;
                var width = _bitmap.PixelWidth * magnification;
                var height = _bitmap.PixelHeight * magnification;

                dc.DrawImage(_bitmap, new Rect(0, 0, width, height));

            }

            public void UpdateCanvas() {
                // image -> bitmap
                byte[,,] pixels = image.GetComposite();
                int channels = pixels.GetLength(0);
                int width = pixels.GetLength(1);
                int height = pixels.GetLength(2);
                _bitmap = BitmapFactory.New(width, height);
                _bitmap.Clear(Colors.Transparent);
                _owner.PixelWidth = width;
                _owner.PixelHeight = height;


                for (int y = 0; y<height; y++) {
                    for (int x = 0; x<width; x++) {
                        if (channels > 1) {
                            _bitmap.SetPixel(x, y, pixels[0, x, y], pixels[1, x, y], pixels[2, x, y], pixels[3, x, y]);
                        }
                    }
                }
                InvalidateVisual();
            }

            internal void SetColor(int x, int y, Color color) {
                _bitmap.SetPixel(x, y, color);
                image.Layers[0].SetPixel(x, y, color.R, color.G, color.B, color.A);

                //UpdateImage();
                if (_owner.autoUpdate)
                    if (Updated != null)
                        Updated.Invoke(new CanvasUpdated(image));
            }
            internal Color GetColor(int x, int y) {
                return _bitmap.GetPixel(x, y);
            }
            internal void EraseColor(int x, int y) {
                _bitmap.SetPixel(x, y, 0x0, Colors.Black);
                image.Layers[0].SetPixel(x, y, 0, 0, 0, 0);

                //UpdateImage();
                if (_owner.autoUpdate)
                    if (Updated != null)
                        Updated.Invoke(new CanvasUpdated(image));
            }
        }

    }

    public class CanvasUpdated : EventArgs {
        public Image image;

        public CanvasUpdated(Image image) {
            this.image = image;
        }
    }

    public enum Tool {
        PEN,
        ERASER,
        BUCKET
    }
    public static class ToolMethods {
        public static Cursor GetCursor(this Tool tool) {
            switch (tool) {
                case Tool.PEN:
                    return Cursors.Pen;
                case Tool.ERASER:
                    return Cursors.No;
                case Tool.BUCKET:
                    return Cursors.Cross;
                default:
                    return Cursors.Pen;
            }
        }
    }
}
