// <copyright file="TrLogEvents.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Helpers
{
    using System.Collections;
    using System.Collections.Generic;

    public class TrLogEvents : IEnumerable
    {
        private List<TrLogEvent> events;

        public int Count { get => events.Count; }

        public void Add(TrLogEvent @event)
        {
            events.Add(@event);
        }

        public void Clear()
        {
            events.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)events).GetEnumerator();
        }

        public TrLogEvents()
        {
            events = new List<TrLogEvent>();
        }
    }
}
