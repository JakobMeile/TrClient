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

        // constructor
        public Histogram(TrDocument document, HistogramType type, int bucketSize)
        {
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
            Debug.Print($"Histogram: allLines.Count = {allLines.Count}");

            ILookup<int, TrTextLine> lookUp;

            // NB: crasher, hvis bucketsize er 0!!
            switch (type)
            {
                case HistogramType.LineLength:
                    lookUp = allLines.ToLookup(x => x.Length / bucketSize);
                    break;

                case HistogramType.LineWidth:
                    lookUp = allLines.ToLookup(x => x.Width / bucketSize);
                    break;

                case HistogramType.LineHpos:
                    lookUp = allLines.ToLookup(x => x.Hpos / bucketSize);
                    break;

                case HistogramType.LineVpos:
                    lookUp = allLines.ToLookup(x => x.Vpos / bucketSize);
                    break;

                default:
                    lookUp = null;
                    break;
            }

            // NB: crasher, hvis sekvenser ikke indeholder elementer, dvs. at der ikke er nogen lines i doc
            if (lookUp != null)
            {
                int smallest = lookUp.Min(x => x.Key);
                int largest = lookUp.Max(x => x.Key);

                var query = from x in Enumerable.Range(smallest, largest - smallest + 1)
                            select new
                            {
                                Range = String.Format("{0}-{1}", x * bucketSize, (x + 1) * bucketSize),
                                Count = lookUp[x].Count(),
                            };

                foreach (var p in query)
                {
                    RangeCountPair newPair = new RangeCountPair(p.Range, p.Count);
                    Result.Add(newPair);
                    Debug.Print($"Range: {p.Range} - Count: {p.Count}");
                }
            }
        }
    }
}
