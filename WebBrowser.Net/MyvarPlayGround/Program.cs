using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBN.Render;

namespace MyvarPlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new HtmlParser(@"
                                    <html>
                                     <head>
                                        <title>Test</title>
                                     </head>

                                     <body>
                                      <div id=testid name=" + '"' + "testname" + '"' + @"></div>
                                     </body>  
     
                                    </html>
                                    ").Length;
        }
    }
}
