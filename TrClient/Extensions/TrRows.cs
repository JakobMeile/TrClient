// <copyright file="TrRows.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using TrClient.Core;

    public class TrRows : IEnumerable
    {
        private List<TrRow> rows;

        public int Count { get => rows.Count; }

        private int maxCellCount = 0;

        public int MaxCellCount
        {
            get
            {
                if (Count > 0)
                {
                    foreach (TrRow row in rows)
                    {
                        if (row.CellCount > maxCellCount)
                        {
                            maxCellCount = row.CellCount;
                        }
                    }
                }

                return maxCellCount;
            }
        }

        public TrTextRegion ParentRegion;

        // constructor
        public TrRows()
        {
            rows = new List<TrRow>();
        }

        public void Add(TrRow row)
        {
            rows.Add(row);
            row.ParentContainer = this;
            row.ParentRegion = ParentRegion;
        }

        public void Delete(TrRow row)
        {
            rows.Remove(row);
        }

        public void Clear()
        {
            rows.Clear();
        }

        public void Sort()
        {
            rows.Sort();
        }

        public void RemoveAt(int i)
        {
            rows.RemoveAt(i);
            ParentRegion.HasChanged = true;
        }

        public TrRow this[int index]
        {
            get { return rows[index]; }
            set { rows[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)rows).GetEnumerator();
        }
    }
}
