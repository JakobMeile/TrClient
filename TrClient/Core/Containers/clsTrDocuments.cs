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

namespace TrClient
{
    public class clsTrDocuments : IEnumerable
    {

        private ObservableCollection<clsTrDocument> Documents;
        // private List<clsTrDocument> Documents;
        public int Count { get => Documents.Count; }

        public clsTrCollection ParentCollection;

        public void Add(clsTrDocument Doc)
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

        public clsTrDocument this[int index]
        {
            get { return Documents[index]; }
            set { Documents[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Documents).GetEnumerator();
        }

        public clsTrDocument GetDocumentFromTitle(string Search)
        {
            var Doc = Documents.Where(d => d.Title == Search).FirstOrDefault();
            return Doc;
        }

        public clsTrDocument GetDocumentFromID(string Search)
        {
            var Doc = Documents.Where(d => d.ID == Search).FirstOrDefault();
            return Doc;
        }

        public string GetIDFromName(string SearchName)
        {
            string Temp = "";
            foreach (clsTrDocument Doc in Documents)
            {
                if (Doc.Title == SearchName)
                {
                    Temp = Doc.ID;
                    break;
                }
            }
            return Temp;
        }



        public clsTrDocuments()
        {
            // Documents = new List<clsTrDocument>();
            Documents = new ObservableCollection<clsTrDocument>();
        }

    }
}
