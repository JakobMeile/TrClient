// <copyright file="Histogram.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using TrClient.Core;

    public enum HistogramType
    {
        LineLength,
        LineWidth,
        LineHpos,
        LineVpos,
    }

    public class Histogram
    {
        public List<RangeCountPair> Result = new List<RangeCountPair>();

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public int BucketSize { get; private set; }

        public int BucketCount { get; private set; }

        public int NumberOfN { get; private set; }

        // constructor
        public Histogram(TrDocument document, HistogramType type, int bucketSize)
        {
            BucketSize = bucketSize;
            NumberOfN = document.NumberOfLines;
            
            // fylder i en normal liste - ikke containerklasse!
            List<TrTextLine> allLines = new List<TrTextLine>();

            foreach (TrPage page in document.Pages)
            {
                if (page.HasRegions)
                {
                    foreach (TrRegion textRegion in page.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                allLines.Add(textLine);
                                textLine.ParentRegion = (TrTextRegion)textRegion;
                            }
                        }
                    }
                }
            }

            NumberOfN = allLines.Count;
#if DEBUG
            // Debug.Print($"Histogram: allLines.Count = {allLines.Count}");
#endif

            ILookup<int, TrTextLine> lookUp;
            ILookup<int, TrTextLine> values;

            // NB: crasher, hvis bucketsize er 0!!
            switch (type)
            {
                case HistogramType.LineLength:
                    lookUp = allLines.ToLookup(x => x.Length / BucketSize);
                    values = allLines.ToLookup(x => x.Length);
                    break;

                case HistogramType.LineWidth:
                    lookUp = allLines.ToLookup(x => x.Width / BucketSize);
                    values = allLines.ToLookup(x => x.Width); 
                    break;

                case HistogramType.LineHpos:
                    lookUp = allLines.ToLookup(x => x.Hpos / BucketSize);
                    values = allLines.ToLookup(x => x.Hpos);
                    break;

                case HistogramType.LineVpos:
                    lookUp = allLines.ToLookup(x => x.Vpos / BucketSize);
                    values = allLines.ToLookup(x => x.Vpos);
                    break;

                default:
                    lookUp = null;
                    values = null;
                    break;
            }

            if (values != null)
            {
                MinValue = values.Min(x => x.Key);
                MaxValue = values.Max(x => x.Key);
#if DEBUG
                // Debug.Print($"Minimum: {MinValue} - Maximum: {MaxValue}");
#endif
            }

            // NB: crasher, hvis sekvenser ikke indeholder elementer, dvs. at der ikke er nogen lines i doc
            if (lookUp != null)
            {
                int smallest = lookUp.Min(x => x.Key);
                int largest = lookUp.Max(x => x.Key);

                BucketCount = largest - smallest + 1;
#if DEBUG
                // Debug.Print($"Smallest: {smallest} - Largest: {largest} - BucketCount: {BucketCount}");
#endif
                var query = from x in Enumerable.Range(smallest, BucketCount)
                            select new
                            {
                                Range = String.Format("{0}-{1}", x * BucketSize, (x + 1) * BucketSize),
                                Count = lookUp[x].Count(),
                            };

                foreach (var p in query)
                {
                    RangeCountPair newPair = new RangeCountPair(p.Range, p.Count);
                    Result.Add(newPair);
#if DEBUG
                    // Debug.Print($"Range: {p.Range} - Count: {p.Count}");
#endif
                }
            }
        }
    }
}
