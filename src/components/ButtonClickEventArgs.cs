using System;
using Microsoft.SPOT;

namespace NetduinoLab.Components
{
    public class ButtonClickEventArgs : EventArgs
    {
        #region Constructors

        public ButtonClickEventArgs(
            ButtonClick buttonClick, int durationInMilliseconds = -1)
        {
            this.ButtonClick = buttonClick;
            this.DurationInMilliseconds = durationInMilliseconds;
        }

        #endregion

        #region Public Properties

        public ButtonClick ButtonClick { get; private set; }

        public int DurationInMilliseconds { get; private set; }

        #endregion
    }
}
