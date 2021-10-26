// <copyright file="RangeCountPair.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
