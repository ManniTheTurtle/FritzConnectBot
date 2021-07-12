using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;

namespace FritzConnectBot
{
    public static class SipSorceryTest
    {
        public static void testsip()
        {
            string USERNAME = "Michaels";
            string PASSWORD = "CloudCall2022";
            string DOMAIN = "11.11.25.1";
            int EXPIRY = 120;

            int SIP_LISTEN_PORT = 5060;
            int SIPS_LISTEN_PORT = 5061;
            int SIP_WEBSOCKET_LISTEN_PORT = 80;
            int SIP_SECURE_WEBSOCKET_LISTEN_PORT = 443;

            var sipTransport = new SIPTransport();


            sipTransport.AddSIPChannel(new SIPTCPChannel(new IPEndPoint(IPAddress.Any, SIP_LISTEN_PORT)));
            sipTransport.AddSIPChannel(new SIPTLSChannel(new X509Certificate2("localhost.pfx"), new IPEndPoint(IPAddress.Any, SIPS_LISTEN_PORT)));
            sipTransport.AddSIPChannel(new SIPWebSocketChannel(IPAddress.Any, SIP_WEBSOCKET_LISTEN_PORT));
            X509Certificate2 localhostCertificate = new X509Certificate2();
            sipTransport.AddSIPChannel(new SIPWebSocketChannel(IPAddress.Any, SIP_SECURE_WEBSOCKET_LISTEN_PORT, localhostCertificate));


            var regUserAgent = new SIPRegistrationUserAgent(sipTransport, USERNAME, PASSWORD, DOMAIN, EXPIRY);

            regUserAgent.RegistrationFailed += (uri, err) => Console.WriteLine($"{uri.ToString()}: {err}");
            regUserAgent.RegistrationTemporaryFailure += (uri, msg) => Console.WriteLine($"{uri.ToString()}: {msg}");
            regUserAgent.RegistrationRemoved += (uri) => Console.WriteLine($"{uri.ToString()} registration failed.");
            regUserAgent.RegistrationSuccessful += (uri) => Console.WriteLine($"{uri.ToString()} registration succeeded.");

            regUserAgent.Start();

            if (regUserAgent.IsRegistered == true)
            {

            }
            
        }
    }
}
