using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrClient
{
    public class clsTrLogEvents : IEnumerable
    {
        private List<clsTrLogEvent> Events;
        public int Count { get => Events.Count; }

        public void Add(clsTrLogEvent Event)
        {
            Events.Add(Event);
        }

        public void Clear()
        {
            Events.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Events).GetEnumerator();
        }

        public clsTrLogEvents()
        {
            Events = new List<clsTrLogEvent>();
        }

    }
}
