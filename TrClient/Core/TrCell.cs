// <copyright file="TrCell.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.ComponentModel;
    using TrClient.Core.Tags;

    public class TrCell : IComparable, INotifyPropertyChanged
    {
        public TrCells ParentContainer;
        public TrTableRegion ParentRegion;

        public string ID { get; set; }

        public string CoordsString { get; set; }

        public string CornerPoints { get; set; }

        //public string TagString { get; set; }

        public TrTags Tags = new TrTags();
        public TrTagStructural StructuralTag;

        public TrTextLines TextLines = new TrTextLines();

        public int Row { get; set; }

        public int Col { get; set; }

        private int sortOrder;

        public int SortOrder
        {
            get
            {
                sortOrder = (Row * 100) + Col;
                return sortOrder;
            }
        }

        private bool hasLines;

        public bool HasLines
        {
            get
            {
                hasLines = TextLines.Count > 0;
                return hasLines;
            }
        }

        public void Move(int horizontally, int vertically)
        {
            TrCoords c = new TrCoords(CoordsString);
            foreach (TrCoord currentCoord in c)
            {
                currentCoord.X = currentCoord.X + horizontally;
                currentCoord.Y = currentCoord.Y + vertically;
            }

            CoordsString = c.ToString();

            if (HasLines)
            {
                foreach (TrTextLine line in TextLines)
                {
                    line.Move(horizontally, vertically);
                }
            }
        }

        // constructor
        public TrCell(string cID, string cRow, string cCol, string cCoords, string cCornerPts)
        {
            ID = cID;
            Row = Convert.ToInt32(cRow);
            Col = Convert.ToInt32(cCol);
            CoordsString = cCoords;
            CornerPoints = cCornerPts;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public int CompareTo(object obj)
        {
            var cell = obj as TrCell;
            return SortOrder.CompareTo(cell.SortOrder);
        }
    }
}
