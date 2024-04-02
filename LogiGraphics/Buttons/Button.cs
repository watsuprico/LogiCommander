using System;

namespace LogiGraphics.Buttons {
    public class Button {
        // Amount of time needed to pass from a button being pressed to being held
        private readonly static short PRESSED_TO_HELD_THRESHOLD = 500;
        private readonly static short RELEASE_THRESHOLD = 50; // Used to "smooth" out releases. Will cause buttons to hang for a bit after release, however to is to prevent accidental releases from smooshy keys


        private int _pressedTime = 0; // Time _held has been true (ms)
        private int _releasedTime = 0; // Time _released has been true (ms)
        private bool _held = false; // internal 'held'. true if the button is being pressed but has yet to enter the "held" state
        private bool _released = false; // internal 'release'. true if the button was just released, waits until next poll to finalize


        /// <summary>
        /// Button is not being pressed or held.
        /// </summary>
        public bool IsInactive {
            get => CurrentState == ButtonStates.INACTIVE || CurrentState == ButtonStates.RELEASED || CurrentState == ButtonStates.RELEASING;
        }
        /// <summary>
        /// Button is currently pressed or being held.
        /// </summary>
        public bool IsActive {
            get => CurrentState == ButtonStates.PRESSED || CurrentState == ButtonStates.HELD || CurrentState == ButtonStates.HOLDING;
        }
        /// <summary>
        /// Button was just pressed.
        /// </summary>
        public bool IsPressed {
            get => CurrentState == ButtonStates.PRESSED;
        }
        /// <summary>
        /// Button was just released.
        /// </summary>
        public bool IsReleased {
            get => CurrentState == ButtonStates.RELEASED;
        }
        /// <summary>
        /// Button pressed long enough to pass the <see cref="PRESSED_TO_HELD_THRESHOLD"/>.
        /// </summary>
        public bool IsHeld {
            get => CurrentState == ButtonStates.HELD;
        }
        /// <summary>
        /// Button is currently being held down.
        /// </summary>
        public bool IsHolding {
            get => CurrentState == ButtonStates.HOLDING;
        }
        /// <summary>
        /// Button has not been pressed/held/holding for <see cref="RELEASE_THRESHOLD">, now transitioning to released/inactive.
        /// </summary>
        public bool IsReleasing {
            get => CurrentState == ButtonStates.RELEASING;
        }

        /// <summary>
        /// Fired when the button becomes inactive (no longer being pressed or held).
        /// </summary>
        public event EventHandler Inactive;

        /// <summary>
        /// Fired when the button is pressed.
        /// </summary>
        public event EventHandler Pressed;
        /// <summary>
        /// Fire when the button is released.
        /// </summary>
        public event EventHandler Released;

        /// <summary>
        /// Fired when the button has be pressed for <see cref="PRESSED_TO_HELD_THRESHOLD"/> milliseconds.
        /// </summary>
        public event EventHandler Held;

        /// <summary>
        /// Fired after <see cref="Held"/>, signifies the button is actively being held down by the user.
        /// </summary>
        public event EventHandler Holding;

        /// <summary>
        /// The current state of the button.
        /// </summary>
        public ButtonStates CurrentState { get; private set; } = ButtonStates.INACTIVE;

        /// <summary>
        /// Update <see cref="CurrentState"/>.
        /// </summary>
        /// <param name="btnPressed">Whether the button in question is being pressed down.</param>
        /// <param name="pollingSpeed">How fast the polling updates are.</param>
        public void Update(bool btnPressed, int pollingSpeed = 5) {
            if (btnPressed) {
                if (CurrentState == ButtonStates.INACTIVE && !_held) { // Button newly pressed
                    CurrentState = ButtonStates.PRESSED;

                    Pressed?.Invoke(this, EventArgs.Empty);

                } else if (_held) { // Already pressed
                    if (_pressedTime > PRESSED_TO_HELD_THRESHOLD) { // Held long enough to trigger state change to held (which next poll becomes "holding")
                        _pressedTime = 0;
                        CurrentState = ButtonStates.HELD;
                        _held = false;

                        Held?.Invoke(this, EventArgs.Empty);
                    } else { // "Holding" (pressed, holding but not yet triggered)
                        _pressedTime += pollingSpeed;
                    }
                } else if (CurrentState == ButtonStates.PRESSED) { // Count how long it's been pressed for
                    _pressedTime += pollingSpeed;
                    _held = true;

                } else if (CurrentState == ButtonStates.HELD) { // Transition from held -> holding
                    CurrentState = ButtonStates.HOLDING;
                    Holding?.Invoke(this, EventArgs.Empty);
                }

                return;
            }

            // Button is not being pressed.
            if (CurrentState == ButtonStates.INACTIVE) { // Nothing to update
                return;
            }

            if (CurrentState == ButtonStates.RELEASING && !_released) {
                CurrentState = ButtonStates.RELEASED;
                _released = false;
                Released?.Invoke(this, EventArgs.Empty);
            } else if (_released) { // Button released, but wait until transitioning to RELEASING
                if (_releasedTime > RELEASE_THRESHOLD) { // The idea is the reverse of pressed -> held -> holding
                    _releasedTime = 0;                  // We wait to go from pressed|holding -> releasing -> released (smooths out rapid fires from sketchy button connections)
                    CurrentState = ButtonStates.RELEASING;

                    _released = false;
                } else {
                    _releasedTime += pollingSpeed;
                }
            } else if (CurrentState == ButtonStates.RELEASED) {
                CurrentState = ButtonStates.INACTIVE;
                Inactive?.Invoke(this, EventArgs.Empty);
            } else {
                //CurrentState = ButtonStates.RELEASED;
                _released = true;
            }

            _held = false;
            _pressedTime = 0;
        }
    }
}
