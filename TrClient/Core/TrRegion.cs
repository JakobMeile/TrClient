// <copyright file="TrRegion.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Xml.Linq;
    using TrClient.Core.Tags;
    using TrClient.Extensions;
    using TrClient.Helpers;
    using TrClient.Libraries;

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
        public TrTagStructural StructuralTag;

        private int readingOrder;

        public int ReadingOrder
        {
            get
            {
                return readingOrder;
            }

            set
            {
                if (HasReadingOrderTag)
                {
                    //Debug.Print($"TrRegion: set RO: HAS RO-tag: Delete it! Count before: {Tags.Count}");
                    DeleteReadingOrderTag();

                    //Debug.Print($"Count after: {Tags.Count}");
                }

                readingOrder = value;
                TrTagReadingOrder rOTag = new TrTagReadingOrder(readingOrder);
                Tags.Add(rOTag);
            }
        }

        // ABSTRACT PROPERTIES ----------------------------------------------------------
        protected bool changesUploaded = false;

        public abstract bool ChangesUploaded { get; set; }

        // protected bool _hasLines;
        public abstract bool HasLines { get; }

        protected TrWords words = new TrWords();

        public abstract TrWords Words { get; }

        public abstract int NumberOfLines { get; }

        // ABSTRACT METHODS
        public abstract List<string> GetStructuralTags();

        public abstract void Move(int horizontally, int vertically);

        public abstract bool DeleteShortBaselines(int limit, TrLog log);

        public abstract void SimplifyBoundingBoxes();

        public abstract void SimplifyBoundingBoxes(int minimumHeight, int maximumHeight);

        public abstract List<string> GetExpandedText(bool refine, bool convertOtrema);

        public abstract void ExtendBaseLines(TrDialogTransferSettings settings, TrLog log);

        public abstract XElement ToXML();

        // -----------------------
        public void FixCoordinates()
        {
            int pageWidth = ParentTranscript.ParentPage.Width;
            int pageHeigth = ParentTranscript.ParentPage.Height;

            TrCoords coords = new TrCoords(CoordsString);

            // ændrer punkter med negative koordinater eller større end siden
            foreach (TrCoord c in coords)
            {
                if (c.X < 0)
                {
                    c.X = 0;
                    HasChanged = true;
                }

                if (c.X > pageWidth)
                {
                    c.X = pageWidth;
                    HasChanged = true;
                }

                if (c.Y < 0)
                {
                    c.Y = 0;
                    HasChanged = true;
                }

                if (c.Y > pageHeigth)
                {
                    c.Y = pageHeigth;
                    HasChanged = true;
                }
            }

            if (HasChanged)
            {
                CoordsString = coords.ToString();
            }
        }

        private void DeleteReadingOrderTag()
        {
            bool foundTag = false;

            if (HasReadingOrderTag)
            {
                foundTag = true;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagReadingOrder))
                    {
                        t.MarkToDeletion = true;
                    }
                }
            }

            if (foundTag)
            {
                for (int i = Tags.Count - 1; i >= 0; i--)
                {
                    if (Tags[i].MarkToDeletion)
                    {
                        Tags.RemoveAt(i);
                    }
                }

                HasChanged = true;
            }
        }

        private int number;

        public int Number
        {
            get
            {
                if (ParentContainer.IsZeroBased)
                {
                    number = ReadingOrder + 1;
                }
                else
                {
                    number = ReadingOrder;
                }

                return number;
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
                if (hasChanged)
                {
                    StatusColor = Brushes.Orange;
                }

                NotifyPropertyChanged("HasChanged");
                ParentTranscript.HasChanged = value;
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

        private bool hasStructuralTag;

        public bool HasStructuralTag
        {
            get
            {
                hasStructuralTag = false;

                foreach (TrTag t in Tags)
                {
                    hasStructuralTag = hasStructuralTag || (t.GetType() == typeof(TrTagStructural));
                }

                return hasStructuralTag;
            }
        }

        private bool hasReadingOrderTag;

        public bool HasReadingOrderTag
        {
            get
            {
                hasReadingOrderTag = false;

                foreach (TrTag t in Tags)
                {
                    hasReadingOrderTag = hasReadingOrderTag || (t.GetType() == typeof(TrTagReadingOrder));
                }

                return hasReadingOrderTag;
            }
        }

        private string structuralTagValue;

        public string StructuralTagValue
        {
            get
            {
                if (HasStructuralTag)
                {
                    structuralTagValue = StructuralTag.SubType;
                }
                else
                {
                    structuralTagValue = string.Empty;
                }

                return structuralTagValue;
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
        private int leftBorder;

        public int LeftBorder
        {
            get
            {
                leftBorder = TrLibrary.GetLeftMostXcoord(CoordsString);
                return leftBorder;
            }
        }

        private int rightBorder;

        public int RightBorder
        {
            get
            {
                rightBorder = TrLibrary.GetRightMostXcoord(CoordsString);
                return rightBorder;
            }
        }

        private int topBorder;

        public int TopBorder
        {
            get
            {
                topBorder = TrLibrary.GetTopYcoord(CoordsString);
                return topBorder;
            }
        }

        private int bottomBorder;

        public int BottomBorder
        {
            get
            {
                bottomBorder = TrLibrary.GetBottomYcoord(CoordsString);
                return bottomBorder;
            }
        }

        private int hPos;

        public int Hpos
        {
            get
            {
                hPos = LeftBorder;
                return hPos;
            }
        }

        private int vPos;

        public int Vpos
        {
            get
            {
                vPos = TrLibrary.GetAverageYcoord(CoordsString);
                return vPos;
            }
        }

        private int horizontalOrder;

        public int HorizontalOrder
        {
            get
            {
                horizontalOrder = (Hpos * 10_000) + Vpos;
                return horizontalOrder;
            }
        }

        private int verticalOrder;

        public int VerticalOrder
        {
            get
            {
                verticalOrder = (Vpos * 10_000) + Hpos;
                return verticalOrder;
            }
        }

        private XElement regionRef;

        public XElement RegionRef
        {
            get
            {
                regionRef = new XElement(
                    TrLibrary.Xmlns + "RegionRefIndexed",
                    new XAttribute("index", ReadingOrder),
                    new XAttribute("regionRef", ID));
                return regionRef;
            }
        }

        // CONSTRUCTORS ---------------------------------------------------------------------------------------

        // constructor ved indlæsning af XDoc
        public TrRegion(string rType, string rID, string rTags, float rOrientation, string rCoords, TrRegions parentContainer)
        {
            //Debug.Print("-----------------------------------------------------------------------------");
            //Debug.Print($"New TrRegion in the making! tags count = {Tags.Count}");
            Type = rType;
            ID = rID;
            TagString = rTags;
            ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordsString = rCoords;
            Orientation = rOrientation;
            ParentContainer = parentContainer;
            ParentTranscript = parentContainer.ParentTranscript;

            //TextLines.ParentRegion = this;
            //Tags.ParentRegion = this;
            Tags.LoadFromCustomAttribute(rTags);

            //Debug.Print($"New TrRegion! RO = {ReadingOrder}, tags count = {Tags.Count}");
            if (Tags.Count > 0)
            {
                foreach (TrTag tag in Tags)
                {
                    if (tag.GetType() == typeof(TrTagStructural))
                    {
                        StructuralTag = (TrTagStructural)tag;
                    }
                }
            }
        }

        // constructor ved skabelse af ny region
        public TrRegion(int rOrder, float rOrientation, string rCoords, TrRegions parentContainer)
        {
            Type = string.Empty;
            ID = "region_" + TrLibrary.GetNewTimeStamp().ToString();
            ReadingOrder = rOrder;
            CoordsString = rCoords;
            Orientation = rOrientation;
            ParentContainer = parentContainer;
            ParentTranscript = parentContainer.ParentTranscript;

            //TextLines.ParentRegion = this;
            //Tags.ParentRegion = this;

            // Debug.WriteLine($"Region (empty) constructed! ID = {ID}, RO = {ReadingOrder}, Coords = {CoordsString}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public int CompareTo(object obj)
        {
            var region = obj as TrTextRegion;
            return ReadingOrder.CompareTo(region.ReadingOrder);
        }
    }
}
