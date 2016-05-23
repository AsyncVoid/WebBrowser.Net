using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBN.Render.Model
{
    public class HtmlElement
    {
        public string Name { get; set; }
        public string InnerHtml { get; set; }
        public string InnerAttributes { get; set; }
        public List<HtmlElement> Children { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
