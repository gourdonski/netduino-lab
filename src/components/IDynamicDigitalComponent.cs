using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NetduinoLab.Components.Behaviors;

namespace NetduinoLab.Components.Abstract
{
    public interface IDynamicDigitalComponent
    {
        //Because behavior can have its own state, we don't expose the behavior itself since there
        //can be an anomaly between component and behavior states if set separately.  
        Type BehaviorType { get; }

        void SetBehavior(DigitalComponentBehavior behavior);
        void ClearBehavior();
    }
}