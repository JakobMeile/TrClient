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
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Helpers
{
    public class TrPairOrderIDs : IEnumerable
    {
        private List<TrPairOrderID> Pairs;
        public int Count { get => Pairs.Count; }

        public TrPairOrderIDs()
        {
            Pairs = new List<TrPairOrderID>();
        }

        public void Add(TrPairOrderID Pair)
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
