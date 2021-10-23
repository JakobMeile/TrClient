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
    public class TrTag_ReadingOrder : TrTag
    {
        int Index;

        // Constructor til både indlæsning og nye regioner/linier
        public TrTag_ReadingOrder(int Order)
        {
            Type = "readingOrder";
            string PropertyValue = Order.ToString();
            TrTagProperty P = new TrTagProperty("index", PropertyValue);
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
        //public TrTag_ReadingOrder(string sType, string sProperties) : base(sType, sProperties)
        //{
        //    if (Properties[0].Name == "index")
        //        Index = (int)Convert.ToInt32(Properties[0].Value);
        //}


    }
}
