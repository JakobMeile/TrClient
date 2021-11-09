// <copyright file="TrTagTextualUnclear.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Core.Tags
{
    using System.Text;
    using System.Text.RegularExpressions;

    public class TrTagTextualUnclear : TrTagTextual
    {
        public string Alternative { get; set; }

        public string Reason { get; set; }

        public TrTagTextualUnclear(int sOffset, int sLength, string sAlternative, string sReason)
            : base("unclear", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            Alternative = sAlternative;
            Reason = sReason;
        }

        public TrTagTextualUnclear(string sProperties)
            : base("unclear", sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            Alternative = string.Empty;
            Reason = string.Empty;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "alternative":
                        Alternative = Regex.Unescape(propValue);
                        break;
                    case "reason":
                        Reason = Regex.Unescape(propValue);
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
                isEmpty = Alternative == string.Empty;
                return isEmpty;
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

            if (Alternative != string.Empty)
            {
                sb.Append("alternative:");
                sb.Append(Alternative);
                sb.Append("; ");
            }

            if (Reason != string.Empty)
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
