// <copyright file="TrCurrent.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Settings
{
    using System.ComponentModel;
    using TrClient.Core;

    public class TrCurrent : INotifyPropertyChanged
    {
        private TrCollection collection;

        public TrCollection Collection
        {
            get
            {
                return collection;
            }

            set
            {
                if (collection != value)
                {
                    collection = value;
                    NotifyPropertyChanged("Collection");

                    // CollectionName = value.Name;
                    // CollectionStatusColor = value.StatusColor;
                }
            }
        }

        //private string _collectionName = "";
        //public string CollectionName
        //{
        //    get { return _collectionName; }
        //    set
        //    {
        //        if (_collectionName != value)
        //        {
        //            _collectionName = value;
        //            NotifyPropertyChanged("CollectionName");
        //        }
        //    }
        //}
        private TrDocument document;

        public TrDocument Document
        {
            get
            {
                return document;
            }

            set
            {
                if (document != value)
                {
                    document = value;
                    NotifyPropertyChanged("Document");

                    // DocumentTitle = value.Title;
                    // DocumentStatusColor = value.StatusColor;
                }
            }
        }

        //private string _documentTitle = "";
        //public string DocumentTitle
        //{
        //    get { return _documentTitle; }
        //    set
        //    {
        //        if (_documentTitle != value)
        //        {
        //            _documentTitle = value;
        //            NotifyPropertyChanged("DocumentTitle");
        //        }
        //    }
        //}
        private TrPage page;

        public TrPage Page
        {
            get
            {
                return page;
            }

            set
            {
                if (page != value)
                {
                    page = value;
                    NotifyPropertyChanged("Page");
                }
            }
        }

        private int pageNumber = 0;

        public int PageNumber
        {
            get
            {
                return pageNumber;
            }

            set
            {
                if (pageNumber != value)
                {
                    pageNumber = value;
                    NotifyPropertyChanged("PageNumber");
                }
            }
        }

        private string transcriptID = string.Empty;

        public string TranscriptID
        {
            get
            {
                return transcriptID;
            }

            set
            {
                if (transcriptID != value)
                {
                    transcriptID = value;
                    NotifyPropertyChanged("TranscriptID");
                }
            }
        }

        //private SolidColorBrush _collectionStatusColor = Brushes.Red;
        //public SolidColorBrush CollectionStatusColor
        //{
        //    get { return _collectionStatusColor; }
        //    set
        //    {
        //        if (_collectionStatusColor != value)
        //        {
        //            _collectionStatusColor = value;
        //            NotifyPropertyChanged("CollectionStatusColor");
        //        }
        //    }
        //}

        //private SolidColorBrush _documentStatusColor = Brushes.Red;
        //public SolidColorBrush DocumentStatusColor
        //{
        //    get { return _documentStatusColor; }
        //    set
        //    {
        //        if (_documentStatusColor != value)
        //        {
        //            _documentStatusColor = value;
        //            NotifyPropertyChanged("DocumentStatusColor");
        //        }
        //    }
        //}
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public TrCurrent()
        {
            // CollectionName = "";
            // DocumentTitle = "";
            PageNumber = 0;
            TranscriptID = string.Empty;
        }
    }
}
