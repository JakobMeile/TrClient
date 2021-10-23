using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Helpers
{
    public class TrDialogTransferSettings : INotifyPropertyChanged
    {
        private bool _deleteShortBaseLines;
        public bool DeleteShortBaseLines
        {
            get { return _deleteShortBaseLines; }
            set
            {
                _deleteShortBaseLines = value;
                NotifyPropertyChanged("DeleteShortBaseLines");
            }
        }

        private int _shortLimit;
        public int ShortLimit
        {
            get { return _shortLimit; }
            set
            {
                _shortLimit = value;
                NotifyPropertyChanged("ShortLimit");
            }
        }

        private bool _extendLeft;
        public bool ExtendLeft
        {
            get { return _extendLeft; }
            set
            {
                _extendLeft = value;
                NotifyPropertyChanged("ExtendLeft");
            }
        }

        private int _leftAmount;
        public int LeftAmount
        {
            get { return _leftAmount; }
            set
            {
                _leftAmount = value;
                NotifyPropertyChanged("LeftAmount");
            }
        }

        private bool _extendRight;
        public bool ExtendRight
        {
            get { return _extendRight; }
            set
            {
                _extendRight = value;
                NotifyPropertyChanged("ExtendRight");
            }
        }

        private int _rightAmount;
        public int RightAmount
        {
            get { return _rightAmount; }
            set
            {
                _rightAmount = value;
                NotifyPropertyChanged("RightAmount");
            }
        }

        private bool _allPages;
        public bool AllPages
        {
            get { return _allPages; }
            set
            {
                _allPages= value;
                NotifyPropertyChanged("AllPages");
            }
        }

        private int _pagesFrom;
        public int PagesFrom
        {
            get { return _pagesFrom; }
            set
            {
                _pagesFrom = value;
                NotifyPropertyChanged("PagesFrom");
            }
        }

        private int _pagesTo;
        public int PagesTo
        {
            get { return _pagesTo; }
            set
            {
                _pagesTo = value;
                NotifyPropertyChanged("PagesTo");
            }
        }

        private bool _allRegions;
        public bool AllRegions
        {
            get { return _allRegions; }
            set
            {
                _allRegions = value;
                NotifyPropertyChanged("AllRegions");
            }
        }

        private int _RegionsFrom;
        public int RegionsFrom
        {
            get { return _RegionsFrom; }
            set
            {
                _RegionsFrom = value;
                NotifyPropertyChanged("RegionsFrom");
            }
        }

        private int _RegionsTo;
        public int RegionsTo
        {
            get { return _RegionsTo; }
            set
            {
                _RegionsTo = value;
                NotifyPropertyChanged("RegionsTo");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        public TrDialogTransferSettings()
        {
            DeleteShortBaseLines = true;
            ShortLimit = 60;
            ExtendLeft = false;
            LeftAmount = 20;
            ExtendRight = true;
            RightAmount = 40;
            AllPages = true;
            //PagesFrom = 1;
            //PagesTo = 1;
            AllRegions = true;
            //RegionsFrom = 1;
            //RegionsTo = 1;
        }

        public TrDialogTransferSettings(int MaxPages) : this()
        {
            PagesTo = MaxPages;
        }

        public TrDialogTransferSettings(int MaxPages, int MaxRegions) : this(MaxPages)
        {
            RegionsTo = MaxRegions;
        }

    }
}
