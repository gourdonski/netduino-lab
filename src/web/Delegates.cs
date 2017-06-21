using System;
using Microsoft.SPOT;
using NetduinoLab.Web.Abstract;

namespace NetduinoLab.Web
{
    public delegate void HandleRequest(HttpRequestContext requestContext, IHttpResponse response);
}
