// <copyright file="TrTagReadingOrder.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core.Tags
{
    public class TrTagReadingOrder : TrTag
    {
        int index;

        // Constructor til både indlæsning og nye regioner/linier
        public TrTagReadingOrder(int order)
        {
            Type = "readingOrder";
            string propertyValue = order.ToString();
            TrTagProperty p = new TrTagProperty("index", propertyValue);
            Properties.Add(p);

            //Debug.Print($"New reading order tag made and added! Order = {Order}");
        }

        public new int SortKey
        {
            get
            {
                sortKey = -1;
                return sortKey;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                isEmpty = Properties.Count < 1;
                return isEmpty;
            }
        }

        // Constructor ved indlæsning af xml - bruges ikke længere
        //public TrTagReadingOrder(string sType, string sProperties) : base(sType, sProperties)
        //{
        //    if (Properties[0].Name == "index")
        //        Index = (int)Convert.ToInt32(Properties[0].Value);
        //}
    }
}
