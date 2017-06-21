using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NetduinoLab.Components.Abstract;
using NetduinoLab.Components.Behaviors;

namespace NetduinoLab.Components
{
    public class DigitalComponent : IDigitalComponent, IDynamicDigitalComponent
    {
        private OutputPort outputPort;
        private DigitalComponentBehavior behavior;

        #region Constructors

        public DigitalComponent(Cpu.Pin cpuPin)
        {
            this.Pin = cpuPin;
            this.outputPort = new OutputPort(cpuPin, false);
        }

        #endregion

        #region Public Properties

        public Cpu.Pin Pin { get; set; }

        #endregion

        #region IDigitalComponent Members

        public bool State { get; private set; }

        public void SetState(bool state)
        {
            this.State = state;

            if (this.behavior == null)
                //Default behavior if nothing specified.
                this.outputPort.Write(state);
            else
                this.behavior.SetState(state);
        }

        #endregion

        #region IDynamicDigitalComponent Members

        public Type BehaviorType { get; private set; }

        public void SetBehavior(DigitalComponentBehavior behavior)
        {
            this.BehaviorType = behavior.GetType();

            this.behavior = behavior;
            this.behavior.Add(this);

            //Update behavior immediately.
            this.behavior.SetState(this.State);
        }

        public void ClearBehavior()
        {
            this.BehaviorType = null;

            this.behavior.Remove(this);
            this.behavior = null;

            this.SetState(this.State);
        }

        #endregion
    }
}
