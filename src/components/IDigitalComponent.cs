using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoLab.Components.Abstract
{
    public interface IDigitalComponent
    {
        bool State { get; }

        void SetState(bool state);
    }
}
