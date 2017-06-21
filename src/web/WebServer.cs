using System;
using Microsoft.SPOT;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections;
using NetduinoLab.Web.Abstract;
using System.IO;
using NetduinoLab.Common;
using NetduinoLab.Common.Abstract;

namespace NetduinoLab.Web
{
    //Based on the following:
    //1. "Getting Started with the Internet of Things" by Cuno Pfister.
    //2. https://netmfwebserver.codeplex.com/SourceControl/latest#WebServer.cs
    //3. Some of the sample projects here: http://forums.netduino.com/

    public class WebServer : IDisposable
    {
        private const int TCP_PORT = 80;
        private const int POLL_TIMEOUT = 5000;
        private const int THREAD_TIMEOUT = 10000;

        private ILogger logger;
        private RouteHandlerCollection routeHandlers = new RouteHandlerCollection();
        private object lockObject = new object();
        private Socket socket;
        private Thread listenThread;
        private bool interrupt;

        #region Constructors

        public WebServer(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion

        ~WebServer()
        {
            this.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            if (this.listenThread == null || !this.listenThread.IsAlive)
            {
                this.interrupt = false;

                this.listenThread =
                    new Thread(new ThreadStart(this.listen));
                this.listenThread.Start();
            }
        }

        public void Stop()
        {
            this.interrupt = true;

            if (this.listenThread != null && this.listenThread.IsAlive)
                this.listenThread.Join(THREAD_TIMEOUT);

            //After forcing listen thread to close, there shouldn't be concurrency issues with this section.
            if (this.socket != null)
            {
                try
                {
                    this.socket.Close();
                }
                //Just in case.  And gulp.
                catch { }
            }
        }

        public void AddRouteHandler(RouteHandler routeHandler)
        {
            lock (this.lockObject)
                this.routeHandlers.Add(routeHandler);
        }

        public void RemoveRouteHandler(RouteHandler routeHandler)
        {
            lock (this.lockObject)
                this.routeHandlers.Remove(routeHandler);
        }

        public void ClearRouteHandlers()
        {
            lock (this.lockObject)
                this.routeHandlers.Clear();
        }

        public bool ContainsRouteHandler(RouteHandler routeHandler)
        {
            //Shouldn't have to lock this.
            return this.routeHandlers.Contains(routeHandler);
        }

        #endregion

        #region Private Methods

        private void listen()
        {
            try
            {
                this.socket = new Socket(
                    AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.Bind(new IPEndPoint(IPAddress.Any, TCP_PORT));
                this.socket.Listen((int)SocketOptionName.MaxConnections);

                while (!this.interrupt)
                    if (this.socket.Poll(POLL_TIMEOUT, SelectMode.SelectRead))
                        using (Socket connection = this.socket.Accept())
                        {
                            try
                            {
                                DateTime requestTime = DateTime.Now;
                                int bytesReceived = connection.Available;

                                if (bytesReceived > 0)
                                {
                                    byte[] requestBuffer = new byte[bytesReceived];

                                    connection.Receive(requestBuffer, bytesReceived, SocketFlags.None);

                                    string httpRequest = new string(Encoding.UTF8.GetChars(requestBuffer));
                                    HttpRequestContext httpRequestContext =  
                                        HttpUtility.GetHttpRequestContext(httpRequest);
                                    var httpResponse = new HttpResponse(connection);

                                    lock (this.lockObject)
                                        foreach (RouteHandler routeHandler in this.routeHandlers)
                                            //Do additional cleanup on the route strings?
                                            if (routeHandler.Route.Trim().ToLower() == 
                                                httpRequestContext.Route.Trim().ToLower() &&
                                                routeHandler.HttpMethod == httpRequestContext.HttpMethod)
                                            {
                                                routeHandler.HandleRequest(httpRequestContext, httpResponse);

                                                //Currently assuming there is only one handler for a given route/HTTP method. 
                                                break;
                                            }
                                }
                            }
                            catch (Exception exception)
                            {
                                //Need to make sure this doesn't rapidly fill the log.
                                this.logger.Log(Severity.Error, exception.ToString());
                            }
                        }
            }
            catch (Exception exception)
            {
                this.logger.Log(Severity.Error, exception.ToString());
            }
        }

        #endregion
    }
}
