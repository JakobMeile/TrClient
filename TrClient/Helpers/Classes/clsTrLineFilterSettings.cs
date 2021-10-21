using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using TrClient;
using System.Windows.Media;

namespace TrClient
{
    public class clsTrLineFilterSettings : INotifyPropertyChanged
    {



        private bool _filterByPageNumber;
        public bool FilterByPageNumber
        {
            get { return _filterByPageNumber; }
            set
            {
                _filterByPageNumber = value;
                NotifyPropertyChanged("FilterByPageNumber");
            }
        }

        private bool _filterByRegEx;
        public bool FilterByRegEx
        {
            get { return _filterByRegEx; }
            set
            {
                _filterByRegEx = value;
                NotifyPropertyChanged("FilterByRegEx");
            }
        }

        private bool _filterByStructuralTag;
        public bool FilterByStructuralTag
        {
            get { return _filterByStructuralTag; }
            set
            {
                _filterByStructuralTag = value;
                NotifyPropertyChanged("FilterByStructuralTag");
            }
        }

        private bool _filterByTextSizeFactor;
        public bool FilterByTextSizeFactor
        {
            get { return _filterByTextSizeFactor; }
            set
            {
                _filterByTextSizeFactor = value;
                NotifyPropertyChanged("FilterByTextSizeFactor");
            }
        }

        private bool _filterByTextLength;
        public bool FilterByTextLength
        {
            get { return _filterByTextLength; }
            set
            {
                _filterByTextLength = value;
                NotifyPropertyChanged("FilterByTextLength");
            }
        }


        private bool _filterByPosition;
        public bool FilterByPosition
        {
            get { return _filterByPosition;  }
            set
            {
                _filterByPosition = value;
                NotifyPropertyChanged("FilterByPosition");
            }
        }





        private int _startPage;
        public int StartPage
        {
            get { return _startPage; }
            set
            {
                _startPage = value;
                NotifyPropertyChanged("StartPage");
            }
        }

        private int _endPage;
        public int EndPage
        {
            get { return _endPage; }
            set
            {
                _endPage = value;
                NotifyPropertyChanged("EndPage");
            }
        }

        private string _rexExPattern;
        public string RegExPattern
        {
            get { return _rexExPattern; }
            set
            {
                _rexExPattern = value;
                NotifyPropertyChanged("RegExPattern");
            }
        }

        private string _structuralTag;
        public string StructuralTag
        {
            get { return _structuralTag; }
            set
            {
                _structuralTag = value;
                NotifyPropertyChanged("StructuralTag");
            }
        }

        private int _lowerLimitTextSizeFactor = 0;
        public int LowerLimitTextSizeFactor
        {
            get { return _lowerLimitTextSizeFactor; }
            set
            {
                _lowerLimitTextSizeFactor = value;
                NotifyPropertyChanged("LowerLimitTextSizeFactor");
            }
        }

        private int _upperLimitTextSizeFactor = 0;
        public int UpperLimitTextSizeFactor
        {
            get { return _upperLimitTextSizeFactor; }
            set
            {
                _upperLimitTextSizeFactor = value;
                NotifyPropertyChanged("UpperLimitTextSizeFactor");
            }
        }

        private int _lowerLimitTextLength = 0;
        public int LowerLimitTextLength
        {
            get { return _lowerLimitTextLength; }
            set
            {
                _lowerLimitTextLength = value;
                NotifyPropertyChanged("LowerLimitTextLength");
            }
        }

        private int _upperLimitTextLength = 0;
        public int UpperLimitTextLength
        {
            get { return _upperLimitTextLength; }
            set
            {
                _upperLimitTextLength = value;
                NotifyPropertyChanged("UpperLimitTextLength");
            }
        }



        private double _topBorder = 0;
        public double TopBorder
        {
            get { return _topBorder; }
            set
            {
                _topBorder = value;
                NotifyPropertyChanged("TopBorder");
            }
        }

        private double _bottomBorder = 100;
        public double BottomBorder
        {
            get { return _bottomBorder; }
            set
            {
                _bottomBorder = value;
                NotifyPropertyChanged("BottomBorder");
            }
        }

        private double _leftBorder = 0;
        public double LeftBorder
        {
            get { return _leftBorder; }
            set
            {
                _leftBorder = value;
                NotifyPropertyChanged("LeftBorder");
            }
        }


        private double _rightBorder = 100;
        public double RightBorder
        {
            get { return _rightBorder; }
            set
            {
                _rightBorder = value;
                NotifyPropertyChanged("RightBorder");
            }
        }

        private double _windowWidth;
        public double WindowWidth
        {
            get
            {
                _windowWidth = RightBorder - LeftBorder;
                return _windowWidth;
            }
            set
            {
                _windowWidth = value;
                NotifyPropertyChanged("WindowWidth");
            }
        }

        private double _windowHeigth;
        public double WindowHeigth
        {
            get
            {
                _windowHeigth = BottomBorder - TopBorder;
                return _windowHeigth;
            }
            set
            {
                _windowHeigth = value;
                NotifyPropertyChanged("WindowHeigth");
            }
        }

        private bool _inside = true;
        public bool Inside
        {
            get { return _inside; }
            set
            {
                _inside = value;
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

        private SolidColorBrush _frontColor = Brushes.Red;
        public SolidColorBrush FrontColor
        {
            get { return _frontColor; }
            set
            {
                if (_frontColor != value)
                {
                    _frontColor = value;
                    NotifyPropertyChanged("FrontColor");
                }
            }
        }

        private SolidColorBrush _backColor = Brushes.Red;
        public SolidColorBrush BackColor
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    NotifyPropertyChanged("BackColor");
                }
            }
        }


        private bool _includeEnding;
        public bool IncludeEnding
        {
            get { return _includeEnding; }
            set
            {
                _includeEnding = value;
                NotifyPropertyChanged("IncludeEnding");
            }
        }

        private bool _excludeOddSizedPages;
        public bool ExcludeOddSizedPages
        {
            get { return _excludeOddSizedPages;  }
            set
            {
                _excludeOddSizedPages = value;
                NotifyPropertyChanged("ExcludeOddSizedPages");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
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
            RegExPattern = "";
            StructuralTag = "";

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

        public clsTrLineFilterSettings()
        {
            Reset();

        }


    }
}
