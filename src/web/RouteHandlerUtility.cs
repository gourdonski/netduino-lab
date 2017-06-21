using System;
using Microsoft.SPOT;
using NetduinoLab.Web.Abstract;
using NetduinoLab.Common;

namespace NetduinoLab.Web
{
    public static class RouteHandlerUtility
    {
        #region Public Methods

        public static void RouteToHtmlFile(
            HttpRequestContext requestContext, IHttpResponse response)
        {
            string route = requestContext.Route;

            //If route doesn't include file type, let's just assume it's html.
            if (FileSystem.GetFileExtension(route) == null)
                route += ".html";

            string text;

            if (FileSystem.ReadFile(FileSystem.RootDirectory + @"\" + route, out text))
                response.Send(HttpResponseStatus.OK, HttpContentType.TextHtml, text);
        }

        public static void RouteToFile(
            HttpRequestContext requestContext, IHttpResponse response)
        {
            string route = requestContext.Route;
            string fileStream = null;
            string extension = null;

            if (FileSystem.ReadFile(
                FileSystem.RootDirectory + @"\" + route, out fileStream))
            {
                int extensionIndex = route.LastIndexOf('.') + 1;
                extension = route.Substring(
                    extensionIndex, route.Length - extensionIndex);
                HttpContentType contentType = HttpUtility.GetHttpContentType(extension);

                response.Send(HttpResponseStatus.OK, contentType, fileStream);
            }
        }

        #endregion
    }
}
