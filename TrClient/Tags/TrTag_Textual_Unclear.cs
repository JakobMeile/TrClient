using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
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
    public class TrTag_Textual_Unclear : TrTag_Textual
    {
        public string Alternative { get; set; }
        public string Reason { get; set; }


        public TrTag_Textual_Unclear(int sOffset, int sLength, string sAlternative, string sReason) : base("unclear", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically

            Alternative = sAlternative;
            Reason = sReason;
        }


        public TrTag_Textual_Unclear(string sProperties) : base("unclear", sProperties)
        {
            // constructor for reading XML files

            string PropName;
            string PropValue;

            Alternative = "";
            Reason = "";

            for (int i = 0; i < Properties.Count; i++)
            {
                PropName = Properties[i].Name;
                PropValue = Properties[i].Value;

                switch (PropName)
                {
                    case "alternative":
                        Alternative = Regex.Unescape(PropValue);
                        break;
                    case "reason":
                        Reason = Regex.Unescape(PropValue);
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
                _isEmpty = Alternative == "";
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

            if (Alternative != "")
            {
                sb.Append("alternative:");
                sb.Append(Alternative);
                sb.Append("; ");
            }

            if (Reason != "")
            {
                sb.Append("reason:");
                sb.Append(Reason);
                sb.Append("; ");
            }

            sb.Append("}");

            return sb.ToString();
        }


    }
}
