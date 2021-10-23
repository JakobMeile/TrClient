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


namespace TrClient.Extensions
{
    public class TrRows : IEnumerable
    {
        private List<TrRow> Rows;
        public int Count { get => Rows.Count; }

        private int _maxCellCount = 0;
        public int MaxCellCount
        {
            get
            {   if (Count > 0)
                    foreach (TrRow Row in Rows)
                        if (Row.CellCount > _maxCellCount)
                            _maxCellCount = Row.CellCount;
                return _maxCellCount;
            }
        }

        public TrRegion_Text ParentRegion;

        // constructor
        public TrRows()
        {
            Rows = new List<TrRow>();
        }


        public void Add(TrRow Row)
        {
            Rows.Add(Row);
            Row.ParentContainer = this;
            Row.ParentRegion = this.ParentRegion;
        }

        public void Delete(TrRow Row)
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

        public TrRow this[int index]
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
