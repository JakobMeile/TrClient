using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Core
{
    public class TrPages : IEnumerable
    {
        private List<TrPage> Pages;
        public int Count { get => Pages.Count; }

        public TrDocument ParentDocument;

        public void Add(TrPage Page)
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

        public TrPage this[int index]
        {
            get { return Pages[index]; }
            set { Pages[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Pages).GetEnumerator();
        }

        public TrPage GetPageFromPageNr(int Search)
        {
            var Page = Pages.Where(p => p.PageNr == Search).FirstOrDefault();
            return Page;
        }

        public TrPage GetPageFromID(string Search)
        {
            var Page = Pages.Where(p => p.ID == Search).FirstOrDefault();
            return Page;
        }
        
        public string GetIDFromPageNumber(int SearchNumber)
        {
            string Temp = "";
            foreach (TrPage Page in Pages)
            {
                if (Page.PageNr == SearchNumber)
                {
                    Temp = Page.ID;
                    break;
                }
            }
            return Temp;
        }

        public TrPages()
        {
            Pages = new List<TrPage>();
        }

    }
}
