using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using TrClient;
using DanishNLP;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Tags
{
    public class TrTag_Textual_RomanNumeral : TrTag_Textual
    {
        private string _romanValue = "";
        public string RomanValue
        {
            get
            {
                if (_romanValue == "")
                {
                    if (ParentLine != null)
                        _romanValue = ParentLine.TextEquiv.Substring(Offset, Length);
                }
                return _romanValue;
            }
            set
            {
                _romanValue = value;
            }
        }

        public int ArabicEquivalent { get; set; }


        public TrTag_Textual_RomanNumeral(int sOffset, int sLength, string sRomanValue) : base("romanNumeral", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            RomanValue = sRomanValue;
            ArabicEquivalent = clsRomanNumerals.RomanToArabic(RomanValue);
            Debug.Print($"Added tag: {RomanValue} = {ArabicEquivalent}");
        }


        public TrTag_Textual_RomanNumeral(string sProperties) : base("romanNumeral", sProperties)
        {
            // constructor for reading XML files

            string PropName;
            string PropValue;

            ArabicEquivalent = 0;

            for (int i = 0; i < Properties.Count; i++)
            {
                PropName = Properties[i].Name;
                PropValue = Properties[i].Value;

                switch (PropName)
                {
                    case "arabicEquivalent":
                        ArabicEquivalent = (int)Convert.ToInt32(PropValue);
                        break;
                    default:
                        break;
                }
            }

        }

        public override bool IsEmpty
        {
            get
            {
                _isEmpty = ArabicEquivalent == 0;
                return _isEmpty;
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Type);
            sb.Append(" {");

            sb.Append("offset:");
            sb.Append(Offset.ToString());
            sb.Append("; ");

            sb.Append("length:");
            sb.Append(Length.ToString());
            sb.Append("; ");

            sb.Append("arabicEquivalent:");
            sb.Append(ArabicEquivalent.ToString());
            sb.Append("; ");

            sb.Append("}");

            return sb.ToString();
        }



    }
}
