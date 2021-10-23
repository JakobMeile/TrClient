using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
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


namespace TrClient.Core
{
    public class TrRegions : IEnumerable
    {
        private List<TrRegion> Regions;
        public int Count { get => Regions.Count; }

        public string OrderedGroupID { get; set; }

        public TrTranscript ParentTranscript;

        private bool _changesUploaded = false;
        public bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                foreach (TrRegion TR in Regions)
                    TR.ChangesUploaded = value;
            }
        }


        private bool _isZeroBased;
        public bool IsZeroBased
        {
            get
            {
                _isZeroBased = (Regions[0].ReadingOrder == 0);
                return _isZeroBased;
            }
        }

        public void Add(TrRegion Region)
        {
            Regions.Add(Region);
            // Region.
            Region.ParentContainer = this;
            Region.ParentTranscript = this.ParentTranscript;
            // Debug.WriteLine($"TrRegion: Region added. Number = {Region.Number}");
        }


        public void Remove(TrRegion Region)
        {
            Regions.Remove(Region);
        }

        public void Clear()
        {
            Regions.Clear();
        }

        public void Sort()
        {
            Regions.Sort();
        }

        public void RemoveAt(int i)
        {
            Regions.RemoveAt(i);
            ParentTranscript.HasChanged = true;
        }


        public TrRegion this[int index]
        {
            get { return Regions[index]; }
            set { Regions[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Regions).GetEnumerator();
        }

        public TrRegion GetRegionFromID(string Search)
        {
            var Region = Regions.Where(r => r.ID == Search).FirstOrDefault();
            return Region;
        }

        public TrRegion GetRegionFromReadingOrder(int Search)
        {
            var Region = Regions.Where(r => r.ReadingOrder == Search).FirstOrDefault();
            return Region;
        }


        public void ReNumberHorizontally()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order

            TrPairOrderIDs Pairs = new TrPairOrderIDs();

            foreach (TrRegion TR in Regions)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                //    $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");

                TrPairOrderID Pair = new TrPairOrderID(TR.HorizontalOrder, TR.ID);
                Pairs.Add(Pair);
            }

            Pairs.Sort();

            int i = 0;
            foreach (TrPairOrderID Pair in Pairs)
            {
                TrRegion CurrentRegion = GetRegionFromID(Pair.ID);
                CurrentRegion.ReadingOrder = i;
                CurrentRegion.HasChanged = true;
                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;

            }
            Regions.Sort();
        }


        public void ReNumberVertically()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order

            TrPairOrderIDs Pairs = new TrPairOrderIDs();

            foreach (TrRegion TR in Regions)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                // $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");

                TrPairOrderID Pair = new TrPairOrderID(TR.VerticalOrder, TR.ID);
                Pairs.Add(Pair);
            }

            Pairs.Sort();

            int i = 0;
            foreach (TrPairOrderID Pair in Pairs)
            {
                TrRegion CurrentRegion = GetRegionFromID(Pair.ID);
                CurrentRegion.ReadingOrder = i;
                CurrentRegion.HasChanged = true;
                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;

            }
            Regions.Sort();
        }



        public TrRegions()
        {
            Regions = new List<TrRegion>();
        }

        public TrRegions(bool NewOrderedGroup)
        {
            Regions = new List<TrRegion>();
            if (NewOrderedGroup)
                OrderedGroupID = "ro_" + TrLibrary.GetNewTimeStamp();
        }



    }
}
