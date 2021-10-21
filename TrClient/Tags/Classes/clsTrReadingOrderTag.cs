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
    public class clsTrReadingOrderTag : clsTrTag
    {
        int Index;

        // Constructor til både indlæsning og nye regioner/linier
        public clsTrReadingOrderTag(int Order)
        {
            Type = "readingOrder";
            string PropertyValue = Order.ToString();
            clsTrTagProperty P = new clsTrTagProperty("index", PropertyValue);
            Properties.Add(P);
            //Debug.Print($"New reading order tag made and added! Order = {Order}");
        }

        public new int SortKey
        {
            get
            {
                _sortKey = -1;
                return _sortKey;
            }
        }


        public override bool IsEmpty
        {
            get
            {
                _isEmpty = Properties.Count < 1;
                return _isEmpty;
            }
        }



        // Constructor ved indlæsning af xml - bruges ikke længere
        //public clsTrReadingOrderTag(string sType, string sProperties) : base(sType, sProperties)
        //{
        //    if (Properties[0].Name == "index")
        //        Index = (int)Convert.ToInt32(Properties[0].Value);
        //}


    }
}
