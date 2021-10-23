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
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Core
{
    public class TrCells : IEnumerable
    {
        private List<TrCell> Cells;
        public int Count { get => Cells.Count; }

        public TrRegion_Table ParentRegion;

        // constructor
        public TrCells()
        {
            Cells = new List<TrCell>();
        }

        public void Add(TrCell Cell)
        {
            Cells.Add(Cell);
            Cell.ParentContainer = this;
            Cell.ParentRegion = this.ParentRegion;
        }

        public void Delete(TrCell Cell)
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

        public TrCell this[int index]
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
