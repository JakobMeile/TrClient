// <copyright file="TrTagTextualComment.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core.Tags
{
    using System.Text;
    using System.Text.RegularExpressions;

    public class TrTagTextualComment : TrTagTextual
    {
        public string Comment { get; set; }

        public TrTagTextualComment(int sOffset, int sLength, string sComment)
            : base("comment", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically
            Comment = sComment;
        }

        public TrTagTextualComment(string sProperties)
            : base("comment", sProperties)
        {
            // constructor for reading XML files
            string propName;
            string propValue;

            Comment = string.Empty;

            for (int i = 0; i < Properties.Count; i++)
            {
                propName = Properties[i].Name;
                propValue = Properties[i].Value;

                switch (propName)
                {
                    case "comment":
                        Comment = Regex.Unescape(propValue);
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
                isEmpty = Comment == string.Empty;
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

            if (Comment != string.Empty)
            {
                sb.Append("comment:");
                sb.Append(Comment);
                sb.Append("; ");
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}
