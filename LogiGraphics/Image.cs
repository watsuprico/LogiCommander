using LogitechSDK;
using System;
using System.Windows.Media;

namespace LogiGraphics {
    public class Image {

        /*
         * 
         * LGI (LogiGraphics Image)
         * 
         * ``       between the two '`'s is a version number of this file format
         *          Following this format: <type>.<version> (This version would be: LGI.1)
         *
         * $        sets the layer number for the data to follow
         * $n{}     sets parameters for the layer n
         * $*{}     sets parameters for the image
         *  {width:10}      sets the width of the image to 10
         *  {height:}       image height
         *  {xOffset:}      image/layer x offset
         *  {yOffset:}      image/layer y offset
         *  {transparency:} layer transparency
         *  {layerType:}    layer type
         *
         *
         *
         * []       a point
         * !        repeats the last point n times on the X axis ([1,1,1]!3 gives us [1,1,1][2,1,1][3,1,1])
         * !!       repeats the last point n times on the Y axis ([1,1,1]!!3 gives us [1,1,1][1,2,1][1,3,1])
         * 
         * 
         * 
         */

        #region Properties
        /// <summary>
        /// Display X axis offset, <see cref="Display.xOffset"/>.
        /// </summary>
        public int X;
        /// <summary>
        /// Display Y axis offset, <see cref="Display.yOffset"/>.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of this image.
        /// </summary>
        public int Width = LogitechGSDK.LOGI_LCD_COLOR_WIDTH;
        /// <summary>
        /// Height of this image.
        /// </summary>
        public int Height = LogitechGSDK.LOGI_LCD_COLOR_HEIGHT;

        public Layer[] Layers = new Layer[0];

        private Display Display {
            get => _display;
            set {
                _display = value;
                value.Drawing = (object dis, EventArgs e) => UpdateDisplay();
            }
        }
        private Display _display;
        #endregion

        #region Events
        //public event EventHandler Modified;
        #endregion

        #region Public API
        public Image(Display display) {
            Display = display;
            AddLayer(new Layer(), 0);
            Width = LogitechGSDK.LOGI_LCD_COLOR_WIDTH;
            Height = LogitechGSDK.LOGI_LCD_COLOR_HEIGHT;
        }

        public Image(Display display, string img) {
            // Load an image from a string
            Display = display;
            Width = LogitechGSDK.LOGI_LCD_COLOR_WIDTH;
            Height = LogitechGSDK.LOGI_LCD_COLOR_HEIGHT;

            LoadString(img);
        }

        public void Draw() {
            Display.Draw();
        }


        public byte[,,] GetComposite() {
            byte[,,] composite = new byte[4, Width, Height];
            for (int i = Layers.Length - 1; i >= 0; i--) {
                int channels = Layers[i].Pixels.GetLength(0);
                int layerWidth = Layers[i].Pixels.GetLength(1);
                int layerHeight = Layers[i].Pixels.GetLength(2);

                int mWidth = Math.Min(Width, layerWidth);
                int mHeight = Math.Min(Height, layerHeight);

                for (int y = 0; y < mHeight; y++) {
                    for (int x = 0; x < mWidth; x++) {
                        //int X = y - Layers[i].xOffset;
                        //int Y = y - Layers[i].yOffset;
                        composite[0, x, y] = Layers[i].Pixels[0, x, y];
                        if (channels >= 3) {
                            composite[1, x, y] = Layers[i].Pixels[1, x, y];
                            composite[2, x, y] = Layers[i].Pixels[2, x, y];
                            composite[3, x, y] = Layers[i].Pixels[3, x, y];
                        }
                    }
                }
            }
            return composite;
        }
        public byte[] GetMatrixComposite(bool isMono = false) {
            UpdateDisplay();

            if (isMono)
                return Display.DisplayObject.monoMatrix;
            else
                return Display.DisplayObject.colorMatrix;
        }
        public Color[,] GetColorComposite() {
            Color[,] colors = new Color[Width, Height];
            for (int i = Layers.Length; i < 0; i++) {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        colors[x, y] = Layers[i].GetPixel(x, y);
                    }
                }
            }


