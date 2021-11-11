// <copyright file="TrDocuments.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using TrClient.Helpers;

    public class TrDocuments : IEnumerable
    {
        private ObservableCollection<TrDocument> documents;

        // private List<TrDocument> Documents;
        public int Count { get => documents.Count; }

        public TrCollection ParentCollection;

        public void Add(TrDocument doc)
        {
            documents.Add(doc);
            doc.ParentContainer = this;
            doc.ParentCollection = ParentCollection;
        }

        public void Sort()
        {
            documents.Sort(i => i.Title);

            // Documents.Sort();
        }

        public void Clear()
        {
            documents.Clear();
        }

        public TrDocument this[int index]
        {
            get { return documents[index]; }
            set { documents[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)documents).GetEnumerator();
        }

        //public TrDocument GetDocumentFromTitle(string search)
        //{
        //    var doc = documents.Where(d => d.Title == search).FirstOrDefault();
        //    return doc;
        //}

        //public TrDocument GetDocumentFromID(string search)
        //{
        //    var doc = documents.Where(d => d.ID == search).FirstOrDefault();
        //    return doc;
        //}

        //public string GetIDFromName(string searchName)
        //{
        //    string temp = string.Empty;
        //    foreach (TrDocument doc in documents)
        //    {
        //        if (doc.Title == searchName)
        //        {
        //            temp = doc.ID;
        //            break;
        //        }
        //    }

        //    return temp;
        //}

        public TrDocuments()
        {
            // Documents = new List<TrDocument>();
            documents = new ObservableCollection<TrDocument>();
        }
    }
}
