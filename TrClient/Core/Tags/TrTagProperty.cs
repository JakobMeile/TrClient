// <copyright file="TrTagProperty.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core.Tags
{
    public class TrTagProperty
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public TrTagProperty(string content)
        {
            content = content.Trim();
            int colonPos = content.IndexOf(':');
            Name = content.Substring(0, colonPos).Trim();
            Value = content.Substring(colonPos + 1).Trim();
        }

        public TrTagProperty(string pName, string pValue)
        {
            Name = pName;
            Value = pValue;

            // Debug.Print($"New tag property: Name= {Name}, Value= {Value}");
        }

        public override string ToString()
        {
            string temp = string.Empty;
            if (Value != string.Empty)
            {
                temp = Name + ":" + Value + "; ";
            }

            return temp;
        }
    }
}
