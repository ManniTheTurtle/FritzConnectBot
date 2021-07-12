using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FritzConnectBot
{
    public partial class FritzConnectorMain : DevExpress.XtraEditors.XtraForm
    {
        XElement info;
        IndependentsoftSIPTest idependentsip;

        public FritzConnectorMain()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //Test();

            //SipSorceryTest.testsip();

            idependentsip = new IndependentsoftSIPTest();

            idependentsip.Register();

            label1.Text = IndependentsoftSIPTest.mylog;
        }

        // Disconnect Button
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            idependentsip.Disconnect();
        }

        // Accept Request Button
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //accept any request from server or another sip user agent
            if (IndependentsoftSIPTest.eingehenderanruf != null)
            {
                IndependentsoftSIPTest.client.AcceptRequest(IndependentsoftSIPTest.eingehenderanruf);

                listBoxControl1.Items.Add(IndependentsoftSIPTest.eingehenderanruf.From);
                listBoxControl1.Items.Add(IndependentsoftSIPTest.eingehenderanruf.To);
                listBoxControl1.Items.Add(IndependentsoftSIPTest.eingehenderanruf.Header);
                //listBoxControl1.Items.Add(IndependentsoftSIPTest.eingehenderanruf.SessionDescription);  => null error
                listBoxControl1.Items.Add(IndependentsoftSIPTest.eingehenderanruf.Uri);
                listBoxControl1.Items.Add(IndependentsoftSIPTest.eingehenderanruf.Via);
            }
        }


        // Login To FritzBox:
        public void Test()
        {
            // https://codedocu.de/Net-Framework/WPF/WPF-Projekte/WPF,C_hash_-Code_colon_-Fritzbox-Login-und-Auswertung-per-c_hash_-code?2127
            //-------------------< Test() >-------------------            
            //*test login and get first page

            //--< login >--
            string benutzername = "Michaels";   // egal, was nutzername ist, wird nicht benötigt
            string kennwort = "CloudCall2022";
            // SessionID ermitteln
            string sid = GetSessionId(benutzername, kennwort);
            listBoxControl1.Items.Add("SID:" + sid);
            //--</ login >--

            //----< tests >----

            //< get home page >



            string sHtml_of_website = SeiteEinlesen(@"http://fritz.box/home/home.lua", sid);
            // anzeige fehlt noch

            //</ get home page >

            //string sTest = "<html>hallo this is a html-String</html>";

            //--< loadHTML String to Browser >--
            //clsWebbrowser_Errors.SuppressScriptErrors(ctlBrowser, true);
            //ctlBrowser.NavigateToString(sTest);
            //--</ loadHTML String to Browser >--

            //ctlBrowser.Navigate("http://fritz.box/home/home.lua");
            //mshtml.HTMLDocument doc = ctlBrowser.Document as mshtml.HTMLDocument;
            //doc = ctlBrowser.Document as HtmlDocument;

            //< as htmldoc from webreqest >
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(sHtml_of_website);
            //</ as htmldoc from webreqest >


        }

        public string SeiteEinlesen(string url, string sid)
        {
            //-------------------< SeiteEinlesen() >-------------------
            //*read page with sid access, by webrequest
            Uri uri = new Uri(url + "?sid=" + sid);
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string str = reader.ReadToEnd();
            return str; //*return string-result
            //-------------------</ SeiteEinlesen() >-------------------
        }

        public string GetSessionId(string benutzername, string kennwort)
        {
            //-------------------< GetSessionId() >-------------------
            //*get the current sessionID for fritz.box
            XDocument doc = XDocument.Load(@"http://fritz.box/login_sid.lua");

            info = doc.FirstNode as XElement;
            string sid = info.Element("SID").Value;     // Value von XDocument.Node nach NodeName

            if (sid == "0000000000000000")
            {
                info = doc.FirstNode as XElement;
                string challenge = info.Element("Challenge").Value;     // Value von XDocument.Node nach NodeName

                listBoxControl1.Items.Add("CHALLENGE STRING:" + challenge);
                listBoxControl1.Items.Add("Challenge: " + challenge + Environment.NewLine);

                string sResponse = fl_GetResponse_by_TempUser_Passwort(challenge, kennwort);
                listBoxControl1.Items.Add("Response of TempUser_Passwort: " + sResponse + Environment.NewLine);

                string uri = @"http://fritz.box/login_sid.lua?username=" + benutzername + @"&response=" + sResponse;
                doc = XDocument.Load(uri);

                info = doc.FirstNode as XElement;
                sid = info.Element("SID").Value;     // Value von XDocument.Node nach NodeName
                listBoxControl1.Items.Add("SID:" + sid + Environment.NewLine);
            }
            return sid;
            //-------------------</ GetSessionId() >-------------------
        }

        public string fl_GetResponse_by_TempUser_Passwort(string challenge, string kennwort)
        {
            //-------------------< get fritzbox-challenge+hashcode () >-------------------
            return challenge + "-" + fl_Get_MD5Hash_of_String(challenge + "-" + kennwort);
            //-------------------</ get fritzbox-challenge+hashcode () >-------------------
        }

        public string fl_Get_MD5Hash_of_String(string input)
        {
            //-------------------< fl_Get_MD5Hash_of_String() >-------------------
            //*create hashcode from string
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Unicode.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
            //-------------------</ fl_Get_MD5Hash_of_String() >-------------------
        }
    }

    public static class clsWebbrowser_Errors
    {
        //*set wpf webbrowser Control to silent
        //*code source: https://social.msdn.microsoft.com/Forums/vstudio/en-US/4f686de1-8884-4a8d-8ec5-ae4eff8ce6db

        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null)
                return;
            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null)
                return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}
