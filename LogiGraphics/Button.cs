using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogiGraphics {

    public class Button {
        // Amount of time needed to pass from a button being pressed to being held
        private readonly static int pressedToHeldTime = 500;

        public enum ButtonState {
            /// <summary>
            /// Not pressed/held, waiting
            /// </summary>
            INACTIVE,
            /// <summary>
            /// Button has just been released
            /// </summary>
            RELEASED,
            /// <summary>
            /// Button just pressed
            /// </summary>
            PRESSED,
            /// <summary>
            /// Has just been held for pressedToHeldTime
            /// </summary>
            HELD,
            /// <summary>
            /// Being constantly held
            /// </summary>
            HOLDING,
        }

        private int _pressedTime = 0; // Time _held has been true (ms)
        private bool _held = false; // internal 'held'

        public bool IsInactive {
            get { return curState == ButtonState.INACTIVE; }
        }
        public bool IsPressed {
            get { return curState == ButtonState.PRESSED; }
        }
        public bool IsReleased {
            get { return curState == ButtonState.RELEASED; }
        }
        public bool IsHeld {
            get { return curState == ButtonState.HELD; }
        }
        public bool IsHolding {
            get { return curState == ButtonState.HOLDING; }
        }

        public event EventHandler Inactive;
        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler Held;
        public event EventHandler Holding;

        public ButtonState curState = ButtonState.INACTIVE;

        public Button Update(bool btnPressed, int pollingSpeed = 5) {
            if (btnPressed) {
                if (curState == ButtonState.INACTIVE && !_held) {
                    curState = ButtonState.PRESSED;

                    if (Pressed != null)
                        Pressed.Invoke(this, EventArgs.Empty);
                } else if (_held)
                    _pressedTime += pollingSpeed;

                else if (curState == ButtonState.PRESSED) {
                    _pressedTime += pollingSpeed;
                    _held = true;
                    curState = ButtonState.INACTIVE;

                    if (Inactive != null)
                        Inactive.Invoke(this, EventArgs.Empty);

                } else if (curState == ButtonState.HELD) {
                    curState = ButtonState.HOLDING;

                    if (Holding != null)
                        Holding.Invoke(this, EventArgs.Empty);

                }

            } else {
                if (_held)
                    curState = ButtonState.RELEASED;

                else if (curState != ButtonState.INACTIVE) {
                    if (curState == ButtonState.RELEASED) {
                        curState = ButtonState.INACTIVE;

                        if (Inactive != null)
                            Inactive.Invoke(this, EventArgs.Empty);

                    } else {
                        curState = ButtonState.RELEASED;

                        if (Released != null)
                            Released.Invoke(this, EventArgs.Empty);

                    }
                }

                _held = false;
                _pressedTime = 0;

                return this;
            }

            if (_pressedTime > pressedToHeldTime) {
                _pressedTime = 0;
                curState = ButtonState.HELD;

                if (Held != null)
                    Held.Invoke(this, EventArgs.Empty);

                _held = false;
            }

            return this;
        }
    }
}
