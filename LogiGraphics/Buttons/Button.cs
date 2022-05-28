using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogiGraphics.Buttons {
    public class Button {
        // Amount of time needed to pass from a button being pressed to being held
        private readonly static short pressedToHeldTime = 500;
        private readonly static short ReleaseThreshold = 50; // Used to "smooth" out releases. Will cause buttons to hang for a bit after release, however to is to prevent accidental releases from smooshy keys



        private int _pressedTime = 0; // Time _held has been true (ms)
        private int _releasedTime = 0; // Time _released has been true (ms)
        private bool _held = false; // internal 'held'. true if the button is being pressed but has yet to enter the "held" state
        private bool _released = false; // internal 'release'. true if the button was just released, waits until next poll to finalize

        /// <summary>
        /// Button is not being pressed, held, or being hold
        /// </summary>
        public bool IsInactive {
            get { return CurrentState == ButtonStates.INACTIVE || CurrentState == ButtonStates.RELEASED || CurrentState == ButtonStates.RELEASING; }
        }
        /// <summary>
        /// Button is currently pressed, held, or being hold
        /// </summary>
        public bool IsActive {
            get { return CurrentState == ButtonStates.PRESSED || CurrentState == ButtonStates.HELD || CurrentState == ButtonStates.HOLDING; }
        }
        /// <summary>
        /// Button just pressed
        /// </summary>
        public bool IsPressed {
            get { return CurrentState == ButtonStates.PRESSED; }
        }
        /// <summary>
        /// Button just released
        /// </summary>
        public bool IsReleased {
            get { return CurrentState == ButtonStates.RELEASED; }
        }
        /// <summary>
        /// Button pressed long enough to pass the 'pressedToHeldTime'
        /// </summary>
        public bool IsHeld {
            get { return CurrentState == ButtonStates.HELD; }
        }
        /// <summary>
        /// Button is being held down
        /// </summary>
        public bool IsHolding {
            get { return CurrentState == ButtonStates.HOLDING; }
        }
        /// <summary>
        /// Button has not been pressed/held/holding for ReleaseThreshold -> transitioning to released/inactive
        /// </summary>
        public bool IsReleasing {
            get { return CurrentState == ButtonStates.RELEASING; }
        }

        public event EventHandler Inactive;
        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler Held;
        public event EventHandler Holding;

        public ButtonStates CurrentState = ButtonStates.INACTIVE;

        public void Update(bool btnPressed, int pollingSpeed = 5) {
            if (btnPressed) {
                if (CurrentState == ButtonStates.INACTIVE && !_held) { // Button newly pressed
                    CurrentState = ButtonStates.PRESSED;

                    if (Pressed != null)
                        Pressed.Invoke(this, EventArgs.Empty);

                } else if (_held) { // Already pressed
                    if (_pressedTime > pressedToHeldTime) { // Held long enough to trigger state change to held (which next poll becomes "holding")
                        _pressedTime = 0;
                        CurrentState = ButtonStates.HELD;

                        if (Held != null)
                            Held.Invoke(this, EventArgs.Empty);

                        _held = false;
                    } else { // "Holding" (pressed, holding but not yet triggered)
                        _pressedTime += pollingSpeed;
                    }

                } else if (CurrentState == ButtonStates.PRESSED) { // Count how long it's been pressed for
                    _pressedTime += pollingSpeed;
                    _held = true;

                } else if (CurrentState == ButtonStates.HELD) { // Transition from held -> holding
                    CurrentState = ButtonStates.HOLDING;

                    if (Holding != null)
                        Holding.Invoke(this, EventArgs.Empty);
                }

            } else {
                if (CurrentState != ButtonStates.INACTIVE) {
                    if (CurrentState == ButtonStates.RELEASING && !_released) {
                        CurrentState = ButtonStates.RELEASED;
                        _released = false;

                        if (Released != null)
                            Released.Invoke(this, EventArgs.Empty);
                    } else if (_released) { // Button released, but wait until transitioning to RELEASING
                        if (_releasedTime > ReleaseThreshold) { // The idea is the reverse of pressed -> held -> holding
                            _releasedTime = 0;                  // We wait to go from pressed|holding -> releasing -> released (smooths out rapid fires from sketchy button connections)
                            CurrentState = ButtonStates.RELEASING;

                            _released = false;
                        } else {
                            _releasedTime += pollingSpeed;
                        }
                    } else if (CurrentState == ButtonStates.RELEASED) {
                        CurrentState = ButtonStates.INACTIVE;

                        if (Inactive != null)
                            Inactive.Invoke(this, EventArgs.Empty);

                    } else {
                        //CurrentState = ButtonStates.RELEASED;
                        _released = true;
                    }
                    _held = false;
                    _pressedTime = 0;
                }
            }
        }
    }
}
