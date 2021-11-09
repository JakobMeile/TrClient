// <copyright file="TrLineFilterSettings.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Helpers
{
    using System.ComponentModel;
    using System.Windows.Media;

    public class TrLineFilterSettings : INotifyPropertyChanged
    {
        private bool filterByPageNumber;

        public bool FilterByPageNumber
        {
            get
            {
                return filterByPageNumber;
            }

            set
            {
                filterByPageNumber = value;
                NotifyPropertyChanged("FilterByPageNumber");
            }
        }

        private bool filterByRegEx;

        public bool FilterByRegEx
        {
            get
            {
                return filterByRegEx;
            }

            set
            {
                filterByRegEx = value;
                NotifyPropertyChanged("FilterByRegEx");
            }
        }

        private bool filterByStructuralTag;

        public bool FilterByStructuralTag
        {
            get
            {
                return filterByStructuralTag;
            }

            set
            {
                filterByStructuralTag = value;
                NotifyPropertyChanged("FilterByStructuralTag");
            }
        }

        private bool filterByTextSizeFactor;

        public bool FilterByTextSizeFactor
        {
            get
            {
                return filterByTextSizeFactor;
            }

            set
            {
                filterByTextSizeFactor = value;
                NotifyPropertyChanged("FilterByTextSizeFactor");
            }
        }

        private bool filterByTextLength;

        public bool FilterByTextLength
        {
            get
            {
                return filterByTextLength;
            }

            set
            {
                filterByTextLength = value;
                NotifyPropertyChanged("FilterByTextLength");
            }
        }

        private bool filterByPosition;

        public bool FilterByPosition
        {
            get
            {
                return filterByPosition;
            }

            set
            {
                filterByPosition = value;
                NotifyPropertyChanged("FilterByPosition");
            }
        }

        private int startPage;

        public int StartPage
        {
            get
            {
                return startPage;
            }

            set
            {
                startPage = value;
                NotifyPropertyChanged("StartPage");
            }
        }

        private int endPage;

        public int EndPage
        {
            get
            {
                return endPage;
            }

            set
            {
                endPage = value;
                NotifyPropertyChanged("EndPage");
            }
        }

        private string rexExPattern;

        public string RegExPattern
        {
            get
            {
                return rexExPattern;
            }

            set
            {
                rexExPattern = value;
                NotifyPropertyChanged("RegExPattern");
            }
        }

        private string structuralTag;

        public string StructuralTag
        {
            get
            {
                return structuralTag;
            }

            set
            {
                structuralTag = value;
                NotifyPropertyChanged("StructuralTag");
            }
        }

        private int lowerLimitTextSizeFactor = 0;

        public int LowerLimitTextSizeFactor
        {
            get
            {
                return lowerLimitTextSizeFactor;
            }

            set
            {
                lowerLimitTextSizeFactor = value;
                NotifyPropertyChanged("LowerLimitTextSizeFactor");
            }
        }

        private int upperLimitTextSizeFactor = 0;

        public int UpperLimitTextSizeFactor
        {
            get
            {
                return upperLimitTextSizeFactor;
            }

            set
            {
                upperLimitTextSizeFactor = value;
                NotifyPropertyChanged("UpperLimitTextSizeFactor");
            }
        }

        private int lowerLimitTextLength = 0;

        public int LowerLimitTextLength
        {
            get
            {
                return lowerLimitTextLength;
            }

            set
            {
                lowerLimitTextLength = value;
                NotifyPropertyChanged("LowerLimitTextLength");
            }
        }

        private int upperLimitTextLength = 0;

        public int UpperLimitTextLength
        {
            get
            {
                return upperLimitTextLength;
            }

            set
            {
                upperLimitTextLength = value;
                NotifyPropertyChanged("UpperLimitTextLength");
            }
        }

        private double topBorder = 0;

        public double TopBorder
        {
            get
            {
                return topBorder;
            }

            set
            {
                topBorder = value;
                NotifyPropertyChanged("TopBorder");
            }
        }

        private double bottomBorder = 100;

        public double BottomBorder
        {
            get
            {
                return bottomBorder;
            }

            set
            {
                bottomBorder = value;
                NotifyPropertyChanged("BottomBorder");
            }
        }

        private double leftBorder = 0;

        public double LeftBorder
        {
            get
            {
                return leftBorder;
            }

            set
            {
                leftBorder = value;
                NotifyPropertyChanged("LeftBorder");
            }
        }

        private double rightBorder = 100;

        public double RightBorder
        {
            get
            {
                return rightBorder;
            }

            set
            {
                rightBorder = value;
                NotifyPropertyChanged("RightBorder");
            }
        }

        private double windowWidth;

        public double WindowWidth
        {
            get
            {
                windowWidth = RightBorder - LeftBorder;
                return windowWidth;
            }

            set
            {
                windowWidth = value;
                NotifyPropertyChanged("WindowWidth");
            }
        }

        private double windowHeigth;

        public double WindowHeigth
        {
            get
            {
                windowHeigth = BottomBorder - TopBorder;
                return windowHeigth;
            }

            set
            {
                windowHeigth = value;
                NotifyPropertyChanged("WindowHeigth");
            }
        }

        private bool inside = true;

        public bool Inside
        {
            get
            {
                return inside;
            }

            set
            {
                inside = value;
                NotifyPropertyChanged("Inside");
                if (Inside)
                {
                    FrontColor = Brushes.LightCyan;
                    BackColor = Brushes.LightBlue;
                }
                else
                {
                    FrontColor = Brushes.LightBlue;
                    BackColor = Brushes.LightCyan;
                }
            }
        }

        private SolidColorBrush frontColor = Brushes.Red;

        public SolidColorBrush FrontColor
        {
            get
            {
                return frontColor;
            }

            set
            {
                if (frontColor != value)
                {
                    frontColor = value;
                    NotifyPropertyChanged("FrontColor");
                }
            }
        }

        private SolidColorBrush backColor = Brushes.Red;

        public SolidColorBrush BackColor
        {
            get
            {
                return backColor;
            }

            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    NotifyPropertyChanged("BackColor");
                }
            }
        }

        private bool includeEnding;

        public bool IncludeEnding
        {
            get
            {
                return includeEnding;
            }

            set
            {
                includeEnding = value;
                NotifyPropertyChanged("IncludeEnding");
            }
        }

        private bool excludeOddSizedPages;

        public bool ExcludeOddSizedPages
        {
            get
            {
                return excludeOddSizedPages;
            }

            set
            {
                excludeOddSizedPages = value;
                NotifyPropertyChanged("ExcludeOddSizedPages");
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

        public void Reset()
        {
            FilterByPageNumber = false;
            FilterByRegEx = false;
            FilterByStructuralTag = false;
            FilterByTextSizeFactor = false;
            FilterByTextLength = false;
            FilterByPosition = false;

            StartPage = 0;
            EndPage = 0;
            RegExPattern = string.Empty;
            StructuralTag = string.Empty;

            LowerLimitTextSizeFactor = 0;
            UpperLimitTextSizeFactor = 0;

            LowerLimitTextLength = 0;
            UpperLimitTextLength = 0;

            TopBorder = 0;
            BottomBorder = 100;
            LeftBorder = 0;
            RightBorder = 100;
            Inside = true;

            IncludeEnding = false;
            ExcludeOddSizedPages = false;
        }

        // Constructor
        public TrLineFilterSettings()
        {
            Reset();
        }
    }
}
