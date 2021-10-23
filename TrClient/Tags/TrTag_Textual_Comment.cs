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
    public class TrTag_Textual_Comment : TrTag_Textual
    {
        public string Comment { get; set; }

        public TrTag_Textual_Comment(int sOffset, int sLength, string sComment) : base("comment", sOffset, sLength)
        {
            // constructor for adding NEW tag programmatically

            Comment = sComment;
        }



        public TrTag_Textual_Comment(string sProperties) : base("comment", sProperties)
        {
            // constructor for reading XML files

            string PropName;
            string PropValue;

            Comment = "";

            for (int i = 0; i < Properties.Count; i++)
            {
                PropName = Properties[i].Name;
                PropValue = Properties[i].Value;

                switch (PropName)
                {
                    case "comment":
                        Comment = Regex.Unescape(PropValue);
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
                _isEmpty = Comment == "";
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

            if (Comment != "")
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
