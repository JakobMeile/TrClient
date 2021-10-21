using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient;

namespace TrClient
{
    public class clsTrPages : IEnumerable
    {
        private List<clsTrPage> Pages;
        public int Count { get => Pages.Count; }

        public clsTrDocument ParentDocument;

        public void Add(clsTrPage Page)
        {
            Pages.Add(Page);
            Page.ParentContainer = this;
            Page.ParentDocument = this.ParentDocument;
        }

        public void Sort()
        {
            Pages.Sort();
        }

        public void Clear()
        {
            Pages.Clear();
        }

        public clsTrPage this[int index]
        {
            get { return Pages[index]; }
            set { Pages[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Pages).GetEnumerator();
        }

        public clsTrPage GetPageFromPageNr(int Search)
        {
            var Page = Pages.Where(p => p.PageNr == Search).FirstOrDefault();
            return Page;
        }

        public clsTrPage GetPageFromID(string Search)
        {
            var Page = Pages.Where(p => p.ID == Search).FirstOrDefault();
            return Page;
        }
        
        public string GetIDFromPageNumber(int SearchNumber)
        {
            string Temp = "";
            foreach (clsTrPage Page in Pages)
            {
                if (Page.PageNr == SearchNumber)
                {
                    Temp = Page.ID;
                    break;
                }
            }
            return Temp;
        }

        public clsTrPages()
        {
            Pages = new List<clsTrPage>();
        }

    }
}
