using System;
using Microsoft.SPOT;
using NetduinoLab.Components.Abstract;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Collections;

namespace NetduinoLab.Components
{
    public class ClickButton : DigitalComponentCollection
    {
        public delegate void ButtonClickEventHandler(object sender, ButtonClickEventArgs e);
        public event ButtonClickEventHandler ButtonClicked;

        private InterruptPort interruptPort;
        private DateTime downClickTime;

        #region Constructors

        //Should resistorMode be made a property on the button so it can be changed during runtime?
        public ClickButton(Cpu.Pin cpuPin, 
            Port.ResistorMode resistorMode = Port.ResistorMode.Disabled) : base()
        {
            this.interruptPort = new InterruptPort(
                //Can we use InterruptMode to ignore whether it was pressed or depressed?
                cpuPin, false, resistorMode, Port.InterruptMode.InterruptEdgeBoth);
            this.interruptPort.OnInterrupt += this.interruptPort_OnInterrupt;
        }

        #endregion

        #region EventHandlers

        private void interruptPort_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            ButtonClick buttonClick = data2 == 1 ? 
                ButtonClick.Up : ButtonClick.Down;

            if (buttonClick == ButtonClick.Down)
                this.downClickTime = time;

            if (this.ButtonClicked != null)
            {
                if (buttonClick == ButtonClick.Down)
                    this.ButtonClicked(this, new ButtonClickEventArgs(buttonClick));
                else
                {
                    TimeSpan clickDuration = time - this.downClickTime;

                    int clickDurationInMilliseconds = 
                        (clickDuration.Seconds * 1000) + clickDuration.Milliseconds;

                    this.ButtonClicked(this, new ButtonClickEventArgs(
                        buttonClick, clickDurationInMilliseconds));
                }
            }
            //Default behavior.
            else
                //Only change state of output component on up click.  
                //This can be made more flexible.
                if (buttonClick == ButtonClick.Up)
                    foreach (IDigitalComponent digitalComponent in this)
                        digitalComponent.SetState(!digitalComponent.State);
        }

        #endregion
    }
}
