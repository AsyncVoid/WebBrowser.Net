using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WBN.Render;

namespace MyvarPlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            var s = new WebClient().DownloadString("https://github.com");
            //var s = File.ReadAllText("test.txt");
            sw.Start();
            var x = new HtmlParser(s);

            sw.Stop();
            Console.WriteLine("Time took (ms): " + sw.Elapsed.Milliseconds);
            Console.ReadKey();
        }
    }
}
