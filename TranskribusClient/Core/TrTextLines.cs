// <copyright file="TrTextLines.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TranskribusClient.Extensions;
    using TranskribusClient.Helpers;

    public class TrTextLines : IEnumerable
    {
        public enum SortType
        {
            LineNumber,
            Vertically,
            Horizontally,
            Logically,
        }

        private SortType sortMethod = SortType.LineNumber;

        public SortType SortMethod
        {
            get
            {
                return sortMethod;
            }

            set
            {
                if (sortMethod != value)
                {
                    sortMethod = value;
                }
            }
        }

        private List<TrTextLine> lines;

        public int Count { get => lines.Count; }

        public TrTextRegion ParentRegion;

        private bool isZeroBased;

        public bool IsZeroBased
        {
            get
            {
                // Debug.WriteLine($"TrTextLines : IsZeroBased: Count = {Count}");
                if (Count > 0)
                {
                    isZeroBased = lines[0].ReadingOrder == 0;
                }
                else
                {
                    isZeroBased = true;
                }

                return isZeroBased;
            }
        }

        public void Add(TrTextLine line)
        {
            lines.Add(line);
            line.ParentContainer = this;
            line.ParentRegion = ParentRegion;

            // Debug.WriteLine($"TrTextLine: Line added. Number = {Line.Number}, ParentRegion = {Line.ParentRegion.Number}");
        }

        // bør IKKE hedde DELETE men REMOVE
        //public void Delete(TrTextLine Line)
        //{
        //    Lines.Remove(Line);
        //}
        public void Clear()
        {
            lines.Clear();
        }

        public void Sort()
        {
            lines.Sort();
        }

        public void RemoveAt(int i)
        {
            lines.RemoveAt(i);
            ParentRegion.HasChanged = true;
        }

        public TrTextLine this[int index]
        {
            get
            {
                return lines[index];
            }

            set
            {
                lines[index] = value;
            }
        }

        public TrTextLine GetLineFromID(string search)
        {
            var line = lines.Where(r => r.ID == search).FirstOrDefault();
            return line;
        }

        //public TrTextLine GetLineFromReadingOrder(int search)
        //{
        //    var line = lines.Where(r => r.ReadingOrder == search).FirstOrDefault();
        //    return line;
        //}

        //public TrTextLine GetLineByNumber(int Number)
        //{
        //    TrTextLine temp = null;
        //    foreach (TrTextLine L in Lines)
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
            TrPairOrderIDs pairs = new TrPairOrderIDs();

            foreach (TrTextLine textLine in lines)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                //    $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");
                TrPairOrderID pair = new TrPairOrderID(textLine.HorizontalOrder, textLine.ID);
                pairs.Add(pair);
            }

            pairs.Sort();

            int i = 0;
            foreach (TrPairOrderID pair in pairs)
            {
                TrTextLine currentLine = GetLineFromID(pair.ID);
                currentLine.ReadingOrder = i;
                currentLine.HasChanged = true;

                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;
            }

            lines.Sort();
        }

        public void ReNumberVertically()
        {
            // NB: ReadingOrder er det centrale - Number dannes af Reading Order
            TrPairOrderIDs pairs = new TrPairOrderIDs();

            foreach (TrTextLine textLine in lines)
            {
                // Debug.WriteLine($"Current ID: {TR.ID} - Current reading order: {TR.ReadingOrder} - " +
                // $"Current Hpos: {TR.Hpos} - Current Vpos: {TR.Vpos}");
                TrPairOrderID pair = new TrPairOrderID(textLine.VerticalOrder, textLine.ID);
                pairs.Add(pair);
            }

            pairs.Sort();

            int i = 0;
            foreach (TrPairOrderID pair in pairs)
            {
                TrTextLine currentLine = GetLineFromID(pair.ID);
                currentLine.ReadingOrder = i;
                currentLine.HasChanged = true;

                // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
                // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
                i++;
            }

            lines.Sort();
        }

        public void ReNumberLogically(int limit)
        {
            // ordner efter de linier, som mennesker opfatter: kræver at betydningen af vpos mindskes
            // sætter også TL.RowNumber og
            // fylder Region.Rows herefter
            int currentVpos = 0;
            int previousVpos = 0;
            int difference = 0;
            int rowCount = 0;

            // string RowTag = "";

            // kræver at linjer først er ordnet helt traditionelt, dvs. vertikalt:
            ReNumberVertically();

            // dernæst gennemløbes alle linier: hvis aktuel vPos er tæt på den forrige linies vPos, sættes aktuel vPos = forrige.vPos
            // "tæt på" defineres som mindre end Limit, sat ovenfor
            foreach (TrTextLine textLine in lines)
            {
                // den første linie bruges som udgangspunkt og sammenlignes IKKE
                if (textLine.Number == 1)
                {
                    // Debug.WriteLine($"Line number = {TL.Number}: Vpos = {TL.Vpos} - Previous = {PreviousVpos} - Current = {CurrentVpos}");
                    previousVpos = textLine.Vpos;
                }
                else
                {
                    currentVpos = textLine.Vpos;
                    difference = Math.Abs(currentVpos - previousVpos);

                    // Debug.WriteLine($"Line number = {TL.Number}: Vpos = {TL.Vpos} - Previous = {PreviousVpos} - Current = {CurrentVpos} - Difference = {Difference}");
                    if (difference < limit)
                    {
                        // samme række
                        textLine.Vpos = previousVpos;

                        // Debug.WriteLine($"Vpos set!");
                    }
                    else
                    {
                        // ny række
                        rowCount++;
                    }

                    previousVpos = textLine.Vpos;
                }

                textLine.RowNumber = rowCount;

                //RowTag = "Row_" + RowCount.ToString("000");
                //TL.AddStructuralTag(RowTag, true);
            }

            // herefter vil VerticalOrder være mere afhængig af hPos end før - og ved en simpel vertikal renummerering, er det fixet
            ReNumberVertically();

            // nu er der orden i sagerne; RowCount = antal rækker minus 1; Alle TL har fået sat RowNumber; nu danner vi det nødvendige antal Rows
            for (int i = 0; i <= rowCount; i++)
            {
                TrRow newRow = new TrRow(i);
                ParentRegion.Rows.Add(newRow);
            }

            // herefter kan alle linierne gennemgås og fordeles på de respektive rows
            foreach (TrTextLine textLine in lines)
            {
                ParentRegion.Rows[textLine.RowNumber].AddCell(textLine);
            }

            int maxCells = ParentRegion.Rows.MaxCellCount;

            // så må vi heller lige teste...
            //foreach (TrRow Row in ParentRegion.Rows)
            //{
            //    if (Row.CellCount == MaxCells)
            //        Debug.WriteLine(Row.ToString());
            //}
        }

        //public void TestSort()
        //{
        //    SortMethod = SortType.Logically;
        //    Lines.Sort();

        //    int i = 0;
        //    foreach (TrTextLine CurrentLine in Lines)
        //    {
        //        CurrentLine.ReadingOrder = i;
        //        CurrentLine.HasChanged = true;
        //        // Debug.WriteLine($"Sorted ID: {CurrentRegion.ID} - Sorted reading order: {CurrentRegion.ReadingOrder} - " +
        //        // $"Sorted Hpos: {CurrentRegion.Hpos} - Sorted Vpos: {CurrentRegion.Vpos}");
        //        i++;
        //    }
        //    SortMethod = SortType.LineNumber;
        //}
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)lines).GetEnumerator();
        }

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
                foreach (TrTextLine textLine in lines)
                {
                    textLine.ChangesUploaded = value;
                }
            }
        }

        // constructor
        public TrTextLines()
        {
            lines = new List<TrTextLine>();
        }
    }
}
