// <copyright file="TrRow.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Text;
    using TrClient.Core;

    public class TrRow : IComparable, INotifyPropertyChanged
    {
        public TrRows ParentContainer;
        public TrTextRegion ParentRegion;

        public int Number { get; set; }

        public TrTextLines Cells = new TrTextLines();

        public int CellCount
        {
            get { return Cells.Count; }
        }

        // constructor
        public TrRow(int rowNumber)
        {
            Number = rowNumber;
        }

        public void AddCell(TrTextLine c)
        {
            Cells.Add(c);
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
            var row = obj as TrTextLine;
            return Number.CompareTo(row.Number);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Row #");
            sb.Append(Number.ToString().PadLeft(3));
            sb.Append(" - ");
            sb.Append("Cells:");
            sb.Append(CellCount.ToString().PadLeft(3));
            sb.Append(" - ");
            sb.Append("Content: ");

            foreach (TrTextLine c in Cells)
            {
                sb.Append(" / ");
                sb.Append(c.TextEquiv);
            }

            return sb.ToString().Trim();
        }
    }
}
