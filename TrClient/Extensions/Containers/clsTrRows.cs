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
    public class clsTrRows : IEnumerable
    {
        private List<clsTrRow> Rows;
        public int Count { get => Rows.Count; }

        private int _maxCellCount = 0;
        public int MaxCellCount
        {
            get
            {   if (Count > 0)
                    foreach (clsTrRow Row in Rows)
                        if (Row.CellCount > _maxCellCount)
                            _maxCellCount = Row.CellCount;
                return _maxCellCount;
            }
        }

        public clsTrTextRegion ParentRegion;

        // constructor
        public clsTrRows()
        {
            Rows = new List<clsTrRow>();
        }


        public void Add(clsTrRow Row)
        {
            Rows.Add(Row);
            Row.ParentContainer = this;
            Row.ParentRegion = this.ParentRegion;
        }

        public void Delete(clsTrRow Row)
        {
            Rows.Remove(Row);
        }

        public void Clear()
        {
            Rows.Clear();
        }

        public void Sort()
        {
            Rows.Sort();
        }

        public void RemoveAt(int i)
        {
            Rows.RemoveAt(i);
            ParentRegion.HasChanged = true;
        }

        public clsTrRow this[int index]
        {
            get { return Rows[index]; }
            set { Rows[index] = value; }
        }


        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Rows).GetEnumerator();
        }

    }
}
