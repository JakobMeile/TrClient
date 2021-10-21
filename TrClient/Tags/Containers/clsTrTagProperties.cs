using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrTagProperties : IEnumerable
    {
        private List<clsTrTagProperty> TagProperties;
        public int Count { get => TagProperties.Count; } // bør måske være -1??? fjernet -1 den 27.10.2020 da det gav bøvl ved nye trtextualtags

        public clsTrTagProperties()
        {
            TagProperties = new List<clsTrTagProperty>();
        }

        public void Add(clsTrTagProperty Property)
        {
            // Debug.Print($"Adding TagProperty: Name= {Property.Name}, Value= {Property.Value}. Property count before: {Count}");
            TagProperties.Add(Property);
            // Debug.Print($"Property count after: {Count}");
        }

        public void Delete(clsTrTagProperty Property)
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

        public clsTrTagProperty this[int index]
        {
            get { return TagProperties[index]; }
            set { TagProperties[index] = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (clsTrTagProperty Property in TagProperties)
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
