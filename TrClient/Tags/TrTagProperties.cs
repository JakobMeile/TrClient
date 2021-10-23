using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class TrTagProperties : IEnumerable
    {
        private List<TrTagProperty> TagProperties;
        public int Count { get => TagProperties.Count; } // bør måske være -1??? fjernet -1 den 27.10.2020 da det gav bøvl ved nye trtextualtags

        public TrTagProperties()
        {
            TagProperties = new List<TrTagProperty>();
        }

        public void Add(TrTagProperty Property)
        {
            // Debug.Print($"Adding TagProperty: Name= {Property.Name}, Value= {Property.Value}. Property count before: {Count}");
            TagProperties.Add(Property);
            // Debug.Print($"Property count after: {Count}");
        }

        public void Delete(TrTagProperty Property)
        {
            TagProperties.Remove(Property);
        }

        public void Clear()
        {
            TagProperties.Clear();
        }

        public void Sort()
        {
            TagProperties.Sort();
        }

        public TrTagProperty this[int index]
        {
            get { return TagProperties[index]; }
            set { TagProperties[index] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (TrTagProperty Property in TagProperties)
            {
                sb.Append(Property.ToString());
            }
            sb.Append("}");

            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)TagProperties).GetEnumerator();
        }


    }
}
