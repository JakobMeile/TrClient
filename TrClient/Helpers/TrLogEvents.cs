using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Helpers
{
    public class TrLogEvents : IEnumerable
    {
        private List<TrLogEvent> Events;
        public int Count { get => Events.Count; }

        public void Add(TrLogEvent Event)
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

        public TrLogEvents()
        {
            Events = new List<TrLogEvent>();
        }

    }
}
