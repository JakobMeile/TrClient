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
    public class TrParagraphs : IEnumerable
    {
        private List<TrParagraph> Paragraphs;
        public int Count { get => Paragraphs.Count; }

        public TrRegion_Text ParentRegion;

        public void Add(TrParagraph Paragraph)
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

        public TrParagraph this[int index]
        {
            get { return Paragraphs[index]; }
            set { Paragraphs[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Paragraphs).GetEnumerator();
        }

        public TrParagraphs()
        {
            Paragraphs = new List<TrParagraph>();
        }

        public List<string> GetNames()
        {
            List<string> temp = new List<string>();

            if (Paragraphs != null)
            {
                foreach (TrParagraph P in Paragraphs)
                    temp.Add(P.Name);
            }
            return temp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Paragraphs != null)
            {
                foreach (TrParagraph P in Paragraphs)
                {
                    sb.Append(P.ToString());
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

    }
}
