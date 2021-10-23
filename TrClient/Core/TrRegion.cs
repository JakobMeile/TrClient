using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using System.IO;
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;


namespace TrClient.Core
{
    public abstract class TrRegion : IComparable, INotifyPropertyChanged
    {
        public TrRegions ParentContainer;
        public TrTranscript ParentTranscript;

        public string Type { get; set; }
        public string ID { get; set; }
        public float Orientation { get; set; }
        public string CoordsString { get; set; }
        public bool MarkToDeletion = false;

        public string TagString { get; set; }
        public TrTags Tags = new TrTags();
        public TrTag_Structural StructuralTag;

        private int _readingOrder;
        public int ReadingOrder
        {
            get { return _readingOrder; }
            set
            {
                if (HasReadingOrderTag)
                {
                    //Debug.Print($"TrRegion: set RO: HAS RO-tag: Delete it! Count before: {Tags.Count}");
                    DeleteReadingOrderTag();
                    //Debug.Print($"Count after: {Tags.Count}");
                }

                _readingOrder = value;
                TrTag_ReadingOrder ROTag = new TrTag_ReadingOrder(_readingOrder);
                Tags.Add(ROTag);
                //Debug.Print($"TrRegion: Added new RO-tag: Count now: {Tags.Count}");
            }
        }

        // ABSTRACT PROPERTIES ----------------------------------------------------------

        protected bool _changesUploaded = false;
        public abstract bool ChangesUploaded { get; set; }

        // protected bool _hasLines;
        public abstract bool HasLines { get; }

        protected TrWords _words = new TrWords();
        public abstract TrWords Words { get; }
        public abstract int NumberOfLines { get; }

        // ABSTRACT METHODS

        public abstract List<string> GetStructuralTags();
        public abstract void Move(int Horizontally, int Vertically);
        public abstract bool DeleteShortBaselines(int Limit, TrLog Log);
        public abstract void SimplifyBoundingBoxes();
        public abstract void SimplifyBoundingBoxes(int MinimumHeight, int MaximumHeight);
        public abstract List<string> GetExpandedText(bool Refine, bool ConvertOtrema);
        public abstract void ExtendBaseLines(TrDialogTransferSettings Settings, TrLog Log);
        


        public abstract XElement ToXML();

        // -----------------------

        public void FixCoordinates()
        {
            int PageWidth = ParentTranscript.ParentPage.Width;
            int PageHeigth = ParentTranscript.ParentPage.Height;

            TrCoords Coords = new TrCoords(CoordsString);

            // ændrer punkter med negative koordinater eller større end siden
            foreach (TrCoord C in Coords)
            {
                if (C.X < 0)
                {
                    C.X = 0;
                    HasChanged = true;
                }

                if (C.X > PageWidth)
                {
                    C.X = PageWidth;
                    HasChanged = true;
                }

                if (C.Y < 0)
                {
                    C.Y = 0;
                    HasChanged = true;
                }

                if (C.Y > PageHeigth)
                {
                    C.Y = PageHeigth;
                    HasChanged = true;
                }
            }

            if (HasChanged)
                CoordsString = Coords.ToString();
        }


        private void DeleteReadingOrderTag()
        {
            bool FoundTag = false;

            if (HasReadingOrderTag)
            {
                FoundTag = true;
                foreach (TrTag T in Tags)
                {
                    if (T.GetType() == typeof(TrTag_ReadingOrder))
                        T.MarkToDeletion = true;
                }
            }

            if (FoundTag)
            {
                for (int i = Tags.Count - 1; i >= 0; i--)
                {
                    if (Tags[i].MarkToDeletion)
                        Tags.RemoveAt(i);
                }
                HasChanged = true;
            }
        }

        

        private int _number;
        public int Number
        {
            get
            {
                if (ParentContainer.IsZeroBased)
                    _number = ReadingOrder + 1;
                else
                    _number = ReadingOrder;
                return _number;
            }
        }
               
