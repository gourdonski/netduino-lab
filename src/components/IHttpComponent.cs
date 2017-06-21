using System;
using Microsoft.SPOT;
using NetduinoLab.Web;
using NetduinoLab.Web.Abstract;

namespace NetduinoLab.Components.Abstract
{
    public interface IHttpComponent
    {
        //Not sure about the verbage here.
        void Poll(HttpRequestContext requestContext, IHttpResponse response);
        void Command(HttpRequestContext requestContext, IHttpResponse response);
    }
}
