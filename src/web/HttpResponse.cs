using System;
using Microsoft.SPOT;
using NetduinoLab.Web.Abstract;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace NetduinoLab.Web
{
    public class HttpResponse : IHttpResponse
    {
        //Store this in a config file?
        private const int BUFFER_SIZE = 1024;

        private Socket connection;

        #region Constructors

        public HttpResponse(Socket connection)
        {
            this.connection = connection;
        }

        #endregion

        #region IHttpResponse Members

        public void Send(HttpResponseStatus responseStatus, 
            HttpContentType contentType, object body)
        {
            //Will need to expand this to support other formats like JSON.
            if (body as string != null)
                this.sendHtml(responseStatus, contentType, (string)body);
            else if (body as FileStream != null)
                this.sendFile(responseStatus, contentType, (FileStream)body);
        }

        #endregion

        #region Private Methods

        private void sendHtml(HttpResponseStatus responseStatus,
            HttpContentType contentType, string html)
        {
            this.connection.Send(Encoding.UTF8.GetBytes(HttpUtility.GetHttpResponseHeader(
                HttpResponseStatus.OK, HttpContentType.TextHtml) + html));
        }

        private void sendFile(HttpResponseStatus responseStatus,
            HttpContentType contentType, FileStream fileStream)
        {
            connection.Send(Encoding.UTF8.GetBytes(HttpUtility
                .GetHttpResponseHeader(HttpResponseStatus.OK, contentType)));

            byte[] fileBuffer;

            if (fileStream.Length > BUFFER_SIZE)
                fileBuffer = new byte[BUFFER_SIZE];
            else
                fileBuffer = new byte[fileStream.Length];

            long unreadBytes = fileStream.Length;

            while (unreadBytes > 0)
            {
                fileStream.Read(fileBuffer, 0, fileBuffer.Length);

                connection.Send(fileBuffer);

                unreadBytes -= fileBuffer.Length;

                if (unreadBytes > 0 && unreadBytes < fileBuffer.Length)
                    fileBuffer = new byte[unreadBytes];
            }
        }

        #endregion
    }
}
