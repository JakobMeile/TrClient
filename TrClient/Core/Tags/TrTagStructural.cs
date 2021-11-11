// <copyright file="TrTagStructural.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core.Tags
{
    public class TrTagStructural : TrTag
    {
        public string SubType = string.Empty;

        public TrTagStructural(string sType, string sProperties)
            : base(sType, sProperties)
        {
            Type = "structure";
            SubType = Properties[0].Value;

            // Debug.WriteLine($"Tag constructed (structural) - parent = {ParentLine.Number}");
        }

        public TrTagStructural(string tagName)
        {
            string propertyString = "type:" + tagName;
            TrTagProperty p = new TrTagProperty(propertyString);
            Properties.Add(p);
            Type = "structure";
            SubType = tagName;
        }

        public new int SortKey
        {
            get
            {
                sortKey = 0;
                return sortKey;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                isEmpty = SubType == string.Empty;
                return isEmpty;
            }
        }
    }
}
