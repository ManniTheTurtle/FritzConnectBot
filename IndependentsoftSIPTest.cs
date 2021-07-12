using Independentsoft.Sip;
using Independentsoft.Sip.Responses;
using Independentsoft.Sip.Sdp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FritzConnectBot
{
    class IndependentsoftSIPTest
    {
        private static Logger logger;
        public static SipClient client;
        System.Net.IPAddress localAddress;

        public static string mylog;

        //https://www.independentsoft.de/sip/tutorial/
        public void Register()
        {
            //client = new SipClient("11.11.25.1", "Michaels", "CloudCall2022");

            ////create logger
            //logger = new Logger();
            //logger.WriteLog += new WriteLogEventHandler(OnWriteLog);
            //client.Logger = logger;

            //client.ReceiveRequest += new ReceiveRequestEventHandler(OnReceiveRequest);
            //client.ReceiveResponse += new ReceiveResponseEventHandler(OnReceiveResponse);

            //System.Net.IPAddress localAddress = System.Net.IPAddress.Parse("11.11.25.57");
            //client.LocalIPEndPoint = new System.Net.IPEndPoint(localAddress, 5060);

            //client.Connect();

            //// Register: (nur einmalig nötig?)
            //client.Register("sip:11.11.25.1", "sip:Michaels@fritz.box", "sip:Michaels@" + client.LocalIPEndPoint.ToString());




            client = new SipClient("11.11.25.1", "Manni0102", "Hack0102");

            //create logger
            logger = new Logger();
            logger.WriteLog += new WriteLogEventHandler(OnWriteLog);
            client.Logger = logger;

            client.ReceiveRequest += new ReceiveRequestEventHandler(OnReceiveRequest);
            client.ReceiveResponse += new ReceiveResponseEventHandler(OnReceiveResponse);

            localAddress = System.Net.IPAddress.Parse("11.11.25.57");
            client.LocalIPEndPoint = new System.Net.IPEndPoint(localAddress, 5060);

            client.Connect();

            // Register: (nur einmalig nötig?)
            client.Register("sip:11.11.25.1", "sip:Manni0102@fritz.box", "sip:Manni0102@" + client.LocalIPEndPoint.ToString());

            // Invite:
            //SessionDescription session = new SessionDescription();
            //session.Version = 0;

            //Owner owner = new Owner();
            //owner.Username = "Manni0102";
            //owner.SessionID = 16264;
            //owner.Version = 18299;
            //owner.Address = "11.11.25.57";

            //session.Owner = owner;
            //session.Name = "SIP Call";

            //Connection connection = new Connection();
            //connection.Address = "11.11.25.57";

            //session.Connection = connection;

            //Time time = new Time(0, 0);
            //session.Time.Add(time);

            //Media media1 = new Media();
            //media1.Type = "audio";
            //media1.Port = 25282;
            //media1.TransportProtocol = "RTP/AVP";
            //media1.MediaFormats.Add("0");
            //media1.MediaFormats.Add("101");

            //media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            //media1.Attributes.Add("rtpmap", "101 telephone-event/8000");
            //media1.Attributes.Add("fmtp", "101 0-11");

            //session.Media.Add(media1);

            //RequestResponse inviteRequestResponse = client.Invite("sip:Manni0102@fritz.box", "sip:Michaels@fritz.box", "sip:Manni0102@" + client.LocalIPEndPoint.ToString(), session);
            //client.Ack(inviteRequestResponse);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public static Request eingehenderanruf;
        private static void OnReceiveRequest(object sender, RequestEventArgs e)
        {
            eingehenderanruf = e.Request;

            //if (e.Request.Method == SipMethod.Invite && e.Request.SessionDescription == null)
            //{
            //    OK ok = new OK();
            //    ok.SessionDescription = GenerateSessionDescription();
            //    client.SendResponse(ok, e.Request);

            //    var x = ok.SessionDescription.Media[0].Connection.Address;
            //}
            //else
            //{
            //    client.AcceptRequest(e.Request);
            //}
        }

        private static SessionDescription GenerateSessionDescription()
        {
            SessionDescription session = new SessionDescription();
            session.Version = 0;

            Owner owner = new Owner();
            owner.Username = "Manni0102";
            owner.SessionID = 16264;
            owner.Version = 18299;
            owner.Address = "11.11.25.57";

            session.Owner = owner;
            session.Name = "SIP Call";

            Connection connection = new Connection();
            connection.Address = "11.11.25.57";

            session.Connection = connection;

            Time time = new Time(0, 0);
            session.Time.Add(time);

            Media media1 = new Media();
            media1.Type = "audio";
            media1.Port = 25282;
            media1.TransportProtocol = "RTP/AVP";
            media1.MediaFormats.Add("0");
            media1.MediaFormats.Add("101");

            media1.Attributes.Add("rtpmap", "0 pcmu/8000");
            media1.Attributes.Add("rtpmap", "101 telephone-event/8000");
            media1.Attributes.Add("fmtp", "101 0-11");

            session.Media.Add(media1);

            return session;
        }

        private static void OnReceiveResponse(object sender, ResponseEventArgs e)
        {
        }

        private static void OnWriteLog(object sender, WriteLogEventArgs e)
        {
            Console.Write(e.Log);
            mylog = e.Log;
        }
    }
}
