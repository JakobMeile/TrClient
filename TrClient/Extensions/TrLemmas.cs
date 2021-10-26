// <copyright file="TrLemmas.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public class TrLemmas : IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        //private TrObservableCollection<TrLemma> Lemmas = new TrObservableCollection<TrLemma>();
        private List<TrLemma> lemmas = new List<TrLemma>();
        private List<string> contentOnly = new List<string>();

        public int Count { get => lemmas.Count(); }

        public int ContentOnlyCount { get => contentOnly.Count; }

        // Constructor
        public TrLemmas()
        {
        }

        public void Clear()
        {
            lemmas.Clear();
            contentOnly.Clear();
            NotifyPropertyChanged("Count");
        }

        public void AddWord(TrWord newWord)
        {
            //TrLemma NewLemma = new TrLemma(NewWord);

            // fra private add:
            // Lemmas.Add(NewLemma);
            // NotifyPropertyChanged("Count");
            string newContent = newWord.Raw;

            // her skal det testes, om lemmaet eksisterer:
            // hvis det gør, skal det eksisterende lemma have den nye reference tilføjet
            // hvis ikke, skal der tilføjes et nyt lemma med den nye reference

            // Debug.Write($"AddReference: lemma: {NewContent} page: {OnPage.Number}");
            if (contentOnly.Contains(newContent))
            {
                // Debug.WriteLine($" - Adding REFERENCE to existing lemma");
                TrLemma existingLemma = GetFromContent(newContent);
                existingLemma.Occurrences.Add(newWord);
            }
            else
            {
                // Debug.WriteLine($" - Adding non-existing LEMMA");
                contentOnly.Add(newContent);

                TrLemma newLemma = new TrLemma(newWord);
                newLemma.Occurrences.Add(newWord);
                lemmas.Add(newLemma);
                NotifyPropertyChanged("Count");
                OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newLemma));
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
            lemmas.Sort((x, y) => y.OccurrenceCount.CompareTo(x.OccurrenceCount));

            //OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
        }

        public void SortAlphabetically()
        {
            lemmas.Sort();

            //OnCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChange(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)lemmas).GetEnumerator();
        }

        public TrLemma this[int index]
        {
            get { return lemmas[index]; }
            set { lemmas[index] = value; }
        }

        public TrLemma GetFromContent(string lemmaContent)
        {
            var obj = lemmas.Where(o => o.Content == lemmaContent).FirstOrDefault();
            return obj;
        }
    }
}
