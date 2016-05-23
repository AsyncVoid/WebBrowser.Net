using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WBN.Net
{
    public class HttpHeader
    {
        public List<Field> Fields { get; set; } = new List<Field>();

        public override string ToString()
        {
            string re = String.Empty;

            foreach (var field in Fields)
            {
                re += field.ToString() + Environment.NewLine;
            }

            re += Environment.NewLine;

            return re;
        }

        public static HttpHeader Parse(string raw)
        {
            var re = new HttpHeader();
            foreach(var split in raw.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var s = new string[2];
                bool secondPart = false;
                string temp = String.Empty;
                for (int i = 0; i < split.Length; i++)
                {
                    if(!secondPart)
                    {
                        if (split[i] == ':')
                        {
                            secondPart = true;
                            s[0] = temp.Trim();
                            temp = String.Empty;
                        }
                        else
                        {
                            temp += split[i];
                        }
                    }
                    else
                    {
                        temp += split[i];
                    }
                }
                s[1] = temp.Trim();
                if (s.Length == 1)
                {
                    re.Fields.Add(new Field(true) { Value = split });
                }
                else
                {
                    re.Fields.Add(new Field { Key = s[0], Value = s[1] });
                }
            }
            return re;
        }
    }
}
