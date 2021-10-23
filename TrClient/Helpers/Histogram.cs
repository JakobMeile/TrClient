using System;
using System.Collections.Generic;
using System.Linq;
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

    public enum HistogramType
    {
        LineLength,
        LineWidth,
        LineHpos,
        LineVpos
    }

    public class RangeCountPair
    {
        public string Range { get; set; }
        public int Count { get; set; }

        public RangeCountPair(string R, int C)
        {
            Range = R;
            Count = C;
        }
    }

    public class Histogram
    {

        public List<RangeCountPair> Result = new List<RangeCountPair>();
        

        // constructor
        public Histogram(TrDocument Document, HistogramType Type, int BucketSize)
        {
            // fylder i en normal liste - ikke containerklasse!

            List<TrTextLine> AllLines = new List<TrTextLine>();

            foreach (TrPage Page in Document.Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (TrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(TrRegion_Text))
                        {
                            foreach (TrTextLine TL in (TR as TrRegion_Text).TextLines)
                            {
                                AllLines.Add(TL);
                                TL.ParentRegion = (TrRegion_Text)TR;
                            }
                        }
                    }
                }
            }


            ILookup<int, TrTextLine> LookUp;


            // BUG: crasher, hvis bucketsize er 0!!
            switch (Type)
            {
                case HistogramType.LineLength:
                    LookUp = AllLines.ToLookup(x => x.Length / BucketSize);
                    break;

                case HistogramType.LineWidth:
                    LookUp = AllLines.ToLookup(x => x.Width/ BucketSize);
                    break;

                case HistogramType.LineHpos:
                    LookUp = AllLines.ToLookup(x => x.Hpos/ BucketSize);
                    break;

                case HistogramType.LineVpos:
                    LookUp = AllLines.ToLookup(x => x.Vpos / BucketSize);
                    break;

                default:
                    LookUp = null;
                    break;
            }
            
            // BUG: crasher, hvis sekvenser ikke indeholder elementer
            if (LookUp != null)
            {
                int Smallest = LookUp.Min(x => x.Key);
                int Largest = LookUp.Max(x => x.Key);

                var Query = from x in Enumerable.Range(Smallest, Largest - Smallest + 1)
                            select new
                            {
                                Range = String.Format("{0}-{1}", x * BucketSize, (x + 1) * BucketSize),
                                Count = LookUp[x].Count(),
                            };

                foreach (var p in Query)
                {
                    RangeCountPair NewPair = new RangeCountPair(p.Range, p.Count);
                    Result.Add(NewPair);
                    Debug.Print($"Range: {p.Range} - Count: {p.Count}");
                }

            }

        }





    }
}
