using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TrClient;
using System.Windows.Media;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Settings
{
    public class TrCurrent : INotifyPropertyChanged
    {
        private TrCollection _collection;
        public TrCollection Collection
        {
            get { return _collection; }
            set
            {
                if (_collection != value)
                {
                    _collection = value;
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




        private TrDocument _document;
        public TrDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;
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

        private TrPage _page;
        public TrPage Page
        {
            get { return _page; }
            set
            {
                if (_page != value)
                {
                    _page = value;
                    NotifyPropertyChanged("Page");
                    
                }
            }
        }


        private int _pageNumber = 0;
        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    NotifyPropertyChanged("PageNumber");
                }
            }
        }

        private string _transcriptID = "";
        public string TranscriptID
        {
            get { return _transcriptID; }
            set
            {
                if (_transcriptID != value)
                {
                    _transcriptID = value;
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
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        public TrCurrent()
        {
            // CollectionName = "";
            // DocumentTitle = "";
            PageNumber = 0;
            TranscriptID = "";
        }


    }
}
