using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    public class TrDocuments : IEnumerable
    {

        private ObservableCollection<TrDocument> Documents;
        // private List<TrDocument> Documents;
        public int Count { get => Documents.Count; }

        public TrCollection ParentCollection;

        public void Add(TrDocument Doc)
        {
            Documents.Add(Doc);
            Doc.ParentContainer = this;
            Doc.ParentCollection = this.ParentCollection;

        }

        public void Sort()
        {
            Documents.Sort(i => i.Title);
            // Documents.Sort();
        }

        public void Clear()
        {
            Documents.Clear();
        }

        public TrDocument this[int index]
        {
            get { return Documents[index]; }
            set { Documents[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Documents).GetEnumerator();
        }

        public TrDocument GetDocumentFromTitle(string Search)
        {
            var Doc = Documents.Where(d => d.Title == Search).FirstOrDefault();
            return Doc;
        }

        public TrDocument GetDocumentFromID(string Search)
        {
            var Doc = Documents.Where(d => d.ID == Search).FirstOrDefault();
            return Doc;
        }

        public string GetIDFromName(string SearchName)
        {
            string Temp = "";
            foreach (TrDocument Doc in Documents)
            {
                if (Doc.Title == SearchName)
                {
                    Temp = Doc.ID;
                    break;
                }
            }
            return Temp;
        }



        public TrDocuments()
        {
            // Documents = new List<TrDocument>();
            Documents = new ObservableCollection<TrDocument>();
        }

    }
}
