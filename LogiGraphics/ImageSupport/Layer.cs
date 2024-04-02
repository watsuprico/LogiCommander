using LogitechSDK;
using System;
using System.Windows.Media;

namespace LogiGraphics {
    public class Layer {
        public LayerType layerType;


        public bool Visible = true;
        public double Transparency = 100;

        // If our pixels only expand 20x40 but the screen is 200x400 and we're in the bottom half, this allows us to save space in the _Pixels array.
        // by only containing those 20x40 pixels rather than the 200x400, however, to still be in the bottom half we need to have an 'offset' which translates those pixels elsewhere.
        public int xOffset = 0;
        public int yOffset = 0;

        /// <summary>
        /// The owner of us (this layer). Used to determine the offsets
        /// </summary>
        //private Image owner;

        // If we're a layer containing a polygon, below is that polygon object.
        public Polygon polygon;

        // At the maximum size allowed by the color display, these should be no more than 75KB each, and 300KB total.
        // This is reasonable, plus, without doing some magic (costly compression), I cannot make it any lower.
        // To make things simple, I made a 3D array that contains all the channels (RGBA) and each pixel (X,Y)
        // The first index is the channel, 0=A, 1=R, 2=G, 3=B
        // The second index is the X value with the last one being for the Y value.

        // So _Colors[2,60,2] get's the amount of green in the pixel at (60,2).
        public byte[,,] Pixels {
            get {
                if (layerType == LayerType.POLYGON)
                    return polygon.Pixels;
                return _pixels;
            }
            set {
                if (layerType == LayerType.POLYGON)
                    layerType = LayerType.PIXELS;

                _pixels = value;
            }
        }

        private byte[,,] _pixels = new byte[4, LogitechGSDK.LOGI_LCD_COLOR_WIDTH, LogitechGSDK.LOGI_LCD_COLOR_HEIGHT];

        public Layer() {
            layerType = LayerType.PIXELS;
        }
        public Layer(byte[,,] pixels, Image owner) {
            Pixels = pixels;
            layerType = LayerType.PIXELS;
        }
        public Layer(Polygon shape, Image owner) {
            polygon = shape;
            layerType = LayerType.POLYGON;
        }



        /// <summary>
        /// Gets the color value for a pixel
        /// </summary>
        /// <param name="x">x cord</param>
        /// <param name="y">y cord</param>
        /// <param name="isMono">Referencing the monochromatic screen?</param>
        /// <returns></returns>
        public Color GetPixel(int x, int y) {
            x -= xOffset;
            y -= yOffset;


            if (layerType == LayerType.PIXELS) {
                int colorChannels = _pixels.GetLength(0);
                int xWidth = _pixels.GetLength(1);
                int yWidth = _pixels.GetLength(2);

                if (x >= 0 && x < xWidth && y >= 0 && y < yWidth) { // Pixel exists?
                    if (colorChannels == 1) {
                        // Mono
                        return Color.FromArgb(_pixels[0, x, y], 0, 0, 0);
                    } else {
                        return Color.FromArgb(_pixels[0, x, y], _pixels[1, x, y], _pixels[2, x, y], _pixels[3, x, y]);
                    }
                }
            } else if (layerType == LayerType.POLYGON) {
                int colorChannels = polygon.Pixels.GetLength(0);
                int xWidth = polygon.Pixels.GetLength(1);
                int yWidth = polygon.Pixels.GetLength(2);

                if (x >= 0 && x < xWidth && y >= 0 && y < yWidth)
                    if (colorChannels == 1) {
                        // Mono
                        return Color.FromArgb(polygon.Pixels[0, x, y], 0, 0, 0);
                    } else {
                        return Color.FromArgb(polygon.Pixels[0, x, y], polygon.Pixels[1, x, y], polygon.Pixels[2, x, y], polygon.Pixels[3, x, y]);
                    }
            }

            return new Color();
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

            int colorChannels = _pixels.GetLength(0);
            int xWidth = _pixels.GetLength(1);
            int yWidth = _pixels.GetLength(2);

            if (layerType == LayerType.PIXELS) {
                if (x >= 0 && x < xWidth && y >= 0 && y < yWidth) { // Pixel exists?
                    if (colorChannels == 1) {
                        // Mono
                        _pixels[0, x, y] = color.A;
                    } else {
                        _pixels[0, x, y] = color.A;
                        _pixels[1, x, y] = color.R;
                        _pixels[2, x, y] = color.G;
                        _pixels[3, x, y] = color.B;
                    }
                }
            }
        }
        public void SetPixel(float x, float y, Color color) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), color);
        }
        public void SetPixel(int x, int y, int r, int g, int b, int alpha) {
            SetPixel(x, y, Color.FromArgb((byte)alpha, (byte)r, (byte)g, (byte)b));
        }
        public void SetPixel(float x, float y, int r, int g, int b, int alpha) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), r, g, b, alpha);
        }
        public void SetPixel(int x, int y, bool active) {
            int c = active ? 255 : 0;
            SetPixel(x, y, c, c, c, c);
        }
        public void SetPixel(float x, float y, bool active) {
            SetPixel((int)Math.Round(x), (int)Math.Round(y), active);
        }

        public void SetPixels(Point[] points) {
            foreach (Point p in points) {
                SetPixel(p.X, p.Y, p.Color);
            }
        }
    }
    public enum LayerType {
        /// <summary>
        /// Basic array of pixels as bytes
        /// </summary>
        PIXELS,
        /// <summary>
        /// A polygon object. <code>Polygon shape</code>
        /// </summary>
        POLYGON
    }
}
