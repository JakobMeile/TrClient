// <copyright file="TrPages.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class TrPages : IEnumerable
    {
        private List<TrPage> pages;

        public int Count { get => pages.Count; }

        public TrDocument ParentDocument;

        public void Add(TrPage page)
        {
            pages.Add(page);
            page.ParentContainer = this;
            page.ParentDocument = ParentDocument;
        }

        public void Sort()
        {
            pages.Sort();
        }

        public void Clear()
        {
            pages.Clear();
        }

        public TrPage this[int index]
        {
            get { return pages[index]; }
            set { pages[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)pages).GetEnumerator();
        }

        public TrPage GetPageFromPageNr(int search)
        {
            var page = pages.Where(p => p.PageNr == search).FirstOrDefault();
            return page;
        }

        public TrPage GetPageFromID(string search)
        {
            var page = pages.Where(p => p.ID == search).FirstOrDefault();
            return page;
        }

        public string GetIDFromPageNumber(int searchNumber)
        {
            string temp = string.Empty;
            foreach (TrPage page in pages)
            {
                if (page.PageNr == searchNumber)
                {
                    temp = page.ID;
                    break;
                }
            }

            return temp;
        }

        public TrPages()
        {
            pages = new List<TrPage>();
        }
    }
}
