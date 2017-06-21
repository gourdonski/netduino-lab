using System;
using Microsoft.SPOT;

namespace NetduinoLab.Web
{
    public class RouteHandler
    {
        public RouteHandler(
            HttpMethod httpMethod, string route, HandleRequest handleRequest)
        {
            this.HttpMethod = httpMethod;
            this.Route = route;
            this.HandleRequest = handleRequest;
        }

        public HttpMethod HttpMethod { get; private set; }
        public string Route { get; private set; }
        public HandleRequest HandleRequest { get; private set; }
    }
}
