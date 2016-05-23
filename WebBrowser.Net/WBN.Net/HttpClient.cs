using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WBN.Net
{
    public class HttpClient
    {
        public HttpResponse HttpPost(Uri uri)
        {
            var header = new HttpHeader();
            header.Fields.Add(new Field(true) { Value = "POST " + uri.ToString() + " HTTP/1.1" });
            header.Fields.Add(new Field(false) { Key = "User-Agent", Value = "WebBrowser.Net/1.0" });
            header.Fields.Add(new Field(false) { Key = "Host", Value = uri.Host});
            header.Fields.Add(new Field(false) { Key = "Accept-Language", Value = "en-us" });
            header.Fields.Add(new Field(false) { Key = "Connection", Value = "Keep-Alive" });
            string head = String.Empty;
            string body = String.Empty;
            string dl = DownloadString(header, uri);
            bool flag = false;
            for (int i = 0; i < dl.Length; i++)
            {
                var x = dl[i];
                if (flag)
                {
                    body += x;
                }
                else
                {
                    if (x == '\r' && dl[i + 1] == '\n' && dl[i + 2] == '\r' && dl[i + 3] == '\n')
                    {
                        i += 3;
                        flag = true;
                    }
                    else
                    {
                        head += x;
                    }
                }
            }
            return new HttpResponse() { Header = HttpHeader.Parse(head), Body = body };
        }

        public HttpResponse HttpGet(Uri uri)
        {
            var header = new HttpHeader();
            header.Fields.Add(new Field(true) { Value = "GET " + uri.ToString() + " HTTP/1.1"});
            header.Fields.Add(new Field(false) { Key = "User-Agent", Value = "WebBrowser.Net/1.0" });
            header.Fields.Add(new Field(false) { Key = "Host", Value = uri.Host });
            header.Fields.Add(new Field(false) { Key = "Accept-Language", Value = "en-us" });
            header.Fields.Add(new Field(false) { Key = "Connection", Value = "Keep-Alive" });
            string head = String.Empty;
            string body = String.Empty;
            string dl = DownloadString(header, uri);
            bool flag = false;
            for (int i = 0; i < dl.Length; i++)
            {
                var x = dl[i];
                if(flag)
                {
                    body += x;
                }
                else
                {
                    if(x == '\r' && dl[i+1] == '\n' && dl[i + 2] == '\r' && dl[i + 3] == '\n')
                    {
                        i += 3;
                        flag = true;
                    }
                    else
                    {
                        head += x;
                    }
                }
            }
            return new HttpResponse() { Header = HttpHeader.Parse(head), Body = body };
        }

        private string DownloadString(HttpHeader header, Uri uri)
        {
            string re = String.Empty;
            var tcp = new TcpClient();
            tcp.Connect(DnsLookup(uri.Host), uri.Port);
            var stream = tcp.GetStream();

            var request = header.ToString();
            stream.Write(Encoding.UTF8.GetBytes(request), 0, request.Length);
            while (tcp.Connected)
            {
                while (stream.DataAvailable)
                {
                    byte[] buffer = new byte[4098];
                    var x = stream.Read(buffer, 0, 4098);
                    Array.Resize<byte>(ref buffer, x);
                    re = Encoding.UTF8.GetString(buffer);
                    tcp.Close();
                    stream.Dispose();
                    break;
                }
            }
            return re;
        }

        private string DnsLookup(string host)
        {
            return Dns.GetHostEntry(host).AddressList[0].ToString();
        }
    }
}
