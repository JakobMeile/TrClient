using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TrClient;
using System.Diagnostics;

namespace TrClient
{
    public class clsTrMarcFields : IEnumerable
    {
        private List<clsTrMarcField> Fields;
        public int Count { get => Fields.Count; }

        public clsTrMarcRecord ParentRecord;

        public void Add(clsTrMarcField Field)
        {
            Fields.Add(Field);
            Field.ParentContainer = this;
            Field.ParentRecord = this.ParentRecord;
        }

        public void Clear()
        {
            Fields.Clear();
        }

        public void Sort()
        {
            Fields.Sort();
        }

        public void RemoveAt(int i)
        {
            Fields.RemoveAt(i);
        }

        public clsTrMarcField this[int index]
        {
            get { return Fields[index]; }
            set { Fields[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Fields).GetEnumerator();
        }

        public clsTrMarcFields()
        {
            Fields = new List<clsTrMarcField>();
        }

    }
}
