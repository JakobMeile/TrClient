// <copyright file="RangeCountPair.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RangeCountPair
    {
        public string Range { get; set; }

        public int Count { get; set; }

        public RangeCountPair(string r, int c)
        {
            Range = r;
            Count = c;
        }
    }
}
