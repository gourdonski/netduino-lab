using System;
using Microsoft.SPOT;

namespace NetduinoLab.Common.Abstract
{
    public interface ILogger
    {
        void Log(Severity severity, string message);
    }
}
