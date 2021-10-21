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
    public class clsTrParagraphs : IEnumerable
    {
        private List<clsTrParagraph> Paragraphs;
        public int Count { get => Paragraphs.Count; }

        public clsTrTextRegion ParentRegion;

        public void Add(clsTrParagraph Paragraph)
        {
            Paragraphs.Add(Paragraph);
            Paragraph.ParentContainer = this;
            Paragraph.ParentRegion = this.ParentRegion;
        }

        public void Clear()
        {
            Paragraphs.Clear();
        }

        public void Sort()
        {
            Paragraphs.Sort();
        }

        public void RemoveAt(int i)
        {
            Paragraphs.RemoveAt(i);
        }

        public clsTrParagraph this[int index]
        {
            get { return Paragraphs[index]; }
            set { Paragraphs[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Paragraphs).GetEnumerator();
        }

        public clsTrParagraphs()
        {
            Paragraphs = new List<clsTrParagraph>();
        }

        public List<string> GetNames()
        {
            List<string> temp = new List<string>();

            if (Paragraphs != null)
            {
                foreach (clsTrParagraph P in Paragraphs)
                    temp.Add(P.Name);
            }
            return temp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Paragraphs != null)
            {
                foreach (clsTrParagraph P in Paragraphs)
                {
                    sb.Append(P.ToString());
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

    }
}
