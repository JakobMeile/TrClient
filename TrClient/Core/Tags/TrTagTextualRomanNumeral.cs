// <copyright file="TrTagTextualRomanNumeral.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core.Tags
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using DanishNLP;

    public class TrTagTextualRomanNumeral : TrTagTextual
    {
        private string romanValue = string.Empty;

        public string RomanValue
        {
            get
            {
                if (romanValue == string.Empty)
                {
                    if (ParentLine != null)
                    {
                        romanValue = ParentLine.TextEquiv.Substring(Offset, Length);
                    }
                }

                return romanValue;
            }

            set
            {
                romanValue = value;
            }
        }

        public int ArabicEquivalent { get; set; }

        public TrTagTextualRomanNumeral(int sOffset, int sLength, string sRomanValue)
            : base("romanNumeral", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            RomanValue = sRomanValue;
            ArabicEquivalent = ClsRomanNumerals.RomanToArabic(RomanValue);
            Debug.Print($"Added tag: {RomanValue} = {ArabicEquivalent}");
        }

        public TrTagTextualRomanNumeral(string sProperties)
            : base("romanNumeral", sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            ArabicEquivalent = 0;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "arabicEquivalent":
                        ArabicEquivalent = (int)Convert.ToInt32(propValue);
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
                isEmpty = ArabicEquivalent == 0;
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

            sb.Append("arabicEquivalent:");
            sb.Append(ArabicEquivalent.ToString());
            sb.Append("; ");

            sb.Append("}");

            return sb.ToString();
        }
    }
}
