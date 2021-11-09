// <copyright file="TrParagraphs.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using TranskribusClient.Core;

    public class TrParagraphs : IEnumerable
    {
        private List<TrParagraph> paragraphs;

        public int Count { get => paragraphs.Count; }

        public TrTextRegion ParentRegion;

        public void Add(TrParagraph paragraph)
        {
            paragraphs.Add(paragraph);
            paragraph.ParentContainer = this;
            paragraph.ParentRegion = ParentRegion;
        }

        public void Clear()
        {
            paragraphs.Clear();
        }

        public void Sort()
        {
            paragraphs.Sort();
        }

        public void RemoveAt(int i)
        {
            paragraphs.RemoveAt(i);
        }

        public TrParagraph this[int index]
        {
            get { return paragraphs[index]; }
            set { paragraphs[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)paragraphs).GetEnumerator();
        }

        public TrParagraphs()
        {
            paragraphs = new List<TrParagraph>();
        }

        public List<string> GetNames()
        {
            List<string> temp = new List<string>();

            if (paragraphs != null)
            {
                foreach (TrParagraph p in paragraphs)
                {
                    temp.Add(p.Name);
                }
            }

            return temp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (paragraphs != null)
            {
                foreach (TrParagraph p in paragraphs)
                {
                    sb.Append(p.ToString());
                    sb.Append(Environment.NewLine);
                }
            }

            return sb.ToString();
        }
    }
}
