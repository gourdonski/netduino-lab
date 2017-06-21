using System;
using Microsoft.SPOT;
using System.Net;
using System.Net.Sockets;

namespace NetduinoLab.Common
{
    //Based on the following:
    //1. http://forums.netduino.com/index.php?/topic/2371-sntp-clock-rss-news-reader/
    //2. http://www.jaypm.com/2011/09/setting-the-netduinos-datetime-automatically/

    public static class NtpUtility
    {
        private const int NTP_PORT = 123;
        private const string DEFAULT_NTP_SERVER = "time.nist.gov";

        private static DateTime _ntpTime;
        private static TimeZone _timeZone;
        private static string _ntpServerUrl;

        #region Public Properties

        public static DateTime NtpTime 
        {
            get
            {
                if (!IsNtpTimeSet)
                    throw new Exception("NTP time has not been retrieved.");

                return _ntpTime;
            }
            private set
            {
                _ntpTime = value;
            }
        }

        public static bool IsNtpTimeSet { get; private set; }

        #endregion

        #region Public Methods

        public static DateTime GetNtpTime(
            TimeZone timeZone = TimeZone.Utc, string ntpServerUrl = null)
        {
            _timeZone = timeZone;
            _ntpServerUrl = ntpServerUrl == null ? DEFAULT_NTP_SERVER : ntpServerUrl;

            byte[] buffer = new byte[48];
            //Not sure what this value means.  Apparently it represents the version?
            buffer[0] = 0x1B;

            IPHostEntry ipHostEntry = Dns.GetHostEntry(_ntpServerUrl);
            IPEndPoint ipEndPoint = new IPEndPoint(ipHostEntry.AddressList[0], NTP_PORT);

            using (Socket socket = 
                new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                //Can unhardcode this.  Doing 10 seconds for now.
                socket.ReceiveTimeout = 10000;

                socket.Send(buffer);

                socket.Receive(buffer, buffer.Length, SocketFlags.None);

                socket.Close();
            }

            uint integralPart = 0;
            
            for (int i = 40; i < 44; i++)
                integralPart = (integralPart << 8) | buffer[i];

            uint fractionPart = 0;

            for (int i = 44; i < 48; i++)
                fractionPart = (fractionPart << 8) | buffer[i];

            ulong milliseconds = ((ulong)fractionPart * 1000) / 0x100000000L;

            var ntpTime = new DateTime(1900, 1, 1)
                .AddSeconds(integralPart)
                .AddMilliseconds(milliseconds);

            NtpTime = adjustUtcTime(ntpTime, timeZone);
            IsNtpTimeSet = true;

            return NtpTime;
        }

        public static DateTime RefreshTime()
        {
            if (!IsNtpTimeSet)
                throw new Exception("NTP time has not been retrieved.");

            return GetNtpTime(_timeZone, _ntpServerUrl);
        }

        #endregion

        #region private methods

        //Would be nice if this could just map one timezone onto another, rather
        //than being fixed with UTC time on one side of the conversion.
        private static DateTime adjustUtcTime(DateTime utcTime, TimeZone timeZone)
        {
            DateTime adjustedTime = utcTime;

            //Flesh this out.
            switch (timeZone)
            {
                case TimeZone.MountainTime:
                    adjustedTime = utcTime.Subtract(new TimeSpan(6, 0, 0));

                    break;
            }

            return adjustedTime;
        }

        #endregion
    }
}
