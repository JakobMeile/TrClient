// <copyright file="TrTagProperties.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core.Tags
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class TrTagProperties : IEnumerable
    {
        private List<TrTagProperty> tagProperties;

        public int Count { get => tagProperties.Count; } // bør måske være -1??? fjernet -1 den 27.10.2020 da det gav bøvl ved nye trtextualtags

        public TrTagProperties()
        {
            tagProperties = new List<TrTagProperty>();
        }

        public void Add(TrTagProperty property)
        {
            // Debug.Print($"Adding TagProperty: Name= {Property.Name}, Value= {Property.Value}. Property count before: {Count}");
            tagProperties.Add(property);

            // Debug.Print($"Property count after: {Count}");
        }

        public void Delete(TrTagProperty property)
        {
            tagProperties.Remove(property);
        }

        public void Clear()
        {
            tagProperties.Clear();
        }

        public void Sort()
        {
            tagProperties.Sort();
        }

        public TrTagProperty this[int index]
        {
            get { return tagProperties[index]; }
            set { tagProperties[index] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (TrTagProperty property in tagProperties)
            {
                sb.Append(property.ToString());
            }

            sb.Append("}");

            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)tagProperties).GetEnumerator();
        }
    }
}
