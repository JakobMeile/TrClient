// <copyright file="TrCells.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.Generic;

    public class TrCells : IEnumerable
    {
        private List<TrCell> cells;

        public int Count { get => cells.Count; }

        public TrTableRegion ParentRegion;

        // constructor
        public TrCells()
        {
            cells = new List<TrCell>();
        }

        public void Add(TrCell cell)
        {
            cells.Add(cell);
            cell.ParentContainer = this;
            cell.ParentRegion = ParentRegion;
        }

        // bør IKKE hedde DELETE men REMOVE
        //public void Delete(TrCell Cell)
        //{
        //    Cells.Remove(Cell);
        //}
        public void Clear()
        {
            cells.Clear();
        }

        public void Sort()
        {
            cells.Sort();
        }

        public void RemoveAt(int i)
        {
            cells.RemoveAt(i);
            ParentRegion.HasChanged = true;
        }

        public TrCell this[int index]
        {
            get { return cells[index]; }
            set { cells[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)cells).GetEnumerator();
        }
    }
}
