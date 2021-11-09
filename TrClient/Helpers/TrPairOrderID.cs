// <copyright file="TrPairOrderID.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Helpers
{
    using System;

    public class TrPairOrderID : IComparable
    {
        public int Order { get; set; }

        public string ID { get; set; }

        // constructor ved skabelse af ny region
        public TrPairOrderID(int inputOrder, string inputID)
        {
            Order = inputOrder;
            ID = inputID;

            // Debug.WriteLine($"New pair! Order = {Order}, ID = {ID}");
        }

        public int CompareTo(object obj)
        {
            var pair = obj as TrPairOrderID;
            return Order.CompareTo(pair.Order);
        }
    }
}
