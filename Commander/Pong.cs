using LogiGraphics;
using LogiGraphics.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LogiCommand {
    class Pong {
        /*
         * Quick and dirty
         * 
         * please do not use
         * 
         * 
         */


        private ButtonPoller ButtonPoller;
        private Display PongDisplay = new();


        private bool bRunning = true;


        private Polygon[] balls = new Polygon[1];
        private Polygon[] oBalls = new Polygon[1];
        private Polygon[] paddles = new Polygon[0];
        private Polygon[] oPaddles = new Polygon[0];



        private static Polygon DrawBox(int x, int y, int width, int height) {
            Polygon box = new(new Point[] {
                new Point() {
                    X = x,
                    Y = y,
                    Color = Colors.White,
                },
                new Point() {
                    X = x+width,
                    Y = y,
                },
                new Point() {
                    X = x+width,
                    Y = y+height,
                },
                new Point() {
                    X = x,
                    Y = y+width,
                }
            });
            return box;
        }
        

        private void DrawBalls() {
            for (int i = 0; i < balls.Length; i++) {
                balls[i].Anchors[0].Color = Colors.Transparent;
                balls[i].Color = Colors.Transparent;
                balls[i].Outline();
                oBalls[i].Outline();
                foreach (Point p in oBalls[i]._points) {
                    PongDisplay.SetPixel(p.X, p.Y, Colors.Transparent);
                }
                foreach (Point p in balls[i]._points) {
                    PongDisplay.SetPixel(p.X, p.Y, Colors.White);
                }
                oBalls[i] = new Polygon(balls[i].Anchors);
            }
        }
        private void DrawPaddles() {
            for (int i = 0; i< paddles.Length; i++) {
                paddles[i].Anchors[0].Color = Colors.White;
                paddles[i].Color = Colors.White;
                paddles[i].Fill();
                oPaddles[i].Fill();
                foreach (Point p in oPaddles[i]._points) {
                    PongDisplay.SetPixel(p.X, p.Y, Colors.Transparent);
                }
                foreach (Point p in paddles[i]._points) {
                    PongDisplay.SetPixel(p.X, p.Y, Colors.White);
                }
                oPaddles[i] = new Polygon(paddles[i].Anchors);
            }
        }


        private void ShiftPolygon(ref Polygon polygon, int x, int y) {
            foreach (Point anchor in polygon.Anchors) {
                anchor.X += x;
                anchor.Y += y;
            }
        }

        public Pong(ButtonPoller poller) {


            ButtonPoller = poller;

            balls[0] = DrawBox(100, 4, 2, 2);
            oBalls[0] = balls[0];

            /*paddles[0] = DrawBox(0, 0, 1, LogitechGSDK.LOGI_LCD_MONO_HEIGHT/3); // Player
            oPaddles[0] = paddles[0];
            paddles[1] = DrawBox(LogitechGSDK.LOGI_LCD_MONO_WIDTH-2, 25, 1, LogitechGSDK.LOGI_LCD_MONO_HEIGHT/3); // Computer
            oPaddles[1] = paddles[1];*/

            int computerScore = 0;
            int userScore = 0;


            // Move ball, detect collision, draw (ball and paddles), grab key
            // On collision, move as close as possible

            int velocityX = 1;
            int velocityY = 1;

            Random rand = new Random();

            while (bRunning) {
                for (int i = 0; i < balls.Length; i++) {
                    ShiftPolygon(ref balls[i], velocityX, velocityY);
                    balls[i].FindMaxMin();

                    if (balls[i].minX<0) {
                        // End-game
                        computerScore++;
                        //balls[i] = DrawBox(rand.Next(4, 15), rand.Next(2, LogitechGSDK.LOGI_LCD_MONO_HEIGHT-2), 2, 2);
                        velocityX *= -1;
                        velocityX -= rand.Next(0, 2);
                        if (velocityX > 3)
                            velocityX = 3;
                        if (velocityX <= 0)
                            velocityX = 1;
                    } else if (balls[i].maxX > LogitechGSDK.LOGI_LCD_MONO_WIDTH) {
                        userScore++;
                        //balls[i] = DrawBox(rand.Next(LogitechGSDK.LOGI_LCD_MONO_WIDTH-20, LogitechGSDK.LOGI_LCD_MONO_WIDTH-5), rand.Next(2, LogitechGSDK.LOGI_LCD_MONO_HEIGHT - 2), 2, 2);
                        velocityX *= -1;
                        velocityX -= rand.Next(0, 2);
                        if (velocityX < -3)
                            velocityX = -3;
                    }

                    if (balls[i].minY < 0) {
                        ShiftPolygon(ref balls[i], 0, -balls[i].minY);
                        velocityY *= -1;
                    } else if (balls[i].maxY >= LogitechGSDK.LOGI_LCD_MONO_HEIGHT) {
                        ShiftPolygon(ref balls[i], 0, LogitechGSDK.LOGI_LCD_MONO_HEIGHT - balls[i].maxY);
                        velocityY *= -1;
                    }

                    for (int x = 0; x<paddles.Length; x++) {
                        paddles[x].FindMaxMin();
                        bool topLeft = balls[i].minX >= paddles[x].minX && balls[i].minX <= paddles[x].maxX && balls[i].minY >= paddles[x].minY && balls[i].minY <= paddles[x].maxY;
                        bool topRight = balls[i].maxX >= paddles[x].minX && balls[i].maxX <= paddles[x].maxX && balls[i].minY >= paddles[x].minY && balls[i].minY <= paddles[x].maxY;

                        bool bottomLeft = balls[i].minX >= paddles[x].minX && balls[i].minX <= paddles[x].maxX && balls[i].maxY >= paddles[x].minY && balls[i].maxY <= paddles[x].maxY;
                        bool bottomRight = balls[i].maxX >= paddles[x].minX && balls[i].maxX <= paddles[x].maxX && balls[i].maxY >= paddles[x].minY && balls[i].maxY <= paddles[x].maxY;

                        if ((topLeft && bottomLeft) || (topRight && bottomRight)) { // Left or right sides clipped
                            if (velocityX < 0) { // going <---
                                ShiftPolygon(ref balls[i], paddles[i].maxX + ((balls[i].maxX - balls[i].minX) / 2), 0);
                            } else { // going --->
                                ShiftPolygon(ref balls[i], paddles[i].minX - ((balls[i].maxX - balls[i].minX) / 2), 0);
                            }
                            velocityX *= -1;
                        }

                        if ((bottomLeft && bottomRight) || (topLeft && topRight)) {
                            if (velocityY < 0) {
                                ShiftPolygon(ref balls[i], 0, paddles[i].maxY + ((balls[i].maxY - balls[i].minY) / 2));
                            } else {
                                ShiftPolygon(ref balls[i], 0, paddles[i].minY + ((balls[i].maxY - balls[i].minY) / 2));
                            }
                            velocityY *= -1;
                        }
                    }
                }

                DrawPaddles();
                DrawBalls();
                PongDisplay.Draw();

                //LogitechGSDK.LogiLcdMonoSetText(0, userScore + " | " + computerScore);

                //if (LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_0)) {
                //    if (paddles[0].minY >= 2)
                //        ShiftPolygon(ref paddles[0], 0, -2);
                //} else if (LogitechGSDK.LogiLcdIsButtonPressed(LogitechGSDK.LOGI_LCD_MONO_BUTTON_1)) {
                //    if (paddles[0].maxY <= LogitechGSDK.LOGI_LCD_MONO_HEIGHT-2)
                //        ShiftPolygon(ref paddles[0], 0, 2);
                //}
            }
        }
    }
}
