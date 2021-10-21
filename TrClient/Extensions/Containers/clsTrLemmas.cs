using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    public class clsTrLemmas : IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        //private clsTrObservableCollection<clsTrLemma> Lemmas = new clsTrObservableCollection<clsTrLemma>();
        
        private List<clsTrLemma> Lemmas = new List<clsTrLemma>();
        private List<string> ContentOnly = new List<string>();

        public int Count { get => Lemmas.Count(); }
        public int ContentOnlyCount { get => ContentOnly.Count; }

        // Constructor
        public clsTrLemmas()
        {

        }

        public void Clear()
        {
            Lemmas.Clear();
            ContentOnly.Clear();
            NotifyPropertyChanged("Count");
        }

        public void AddWord(clsTrWord NewWord)
        {
            //clsTrLemma NewLemma = new clsTrLemma(NewWord);

            // fra private add:
            // Lemmas.Add(NewLemma);
            // NotifyPropertyChanged("Count");

            string NewContent = NewWord.Raw;

            // her skal det testes, om lemmaet eksisterer:
            // hvis det gør, skal det eksisterende lemma have den nye reference tilføjet
            // hvis ikke, skal der tilføjes et nyt lemma med den nye reference

            // Debug.Write($"AddReference: lemma: {NewContent} page: {OnPage.Number}");

            if (ContentOnly.Contains(NewContent))
            {
                // Debug.WriteLine($" - Adding REFERENCE to existing lemma");
                clsTrLemma ExistingLemma = GetFromContent(NewContent);
                ExistingLemma.Occurrences.Add(NewWord);
            }
            else
            {
                // Debug.WriteLine($" - Adding non-existing LEMMA");
                ContentOnly.Add(NewContent);

                clsTrLemma NewLemma = new clsTrLemma(NewWord);
                NewLemma.Occurrences.Add(NewWord);
                Lemmas.Add(NewLemma);
                NotifyPropertyChanged("Count");
                OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, NewLemma));
            }

        }


        //public void MergeSimilar()
        //{
        //    Sort();

        //    // først ser vi på samme lemma, men med hhv. stort og lille begyndelsesbogstav - dvs. der indledes med en titel
        //    for (int i = 0; i < Lemmas.Count - 1; i++)
        //    {
        //        string CurrentLemma = Lemmas[i].Content;
        //        string NextLemma = Lemmas[i + 1].Content;
        //        if (CurrentLemma.Length == NextLemma.Length)
        //            if (CurrentLemma.Substring(1) == NextLemma.Substring(1))
        //            {
        //                Debug.WriteLine($"Merging lemma {CurrentLemma} with {NextLemma} >>> {CurrentLemma}");

        //                // den første er god nok, så så vi kopierer alle referencer fra den næste til den første
        //                foreach (clsPageReference Ref in Lemmas[i + 1].OnPages)
        //                    Lemmas[i].AddPageReference(Ref.Page, Ref.Highlighted);
        //                // og så skal de sorteres
        //                Lemmas[i].OnPages.Sort();
        //                // den næste er med stort, så den indstilles til sletning
        //                Lemmas[i + 1].MarkToDeletion = true;
        //            }
        //    }
        //    // nu kan vi slette dem, der er markeret
        //    for (int i = Lemmas.Count - 1; i >= 0; i--)
        //    {
        //        if (Lemmas[i].MarkToDeletion)
        //            Lemmas.RemoveAt(i);
        //    }


        //    // dernæst ser vi på, om et lemma følges af det samme, blot med -s på
        //    for (int i = 0; i < Lemmas.Count - 1; i++)
        //    {
        //        string CurrentLemma = Lemmas[i].Content;
        //        string NextLemma = Lemmas[i + 1].Content;
        //        if (CurrentLemma.Length + 1 == NextLemma.Length)
        //            if (CurrentLemma == NextLemma.Substring(0, CurrentLemma.Length) && NextLemma.Last() == 's')
        //            {
        //                Debug.WriteLine($"Merging lemma {CurrentLemma} with {NextLemma} >>> {CurrentLemma}");
        //                // den første er god nok, så så vi kopierer alle referencer fra den næste til den første
        //                foreach (clsPageReference Ref in Lemmas[i + 1].OnPages)
        //                    Lemmas[i].AddPageReference(Ref.Page, Ref.Highlighted);
        //                // og så skal de sorteres
        //                Lemmas[i].OnPages.Sort();
        //                // den næste er med s, så den indstilles til sletning
        //                Lemmas[i + 1].MarkToDeletion = true;
        //            }
        //    }
        //    // og igen kan vi slette dem, der er markeret
        //    for (int i = Lemmas.Count - 1; i >= 0; i--)
        //    {
        //        if (Lemmas[i].MarkToDeletion)
        //            Lemmas.RemoveAt(i);
        //    }

        //    // nu er contentonly syg - så vi sletter og fylder igen
        //    ContentOnly.Clear();
        //    foreach (clsLemma CurrentLemma in Lemmas)
        //        ContentOnly.Add(CurrentLemma.Content);

        //}

        public void SortAfterFrequency()
        {
            Lemmas.Sort((x, y) => y.OccurrenceCount.CompareTo(x.OccurrenceCount));
            //OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));

        }

        public void SortAlphabetically()
        {
            Lemmas.Sort();
            //OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChange(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Lemmas).GetEnumerator();
        }

        public clsTrLemma this[int index]
        {
            get { return Lemmas[index]; }
            set { Lemmas[index] = value; }
        }

        public clsTrLemma GetFromContent(string LemmaContent)
        {
            var obj = Lemmas.Where(o => o.Content == LemmaContent).FirstOrDefault();
            return obj;
        }


    }
}
