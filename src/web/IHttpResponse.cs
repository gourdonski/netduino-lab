using System;
using Microsoft.SPOT;
using System.Collections;

namespace NetduinoLab.Web.Abstract
{
    //This needs to be able to handle sending a response with non-standard headers.
    public interface IHttpResponse
    {
        void Send(HttpResponseStatus responseStatus, HttpContentType contentType, object body);
    }
}
