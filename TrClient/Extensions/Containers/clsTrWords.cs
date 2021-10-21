using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrClient;


namespace TrClient
{
    public class clsTrWords : IEnumerable, INotifyPropertyChanged
    {
        protected List<clsTrWord> Words = new List<clsTrWord>();

        public int Count { get => Words.Count; }
               
        // Constructor
        public clsTrWords()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        public void Add(clsTrWord NewWord)
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

        public clsTrWord this[int index]
        {
            get { return Words[index]; }
            set { Words[index] = value; }
        }



    }
}
