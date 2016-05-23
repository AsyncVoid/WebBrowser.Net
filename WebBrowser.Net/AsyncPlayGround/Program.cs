using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBN.Net;
using WBN.Render;

namespace AsyncPlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            string dl = DownloadString("http://google.com");
            var x = new HtmlParser(dl);

        }

        public static string DownloadString(string url)
        {
            var client = new HttpClient();
            var x = client.HttpGet(new Uri(url));
            var statusCode = int.Parse(x.Header.Fields[0].Value.Split(' ')[1]);
            switch (statusCode)
            {
                case 301:
                case 302:
                    var loc = x.Header.Fields.Where((y) => { return y.Key == "Location"; }).First().Value;
                    return DownloadString(loc == "https:///" ? "https://" + new Uri(url).Host : loc);
                case 200:

                    return x.Body;
                default:
                    return statusCode+ "";
            }
        }
    }
}
