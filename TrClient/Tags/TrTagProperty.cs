using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Tags
{
    public class TrTagProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public TrTagProperty(string Content)
        {
            Content = Content.Trim();
            int ColonPos = Content.IndexOf(':');
            Name = Content.Substring(0, ColonPos).Trim();
            Value = Content.Substring(ColonPos + 1).Trim();
        }

        public TrTagProperty(string pName, string pValue)
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
