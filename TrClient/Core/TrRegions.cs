// <copyright file="TrRegions.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TrClient.Helpers;
    using TrClient.Libraries;

    public class TrRegions : IEnumerable
    {
        private List<TrRegion> regions;

        public int Count { get => regions.Count; }

        public string OrderedGroupID { get; set; }

        public TrTranscript ParentTranscript;

        private bool changesUploaded = false;

        public bool ChangesUploaded
        {
            get
            {
                return changesUploaded;
            }

            set
            {
                changesUploaded = value;
                foreach (TrRegion textRegion in regions)
                {
                    textRegion.ChangesUploaded = value;
                }
            }
        }

        private bool isZeroBased;

        public bool IsZeroBased
        {
            get
            {
                isZeroBased = regions[0].ReadingOrder == 0;
                return isZeroBased;
            }
        }

        public void Add(TrRegion region)
        {
            regions.Add(region);

            // Region.
            region.ParentContainer = this;
            region.ParentTranscript = ParentTranscript;

            // Debug.WriteLine($"TrRegion: Region added. Number = {Region.Number}");
        }

        //public void Remove(TrRegion region)
        //{
        //    regions.Remove(region);
        //}

        //public void Clear()
        //{
        //    regions.Clear();
        //}

        //public void Sort()
        //{
        //    regions.Sort();
        //}

        public void RemoveAt(int i)
        {
            regions.RemoveAt(i);
            ParentTranscript.HasChanged = true;
        }

        public TrRegion this[int index]
        {
            get { return regions[index]; }
            set { regions[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)regions).GetEnumerator();
        }

        public TrRegion GetRegionFromID(string search)
        {
            var region = regions.Where(r => r.ID == search).FirstOrDefault();
            return region;
        }

        //public TrRegion GetRegionFromReadingOrder(int search)
        //{
        //    var region = regions.Where(r => r.ReadingOrder == search).FirstOrDefault();
        //    return region;
        //}

        public void ReNumberHorizontally()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order
            TrPairOrderIDs pairs = new TrPairOrderIDs();

            foreach (TrRegion textRegion in regions)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                //    $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");
                TrPairOrderID pair = new TrPairOrderID(textRegion.HorizontalOrder, textRegion.ID);
                pairs.Add(pair);
            }

            pairs.Sort();

            int i = 0;
            foreach (TrPairOrderID pair in pairs)
            {
                TrRegion currentRegion = GetRegionFromID(pair.ID);
                currentRegion.ReadingOrder = i;
                currentRegion.HasChanged = true;

                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;
            }

            regions.Sort();
        }

        public void ReNumberVertically()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order
            TrPairOrderIDs pairs = new TrPairOrderIDs();

            foreach (TrRegion textRegion in regions)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                // $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");
                TrPairOrderID pair = new TrPairOrderID(textRegion.VerticalOrder, textRegion.ID);
                pairs.Add(pair);
            }

            pairs.Sort();

            int i = 0;
            foreach (TrPairOrderID pair in pairs)
            {
                TrRegion currentRegion = GetRegionFromID(pair.ID);
                currentRegion.ReadingOrder = i;
                currentRegion.HasChanged = true;

                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;
            }

            regions.Sort();
        }

        public TrRegions()
        {
            regions = new List<TrRegion>();
        }

        public TrRegions(bool newOrderedGroup)
        {
            regions = new List<TrRegion>();
            if (newOrderedGroup)
            {
                OrderedGroupID = "ro_" + TrLibrary.GetNewTimeStamp();
            }
        }
    }
}
