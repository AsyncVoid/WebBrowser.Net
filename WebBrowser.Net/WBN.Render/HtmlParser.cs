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

        /// <summary>
        /// Create a new HtmlParser
        /// </summary>
        /// <param name="raw">The raw html string</param>
        public HtmlParser(string raw)
        {
            _rawHtml = raw;
            Parse();
        }

        /// <summary>
        /// Init the parseing
        /// </summary>
        private void Parse()
        {
            //the html tag should alway be the first Element
           Root = IterateElement(_rawHtml)[0];
        }

        /// <summary>
        /// Parses raw atributes
        /// </summary>
        /// <param name="attribtesRaw">The raw attributes strubg</param>
        /// <returns></returns>
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
                    //replace origonal value back in
                    val = val.Replace("\"" + z.Key + "\"", z.Value);
                }

                re.Add(x[0].Trim(), val);
            }


            return re;
        }

        /// <summary>
        /// Main Parser loop
        /// </summary>
        /// <param name="raw">The raw html</param>
        /// <returns></returns>
        private List<HtmlElement> IterateElement(string raw)
        {
            var reList = new List<HtmlElement>();

            //tmp vars
            byte parseState = 0;
            byte ParseDepth = 0;
            string tmpName = "";
            string tmpBody = "";
            string tmpAttribute = "";
            var tmpElement = new HtmlElement();
            
            //main loop
            for (int i = 0; i < raw.Length; i++)
            {
                var x = raw[i];

                switch (parseState)
                {
                    case 0:
                        // we are in open tag
                        if (x == '<')
                        {
                            parseState = 1;
                        }
                        break;
                    case 1:
                        if (x == ' ')// there are attributes
                        {
                            parseState = 2;
                        }
                        else
                        {
                            if (x == '>')// no attributes in this tag
                            {
                                parseState = 3;
                                tmpElement.Name = tmpName.Trim();// we have the name store it
                                tmpName = "";

                                //set the atributes to zero
                                tmpElement.Attributes = new Dictionary<string, string>();

                            }
                            else
                            {
                                tmpName += x;//build tag name
                            }
                        }
                        break;
                    case 2:
                        if (x == '>')//tag is done
                        {
                            parseState = 3;
                            tmpElement.Name = tmpName.Trim();// we have the name store it
                            tmpName = "";

                            tmpElement.InnerAttributes = tmpAttribute.Trim();// we have the attributes store there raw form
                            tmpAttribute = "";

                            //parse the raw attribtes
                            tmpElement.Attributes = ParseAttributes(tmpElement.InnerAttributes);
                        }
                        else
                        {
                            //build the attributes raw
                            tmpAttribute += x;
                        }
                        break;
                    case 3:
                        if (x == '<' && raw[i + 1] == '/' && ParseDepth == 0)//we have found the closing tag
                        {
                            parseState = 4;
                            tmpElement.InnerHtml = tmpBody.Trim();// we have the body store it
                            tmpBody = "";
                        }
                        else //we are in the elements body
                        {
                            /*
                             * 
                             * Myvar:
                             * This is the system i use to make sure all the open tags are matching there closing tags
                             */

                            //depth cheack
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

                            //build the elment body
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
                            // we are done with this element

                            if (tmpElement.Name == tmpName.Trim())
                            {
                                //here we recursively parse the body
                                tmpElement.Children = IterateElement(tmpElement.InnerHtml);
                                reList.Add(tmpElement);
                            }
                            else
                            {
                                //TODO: no closing tag error
                            }



                            //reset for next element
                            parseState = 0;
                            ParseDepth = 0;
                            tmpName = "";
                            tmpBody = "";
                            tmpAttribute = "";
                            tmpElement = new HtmlElement();
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

        /// <summary>
        /// Create a new HtmlParser from a file
        /// </summary>
        /// <param name="file">The file path</param>
        /// <returns></returns>
        public static HtmlParser FromFile(string file)
        {
            return new HtmlParser(File.ReadAllText(file));
        }
    }
}
