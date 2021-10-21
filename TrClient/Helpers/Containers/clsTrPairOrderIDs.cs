using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrPairOrderIDs : IEnumerable
    {
        private List<clsTrPairOrderID> Pairs;
        public int Count { get => Pairs.Count; }

        public clsTrPairOrderIDs()
        {
            Pairs = new List<clsTrPairOrderID>();
        }

        public void Add(clsTrPairOrderID Pair)
        {
            Pairs.Add(Pair);
            // Debug.WriteLine($"Pair added. Order = {Pair.Order}");
        }

        public void Sort()
        {
            Pairs.Sort();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Pairs).GetEnumerator();
        }

    }
}
