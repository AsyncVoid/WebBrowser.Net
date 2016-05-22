using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WBN.Core;
using WBN.Render.Model;

namespace WBN.Render
{
    public class HtmlParser
    {
        private string _rawHtml;

        public HtmlElement Root { get; set; }

        public int Length
        {
            get { return _rawHtml.Length; }
        }        

        public HtmlParser(string raw)
        {
            _rawHtml = raw;
            Parse();
        }

        private void Parse()
        {
           Root = IterateElement(_rawHtml)[0];
        }

        private Dictionary<string, string> ParseAttributes(string attribtesRaw)
        {
            var re = new Dictionary<string, string>();

            var stringIndex = new Dictionary<string, string>();

            //add to tmp list to remove duplicates
            List<string> tmp = new List<string>();
            foreach (Match match in Regex.Matches(attribtesRaw, "\"(?<data>([^\"]*))\""))
            {
                var val = match.Groups["data"].Value;
                if (!tmp.Contains(val))
                {
                    tmp.Add(val);
                }
            }

            //loop throw dumplicate free list
            foreach(var i in tmp)
            {
                string placeHolder = "{{" + Utils.RandomString(5) + "}}";
                stringIndex.Add(placeHolder, i);
                attribtesRaw = attribtesRaw.Replace(i, placeHolder);
            }

            //parse string safe atrubutes now
            foreach(var i in attribtesRaw.Split(' '))
            {
                var x = i.Trim().Split('=');
                var val = x[1].Trim();

                foreach(var z in stringIndex)
                {
                    val = val.Replace("\"" + z.Key + "\"", z.Value);
                }

                re.Add(x[0].Trim(), val);
            }


            return re;
        }

        

        private List<HtmlElement> IterateElement(string raw)
        {            
            byte parseState = 0;
            byte ParseDepth = 0;
            string tmpName = "";
            string tmpBody = "";
            string tmpAttribute = "";
            var re = new HtmlElement();
            var reList = new List<HtmlElement>();

            for (int i = 0; i < raw.Length; i++)
            {
                var x = raw[i];

                switch (parseState)
                {
                    case 0:
                        if (x == '<')
                        {
                            parseState = 1;
                        }
                        break;
                    case 1:
                        if (x == ' ')
                        {
                            parseState = 2;
                        }
                        else
                        {
                            if (x == '>')
                            {
                                parseState = 3;
                                re.Name = tmpName.Trim();
                                tmpName = "";
                            }
                            else
                            {
                                tmpName += x;
                            }
                        }
                        break;
                    case 2:
                        if (x == '>')
                        {
                            parseState = 3;
                            re.Name = tmpName.Trim();
                            tmpName = "";

                            re.InnerAttributes = tmpAttribute.Trim();
                            tmpAttribute = "";

                            re.Attributes = ParseAttributes(re.InnerAttributes);
                        }
                        else
                        {
                            tmpAttribute += x;
                        }
                        break;
                    case 3:
                        if (x == '<' && raw[i + 1] == '/' && ParseDepth == 0)
                        {
                            parseState = 4;
                            re.InnerHtml = tmpBody.Trim();
                            tmpBody = "";
                        }
                        else
                        {
                            //<tag></tag>
                            if (x == '<')
                            {
                                ParseDepth += 2;
                            }
                            if (x == '/')
                            {
                                ParseDepth -= 2;
                            }
                            if (x == '>')
                            {
                                ParseDepth--;
                            }
                            //we are in the elements body
                            tmpBody += x;
                        }
                        break;
                    case 4:
                        if (x == '/')
                        {
                            parseState = 5;
                        }
                        break;
                    case 5:
                        if (x == '>')
                        {
                            parseState = 6;


                            if (re.Name == tmpName.Trim())
                            {
                                re.Children = IterateElement(re.InnerHtml);
                                reList.Add(re);
                            }
                            else
                            {
                                //noclosing tag error
                            }



                            //reset for next element
                            parseState = 0;
                            ParseDepth = 0;
                            tmpName = "";
                            tmpBody = "";
                            tmpAttribute = "";
                            re = new HtmlElement();
                        }
                        else
                        {
                            //closing tag name
                            tmpName += x;

                            
                            
                        }
                        break;
                }

            }

            return reList;
        }

        public static HtmlParser FromFile(string file)
        {
            return new HtmlParser(File.ReadAllText(file));
        }
    }
}
