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
    public class clsTrCells : IEnumerable
    {
        private List<clsTrCell> Cells;
        public int Count { get => Cells.Count; }

        public clsTrTableRegion ParentRegion;

        // constructor
        public clsTrCells()
        {
            Cells = new List<clsTrCell>();
        }

        public void Add(clsTrCell Cell)
        {
            Cells.Add(Cell);
            Cell.ParentContainer = this;
            Cell.ParentRegion = this.ParentRegion;
        }

        public void Delete(clsTrCell Cell)
        {
            Cells.Remove(Cell);
        }

        public void Clear()
        {
            Cells.Clear();
        }

        public void Sort()
        {
            Cells.Sort();
        }

        public void RemoveAt(int i)
        {
            Cells.RemoveAt(i);
            ParentRegion.HasChanged = true;
        }

        public clsTrCell this[int index]
        {
            get { return Cells[index]; }
            set { Cells[index] = value; }
        }


        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Cells).GetEnumerator();
        }


    }
}