            return colors;
        }
        public DisplayObject GetDisplayComposite() {
            UpdateDisplay();
            return Display.DisplayObject;
        }


        public void AddLayer(Layer layer, int index = -1) {
            if (index < 0 || index > Layers.Length)
                index = Layers.Length;

            Layer[] newLayers = new Layer[Layers.Length + 1];
            Layers.CopyTo(newLayers, 0);
            newLayers[Layers.Length] = layer;
            Layers = newLayers;
            UpdateDisplay();
        }
        public void AddLayer(byte[,,] pixels, int index = -1) {
            AddLayer(new Layer(pixels, this), index);
        }
        public void AddLayer(Polygon shape, int index = -1) {
            AddLayer(new Layer(shape, this), index);
        }

        public void Clear() {
            Layers = new Layer[1];
            Layers[0] = new Layer();
            UpdateDisplay();
        }



        private enum Reader {
            Version,
            LayerNumber,
            PointX,
            PointY,
            PointA,
            PointR,
            PointG,
            PointB,
            Parameter,
            Width,
            Height,
            xOffset,
            yOffset,
            Transparency,
            LayerType,
            RepeatX,
            RepeatY,
            Wait
        }
        public void LoadString(string str) {
            string version;
            Reader next = Reader.Wait;
            int curLayer = 0;
            Layers = new Layer[1];
            Layers[0] = new Layer();

            string tmpStr = "";
            int tmpInt;
            Point tmpPoint = new Point();

            foreach (char c in str) {
                if (next == Reader.Version) {
                    if (c == '`') {
                        version = tmpStr;
                        next = Reader.Wait;
                        tmpStr = "";
                    } else {
                        tmpStr += c;
                    }
                    continue;
                } else if (next == Reader.LayerNumber || next == Reader.PointX || next == Reader.PointY || next == Reader.PointA || next == Reader.PointR || next == Reader.PointG || next == Reader.PointB) {
                    if (Char.IsDigit(c)) {
                        tmpStr += c;
                        continue;
                    } else if (next != Reader.LayerNumber) {
                        if (c == ',') {
                            int.TryParse(tmpStr, out tmpInt);

                            if (next == Reader.PointX) {
                                tmpPoint.X = (byte)tmpInt;
                                next = Reader.PointY;
                            } else if (next == Reader.PointY) {
                                tmpPoint.Y = (byte)tmpInt;
                                next = Reader.PointA;
                            } else if (next == Reader.PointA) {
                                tmpPoint.Color.A = (byte)tmpInt;
                                next = Reader.PointR;
                            } else if (next == Reader.PointR) {
                                tmpPoint.Color.R = (byte)tmpInt;
                                next = Reader.PointG;
                            } else if (next == Reader.PointG) {
                                tmpPoint.Color.G = (byte)tmpInt;
                                next = Reader.PointB;
                            } else if (next == Reader.PointB) {
                                tmpPoint.Color.B = (byte)tmpInt;
                                Layers[curLayer].SetPixel(tmpPoint.X, tmpPoint.Y, tmpPoint.Color);
                            }
                            tmpStr = "";
                            tmpInt = 0;
                            continue;
                        } else if (c == ']') {
                            Layers[curLayer].SetPixel(tmpPoint.X, tmpPoint.Y, tmpPoint.Color);
                            next = Reader.Wait;
                            tmpStr = "";
                            tmpInt = 0;
                            continue;
                        }
                    } else if (next == Reader.LayerNumber) {
                        // not a number, add layer to array if needed, change curLayer
                        if (c == '*')
                            continue;

                        int.TryParse(tmpStr, out curLayer);
                        int createLayers = 1 + curLayer - Layers.Length;
                        if (createLayers > 0) {
                            for (int i = 0; i < createLayers; i++)
                                AddLayer(new Layer());
                        }
                        next = Reader.Wait;
                    }
                } else if (next == Reader.Width || next == Reader.Height || next == Reader.xOffset || next == Reader.yOffset || next == Reader.Transparency || next == Reader.LayerType) {
                    if (c == '}') {

                        tmpStr = tmpStr.ToLower();
                        int.TryParse(tmpStr, out tmpInt);

                        if (next == Reader.Width) {
                            Width = tmpInt;
                        } else if (next == Reader.Height) {
                            Height = tmpInt;
                        } else if (next == Reader.xOffset) {
                            if (curLayer == -1)
                                X = tmpInt;
                            else if (curLayer < Layers.Length)
                                Layers[curLayer].xOffset = tmpInt;
                        } else if (next == Reader.yOffset) {
                            if (curLayer == -1)
                                X = tmpInt;
                            else if (curLayer < Layers.Length)
                                Layers[curLayer].yOffset = tmpInt;
                        } else if (next == Reader.Transparency) {
                            if (curLayer < Layers.Length)
                                Layers[curLayer].Transparency = tmpInt;
                        } else if (next == Reader.LayerType) {
                            if (curLayer < Layers.Length) {
                                if (tmpStr == "pixels")
                                    Layers[curLayer].layerType = LayerType.PIXELS;
                                else if (tmpStr == "polygon")
                                    Layers[curLayer].layerType = LayerType.POLYGON;
                            }
                        }
                        next = Reader.Wait;
                        tmpInt = 0;
                        tmpStr = "";
                        continue;
                    } else {
                        if (next != Reader.LayerType) {
                            if (Char.IsDigit(c))
                                tmpStr += c;
                        } else {
                            tmpStr += c;
                        }
                        continue;
                    }



                } else if (next == Reader.Parameter) {
                    if (c == ':') {
                        // Set parameter
                        tmpStr = tmpStr.ToLower();
                        switch (tmpStr) {
                            case "width":
                                next = Reader.Width;
                                break;
                            case "height":
                                next = Reader.Height;
                                break;
                            case "xoffset":
                                next = Reader.xOffset;
                                break;
                            case "yoffset":
                                next = Reader.yOffset;
                                break;
                            case "transparency":
                                next = Reader.Transparency;
                                break;
                            case "layertype":
                                next = Reader.LayerType;
                                break;
                            default:
                                next = Reader.Wait;
                                break;
                        }
                        tmpStr = "";
                    } else if (c == '}') {
                        // lol what
                        next = Reader.Wait;
                    } else {
                        tmpStr += c;
                    }
                    continue;
                } else if (c == '!') {
                    if (next == Reader.RepeatX)
                        next = Reader.RepeatY;
                    else if (next == Reader.Wait)
                        next = Reader.RepeatX;
                    continue;
                }

                if (next == Reader.RepeatX) {
                    // later

                } else if (next == Reader.RepeatY) {
                    // later
                }

                switch (c) {
                    case '`':
                        next = Reader.Version;
                        break;
                    case '$':
                        next = Reader.LayerNumber;
                        break;
                    case '[':
                        tmpPoint = new Point();
                        next = Reader.PointX;
                        break;
                    case '{':
                        next = Reader.Parameter;
                        break;
                    case '!':
                        next = Reader.RepeatX;
                        break;
                }
            }
        }


        public override string ToString() {
            string str = "`LGI.1`" +
                "$*{width:" + Width + "}" +
                "$*{height:" + Height + "}" +
                "$*{xOffset:" + X + "}" +
                "$*{yOffset:" + Y + "}";

            for (int i = Layers.Length - 1; i >= 0; i--) {
                Layer layer = Layers[i];
                str += "$" + i +
                    "{xOffset:" + layer.xOffset + "}" +
                    "{yOffset:" + layer.yOffset + "}" +
                    "{transparency:" + layer.Transparency + "}" +
                    "{layerType:" + layer.layerType + "}" +
                    "{channels:" + layer.Pixels.GetLength(0) + "}";

                // if a pixel a value is 0, we move on
                for (int y = 0; y < layer.Pixels.GetLength(2); y++) {
                    for (int x = 0; x < layer.Pixels.GetLength(1); x++) {
                        if (layer.Pixels[0, x, y] > 0) {
                            str += $"[{x},{y},{layer.Pixels[0, x, y]},{layer.Pixels[1, x, y]},{layer.Pixels[2, x, y]},{layer.Pixels[3, x, y]}]";
                        }
                    }
                }

            }

            return str;
        }
        #endregion

        #region Private API
        private void UpdateDisplay() {
            //Display.Clear();
            Display.xOffset = X;
            Display.yOffset = Y;

            Display.SetPixels(GetComposite());
        }
        #endregion
    }
}
