// <copyright file="TrMarcFields.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using System.Collections;
    using System.Collections.Generic;

    public class TrMarcFields : IEnumerable
    {
        private List<TrMarcField> fields;

        public int Count { get => fields.Count; }

        public TrMarcRecord ParentRecord;

        public void Add(TrMarcField field)
        {
            fields.Add(field);
            field.ParentContainer = this;
            field.ParentRecord = ParentRecord;
        }

        public void Clear()
        {
            fields.Clear();
        }

        public void Sort()
        {
            fields.Sort();
        }

        public void RemoveAt(int i)
        {
            fields.RemoveAt(i);
        }

        public TrMarcField this[int index]
        {
            get { return fields[index]; }
            set { fields[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)fields).GetEnumerator();
        }

        public TrMarcFields()
        {
            fields = new List<TrMarcField>();
        }
    }
}
