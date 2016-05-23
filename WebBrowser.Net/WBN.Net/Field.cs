using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBN.Net
{
    public class Field
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsSpecial { get; set; }
        public Field(bool special = false)
        {
            IsSpecial = special;
        }
        public override string ToString()
        {
            return IsSpecial ? Value : Key + ": " + Value;
        }
    }
}
