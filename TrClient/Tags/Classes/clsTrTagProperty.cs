using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrTagProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public clsTrTagProperty(string Content)
        {
            Content = Content.Trim();
            int ColonPos = Content.IndexOf(':');
            Name = Content.Substring(0, ColonPos).Trim();
            Value = Content.Substring(ColonPos + 1).Trim();
        }

        public clsTrTagProperty(string pName, string pValue)
        {
            Name = pName;
            Value = pValue;
            // Debug.Print($"New tag property: Name= {Name}, Value= {Value}");
        }

        public override string ToString()
        {
            string Temp = "";
            if (Value != "")
            {
                Temp = Name + ":" + Value + "; ";
            }
            return Temp;
        }

    }
}
