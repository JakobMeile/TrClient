using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrTextLines : IEnumerable
    {
        public enum SortType
        {
            LineNumber,
            Vertically,
            Horizontally,
            Logically
        }

        private SortType _sortMethod = SortType.LineNumber;
        public SortType SortMethod
        {
            get
            {
                return _sortMethod;
            }
                set
            {
                if (_sortMethod != value)
                    _sortMethod = value;
            }
        }

        private List<clsTrTextLine> Lines;
        public int Count { get => Lines.Count; }

        public clsTrTextRegion ParentRegion;

        private bool _isZeroBased;
        public bool IsZeroBased
        {
            get
            {
                // Debug.WriteLine($"clsTrTextLines : IsZeroBased: Count = {Count}");
                if (Count > 0)
                    _isZeroBased = (Lines[0].ReadingOrder == 0);
                else
                    _isZeroBased = true;
                return _isZeroBased;
            }
        }


        public void Add(clsTrTextLine Line)
        {
            Lines.Add(Line);
            Line.ParentContainer = this;
            Line.ParentRegion = this.ParentRegion;
            // Debug.WriteLine($"clsTrTextLine: Line added. Number = {Line.Number}, ParentRegion = {Line.ParentRegion.Number}");
        }

        public void Delete(clsTrTextLine Line)
        {
            Lines.Remove(Line);
        }
        
        public void Clear()
        {
            Lines.Clear();
        }

        public void Sort()
        {
            Lines.Sort();
        }

        public void RemoveAt(int i)
        {
            Lines.RemoveAt(i);
            ParentRegion.HasChanged = true;
        }

        public clsTrTextLine this[int index]
        {
            get
            {
                //int Temp;
                //if (IsZeroBased)
                //    Temp = index;
                //else
                //    Temp = index - 1;
                //return Lines[Temp];
                return Lines[index];
            }
            set { Lines[index] = value; }
        }


        public clsTrTextLine GetLineFromID(string Search)
        {
            var Line = Lines.Where(r => r.ID == Search).FirstOrDefault();
            return Line;
        }

        public clsTrTextLine GetLineFromReadingOrder(int Search)
        {
            var Line = Lines.Where(r => r.ReadingOrder == Search).FirstOrDefault();
            return Line;
        }

        //public clsTrTextLine GetLineByNumber(int Number)
        //{
        //    clsTrTextLine temp = null;
        //    foreach (clsTrTextLine L in Lines)
        //    {
        //        if (L.Number == Number)
        //            temp = L;
        //        break;
        //    }
        //    return temp;
        //}


        public void ReNumberHorizontally()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order

            clsTrPairOrderIDs Pairs = new clsTrPairOrderIDs();

            foreach (clsTrTextLine TL in Lines)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                //    $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");

                clsTrPairOrderID Pair = new clsTrPairOrderID(TL.HorizontalOrder, TL.ID);
                Pairs.Add(Pair);
            }

            Pairs.Sort();

            int i = 0;
            foreach (clsTrPairOrderID Pair in Pairs)
            {
                clsTrTextLine CurrentLine = GetLineFromID(Pair.ID);
                CurrentLine.ReadingOrder = i;
                CurrentLine.HasChanged = true;
                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;

            }
            Lines.Sort();
        }


        public void ReNumberVertically()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order

            clsTrPairOrderIDs Pairs = new clsTrPairOrderIDs();

            foreach (clsTrTextLine TL in Lines)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                // $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");

                clsTrPairOrderID Pair = new clsTrPairOrderID(TL.VerticalOrder, TL.ID);
                Pairs.Add(Pair);
            }

            Pairs.Sort();

            int i = 0;
            foreach (clsTrPairOrderID Pair in Pairs)
            {
                clsTrTextLine CurrentLine = GetLineFromID(Pair.ID);
                CurrentLine.ReadingOrder = i;
                CurrentLine.HasChanged = true;
                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;

            }
            Lines.Sort();
        }

        public void ReNumberLogically(int Limit)
        {
            // ordner efter de linier, som mennesker opfatter: kræver at betydningen af vpos mindskes
            // sætter også TL.RowNumber og
            // fylder Region.Rows herefter

            int CurrentVpos = 0;
            int PreviousVpos = 0;
            int Difference = 0;
            int RowCount = 0;
            // string RowTag = "";

            // kræver at linjer først er ordnet helt traditionelt, dvs. vertikalt:
            ReNumberVertically();

            // dernæst gennemløbes alle linier: hvis aktuel vPos er tæt på den forrige linies vPos, sættes aktuel vPos = forrige.vPos
            // "tæt på" defineres som mindre end Limit, sat ovenfor
            foreach (clsTrTextLine TL in Lines)
            {

                // den første linie bruges som udgangspunkt og sammenlignes IKKE
                if (TL.Number == 1)
                {
                    // Debug.WriteLine($"Line number = {TL.Number}: Vpos = {TL.Vpos} - Previous = {PreviousVpos} - Current = {CurrentVpos}");
                    PreviousVpos = TL.Vpos;
                }
                else
                {
                    CurrentVpos = TL.Vpos;
                    Difference = Math.Abs(CurrentVpos - PreviousVpos);
                    // Debug.WriteLine($"Line number = {TL.Number}: Vpos = {TL.Vpos} - Previous = {PreviousVpos} - Current = {CurrentVpos} - Difference = {Difference}");
                    if (Difference < Limit)
                    {
                        // samme række
                        TL.Vpos = PreviousVpos;
                        // Debug.WriteLine($"Vpos set!");
                    }
                    else
                    {
                        // ny række
                        RowCount++;
                    }
                    PreviousVpos = TL.Vpos;
                }

                TL.RowNumber = RowCount;
                
                //RowTag = "Row_" + RowCount.ToString("000");
                //TL.AddStructuralTag(RowTag, true);
            }

            // herefter vil VerticalOrder være mere afhængig af hPos end før - og ved en simpel vertikal renummerering, er det fixet
            ReNumberVertically();

            // nu er der orden i sagerne; RowCount = antal rækker minus 1; Alle TL har fået sat RowNumber; nu danner vi det nødvendige antal Rows 
            for (int i = 0; i <= RowCount; i++)
            {
                clsTrRow NewRow = new clsTrRow(i);
                ParentRegion.Rows.Add(NewRow);
            }

            // herefter kan alle linierne gennemgås og fordeles på de respektive rows
            foreach (clsTrTextLine TL in Lines)
            {
                ParentRegion.Rows[TL.RowNumber].AddCell(TL);
            }

            int MaxCells = ParentRegion.Rows.MaxCellCount;
            // så må vi heller lige teste...
            //foreach (clsTrRow Row in ParentRegion.Rows)
            //{
            //    if (Row.CellCount == MaxCells)
            //        Debug.WriteLine(Row.ToString());
            //}
        }


        public void TestSort()
        {
            SortMethod = SortType.Logically;
            Lines.Sort();

            int i = 0;
            foreach (clsTrTextLine CurrentLine in Lines)
            {
                CurrentLine.ReadingOrder = i;
                CurrentLine.HasChanged = true;
                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;
            }
            SortMethod = SortType.LineNumber;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Lines).GetEnumerator();
        }

        private bool _changesUploaded = false;
        public bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                foreach (clsTrTextLine TL in Lines)
                    TL.ChangesUploaded = value;
            }
        }

        // constructor
        public clsTrTextLines()
        {
            Lines = new List<clsTrTextLine>();
        }
    }
}
