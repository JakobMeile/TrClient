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
    public class TrTag_Textual_Sic : TrTag_Textual
    {
        public string Correction { get; set; }


        public TrTag_Textual_Sic(int sOffset, int sLength, string sCorrection) : base("sic", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically

            Correction = sCorrection;
        }


        public TrTag_Textual_Sic(string sProperties) : base("sic", sProperties)
        {
            // constructor for reading XML files

            string PropName;
            string PropValue;

            Correction = "";

            for (int i = 0; i < Properties.Count; i++)
            {
                PropName = Properties[i].Name;
                PropValue = Properties[i].Value;

                switch (PropName)
                {
                    case "correction":
                        Correction = Regex.Unescape(PropValue);
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
                _isEmpty = Correction == "";
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

            if (Correction != "")
            {
                sb.Append("correction:");
                sb.Append(Correction);
                sb.Append("; ");
            }

            sb.Append("}");

            return sb.ToString();
        }



    }
}
