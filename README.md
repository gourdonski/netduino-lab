# Netduino Lab

This is a library that targets the <a href="http://www.netmf.com/" target="_blank">.NET Micro Framework</a> and includes a light-weight framework for interacting with your components.  It wasn't finished but has the following features:
* Web server that supports standard HTTP methods and uses delegates to handle routing
* Utility for setting device clock from an NTP server
* Rolling file logger
* MicroSD file system access
* Ability to sync and manipulate collections of components
* Pulsing behavior that can be dynamically applied to any digital component
* Seven segment display that can be manipulated over the internet

To run the web server, check out the BasicWebServerSample in the repository, which is where the following snippet is from:

```c#
using (var webServer = new WebServer(fileLogger))
{
  webServer.AddRouteHandler(new RouteHandler(
    HttpMethod.Get, "test", program.handleTestRequest));

  webServer.Start();

  Thread.Sleep(webServerDuration);
}
```

**Note:** this library was developed with the Netduino Plus 2 board.

![httpatomoreillycomsourceoreillyimages9857142](https://cloud.githubusercontent.com/assets/4038675/13768666/5651174a-ea3e-11e5-9bdd-40a9e26f118c.png)

