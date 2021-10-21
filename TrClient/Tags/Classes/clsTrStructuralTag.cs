using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrStructuralTag : clsTrTag
    {
        public string SubType = "";

        public clsTrStructuralTag(string sType, string sProperties) : base(sType, sProperties)
        {
            Type = "structure";
            SubType = Properties[0].Value;

            // Debug.WriteLine($"Tag constructed (structural) - parent = {ParentLine.Number}");

        }

        public clsTrStructuralTag(string TagName)
        {
            string PropertyString = "type:" + TagName;
            clsTrTagProperty P = new clsTrTagProperty(PropertyString);
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
