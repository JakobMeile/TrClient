// <copyright file="TrTagTextualAbbrev.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core.Tags
{
    using System.Text;
    using System.Text.RegularExpressions;
    using TrClient.Libraries;

    public class TrTagTextualAbbrev : TrTagTextual
    {
        public string Expansion { get; set; }

        public TrTagTextualAbbrev(int sOffset, int sLength, string sExpansion)
            : base("abbrev", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            Expansion = sExpansion;
        }

        public TrTagTextualAbbrev(string sProperties)
            : base("abbrev", sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            Expansion = string.Empty;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "expansion":
                        Expansion = Regex.Unescape(propValue);
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
                isEmpty = Expansion == string.Empty;
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

            if (Expansion != string.Empty)
            {
                sb.Append("expansion:");
                sb.Append(TrLibrary.EscapeString(Expansion));
                sb.Append("; ");
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
