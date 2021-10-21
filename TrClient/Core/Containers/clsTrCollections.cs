using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrClient;
using System.ComponentModel;
using System.Windows.Media;


namespace TrClient
{
    public class clsTrCollections : IEnumerable, INotifyPropertyChanged
    {
        private List<clsTrCollection> Collections;
        public int Count { get => Collections.Count; }

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                    switch (_isLoaded)
                    {
                        case true:
                            StatusColor = Brushes.LimeGreen;
                            break;
                        case false:
                            StatusColor = Brushes.Red;
                            break;
                    }
                }
            }
        }

        private SolidColorBrush _statusColor = Brushes.Red;
        public SolidColorBrush StatusColor
        {
            get { return _statusColor; }
            set
            {
                if (_statusColor != value)
                {
                    _statusColor = value;
                    NotifyPropertyChanged("StatusColor");
                }
            }
        }

        private bool _hasChanged = false;
        public bool HasChanged
        {
            get { return _hasChanged; }
            set
            {
                _hasChanged = value;
                NotifyPropertyChanged("HasChanged");
                if (_hasChanged)
                    StatusColor = Brushes.Orange;
            }
        }

        private bool _changesUploaded = false;
        public bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                NotifyPropertyChanged("ChangesUploaded");
                if (_changesUploaded)
                    StatusColor = Brushes.DarkViolet;
            }
        }




        public void Add(clsTrCollection Coll)
        {
            Collections.Add(Coll);
            Coll.ParentContainer = this;
        }

        public void Sort()
        {
            Collections.Sort();
        }

        public void Clear()
        {
            Collections.Clear();
        }

        public clsTrCollection this[int index]
        {
            get { return Collections[index]; }
            set { Collections[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Collections).GetEnumerator();
        }

        public clsTrCollection GetCollectionFromName(string Search)
        {
            var Coll = Collections.Where(c => c.Name == Search).FirstOrDefault();
            return Coll;
        }

        public clsTrCollection GetCollectionFromID(string Search)
        {
            var Coll = Collections.Where(c => c.ID == Search).FirstOrDefault();
            return Coll;
        }

        public string GetIDFromName(string SearchName)
        {
            string Temp = "";
            foreach (clsTrCollection Coll in Collections)
            {
                if (Coll.Name == SearchName)
                {
                    Temp = Coll.ID;
                    break;
                }
            }
            return Temp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }



        public clsTrCollections()
        {
            Collections = new List<clsTrCollection>();
        }

    }
}
