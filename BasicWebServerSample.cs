using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using NetduinoLab.Web;
using NetduinoLab.Common;
using NetduinoLab.Web.Abstract;

namespace NetduinoLab.Device
{
    public class Program
    {
        public static void Main()
        {
            var program = new Program();

            int webServerDuration = 600000;
            FileLogger fileLogger = null;

            try
            {
                NtpUtility.RetrieveNtpTime();
                Utility.SetLocalTime(NtpUtility.NtpTime);

		string filePath = FileSystem.RootDirectory + @"\" + "WebServer.log";
                long maxFileSize = 4000;
                bool isRolling = false;
				
                fileLogger = FileLogger.Create(filePath, maxFileSize, isRolling);
                fileLogger.Log(Severity.Info, "Initializing web server.");

                using (var webServer = new WebServer(fileLogger))
                {
                    webServer.AddRouteHandler(new RouteHandler(
                        //Url for this is http://<device-ip-or-hostname>/test...
                        HttpMethod.Get, "test", program.handleTestRequest));

                    webServer.Start();

                    Thread.Sleep(webServerDuration);
                }

                fileLogger.Log(Severity.Info, "Web server shut down.");
            }
            catch (Exception exception)
            {
                if (fileLogger != null)
                    fileLogger.Log(Severity.Error, exception.ToString());
            }
        }

        private void handleTestRequest(
            HttpRequestContext requestContext, IHttpResponse response)
        {
            response.Send(
                HttpResponseStatus.OK, HttpContentType.TextHtml, "<h1>Test succeeded!</h1>");
        }
    }
}