        private bool _hasChanged = false;
        public bool HasChanged
        {
            get { return _hasChanged; }
            set
            {
                _hasChanged = value;
                if (_hasChanged)
                    StatusColor = Brushes.Orange;
                NotifyPropertyChanged("HasChanged");
                ParentTranscript.HasChanged = value;
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

        private bool _hasStructuralTag;
        public bool HasStructuralTag
        {
            get
            {
                _hasStructuralTag = false;

                foreach (TrTag T in Tags)
                {
                    _hasStructuralTag = _hasStructuralTag || (T.GetType() == typeof(TrTag_Structural));
                }
                return _hasStructuralTag;

            }
        }

        private bool _hasReadingOrderTag;
        public bool HasReadingOrderTag
        {
            get
            {
                _hasReadingOrderTag = false;

                foreach (TrTag T in Tags)
                {
                    _hasReadingOrderTag = _hasReadingOrderTag || (T.GetType() == typeof(TrTag_ReadingOrder));
                }
                return _hasReadingOrderTag;

            }
        }
               
        private string _structuralTagValue;
        public string StructuralTagValue
        {
            get
            {
                if (HasStructuralTag)
                    _structuralTagValue = StructuralTag.SubType;
                else
                    _structuralTagValue = "";
                return _structuralTagValue;
            }

        }

        //public List<string> GetStructuralTags()
        //{
        //    List<string> TempList = new List<string>();

        //    foreach (TrTextLine TL in TextLines)
        //        if (TL.HasStructuralTag)
        //            TempList.Add(TL.StructuralTagValue);

        //    List<string> TagList = TempList.Distinct().ToList();
        //    TagList.Sort();
        //    return TagList;
        //}



        private int _leftBorder;
        public int LeftBorder
        {
            get
            {
                _leftBorder = TrLibrary.GetLeftMostXcoord(CoordsString);
                return _leftBorder;
            }
        }

        private int _rightBorder;
        public int RightBorder
        {
            get
            {
                _rightBorder = TrLibrary.GetRightMostXcoord(CoordsString);
                return _rightBorder;
            }
        }

        private int _topBorder;
        public int TopBorder
        {
            get
            {
                _topBorder = TrLibrary.GetTopYcoord(CoordsString);
                return _topBorder;
            }
        }


        private int _bottomBorder;
        public int BottomBorder
        {
            get
            {
                _bottomBorder = TrLibrary.GetBottomYcoord(CoordsString);
                return _bottomBorder;
            }
        }


        private int _hPos;
        public int Hpos
        {
            get
            {
                _hPos = LeftBorder;
                return _hPos;
            }
        }


        private int _vPos;
        public int Vpos
        {
            get
            {
                _vPos = TrLibrary.GetAverageYcoord(CoordsString);
                return _vPos;
            }
        }

        private int _horizontalOrder;
        public int HorizontalOrder
        {
            get
            {
                _horizontalOrder = Hpos * 10_000 + Vpos;
                return _horizontalOrder;
            }
        }

        private int _verticalOrder;
        public int VerticalOrder
        {
            get
            {
                _verticalOrder = Vpos * 10_000 + Hpos;
                return _verticalOrder;
            }
        }


        private XElement _regionRef;
        public XElement RegionRef
        {
            get
            {
                _regionRef = new XElement(TrLibrary.xmlns + "RegionRefIndexed",
                    new XAttribute("index", ReadingOrder),
                    new XAttribute("regionRef", ID));
                return _regionRef;

            }
        }

        // CONSTRUCTORS ---------------------------------------------------------------------------------------
        

        // constructor ved indlæsning af XDoc
        public TrRegion(string rType, string rID, string rTags, float rOrientation, string rCoords)
        {
            //Debug.Print("-----------------------------------------------------------------------------");
            //Debug.Print($"New TrRegion in the making! tags count = {Tags.Count}");

            Type = rType;
            ID = rID;
            TagString = rTags;
            ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordsString = rCoords;
            Orientation = rOrientation;
            //TextLines.ParentRegion = this;
            //Tags.ParentRegion = this;

            Tags.LoadFromCustomAttribute(rTags);
            //Debug.Print($"New TrRegion! RO = {ReadingOrder}, tags count = {Tags.Count}");


            if (Tags.Count > 0)
            {
                foreach (TrTag Tag in Tags)
                {
                    if (Tag.GetType() == typeof(TrTag_Structural))
                    {
                        StructuralTag = (TrTag_Structural)Tag;
                    }
                }
            }
        }

        // constructor ved skabelse af ny region
        public TrRegion(int rOrder, float rOrientation, string rCoords)
        {
            Type = "";
            ID = "region_" + TrLibrary.GetNewTimeStamp().ToString();
            ReadingOrder = rOrder;
            CoordsString = rCoords;
            Orientation = rOrientation;

            //TextLines.ParentRegion = this;
            //Tags.ParentRegion = this;

            // Debug.WriteLine($"Region (empty) constructed! ID = {ID}, RO = {ReadingOrder}, Coords = {CoordsString}");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        public int CompareTo(object obj)
        {
            var region = obj as TrRegion_Text;
            return ReadingOrder.CompareTo(region.ReadingOrder);
        }



    }
}
