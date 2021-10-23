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
    public class TrRegion_Text : TrRegion // IComparable, INotifyPropertyChanged
    {
        
        public TrTextLines TextLines = new TrTextLines();
        public TrRows Rows = new TrRows();


        // OVERRIDE PROPERTIES -------------------------------------------------------------------------------

        public override bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                if (_changesUploaded)
                    StatusColor = Brushes.DarkViolet;
                NotifyPropertyChanged("ChangesUploaded");
                TextLines.ChangesUploaded = value;          // OVERRIDE pga denne linie 
            }
        }

        public override bool HasLines
        {
            get
            {
                return (TextLines.Count > 0);
            }
        }

        private int _numberOfLines = 0;
        public override int NumberOfLines
        {
            get
            {
                _numberOfLines = TextLines.Count;
                return _numberOfLines;
            }
        }


        // OVERRIDE METHODS ---------------------------------------------------------------------------------

        public override List<string> GetStructuralTags()
        {
            List<string> TempList = new List<string>();

            foreach (TrTextLine TL in TextLines)        // OVERRIDE pga denne line
                if (TL.HasStructuralTag)
                    TempList.Add(TL.StructuralTagValue);

            List<string> TagList = TempList.Distinct().ToList();
            TagList.Sort();
            return TagList;
        }


        public override void Move(int Horizontally, int Vertically)
        {
            TrCoords C = new TrCoords(CoordsString);
            foreach (TrCoord CurrentCoord in C)
            {
                CurrentCoord.X = CurrentCoord.X + Horizontally;
                CurrentCoord.Y = CurrentCoord.Y + Vertically;
            }
            CoordsString = C.ToString();

            if (HasLines)
            {
                foreach (TrTextLine Line in TextLines)
                {
                    Line.Move(Horizontally, Vertically);
                }
            }
        }

        public override bool DeleteShortBaselines(int Limit, TrLog Log)
        {
            bool RegionIsOK = true;
            string ErrorMessage;

            foreach (TrTextLine Line in TextLines)
            {
                if (Line.Width < Limit)
                {
                    RegionIsOK = false;
                    Line.MarkToDeletion = true;
                    ErrorMessage = $"Width = {Line.Width}: Line deleted!";
                    Log.Add(Line, ErrorMessage);
                }
            }
            if (!RegionIsOK)
                for (int i = TextLines.Count - 1; i >= 0; i--)
                {
                    if (TextLines[i].MarkToDeletion)
                        TextLines.RemoveAt(i);
                }
            return RegionIsOK;
        }


        public override void SimplifyBoundingBoxes()
        {
            foreach (TrTextLine Line in TextLines)
                Line.SimplifyBoundingBox();
        }

        public override void SimplifyBoundingBoxes(int MinimumHeight, int MaximumHeight)
        {
            foreach (TrTextLine Line in TextLines)
                Line.SimplifyBoundingBox(MinimumHeight, MaximumHeight);
        }

        public override List<string> GetExpandedText(bool Refine, bool ConvertOtrema)
        {
            List<string> TempList = new List<string>();

            foreach (TrTextLine TL in TextLines)
                TempList.Add(TL.ExpandedText);

            return TempList;
        }

        public override TrWords Words
        {
            get
            {
                foreach (TrTextLine TL in TextLines)
                {
                    foreach (TrWord W in TL.Words)
                        _words.Add(W);
                }
                return _words;
            }
        }

        public override void ExtendBaseLines(TrDialogTransferSettings Settings, TrLog Log)
        {
            // Debug.WriteLine($"TrRegion_Text : ExtendBaseLines");

            string ErrorMessage;

            if (HasLines)
            {
                foreach (TrTextLine Line in TextLines)
                {
                    if (Settings.ExtendLeft)
                    {
                        Line.ExtendLeft(Settings.LeftAmount);
                        ErrorMessage = $"Line extended left with {Settings.LeftAmount}!";
                        Log.Add(Line, ErrorMessage);
                    }
                    if (Settings.ExtendRight)
                    {
                        Line.ExtendRight(Settings.RightAmount);
                        ErrorMessage = $"Line extended right with {Settings.RightAmount}!";
                        Log.Add(Line, ErrorMessage);
                    }
                }
            }
        }

        public override XElement ToXML()
        {
            string CustomString = Tags.ToString();

            XElement xRegion = new XElement(TrLibrary.xmlns + "TextRegion",
                new XAttribute("type", Type.ToString()),
                new XAttribute("orientation", Orientation.ToString()),
                new XAttribute("id", ID),
                new XAttribute("custom", CustomString),
                new XElement(TrLibrary.xmlns + "Coords",
                new XAttribute("points", CoordsString)));

            StringBuilder sb = new StringBuilder();

            foreach (TrTextLine Line in TextLines)
            {
                xRegion.Add(Line.ToXML());
                sb.Append(Line.TextEquiv);
                sb.Append(Environment.NewLine);
            }

            XElement xRegionText = new XElement(TrLibrary.xmlns + "TextEquiv",
                new XElement(TrLibrary.xmlns + "Unicode", sb.ToString()));
            xRegion.Add(xRegionText);

            // Debug.WriteLine(XRegion.ToString());
            return xRegion;
        }



        // CONSTRUCTORS ---------------------------------------------------------------------------------------

        // constructor ved indlæsning af XDoc
        public TrRegion_Text(string rType, string rID, string rTags, float rOrientation, string rCoords) : base(rType, rID, rTags, rOrientation, rCoords)
        {
            //Debug.Print("-----------------------------------------------------------------------------");
            //Debug.Print($"New Tr TEXT Region in the making! ");
            TextLines.ParentRegion = this;
            Tags.ParentRegion = this;
        }

        // constructor ved skabelse af ny region
        public TrRegion_Text(int rOrder, float rOrientation, string rCoords) : base(rOrder, rOrientation, rCoords)
        {
            TextLines.ParentRegion = this;
            Tags.ParentRegion = this;
        }



        // -------------------------------------------------------------------------------------------------------------------------------

        public TrTextLine GetLineByNumber(int LineNumber)
        {
            if (TextLines != null)
            {
                if (LineNumber >= 1 && LineNumber <= TextLines.Count)
                {
                    return TextLines[LineNumber - 1];
                }
                else
                {
                    Debug.WriteLine($"Line nr. {LineNumber } eksisterer ikke!");
                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"Denne region indeholder ikke lines!");
                return null;
            }
        }
        





        public bool ExistsLineNumber(int LineNumber)
        {
            bool temp = false;
            if (HasLines)
                if (LineNumber <= TextLines.Count)
                    temp = true;
            return temp;
        }









        public TrParagraphs GetParagraphs()
        {
            TrParagraphs temp = new TrParagraphs();
            temp.ParentRegion = this;
            Debug.WriteLine($"TextRegion:GetP - parentregion {temp.ParentRegion.Number}");

            int Max = TextLines.Count - 1;
            Debug.WriteLine($"TextRegion:GetP - max = {Max}");
            for (int i = 0; i < Max; i++)
            {
                Debug.WriteLine($"TextRegion:GetP - i = {i}");

                TrTextLine CurrentLine = TextLines[i];
                TrTextLine NextLine = TextLines[i + 1];
                if (CurrentLine.HasStructuralTag && NextLine.HasStructuralTag)
                    if (CurrentLine.StructuralTagValue == NextLine.StructuralTagValue)
                    {
                        CurrentLine.Next = NextLine;
                        NextLine.Previous = CurrentLine;
                    }
            }
            Debug.WriteLine($"TextRegion:GetP - next/previous OK");

            int n = 0;
            foreach (TrTextLine TL in TextLines)
            {
                Debug.WriteLine($"TextRegion:GetP - foreach TL, linenumber = {TL.Number}");

                if (TL.Previous == null && TL.HasStructuralTag)
                {
                    Debug.WriteLine($"TextRegion:GetP - TL.previous = {TL.Previous}, TL.hasstruct = {TL.HasStructuralTag}");

                    n++;
                    Debug.WriteLine($"TextRegion:GetP - n = {n}");
                    TrParagraph NewParagraph = new TrParagraph(n, TL);
                    temp.Add(NewParagraph);
                    NewParagraph.ParentRegion = this;
                }
            }

            if (temp == null)
                Debug.WriteLine($"TextRegion:GetP - temp = null!");
            else
                Debug.WriteLine($"TextRegion:GetP - temp != null!");

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
        //public TrTag_Structural StructuralTag;
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
        //        TrTag_ReadingOrder ROTag = new TrTag_ReadingOrder(_readingOrder);
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
        //            if (T.GetType() == typeof(TrTag_ReadingOrder))
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
        //            _hasStructuralTag = _hasStructuralTag || (T.GetType() == typeof(TrTag_Structural));
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
        //            _hasReadingOrderTag = _hasReadingOrderTag || (T.GetType() == typeof(TrTag_ReadingOrder));
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

