namespace LogiGraphics.Buttons {
    /// <summary>
    /// Represents one of the many states a button could be in
    /// </summary>
    public enum ButtonStates : int {
        /// <summary>
        /// Not pressed/held, waiting
        /// </summary>
        INACTIVE = 0,

        /// <summary>
        /// Button has been released
        /// </summary>
        RELEASED = 1,

        /// <summary>
        /// Button just pressed
        /// </summary>
        PRESSED = 2,

        /// <summary>
        /// Button held for enough time to transition into "holding" (transitional state)
        /// </summary>
        HELD = 3,

        /// <summary>
        /// Being constantly held
        /// </summary>
        HOLDING = 4,

        /// <summary>
        /// The button was just let go, will transition to RELEASED on next poll update
        /// </summary>
        RELEASING = 5,
    }
}
