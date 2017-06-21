using System;
using Microsoft.SPOT;
using NetduinoLab.Common;
using System.Text;

namespace NetduinoLab.Web
{
    internal static class HttpUtility
    {
        public static HttpRequestContext GetHttpRequestContext(string httpRequest)
        {
            HttpMethod httpMethod = HttpMethod.Null;
            string method = httpRequest.Substring(0, 4).ToLower().Trim();

            switch (method)
            {
                case "delete":
                    httpMethod = HttpMethod.Delete;
                    break;
                case "get":
                    httpMethod = HttpMethod.Get;
                    break;
                case "head":
                    httpMethod = HttpMethod.Head;
                    break;
                case "post":
                    httpMethod = HttpMethod.Post;
                    break;
                case "put":
                    httpMethod = HttpMethod.Put;
                    break;
            }

            string path = null;
            int pathStartIndex = httpRequest.IndexOf('/') + 1;
            int pathLength;

            Parameter[] parameters = null;
            string parameterSegment = null;

            //Could there be question marks elsewhere in the request that would cause a false positive here?
            if (httpRequest.IndexOf('?') > -1)
            {
                int parameterStartIndex = httpRequest.IndexOf('?') + 1;
                //Not case-insensitive.  Too brittle?
                int parameterLength = httpRequest.IndexOf(" HTTP") - parameterStartIndex;
                parameterSegment = httpRequest
                    .Substring(parameterStartIndex, parameterLength);
                string[] urlParameters = parameterSegment.Split(new char[] { '&' });
                parameters = new Parameter[urlParameters.Length];

                for (int i = 0; i < urlParameters.Length; i++)
                {
                    string[] urlParameterParts =
                        urlParameters[i].Split(new char[] { '=' });

                    var parameter = new Parameter
                    {
                        Key = urlParameterParts[0],
                        Value = urlParameterParts[1]
                    };

                    parameters[i] = parameter;
                }

                pathLength = parameterStartIndex - pathStartIndex - 1;
            }
            else
                pathLength = httpRequest.IndexOf(" HTTP") - pathStartIndex;

            if (pathLength > 0)
                path = httpRequest.Substring(pathStartIndex, pathLength);

            //Is there always a space after the 'Host:'?
            int hostnameStartIndex = httpRequest.IndexOf("Host: ") + 6;
            //Not case-insensitive.  
            int hostnameLength = httpRequest
                .Substring(hostnameStartIndex, httpRequest.Length - hostnameStartIndex)
                .IndexOf("\r\n");
            string hostname = httpRequest.Substring(hostnameStartIndex, hostnameLength);

            var httpRequestContext = new HttpRequestContext
            {
                HttpMethod = httpMethod,
                Hostname = hostname,
                Route = path,
                Parameters = parameters,
                Url = hostname +
                    (path != null || parameterSegment != null ? "/" : null) + path +
                    (parameterSegment != null ? "?" + parameterSegment : null),
                RawRequest = httpRequest
            };

            return httpRequestContext;
        }

        public static string GetHttpResponseHeader(
            HttpResponseStatus responseStatus, HttpContentType contentType)
        {
            //Not sure if response can be better formed.  
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("HTTP/1.1 ");

            string status = null;

            switch (responseStatus)
            {
                case HttpResponseStatus.OK:
                    status = "200 OK";
                    break;
                case HttpResponseStatus.Accepted:
                    status = "202 Accepted";
                    break;
                case HttpResponseStatus.MovedPermanently:
                    status = "301 Moved Permanently";
                    break;
                case HttpResponseStatus.BadRequest:
                    status = "400 Bad Request";
                    break;
                case HttpResponseStatus.Unauthorized:
                    status = "401 Unauthorized";
                    break;
                case HttpResponseStatus.Forbidden:
                    status = "403 Forbidden";
                    break;
                case HttpResponseStatus.NotFound:
                    status = "404 Not Found";
                    break;
                case HttpResponseStatus.InternalServerError:
                default:
                    status = "500 Server Error";
                    break;
            }

            stringBuilder.Append(status);

            if (contentType != HttpContentType.Null)
            {
                stringBuilder.Append(Constants.NewLine);
                stringBuilder.Append("Content-type: ");

                string type = null;

                //Expand this.
                switch (contentType)
                {
                    case HttpContentType.ApplicationJson:
                        type = "application/json";
                        break;
                    case HttpContentType.Image:
                        type = "image";
                        break;
                    case HttpContentType.TextCss:
                        type = "text/css";
                        break;
                    case HttpContentType.TextHtml:
                        type = "text/html";
                        break;
                    case HttpContentType.TextPlain:
                        type = "text/plain";
                        break;
                }

                stringBuilder.Append(type);
            }

            stringBuilder.Append("; ");
            stringBuilder.Append("charset=UTF-8");
            stringBuilder.Append(Constants.NewLine);
            stringBuilder.Append("Cache-Control: no-cache");
            stringBuilder.Append(Constants.NewLine);
            stringBuilder.Append("Connection: close");
            stringBuilder.Append(Constants.NewLine);
            stringBuilder.Append(Constants.NewLine);

            return stringBuilder.ToString();
        }

        public static HttpContentType GetHttpContentType(string fileExtension)
        {
            HttpContentType httpContentType = HttpContentType.Null;

            //Trim leading period, if it has one.
            if (fileExtension[0] == '.')
                fileExtension = fileExtension.Substring(1, fileExtension.Length - 1);

            //Expand this.
            switch (fileExtension.Trim().ToLower())
            {
                case "bmp":
                case "gif":
                case "jpg":
                case "jpeg":
                case "png":
                    httpContentType = HttpContentType.Image;
                    break;
                case "css":
                    httpContentType = HttpContentType.TextCss;
                    break;
                case "html":
                    httpContentType = HttpContentType.TextHtml;
                    break;
                case "txt":
                    httpContentType = HttpContentType.TextPlain;
                    break;
            }

            return httpContentType;
        }
    }
}
