using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public class TrWords : IEnumerable, INotifyPropertyChanged
    {
        protected List<TrWord> Words = new List<TrWord>();

        public int Count { get => Words.Count; }
               
        // Constructor
        public TrWords()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        public void Add(TrWord NewWord)
        {
            NewWord.ID = Count + 1;
            NewWord.ParentContainer = this;

            if (Count == 0)
                NewWord.Previous = null;
            else
            {
                NewWord.Previous = Words[Count - 1];
                NewWord.Previous.Next = NewWord;
            }

            Words.Add(NewWord);
            NotifyPropertyChanged("Count");

        }

        public void Sort()
        {
            Words.Sort();
        }
        

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Words).GetEnumerator();
        }

        public TrWord this[int index]
        {
            get { return Words[index]; }
            set { Words[index] = value; }
        }



    }
}
