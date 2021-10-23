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
    public class TrTag_Structural : TrTag
    {
        public string SubType = "";

        public TrTag_Structural(string sType, string sProperties) : base(sType, sProperties)
        {
            Type = "structure";
            SubType = Properties[0].Value;

            // Debug.WriteLine($"Tag constructed (structural) - parent = {ParentLine.Number}");

        }

        public TrTag_Structural(string TagName)
        {
            string PropertyString = "type:" + TagName;
            TrTagProperty P = new TrTagProperty(PropertyString);
            Properties.Add(P);
            Type = "structure";
            SubType = TagName;
        }

        public new int SortKey
        {
            get
            {
                _sortKey = 0;
                return _sortKey;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                _isEmpty = SubType == "";
                return _isEmpty;
            }
        }

    }
}
