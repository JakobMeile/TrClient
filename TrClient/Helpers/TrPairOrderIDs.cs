﻿// <copyright file="TrPairOrderIDs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Helpers
{
    using System.Collections;
    using System.Collections.Generic;

    public class TrPairOrderIDs : IEnumerable
    {
        private List<TrPairOrderID> pairs;

        public int Count { get => pairs.Count; }

        public TrPairOrderIDs()
        {
            pairs = new List<TrPairOrderID>();
        }

        public void Add(TrPairOrderID pair)
        {
            pairs.Add(pair);

            // Debug.WriteLine($"Pair added. Order = {Pair.Order}");
        }

        public void Sort()
        {
            pairs.Sort();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)pairs).GetEnumerator();
        }
    }
}
