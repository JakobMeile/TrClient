// <copyright file="TrDialogTransferSettings.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Helpers
{
    using System.ComponentModel;

    public class TrDialogTransferSettings : INotifyPropertyChanged
    {
        private bool deleteShortBaseLines;

        public bool DeleteShortBaseLines
        {
            get
            {
                return deleteShortBaseLines;
            }

            set
            {
                deleteShortBaseLines = value;
                NotifyPropertyChanged("DeleteShortBaseLines");
            }
        }

        private int shortLimit;

        public int ShortLimit
        {
            get
            {
                return shortLimit;
            }

            set
            {
                shortLimit = value;
                NotifyPropertyChanged("ShortLimit");
            }
        }

        private bool extendLeft;

        public bool ExtendLeft
        {
            get
            {
                return extendLeft;
            }

            set
            {
                extendLeft = value;
                NotifyPropertyChanged("ExtendLeft");
            }
        }

        private int leftAmount;

        public int LeftAmount
        {
            get
            {
                return leftAmount;
            }

            set
            {
                leftAmount = value;
                NotifyPropertyChanged("LeftAmount");
            }
        }

        private bool extendRight;

        public bool ExtendRight
        {
            get
            {
                return extendRight;
            }

            set
            {
                extendRight = value;
                NotifyPropertyChanged("ExtendRight");
            }
        }

        private int rightAmount;

        public int RightAmount
        {
            get
            {
                return rightAmount;
            }

            set
            {
                rightAmount = value;
                NotifyPropertyChanged("RightAmount");
            }
        }

        private bool allPages;

        public bool AllPages
        {
            get
            {
                return allPages;
            }

            set
            {
                allPages = value;
                NotifyPropertyChanged("AllPages");
            }
        }

        private int pagesFrom;

        public int PagesFrom
        {
            get
            {
                return pagesFrom;
            }

            set
            {
                pagesFrom = value;
                NotifyPropertyChanged("PagesFrom");
            }
        }

        private int pagesTo;

        public int PagesTo
        {
            get
            {
                return pagesTo;
            }

            set
            {
                pagesTo = value;
                NotifyPropertyChanged("PagesTo");
            }
        }

        private bool allRegions;

        public bool AllRegions
        {
            get
            {
                return allRegions;
            }

            set
            {
                allRegions = value;
                NotifyPropertyChanged("AllRegions");
            }
        }

        private int regionsFrom;

        public int RegionsFrom
        {
            get
            {
                return regionsFrom;
            }

            set
            {
                regionsFrom = value;
                NotifyPropertyChanged("RegionsFrom");
            }
        }

        private int regionsTo;

        public int RegionsTo
        {
            get
            {
                return regionsTo;
            }

            set
            {
                regionsTo = value;
                NotifyPropertyChanged("RegionsTo");
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

        public TrDialogTransferSettings(int maxPages)
            : this()
        {
            PagesTo = maxPages;
        }

        public TrDialogTransferSettings(int maxPages, int maxRegions)
            : this(maxPages)
        {
            RegionsTo = maxRegions;
        }
    }
}
