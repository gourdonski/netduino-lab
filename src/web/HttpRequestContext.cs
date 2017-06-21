using System;
using Microsoft.SPOT;
using NetduinoLab.Common;

namespace NetduinoLab.Web
{
    public class HttpRequestContext
    {
        public HttpMethod HttpMethod { get; set; }
        public string Hostname { get; set; }
        public string Route { get; set; }
        public Parameter[] Parameters { get; set; }
        public string Url { get; set; }
        public string RawRequest { get; set; }
    }
}
