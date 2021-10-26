// <copyright file="TrCollections.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Media;

    public class TrCollections : IEnumerable, INotifyPropertyChanged
    {
        private List<TrCollection> collections;

        public int Count { get => collections.Count; }

        private bool isLoaded = false;

        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }

            set
            {
                if (isLoaded != value)
                {
                    isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                    switch (isLoaded)
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

        private SolidColorBrush statusColor = Brushes.Red;

        public SolidColorBrush StatusColor
        {
            get
            {
                return statusColor;
            }

            set
            {
                if (statusColor != value)
                {
                    statusColor = value;
                    NotifyPropertyChanged("StatusColor");
                }
            }
        }

        private bool hasChanged = false;

        public bool HasChanged
        {
            get
            {
                return hasChanged;
            }

            set
            {
                hasChanged = value;
                NotifyPropertyChanged("HasChanged");
                if (hasChanged)
                {
                    StatusColor = Brushes.Orange;
                }
            }
        }

        private bool changesUploaded = false;

        public bool ChangesUploaded
        {
            get
            {
                return changesUploaded;
            }

            set
            {
                changesUploaded = value;
                NotifyPropertyChanged("ChangesUploaded");
                if (changesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }
            }
        }

        public void Add(TrCollection coll)
        {
            collections.Add(coll);
            coll.ParentContainer = this;
        }

        public void Sort()
        {
            collections.Sort();
        }

        public void Clear()
        {
            collections.Clear();
        }

        public TrCollection this[int index]
        {
            get { return collections[index]; }
            set { collections[index] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)collections).GetEnumerator();
        }

        public TrCollection GetCollectionFromName(string search)
        {
            var coll = collections.Where(c => c.Name == search).FirstOrDefault();
            return coll;
        }

        public TrCollection GetCollectionFromID(string search)
        {
            var coll = collections.Where(c => c.ID == search).FirstOrDefault();
            return coll;
        }

        public string GetIDFromName(string searchName)
        {
            string temp = string.Empty;
            foreach (TrCollection coll in collections)
            {
                if (coll.Name == searchName)
                {
                    temp = coll.ID;
                    break;
                }
            }

            return temp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public TrCollections()
        {
            collections = new List<TrCollection>();
        }
    }
}
