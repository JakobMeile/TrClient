// <copyright file="TrWords.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class TrWords : IEnumerable, INotifyPropertyChanged
    {
        protected List<TrWord> words = new List<TrWord>();

        public int Count { get => words.Count; }

        // Constructor
        public TrWords()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public void Add(TrWord newWord)
        {
            newWord.ID = Count + 1;
            newWord.ParentContainer = this;

            if (Count == 0)
            {
                newWord.Previous = null;
            }
            else
            {
                newWord.Previous = words[Count - 1];
                newWord.Previous.Next = newWord;
            }

            words.Add(newWord);
            NotifyPropertyChanged("Count");
        }

        public void Sort()
        {
            words.Sort();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)words).GetEnumerator();
        }

        public TrWord this[int index]
        {
            get { return words[index]; }
            set { words[index] = value; }
        }
    }
}
