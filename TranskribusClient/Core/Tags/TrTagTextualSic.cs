// <copyright file="TrTagTextualSic.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Core.Tags
{
    using System.Text;
    using System.Text.RegularExpressions;

    public class TrTagTextualSic : TrTagTextual
    {
        public string Correction { get; set; }

        public TrTagTextualSic(int sOffset, int sLength, string sCorrection)
            : base("sic", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            Correction = sCorrection;
        }

        public TrTagTextualSic(string sProperties)
            : base("sic", sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            Correction = string.Empty;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "correction":
                        Correction = Regex.Unescape(propValue);
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
                isEmpty = Correction == string.Empty;
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

            if (Correction != string.Empty)
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
