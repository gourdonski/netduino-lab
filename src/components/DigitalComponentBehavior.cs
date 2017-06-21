using System;
using Microsoft.SPOT;
using NetduinoLab.Components.Abstract;

namespace NetduinoLab.Components.Behaviors
{
    public abstract class DigitalComponentBehavior : 
        DigitalComponentCollection, IDigitalComponent
    {
        #region IDigitalComponent Members

        public abstract bool State { get; protected set; }

        public abstract void SetState(bool state);

        #endregion
    }
}
