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

namespace TrClient
{
    public class clsTrRegions : IEnumerable
    {
        private List<clsTrRegion> Regions;
        public int Count { get => Regions.Count; }

        public string OrderedGroupID { get; set; }

        public clsTrTranscript ParentTranscript;

        private bool _changesUploaded = false;
        public bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                foreach (clsTrRegion TR in Regions)
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

        public void Add(clsTrRegion Region)
        {
            Regions.Add(Region);
            // Region.
            Region.ParentContainer = this;
            Region.ParentTranscript = this.ParentTranscript;
            // Debug.WriteLine($"clsTrRegion: Region added. Number = {Region.Number}");
        }


        public void Remove(clsTrRegion Region)
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


        public clsTrRegion this[int index]
        {
            get { return Regions[index]; }
            set { Regions[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Regions).GetEnumerator();
        }

        public clsTrRegion GetRegionFromID(string Search)
        {
            var Region = Regions.Where(r => r.ID == Search).FirstOrDefault();
            return Region;
        }

        public clsTrRegion GetRegionFromReadingOrder(int Search)
        {
            var Region = Regions.Where(r => r.ReadingOrder == Search).FirstOrDefault();
            return Region;
        }


        public void ReNumberHorizontally()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order

            clsTrPairOrderIDs Pairs = new clsTrPairOrderIDs();

            foreach (clsTrRegion TR in Regions)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                //    $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");

                clsTrPairOrderID Pair = new clsTrPairOrderID(TR.HorizontalOrder, TR.ID);
                Pairs.Add(Pair);
            }

            Pairs.Sort();

            int i = 0;
            foreach (clsTrPairOrderID Pair in Pairs)
            {
                clsTrRegion CurrentRegion = GetRegionFromID(Pair.ID);
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

            clsTrPairOrderIDs Pairs = new clsTrPairOrderIDs();

            foreach (clsTrRegion TR in Regions)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                // $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");

                clsTrPairOrderID Pair = new clsTrPairOrderID(TR.VerticalOrder, TR.ID);
                Pairs.Add(Pair);
            }

            Pairs.Sort();

            int i = 0;
            foreach (clsTrPairOrderID Pair in Pairs)
            {
                clsTrRegion CurrentRegion = GetRegionFromID(Pair.ID);
                CurrentRegion.ReadingOrder = i;
                CurrentRegion.HasChanged = true;
                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;

            }
            Regions.Sort();
        }



        public clsTrRegions()
        {
            Regions = new List<clsTrRegion>();
        }

        public clsTrRegions(bool NewOrderedGroup)
        {
            Regions = new List<clsTrRegion>();
            if (NewOrderedGroup)
                OrderedGroupID = "ro_" + clsTrLibrary.GetNewTimeStamp();
        }



    }
}
