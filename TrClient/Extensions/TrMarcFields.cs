using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

using System.Diagnostics;

namespace TrClient.Extensions
{
    public class TrMarcFields : IEnumerable
    {
        private List<TrMarcField> Fields;
        public int Count { get => Fields.Count; }

        public TrMarcRecord ParentRecord;

        public void Add(TrMarcField Field)
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

        public TrMarcField this[int index]
        {
            get { return Fields[index]; }
            set { Fields[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Fields).GetEnumerator();
        }

        public TrMarcFields()
        {
            Fields = new List<TrMarcField>();
        }

    }
}
