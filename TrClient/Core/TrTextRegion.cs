// <copyright file="TrTextRegion.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows.Media;
    using System.Xml.Linq;
    using TrClient.Extensions;
    using TrClient.Helpers;
    using TrClient.Libraries;

    public class TrTextRegion : TrRegion // IComparable, INotifyPropertyChanged
    {
        public TrTextLines TextLines = new TrTextLines();
        public TrRows Rows = new TrRows();

        // OVERRIDE PROPERTIES -------------------------------------------------------------------------------
        public override bool ChangesUploaded
        {
            get
            {
                return changesUploaded;
            }

            set
            {
                changesUploaded = value;
                if (changesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }

                NotifyPropertyChanged("ChangesUploaded");
                TextLines.ChangesUploaded = value;
            }
        }

        public override bool HasLines
        {
            get
            {
                return TextLines.Count > 0;
            }
        }

        private int numberOfLines = 0;

        public override int NumberOfLines
        {
            get
            {
                numberOfLines = TextLines.Count;
                return numberOfLines;
            }
        }

        // OVERRIDE METHODS ---------------------------------------------------------------------------------
        public override List<string> GetStructuralTags()
        {
            List<string> tempList = new List<string>();

            foreach (TrTextLine textLine in TextLines)        // OVERRIDE pga denne line
            {
                if (textLine.HasStructuralTag)
                {
                    tempList.Add(textLine.StructuralTagValue);
                }
            }

            List<string> tagList = tempList.Distinct().ToList();
            tagList.Sort();
            return tagList;
        }

        public override void Move(int horizontally, int vertically)
        {
            TrCoords c = new TrCoords(CoordsString);
            foreach (TrCoord currentCoord in c)
            {
                currentCoord.X = currentCoord.X + horizontally;
                currentCoord.Y = currentCoord.Y + vertically;
            }

            CoordsString = c.ToString();

            if (HasLines)
            {
                foreach (TrTextLine line in TextLines)
                {
                    line.Move(horizontally, vertically);
                }
            }
        }

        public override bool DeleteShortBaselines(int limit, TrLog log)
        {
            bool regionIsOK = true;
            string errorMessage;

            foreach (TrTextLine line in TextLines)
            {
                if (line.Width < limit)
                {
                    regionIsOK = false;
                    line.MarkToDeletion = true;
                    errorMessage = $"Width = {line.Width}: Line deleted!";
                    log.Add(line, errorMessage);
                }
            }

            if (!regionIsOK)
            {
                for (int i = TextLines.Count - 1; i >= 0; i--)
                {
                    if (TextLines[i].MarkToDeletion)
                    {
                        TextLines.RemoveAt(i);
                    }
                }
            }

            return regionIsOK;
        }

        public override void SimplifyBoundingBoxes()
        {
            foreach (TrTextLine line in TextLines)
            {
                line.SimplifyBoundingBox();
            }
        }

        public override void SimplifyBoundingBoxes(int minimumHeight, int maximumHeight)
        {
            foreach (TrTextLine line in TextLines)
            {
                line.SimplifyBoundingBox(minimumHeight, maximumHeight);
            }
        }

        public override List<string> GetExpandedText(bool refine, bool convertOtrema)
        {
            List<string> tempList = new List<string>();

            foreach (TrTextLine textLine in TextLines)
            {
                tempList.Add(textLine.ExpandedText);
            }

            return tempList;
        }

        public override TrWords Words
        {
            get
            {
                foreach (TrTextLine textLine in TextLines)
                {
                    foreach (TrWord w in textLine.Words)
                    {
                        words.Add(w);
                    }
                }

                return words;
            }
        }

        public override void ExtendBaseLines(TrDialogTransferSettings settings, TrLog log)
        {
            // Debug.WriteLine($"TrRegion_Text : ExtendBaseLines");
            string errorMessage;

            if (HasLines)
            {
                foreach (TrTextLine line in TextLines)
                {
                    if (settings.ExtendLeft)
                    {
                        line.ExtendLeft(settings.LeftAmount);
                        errorMessage = $"Line extended left with {settings.LeftAmount}!";
                        log.Add(line, errorMessage);
                    }

                    if (settings.ExtendRight)
                    {
                        line.ExtendRight(settings.RightAmount);
                        errorMessage = $"Line extended right with {settings.RightAmount}!";
                        log.Add(line, errorMessage);
                    }
                }
            }
        }

        public override XElement ToXML()
        {
            string customString = Tags.ToString();

            XElement xRegion = new XElement(
                TrLibrary.Xmlns + "TextRegion",
                new XAttribute("type", Type.ToString()),
                new XAttribute("orientation", Orientation.ToString()),
                new XAttribute("id", ID),
                new XAttribute("custom", customString),
                new XElement(
                    TrLibrary.Xmlns + "Coords",
                    new XAttribute("points", CoordsString)));

            StringBuilder sb = new StringBuilder();

            foreach (TrTextLine line in TextLines)
            {
                xRegion.Add(line.ToXML());
                sb.Append(line.TextEquiv);
                sb.Append(Environment.NewLine);
            }

            XElement xRegionText = new XElement(
                TrLibrary.Xmlns + "TextEquiv",
                new XElement(TrLibrary.Xmlns + "Unicode", sb.ToString()));
            xRegion.Add(xRegionText);

            // Debug.WriteLine(XRegion.ToString());
            return xRegion;
        }

        // CONSTRUCTORS ---------------------------------------------------------------------------------------

        // constructor ved indlæsning af XDoc
        public TrTextRegion(string rType, string rID, string rTags, float rOrientation, string rCoords, TrRegions parentContainer)
            : base(rType, rID, rTags, rOrientation, rCoords, parentContainer)
        {
            //Debug.Print("-----------------------------------------------------------------------------");
            //Debug.Print($"New Tr TEXT Region in the making! ");
            TextLines.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // constructor ved skabelse af ny region
        public TrTextRegion(int rOrder, float rOrientation, string rCoords, TrRegions parentContainer)
            : base(rOrder, rOrientation, rCoords, parentContainer)
        {
            ParentContainer = parentContainer;
            ParentTranscript = parentContainer.ParentTranscript;
            TextLines.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // -------------------------------------------------------------------------------------------------------------------------------
        public TrTextLine GetLineByNumber(int lineNumber)
        {
            if (TextLines != null)
            {
                if (lineNumber >= 1 && lineNumber <= TextLines.Count)
                {
                    return TextLines[lineNumber - 1];
                }
                else
                {
                    Debug.WriteLine($"Line nr. {lineNumber} eksisterer ikke!");
                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"Denne region indeholder ikke lines!");
                return null;
            }
        }

        public bool ExistsLineNumber(int lineNumber)
        {
            bool temp = false;
            if (HasLines)
            {
                if (lineNumber <= TextLines.Count)
                {
                    temp = true;
                }
            }

            return temp;
        }

        public TrParagraphs GetParagraphs()
        {
            TrParagraphs temp = new TrParagraphs();
            temp.ParentRegion = this;
            //Debug.WriteLine($"TextRegion:GetP - parentregion {temp.ParentRegion.Number}");

            int max = TextLines.Count - 1;
            //Debug.WriteLine($"TextRegion:GetP - max = {max}");
            for (int i = 0; i < max; i++)
            {
                //Debug.WriteLine($"TextRegion:GetP - i = {i}");

                TrTextLine currentLine = TextLines[i];
                TrTextLine nextLine = TextLines[i + 1];
                if (currentLine.HasStructuralTag && nextLine.HasStructuralTag)
                {
                    if (currentLine.StructuralTagValue == nextLine.StructuralTagValue)
                    {
                        currentLine.Next = nextLine;
                        nextLine.Previous = currentLine;
                    }
                }
            }

            //Debug.WriteLine($"TextRegion:GetP - next/previous OK");

            int n = 0;
            foreach (TrTextLine textLine in TextLines)
            {
                //Debug.WriteLine($"TextRegion:GetP - foreach TL, linenumber = {textLine.Number}");

                if (textLine.Previous == null && textLine.HasStructuralTag)
                {
                    //Debug.WriteLine($"TextRegion:GetP - TL.previous = {textLine.Previous}, TL.hasstruct = {textLine.HasStructuralTag}");

                    n++;
                    //Debug.WriteLine($"TextRegion:GetP - n = {n}");
                    TrParagraph newParagraph = new TrParagraph(n, textLine);
                    temp.Add(newParagraph);
                    newParagraph.ParentRegion = this;
                }
            }

            if (temp == null)
            {
                //Debug.WriteLine($"TextRegion:GetP - temp = null!");
            }
            else
            {
                //Debug.WriteLine($"TextRegion:GetP - temp != null!");
            }

            return temp;
        }

        // flyttet til overordnet klasse ---------------------------------

        //private XElement _regionRef;
        //public XElement RegionRef
        //{
        //    get
        //    {
        //        _regionRef = new XElement(TrLibrary.xmlns + "RegionRefIndexed",
        //            new XAttribute("index", ReadingOrder),
        //            new XAttribute("regionRef", ID));
        //        return _regionRef;

        //    }
        //}

        //public string ID { get; set; }
        //public float Orientation { get; set; }
        //public string CoordsString { get; set; }
        //public TrTags Tags = new TrTags();
        //public TrTagStructural StructuralTag;
        //public TrRegions ParentContainer;
        //public TrTranscript ParentTranscript;

        //private int _readingOrder;
        //public int ReadingOrder
        //{
        //    get { return _readingOrder; }
        //    set
        //    {
        //        if (HasReadingOrderTag)
        //            DeleteReadingOrderTag();

        //        _readingOrder = value;
        //        TrTagReadingOrder ROTag = new TrTagReadingOrder(_readingOrder);
        //        Tags.Add(ROTag);
        //    }
        //}

        //private void DeleteReadingOrderTag()
        //{
        //    bool FoundTag = false;

        //    if (HasReadingOrderTag)
        //    {
        //        FoundTag = true;
        //        foreach (TrTag T in Tags)
        //        {
        //            if (T.GetType() == typeof(TrTagReadingOrder))
        //                T.MarkToDeletion = true;
        //        }
        //    }

        //    if (FoundTag)
        //    {
        //        for (int i = Tags.Count - 1; i >= 0; i--)
        //        {
        //            if (Tags[i].MarkToDeletion)
        //                Tags.RemoveAt(i);
        //        }
        //        HasChanged = true;
        //    }
        //}

        //private int _number;
        //public int Number
        //{
        //    get
        //    {
        //        if (ParentContainer.IsZeroBased)
        //            _number = ReadingOrder + 1;
        //        else
        //            _number = ReadingOrder;
        //        return _number;
        //    }
        //}

        //private bool _hasChanged = false;
        //public bool HasChanged
        //{
        //    get { return _hasChanged; }
        //    set
        //    {
        //        _hasChanged = value;
        //        if (_hasChanged)
        //            StatusColor = Brushes.Orange;
        //        NotifyPropertyChanged("HasChanged");
        //        ParentTranscript.HasChanged = value;
        //    }
        //}

        //private SolidColorBrush _statusColor = Brushes.Red;
        //public SolidColorBrush StatusColor
        //{
        //    get { return _statusColor; }
        //    set
        //    {
        //        if (_statusColor != value)
        //        {
        //            _statusColor = value;
        //            NotifyPropertyChanged("StatusColor");
        //        }
        //    }
        //}

        //private bool _hasStructuralTag;
        //public bool HasStructuralTag
        //{
        //    get
        //    {
        //        _hasStructuralTag = false;

        //        foreach (TrTag T in Tags)
        //        {
        //            _hasStructuralTag = _hasStructuralTag || (T.GetType() == typeof(TrTagStructural));
        //        }
        //        return _hasStructuralTag;

        //    }
        //}

        //private bool _hasReadingOrderTag;
        //public bool HasReadingOrderTag
        //{
        //    get
        //    {
        //        _hasReadingOrderTag = false;

        //        foreach (TrTag T in Tags)
        //        {
        //            _hasReadingOrderTag = _hasReadingOrderTag || (T.GetType() == typeof(TrTagReadingOrder));
        //        }
        //        return _hasReadingOrderTag;

        //    }
        //}

        //private string _structuralTagValue;
        //public string StructuralTagValue
        //{
        //    get
        //    {
        //        if (HasStructuralTag)
        //            _structuralTagValue = StructuralTag.SubType;
        //        else
        //            _structuralTagValue = "";
        //        return _structuralTagValue;
        //    }

        //}

        //public event PropertyChangedEventHandler PropertyChanged;

        //public void NotifyPropertyChanged(string propName)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(propName));
        //}

        //private int _leftBorder;
        //public int LeftBorder
        //{
        //    get
        //    {
        //        _leftBorder = TrLibrary.GetLeftMostXcoord(CoordsString);
        //        return _leftBorder;
        //    }
        //}

        //private int _rightBorder;
        //public int RightBorder
        //{
        //    get
        //    {
        //        _rightBorder = TrLibrary.GetRightMostXcoord(CoordsString);
        //        return _rightBorder;
        //    }
        //}

        //private int _topBorder;
        //public int TopBorder
        //{
        //    get
        //    {
        //        _topBorder = TrLibrary.GetTopYcoord(CoordsString);
        //        return _topBorder;
        //    }
        //}

        //private int _bottomBorder;
        //public int BottomBorder
        //{
        //    get
        //    {
        //        _bottomBorder = TrLibrary.GetBottomYcoord(CoordsString);
        //        return _bottomBorder;
        //    }
        //}

        //private int _hPos;
        //public int Hpos
        //{
        //    get
        //    {
        //        _hPos = LeftBorder;
        //        return _hPos;
        //    }
        //}

        //private int _vPos;
        //public int Vpos
        //{
        //    get
        //    {
        //        _vPos = TrLibrary.GetAverageYcoord(CoordsString);
        //        return _vPos;
        //    }
        //}

        //private int _horizontalOrder;
        //public int HorizontalOrder
        //{
        //    get
        //    {
        //        _horizontalOrder = Hpos * 10_000 + Vpos;
        //        return _horizontalOrder;
        //    }
        //}

        //private int _verticalOrder;
        //public int VerticalOrder
        //{
        //    get
        //    {
        //        _verticalOrder = Vpos * 10_000 + Hpos;
        //        return _verticalOrder;
        //    }
        //}

        //public int CompareTo(object obj)
        //{
        //    var region = obj as TrRegion_Text;
        //    return ReadingOrder.CompareTo(region.ReadingOrder);
        //}

        // GAMMEL implementering
        //public bool RepairBaseLines(TrLog Log)
        //{
        //    bool RegionIsOK = true;
        //    string ErrorMessage;

        //    foreach (TrTextLine Line in TextLines)
        //    {
        //        if (Line.HasBaseLine)   // Line.IsBaseLineStraight &&
        //        {
        //            // if (!Line.IsCoordinatesPositive || !Line.IsBaseLineDirectionOK)
        //            {
        //                RegionIsOK = false;
        //                Line.RepairBaseLine();
        //                // ErrorMessage = "Direction error or outside edges. Error fixed.";
        //                // Log.Add(Line, ErrorMessage);
        //            }
        //        }
        //        else
        //        {
        //            RegionIsOK = false;
        //            ErrorMessage = "Critical error! Please fix manually.";
        //            Log.Add(Line, ErrorMessage);
        //        }
        //    }
        //    return RegionIsOK;
        //}
    }
}
