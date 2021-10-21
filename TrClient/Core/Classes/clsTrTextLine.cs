using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TrClient;
using DanishNLP;

namespace TrClient
{
    public enum contentType
    {
        undefined = 0,
        date = 1,
        accNumber = 2,
        plain = 3,
        caption = 100
    }

    public enum visualOffsetType
    {
        none = 0,
        smallBoundingBox = 1,
        largeBoundingBox = 2
    }

    public class clsTrTextLine : IComparable, INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string TagString { get; set; }
        public string CoordsString { get; set; }
        public string BaseLineCoordsString { get; set; }
        public bool MarkToDeletion = false;
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }

        private int _readingOrder;
        public int ReadingOrder
        {
            get { return _readingOrder; }
            set
            {
                if (HasReadingOrderTag)
                {
                    //Debug.Print($"clsTrTextLine: set RO: HAS RO-tag: Delete it! Count before: {Tags.Count}");
                    DeleteReadingOrderTag();
                    //Debug.Print($"Count after: {Tags.Count}");
                }

                _readingOrder = value;
                clsTrReadingOrderTag ROTag = new clsTrReadingOrderTag(_readingOrder);
                Tags.Add(ROTag);
                //Debug.Print($"clsTrTextLine: Added new RO-tag: Count now: {Tags.Count}");
            }
        }

        private bool _hasReadingOrderTag;
        public bool HasReadingOrderTag
        {
            get
            {
                _hasReadingOrderTag = false;

                foreach (clsTrTag T in Tags)
                {
                    _hasReadingOrderTag = _hasReadingOrderTag || (T.GetType() == typeof(clsTrReadingOrderTag));
                }
                return _hasReadingOrderTag;

            }
        }

        private void DeleteReadingOrderTag()
        {
            bool FoundTag = false;

            if (HasReadingOrderTag)
            {
                FoundTag = true;
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrReadingOrderTag))
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

        private clsTrTextLine _previous;
        public clsTrTextLine Previous
        {
            get
            {
                if (Number == 1)
                    _previous = null;
                else
                    try
                    {
                        _previous = ParentRegion.GetLineByNumber(Number - 1);

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Error! Exception message when getting previous in TL {Number.ToString()}: {e.Message}");

                    }
                return _previous;
            }
            set
            {
                if (_previous != value)
                {
                    _previous = value;
                }
            }
        }

        private clsTrTextLine _next;
        public clsTrTextLine Next
        {
            get
            {
                if (Number == ParentContainer.Count)
                    _next = null;
                else
                    _next = ParentRegion.GetLineByNumber(Number + 1);
                return _next;
            }
            set
            {
                if (_next != value)
                {
                    _next = value;
                }
            }
        }


        private clsTrWords _words = new clsTrWords();
        public clsTrWords Words
        {
            get
            {
                var WordsArray = TextEquiv.Split(' ').ToArray();
                int WordsCount = WordsArray.Length;
                //int PositionPointer = 0;

                for (int i = 0; i < WordsCount; i++)
                {
                    if (WordsArray[i].ToString().Trim() != "")
                    {
                        // NEJ ikke mere: vi fjerner lige tallene
                        //if (!clsLanguageLibrary.IsNumeric(WordsArray[i]))
                        {
                            clsTrWord Word = new clsTrWord(WordsArray[i].ToString(), this);
                            _words.Add(Word);
                        }
                    }
                }

                return _words;
            }
        }


        private int Amount = 50; // "delta" for BoundingBoxLarge

        private Polygon _visualLineArea;
        public Polygon VisualLineArea
        {
            get
            {
                visualOffsetType Offset = visualOffsetType.largeBoundingBox;
                Polygon Temp = new Polygon();
                PointCollection LinePoints = new PointCollection();
                clsTrCoords NewCoords = new clsTrCoords(CoordsString);

                foreach (clsTrCoord C in NewCoords)
                {
                    clsTrCoord TempC;
                    if (Offset != visualOffsetType.none)
                    {
                        if (Offset == visualOffsetType.smallBoundingBox)
                            TempC = C.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetSmall);
                        else
                            TempC = C.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetLarge);
                    }
                    else
                        TempC = C;

                    // SCALING
                    double ScalingFactor = 0.25;
                    double TempX = (double)TempC.X * ScalingFactor;
                    double TempY = (double)TempC.Y * ScalingFactor;

                    Point CurrentPoint = new Point(TempX, TempY);
                    LinePoints.Add(CurrentPoint);
                }
                Temp.Points = LinePoints;
                return Temp;

            }
        }

        private Polyline _visualBaseLine;
        public Polyline VisualBaseLine
        {
            get
            {
                visualOffsetType Offset = visualOffsetType.largeBoundingBox;
                Polyline Temp = new Polyline();
                PointCollection BaseLinePoints = new PointCollection();
                clsTrCoords NewCoords = new clsTrCoords(BaseLineCoordsString);

                foreach (clsTrCoord C in NewCoords)
                {
                    clsTrCoord TempC;
                    if (Offset != visualOffsetType.none)
                    {
                        if (Offset == visualOffsetType.smallBoundingBox)
                            TempC = C.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetSmall);
                        else
                            TempC = C.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetLarge);
                    }
                    else
                        TempC = C;

                    // SCALING
                    double ScalingFactor = 0.25;
                    double TempX = (double)TempC.X * ScalingFactor;
                    double TempY = (double)TempC.Y * ScalingFactor;

                    Point CurrentPoint = new Point(TempX, TempY);
                    BaseLinePoints.Add(CurrentPoint);
                }
                Temp.Points = BaseLinePoints;
                return Temp;
            }
        }


        private string _textEquiv;
        public string TextEquiv
        {
            get { return _textEquiv; }
            set
            {
                if (_textEquiv != value)
                {
                    _textEquiv = value;
                    NotifyPropertyChanged("TextEquiv");
                }
            }
        }



        private bool _endsWithHyphen;
        public bool EndsWithHyphen
        {
            get
            {
                _endsWithHyphen = false;
                string temp = GetExpandedText(false, false);
                if (temp.Length > 2)
                {
                    char last = temp[temp.Length - 1];
                    if (last == '-' || last == '=')
                        _endsWithHyphen = true;
                }
                return _endsWithHyphen;
            }
        }

        private bool _startsWithSmallLetter;
        public bool StartsWithSmallLetter
        {
            get
            {
                string temp = GetExpandedText(false, false);
                if (temp.Length > 0)
                {
                    char first = temp[0];
                    _startsWithSmallLetter = char.IsLower(first);
                }
                else
                    _startsWithSmallLetter = false;
                return _startsWithSmallLetter;
            }
        }

        private clsTrStructuralTag _structuralTag;
        public clsTrStructuralTag StructuralTag
        {
            get { return _structuralTag; }
            set
            {
                if (_structuralTag != value)
                {
                    _structuralTag = value;
                    NotifyPropertyChanged("StructuralTag");
                    // StructuralTagValue = value.SubType;
                }
            }
        }

        public void WrapSuperAndSubscriptWithSpaces()
        {
            if (HasSuperscriptTag || HasSubscriptTag)
            {

                clsTrTags SuperAndSubscriptTags = new clsTrTags();
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrStyleTag))
                        if ((T as clsTrStyleTag).Superscript || (T as clsTrStyleTag).Subscript)
                            SuperAndSubscriptTags.Add(T);
                }

                int SuperAndSubscriptCount = SuperAndSubscriptTags.Count;
                //Debug.Print($"Super- and Subscripts: {SuperAndSubscriptCount}");

                Regex RomanNumbers = new Regex(@"\b[IVXLCDM]+\b");

                for (int i = 0; i < SuperAndSubscriptCount; i++)
                {
                    clsTrTag S = SuperAndSubscriptTags[i];
                    int Offset = (S as clsTrTextualTag).Offset;
                    int EndPosition = (S as clsTrTextualTag).EndPosition;
                    int Length = (S as clsTrTextualTag).Length;
                    string TagContent = TextEquiv.Substring(Offset, Length);

                    //Debug.Print($"Content: {TagContent}");
                    //Debug.Print($"Offset: {Offset}, Endpos: {EndPosition}");

                    MatchCollection RomanNumberMatches = RomanNumbers.Matches(TagContent);

                    // det er dog kun, hvis selve tagget er et romertal eller et arabertal, at vi indsætter space
                    if (RomanNumberMatches.Count > 0 || clsTrLibrary.IsNumeric(TagContent))
                    {
                        // da vi går baglæns, tester vi først endposition og EFTER
                        if (EndPosition < Length - 1)
                        {

                            // så står superscriptet IKKE helt ude for enden
                            // hvad står der EFTER?
                            char After = TextEquiv[EndPosition + 1];
                            //Debug.Print($"After: _{After}_");

                            // hvis det IKKE er et mellemrum, og hvis det IKKE er et bogstav, indsætter vi et mellemrum
                            if (!char.IsWhiteSpace(After) && !char.IsLetter(After))
                            {
                                TextEquiv = TextEquiv.Insert(EndPosition + 1, " ");
                                Tags.Move(EndPosition + 1, 1, true);
                                HasChanged = true;
                                //Debug.Print($"Space added!");
                            }
                        }

                        // dernæst tester vi INDEN, dvs. via offset
                        if (Offset > 0)
                        {
                            // hvis ikke begyndelsen af linien
                            // hvad står der før?
                            char Before = TextEquiv[Offset - 1];
                            //Debug.Print($"Before: _{Before}_");

                            // hvis det IKKE er et mellemrum, og hvis det IKKE er et bogstav, indsætter vi et mellemrum
                            if (!char.IsWhiteSpace(Before) && !char.IsLetter(Before))
                            {
                                TextEquiv = TextEquiv.Insert(Offset, " ");
                                Tags.Move(Offset - 1, 1, true);
                                HasChanged = true;
                                //Debug.Print($"Space added!");
                            }
                        }
                    }

                }
            }
        }


        public void Replace(string Find, string ReplaceWith)
        {
            int Offset;
            int Length;
            int Difference;


            // MEN hvis man ønsker at finde noget, der ender med mellemrum, skal der tages højde for det:
            if (Find.EndsWith(" "))
            {
                Length = Find.Trim().Length;

                // i dette tilfælde vil man også tit gerne sætte noget ind med et mellemrum: det skal så ædndres
                Difference = ReplaceWith.Trim().Length - Find.Trim().Length;

                string SearchForTrimmed = Regex.Escape(Find.Trim()) + "$";
                //Debug.Print($"SearchForTrimmed = _{SearchForTrimmed}_");

                Regex ToFindTrimmed = new Regex(SearchForTrimmed);
                MatchCollection TrimmedMatches = ToFindTrimmed.Matches(TextEquiv);
                if (TrimmedMatches.Count > 1)
                {
                    // noget er helt galt!
                    Debug.Print($"FATAL REGEX ERROR: TextEquiv: {TextEquiv}, Mathces: {TrimmedMatches.Count}");
                }
                else if (TrimmedMatches.Count == 1)
                {
                    Match End = TrimmedMatches[0];
                    Offset = End.Index;

                    if (Offset >= 0)
                    {
                        TextEquiv = TextEquiv.Remove(Offset, Length);

                        // der indsættes en trimmet udgave
                        TextEquiv = TextEquiv.Insert(Offset, ReplaceWith.Trim());
                        Tags.Move(Offset, Difference, true);
                        HasChanged = true;
                    }
                }
            }

            // derefter kan vi gøre det hele mere normalt, dvs. inde i strengen
            Length = Find.Length;
            Difference = ReplaceWith.Length - Find.Length;

            string SearchForFull = Regex.Escape(Find);
            //Debug.Print($"SearchForFull = _{SearchForFull}_");

            Regex ToFindFull = new Regex(SearchForFull);
            MatchCollection FullMatches = ToFindFull.Matches(TextEquiv);

            // så gennemløber vi baglæns
            for (int i = FullMatches.Count - 1; i >= 0; i--)
            {
                // Debug.WriteLine($"clsTrTextLine : Replace : Find = {Find }, ReplaceWith = {ReplaceWith }, i = {i}");

                Match M = FullMatches[i];
                Offset = M.Index;

                if (Offset >= 0)
                {
                    TextEquiv = TextEquiv.Remove(Offset, Length);
                    TextEquiv = TextEquiv.Insert(Offset, ReplaceWith);
                    Tags.Move(Offset, Difference, true);
                    HasChanged = true;
                }

            }


        }

        private int _length = 0;
        public int Length
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _length = TextEquiv.Length;
                else
                    _length = 0;
                return _length;
            }
        }

        private int _number;
        public int Number
        {
            get
            {
                // Debug.WriteLine($"clsTrTextLine : Number: ReadingOrder = {ReadingOrder}");

                if (ParentContainer.IsZeroBased)
                    _number = ReadingOrder + 1;
                else
                    _number = ReadingOrder;
                return _number;
            }
        }

        private string _parentDocTitle;
        public string ParentDocTitle
        {
            get
            {
                _parentDocTitle = ParentRegion.ParentTranscript.ParentPage.ParentDocument.Title;
                return _parentDocTitle;
            }
        }

        private int _parentPageNr;
        public int ParentPageNr
        {
            get
            {
                _parentPageNr = ParentRegion.ParentTranscript.ParentPage.PageNr;
                return _parentPageNr;
            }
        }

        private int _parentRegionNr;
        public int ParentRegionNr
        {
            get
            {
                _parentRegionNr = ParentRegion.Number;
                return _parentRegionNr;
            }
        }

        private bool _isEmpty = false;
        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(TextEquiv))
                    _isEmpty = true;
                else if (string.IsNullOrEmpty(TextEquiv.Trim()))
                    _isEmpty = true;
                return _isEmpty;
            }
        }

        public double MaxAllowedBaseLineAngle = 10.0; // gælder kun FREMAD - hvis linjen går baglæns er den altid gal

        public clsTrTags Tags = new clsTrTags();

        public clsTrTextLines ParentContainer;
        public clsTrTextRegion ParentRegion;

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                    switch (_isLoaded)
                    {
                        case true:
                            StatusColor = Brushes.LimeGreen;
                            break;
                        case false:
                            StatusColor = Brushes.Red;
                            break;
                    }
                }
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

                ParentRegion.HasChanged = value;
            }
        }

        private bool _changesUploaded = false;
        public bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                if (_changesUploaded)
                    StatusColor = Brushes.DarkViolet;
                NotifyPropertyChanged("ChangesUploaded");
            }
        }

        private Int32Rect _boundingBoxSmall;
        public Int32Rect BoundingBoxSmall
        {
            get
            {
                _boundingBoxSmall = new Int32Rect(LeftBorder, TopBorder, BoundingBoxWidth, BoundingBoxHeight);
                return _boundingBoxSmall;
            }
        }

        private clsTrCoord _boundingBoxOffsetSmall;
        public clsTrCoord BoundingBoxOffsetSmall
        {
            get
            {
                clsTrCoord Temp = new clsTrCoord(LeftBorder, TopBorder);
                return Temp;
            }
        }

        private clsTrCoord _boundingBoxOffsetLarge;
        public clsTrCoord BoundingBoxOffsetLarge
        {
            get
            {
                int TempLeft;
                int TempTop;

                if (LeftBorder - Amount > 0)
                    TempLeft = Amount;
                else
                    TempLeft = LeftBorder;

                if (TopBorder - Amount > 0)
                    TempTop = Amount;
                else
                    TempTop = TopBorder;

                clsTrCoord Temp = new clsTrCoord(TempLeft, TempTop);
                return Temp;
            }
        }

        private Int32Rect _boundingBoxLarge;
        public Int32Rect BoundingBoxLarge
        {
            get
            {
                int PageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
                int PageHeight = ParentRegion.ParentTranscript.ParentPage.Height;

                int NewLeft = LeftBorder - Amount;
                if (NewLeft < 0)
                    NewLeft = 0;
                int NewTop = TopBorder - Amount;
                if (NewTop < 0)
                    NewTop = 0;
                int NewRight = RightBorder + Amount;
                if (NewRight > PageWidth)
                    NewRight = PageWidth;
                int NewBottom = BottomBorder + Amount;
                if (NewBottom > PageHeight)
                    NewBottom = PageHeight;

                int NewWidth = NewRight - NewLeft;
                int NewHeight = NewBottom - NewTop;

                _boundingBoxLarge = new Int32Rect(NewLeft, NewTop, NewWidth, NewHeight);
                return _boundingBoxLarge;
            }
        }

        private int _leftBorder;
        public int LeftBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    int BoundingBoxValue = clsTrLibrary.GetLeftMostXcoord(CoordsString);
                    int BaseLineValue = clsTrLibrary.GetLeftMostXcoord(BaseLineCoordsString);

                    if (BoundingBoxValue < BaseLineValue)
                        _leftBorder = BoundingBoxValue;
                    else
                        _leftBorder = BaseLineValue;
                }
                else
                    _leftBorder = -1;

                return _leftBorder;
            }
        }

        private int _rightBorder;
        public int RightBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    int BoundingBoxValue = clsTrLibrary.GetRightMostXcoord(CoordsString);
                    int BaseLineValue = clsTrLibrary.GetRightMostXcoord(BaseLineCoordsString);

                    if (BoundingBoxValue > BaseLineValue)
                        _rightBorder = BoundingBoxValue;
                    else
                        _rightBorder = BaseLineValue;
                }
                else
                    _rightBorder = -1;

                return _rightBorder;
            }
        }

        private int _boundingBoxWidth;
        public int BoundingBoxWidth
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _boundingBoxWidth = RightBorder - LeftBorder;
                else
                    _boundingBoxWidth = -1;
                return _boundingBoxWidth;
            }
        }

        private int _width;
        public int Width
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _width = RightBorder - LeftBorder;
                else
                    _width = -1;
                return _width;
            }
        }

        private int _topBorder;
        public int TopBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _topBorder = clsTrLibrary.GetTopYcoord(CoordsString);
                else
                    _topBorder = -1;
                return _topBorder;
            }
        }

        private int _bottomBorder;
        public int BottomBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _bottomBorder = clsTrLibrary.GetBottomYcoord(CoordsString);
                else
                    _bottomBorder = -1;
                return _bottomBorder;
            }
        }

        private int _boundingBoxHeight;
        public int BoundingBoxHeight
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _boundingBoxHeight = BottomBorder - TopBorder;
                else
                    _boundingBoxHeight = -1;
                return _boundingBoxHeight;
            }
        }

        private int _height;
        public int Height
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _height = BottomBorder - TopBorder;
                else
                    _height = -1;
                return _height;
            }
        }

        private int _hPos;
        public int Hpos
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _hPos = LeftBorder;
                else
                    _hPos = -1;
                return _hPos;
            }
        }

        private int _vPos = 0;
        public int Vpos
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    // først testes, om Vpos er blevet manuelt sat (dvs. forskellig fra 0); i givet fald returneres den blot; ellers beregnes den først
                    if (_vPos == 0)
                        _vPos = clsTrLibrary.GetAverageYcoord(BaseLineCoordsString);
                }
                else
                    _vPos = -1;
                return _vPos;
            }
            set
            {
                // Vpos bør KUN SÆTTES ifm. logisk arrangerering af linierne (kaldes af clsTrTextLines.RenumberLogically)
                _vPos = value;
            }
        }

        private int _textSizeFactor;
        public int TextSizeFactor
        {
            get
            {
                _textSizeFactor = Width / (Length);
                return _textSizeFactor;
            }
        }


        public clsTrTextLine GetTextLineAbove()
        {
            int Current = this.Number - 1;
            clsTrTextLine LineAbove = null;

            // Debug.Print($"clsTrTextLine: GetTextLineAbove: Current = {Current}, TextLineCount: {ParentRegion.TextLines.Count}");
            int Middle = this.Hpos + (this.HendPos - this.Hpos) / 2;

            for (int i = Current - 1; i >= 1; i--)
            {
                if (Middle > ParentRegion.TextLines[i].Hpos && Middle < ParentRegion.TextLines[i].HendPos)
                {
                    LineAbove = ParentRegion.TextLines[i];
                    break;
                }
            }
            return LineAbove;
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

        private int _logicalOrder;
        public int LogicalOrder
        {
            get
            {
                _logicalOrder = ((Vpos + SortDeltaY) / 100) * 1_000_000 + Hpos + AngleFromOrigo; // 
                return _logicalOrder;
            }
        }

        private int _sortDeltaY = 0;
        public int SortDeltaY
        {
            get
            {
                return _sortDeltaY;
            }
            set
            {
                if (value != _sortDeltaY)
                    _sortDeltaY = value;
            }
        }

        private int _angleFromOrigo = 0;
        public int AngleFromOrigo
        {
            // returnerer vinklen fra sidens øverste venstre hjørne til beg.punkt
            //
            get
            {
                if (Vpos > 0)
                {
                    double Ratio = (double)Hpos / (double)Vpos;
                    double Angle = System.Math.Atan(Ratio) * (180 / Math.PI);
                    _angleFromOrigo = Convert.ToInt32(Angle);

                }
                return _angleFromOrigo;
            }
        }

        private int _hEndPos;
        public int HendPos
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _hEndPos = RightBorder;
                else
                    _hEndPos = -1;
                return _hEndPos;
            }
        }

        private double _percentualVpos;
        public double PercentualVpos
        {
            get
            {
                int PageHeight = ParentRegion.ParentTranscript.ParentPage.Height;
                _percentualVpos = (double)Vpos / (double)PageHeight * 100.0;
                // Debug.WriteLine($"Pageheigth = {PageHeight}, Vpos = {Vpos}, PercentualVpos = {_percentualVpos}");
                return _percentualVpos;
            }
        }

        private double _percentualHpos;
        public double PercentualHpos
        {
            get
            {
                int PageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
                _percentualHpos = (double)Hpos / (double)PageWidth * 100.0;
                // Debug.WriteLine($"Pagewidth = {PageWidth}, Hpos = {Hpos}, PercentualHpos = {_percentualHpos}");
                return _percentualHpos;
            }
        }

        private double _percentualHendPos;
        public double PercentualHendPos
        {
            get
            {
                int PageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
                _percentualHendPos = (double)HendPos / (double)PageWidth * 100.0;
                return _percentualHendPos;
            }
        }
               
        public bool IsOKwithBaseLineFilter(clsTrBaseLineFilter CurrentFilter)
        {
            bool temp;

            bool PositiveCheck;
            bool StraightCheck;
            bool DirectionCheck;

            PositiveCheck = (IsCoordinatesPositive == CurrentFilter.CoordinatesPositive);
            StraightCheck = (IsBaseLineStraight == CurrentFilter.BaseLineStraight);
            DirectionCheck = (IsBaseLineDirectionOK == CurrentFilter.BaseLineDirectionOK);

            temp = PositiveCheck && StraightCheck && DirectionCheck;

            return temp;
        }


        public bool MeetsFilterConditions(clsTrLineFilterSettings CurrentFilter)
        {
            bool temp;

            // sætter alt til true - og hvis de enkelte dele rent faktisk skal testes, kan de blive false
            bool CheckPageNumber = true;
            bool CheckRegEx = true;
            bool CheckStructuralTag = true;
            bool CheckTextSizeFactor = true;
            bool CheckTextLength = true;
            bool CheckPosition = true;

            if (CurrentFilter.FilterByPageNumber)
            {
                CheckPageNumber = (ParentPageNr >= CurrentFilter.StartPage) && (ParentPageNr <= CurrentFilter.EndPage);
            }

            if (CurrentFilter.FilterByRegEx)
            {
                CheckRegEx = MatchesRegex(CurrentFilter.RegExPattern);
            }
            
            if (CurrentFilter.FilterByStructuralTag)
            {
                if (CurrentFilter.StructuralTag != "")
                    CheckStructuralTag = HasSpecificStructuralTag(CurrentFilter.StructuralTag);
            }

            if (CurrentFilter.FilterByTextSizeFactor)
            {
                CheckTextSizeFactor = MatchesTextSize(CurrentFilter);
            }

            if (CurrentFilter.FilterByTextLength)
            {
                CheckTextLength = MatchesTextLength(CurrentFilter);
            }

            if (CurrentFilter.FilterByPosition)
            {
                CheckPosition = MatchesWindow(CurrentFilter);
            }

            temp = CheckPageNumber && CheckRegEx && CheckStructuralTag && CheckTextSizeFactor && CheckTextLength && CheckPosition;

            return temp;
        }



        public bool MatchesTextSize(clsTrLineFilterSettings FilterSettings)
        {
            bool temp;

            bool LowerOK;
            bool UpperOK;
            
            if (FilterSettings.LowerLimitTextSizeFactor == 0)
            {
                LowerOK = true;
            }
            else
            {
                LowerOK = TextSizeFactor >= FilterSettings.LowerLimitTextSizeFactor;
            }

            if (FilterSettings.UpperLimitTextSizeFactor == 0)
            {
                UpperOK = true;
            }
            else
            {
                UpperOK = TextSizeFactor <= FilterSettings.UpperLimitTextSizeFactor;
            }

            temp = LowerOK && UpperOK;

            return temp;
        }

        public bool MatchesTextLength(clsTrLineFilterSettings FilterSettings)
        {
            bool temp;

            bool LowerOK;
            bool UpperOK;

            if (FilterSettings.LowerLimitTextLength == 0)
            {
                LowerOK = true;
            }
            else
            {
                LowerOK = Length >= FilterSettings.LowerLimitTextLength;
            }

            if (FilterSettings.UpperLimitTextLength == 0)
            {
                UpperOK = true;
            }
            else
            {
                UpperOK = Length <= FilterSettings.UpperLimitTextLength;
            }

            temp = LowerOK && UpperOK;

            return temp;
        }


        public bool MatchesWindow(clsTrLineFilterSettings FilterSettings)
        {
            bool temp;


            if (FilterSettings.Inside)
            {
                temp = IsInWindow(FilterSettings);
            }
            else
            {
                temp = !IsInWindow(FilterSettings);
            }

            return temp;
        }


        public bool IsInWindow(clsTrLineFilterSettings FilterSettings)
        {
            bool temp;

            bool Vcheck;
            bool Hcheck;
            bool HendCheck;

            Vcheck = (PercentualVpos >= FilterSettings.TopBorder) && (PercentualVpos <= FilterSettings.BottomBorder);
            Hcheck = (PercentualHpos >= FilterSettings.LeftBorder) && (PercentualHpos <= FilterSettings.RightBorder);


            HendCheck = (PercentualHendPos >= FilterSettings.LeftBorder) && (PercentualHendPos <= FilterSettings.RightBorder);

            temp = Vcheck && Hcheck;

            if (FilterSettings.IncludeEnding)
                temp = temp && HendCheck;

            //Debug.WriteLine($"PVpos = {PercentualVpos}, Top = {CurrentWindow.TopBorder}, Bottom = {CurrentWindow.BottomBorder}, " +
            //    $"PHpos = {PercentualHpos}, Left = {CurrentWindow.LeftBorder}, Right = {CurrentWindow.RightBorder}, OK? {temp}");

            return temp;

        }
        
        public bool MatchesRegex(string RegExPattern) // Regex MatchPattern
        {
            bool temp = false;

            Regex MatchPattern = new Regex(RegExPattern);
            MatchCollection Matches = MatchPattern.Matches(ExpandedText);
            if (Matches.Count > 0)
                temp = true;
            else
                temp = false;

            return temp;
        }

        private bool _hasAbbrevTag;
        public bool HasAbbrevTag
        {
            get
            {
                _hasAbbrevTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrAbbrevTag))
                        _hasAbbrevTag = _hasAbbrevTag || true;
                return _hasAbbrevTag;
            }
        }

        private bool _hasEmptyAbbrevTag;
        public bool HasEmptyAbbrevTag
        {
            get
            {
                _hasEmptyAbbrevTag = false;
                if (HasAbbrevTag)
                {
                    foreach (clsTrTag T in Tags)
                        if (T.GetType() == typeof(clsTrAbbrevTag))
                            _hasEmptyAbbrevTag = _hasEmptyAbbrevTag || (T as clsTrAbbrevTag).IsEmpty;
                }
                return _hasEmptyAbbrevTag;
            }
        }


        private bool _hasSicTag;
        public bool HasSicTag
        {
            get
            {
                _hasSicTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrSicTag))
                        _hasSicTag = _hasSicTag || true;
                return _hasSicTag;
            }
        }

        private bool _hasCommentTag;
        public bool HasCommentTag
        {
            get
            {
                _hasCommentTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrCommentTag))
                        _hasCommentTag = _hasCommentTag || true;
                return _hasCommentTag;
            }
        }


        private bool _hasUnclearTag;
        public bool HasUnclearTag
        {
            get
            {
                _hasUnclearTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrUnclearTag))
                        _hasUnclearTag = _hasUnclearTag || true;
                return _hasUnclearTag;
            }
        }


        private bool _hasDateTag;
        public bool HasDateTag
        {
            get
            {
                _hasDateTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrDateTag))
                        _hasDateTag = _hasDateTag || true;
                return _hasDateTag;
            }
        }


        private bool _hasStrikethroughTag;
        public bool HasStrikethroughTag
        {
            get
            {
                _hasStrikethroughTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrStyleTag))
                    {
                        if ((T as clsTrStyleTag).Strikethrough)
                            _hasStrikethroughTag = _hasStrikethroughTag || true;
                    }
                return _hasStrikethroughTag;
            }
        }

        private bool _hasSuperscriptTag;
        public bool HasSuperscriptTag
        {
            get
            {
                _hasSuperscriptTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrStyleTag))
                    {
                        if ((T as clsTrStyleTag).Superscript)
                            _hasSuperscriptTag = _hasSuperscriptTag || true;
                    }
                return _hasSuperscriptTag;
            }
        }

        private bool _hasSubscriptTag;
        public bool HasSubscriptTag
        {
            get
            {
                _hasSubscriptTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrStyleTag))
                    {
                        if ((T as clsTrStyleTag).Subscript)
                            _hasSubscriptTag = _hasSubscriptTag || true;
                    }
                return _hasSubscriptTag;
            }
        }

        private bool _hasRomanNumeralTag;
        public bool HasRomanNumeralTag
        {
            get
            {
                _hasRomanNumeralTag = false;
                foreach (clsTrTag T in Tags)
                    if (T.GetType() == typeof(clsTrRomanNumeralTag))
                        _hasRomanNumeralTag = _hasRomanNumeralTag || true;
                return _hasRomanNumeralTag;
            }
        }


        private bool _hasFullStrikethroughTag;
        public bool HasFullStrikethroughTag
        {
            get
            {
                _hasFullStrikethroughTag = false;
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrTextualTag))
                    {
                        _hasFullStrikethroughTag = ((T as clsTrTextualTag).IsStrikethrough && (T as clsTrTextualTag).IsFull) || _hasFullStrikethroughTag;
                    }
                }
                return _hasFullStrikethroughTag;
            }
        }


        private bool _hasFullDateTag;
        public bool HasFullDateTag
        {
            get
            {
                _hasFullDateTag = false;
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrTextualTag))
                    {
                        _hasFullDateTag = ((T as clsTrTextualTag).IsDate && (T as clsTrTextualTag).IsFull) || _hasFullDateTag;
                    }
                }
                return _hasFullDateTag;
            }
        }


        public bool Contains(string SearchFor)
        {
            return TextEquiv.Contains(SearchFor);
        }

        private string _quickExpandedText;
        public string QuickExpandedText
        {
            get
            {
                _quickExpandedText = GetExpandedText(true, true);
                return _quickExpandedText;
            }
        }


        private string _expandedText;
        public string ExpandedText
        {
            get
            {
                _expandedText = GetExpandedText(true, true);
                return _expandedText;
            }
        }
               
        private string GetExpandedText(bool Refine, bool ConvertOtrema)
        {
            //Debug.Print($"GetExpText: Page: {ParentPageNr} TL # {Number}");

            string temp = TextEquiv;
            int Offset;
            int TagLength;

            int CurrentDelta = 0;
            string OldContent = "";
            string NewContent = "";

            // --- 1 --- konverter roman numbers -------------------------------------------------
            if (HasRomanNumeralTag)
            {
                //Debug.Print($"GetExpText: Roman numbers temp = _{temp}_");
                for (int i = Tags.Count - 1; i >= 0; i--)
                {
                    clsTrTag TestTag = Tags[i];
                    if (TestTag.GetType() == typeof(clsTrRomanNumeralTag))
                    {
                        clsTrTag R = (TestTag as clsTrRomanNumeralTag);
                        Offset = (R as clsTrTextualTag).Offset;
                        TagLength = (R as clsTrTextualTag).Length;

                        OldContent = TextEquiv.Substring(Offset, TagLength);
                        NewContent = (R as clsTrRomanNumeralTag).ArabicEquivalent.ToString();

                        if (TagLength == this.Length)
                        {
                            temp = NewContent;
                        }
                        else if (Offset >= 0 && TagLength < this.Length)
                        {   
                            temp = temp.Remove(Offset, TagLength);
                            if (NewContent != "" && Offset < this.Length)
                                temp = temp.Insert(Offset, NewContent);
                        }
                        // CurrentDelta sættes; den indeholder forskellen i længde på gammelt og nyt indhold
                        CurrentDelta = NewContent.Length - OldContent.Length; // tidl. - taglength
                        // Nu er problemet så, at tags længere ude i strengen har forkerte Offset og Length-værdier
                        // Det ordnes ved at gennemløbe disse og sætte deres Delta-værdi
                        //Debug.Print($"GetExpText: Roman numbers - currentdelta: {CurrentDelta}");

                        if (CurrentDelta != 0)
                            Tags.Move(Offset, CurrentDelta, false);
                    }
                }
            }

            // --- 2 --- konverterer superscript --------------------------------------------------------
            if (HasSuperscriptTag)
            {
                //Debug.Print($"GetExpText: Superscripts: temp = _{temp}_");
                clsTrTags SuperscriptTags = new clsTrTags();
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrStyleTag))
                        if ((T as clsTrStyleTag).Superscript)
                            SuperscriptTags.Add(T);
                }

                int SuperscriptCount = SuperscriptTags.Count;

                string OldNumbers;
                string NewNumbers = "";
                char CurrentChar;
                char NewChar = ' ';
                Offset = 0;
                TagLength = 0;

                for (int i = 0; i < SuperscriptCount ; i++)
                {
                    clsTrTag S = SuperscriptTags[i];
                    Offset = (S as clsTrTextualTag).Offset;
                    TagLength = (S as clsTrTextualTag).Length;
                    OldNumbers = temp.Substring(Offset, TagLength);

                    if (OldNumbers.Length > 0)
                    {
                        for (int k = 0; k < OldNumbers.Length; k++)
                        {
                            CurrentChar = OldNumbers[k];
                            if (char.IsNumber(CurrentChar))
                            {
                                NewChar = clsTrLibrary.GetSuperscriptChar(CurrentChar);
                            }
                            else
                            {
                                NewChar = CurrentChar;
                            }
                            NewNumbers = NewNumbers + NewChar.ToString();
                        }

                        // erstat
                        temp = temp.Remove(Offset, TagLength);
                        temp = temp.Insert(Offset, NewNumbers);
                    }
                }
            }

            // --- 3 --- ordinære tags! -------------------------------------------------------------------
            // henter alle Tags, som er af disse relevante typer 
            if (HasAbbrevTag || HasSicTag || HasUnclearTag || HasStrikethroughTag || HasCommentTag || HasDateTag)
            {
                OldContent = "";
                clsTrTags TextualTags = new clsTrTags();
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrSicTag) || T.GetType() == typeof(clsTrAbbrevTag) || T.GetType() == typeof(clsTrUnclearTag) || T.GetType() == typeof(clsTrCommentTag) || T.GetType() == typeof(clsTrDateTag))
                    {
                        //Debug.Print($"GetExpText: Fundet Sic/Abbrev/Date/Unclear temp = _{temp}_");
                        TextualTags.Add(T);
                        T.ParentLine = this;
                    }
                    else if (T.GetType() == typeof(clsTrStyleTag))
                    {
                        // det eneste styletag, som er relevant, er Strikethrough
                        if ((T as clsTrStyleTag).Strikethrough)
                        {
                            // Debug.Print($"GetExpText: Fundet Strike");
                            TextualTags.Add(T);
                            T.ParentLine = this;
                        }
                    }
                }
                int Max = TextualTags.Count;

                // TextualTags indeholder nu kun de tags, der skal bruges til at ændre teksten

                // --- 3.1. --- først testes, om der findes et FULL StrikethroughTag
                if (HasFullStrikethroughTag)
                {
                    //Debug.Print($"GetExpText: Has Full Strike");
                    // i givet fald er hele teksten streget ud; der skal returneres en tom streng
                    OldContent = temp;
                    temp = "";

                }
                // --- 3.2. --- dernæst testes, om der findes et FULL DateTag
                else if (HasFullDateTag)
                {
                    //Debug.Print($"GetExpText: Has Full Date");
                    // i givet fald skal det pågældende datetag findes; dette returneres alene
                    foreach (clsTrTag T in TextualTags)
                    {
                        if (T.GetType() == typeof(clsTrDateTag))
                        {
                            OldContent = temp;
                            temp = (T as clsTrDateTag).ExpandedDate;
                        }
                    }
                }
                // --- 3.3. --- hvis hverken full strikethrough eller full date, skal alle gennemgås:
                else
                {
                    //Debug.Print($"GetExpText: Std. tag procedure");
                    // de fundne tags sorteres (efter SortKey)
                    TextualTags.Sort();

                    NewContent = "";
                    OldContent = "";
                    Offset = 0;
                    TagLength = 0;
                    CurrentDelta = 0;

                    // kører BAGLÆNS for ikke at få problemer med de øvrige tags undervejs 

                    // 1. gennemløb: ALLE PÅNÆR DATE
                    for (int i = Max - 1; i >= 0; i--)
                    {
                        //Debug.Print($"GetExpText: 1. gennemløb");
                        clsTrTag T = TextualTags[i];

                        if (!(T as clsTrTextualTag).IsDate)
                        {
                            //Debug.Print($"GetExpText: is NOT date");
                            Offset = (T as clsTrTextualTag).Offset;
                            TagLength = (T as clsTrTextualTag).Length;
                            //Debug.Print($"Ikke date: Number: {Number}, Type: {T.Type}, Offset: {Offset}, Length: {TagLength}, Text: {TextEquiv}");

                            if (TagLength > temp.Length || Offset > temp.Length)
                            //if (TagLength > TextEquiv.Length || Offset > TextEquiv.Length)
                            {
                                Debug.Print($"FATAL ERROR: Offset: {Offset}, Taglength: {TagLength}, Textequiv.Length: {TextEquiv.Length}");
                            }
                            else
                            {
                                if (TagLength + Offset > temp.Length)
                                    Debug.Print($"FATAL ERROR: Offset: {Offset}, Taglength: {TagLength}, Textequiv.Length: {TextEquiv.Length}");
                                else
                                    OldContent = temp.Substring(Offset, TagLength); // rettet fra textequiv.substring
                            }

                            // hvis det enkelte tag har et overlappende datetag, erstattes teksten med dette datetags expansion
                            if ((T as clsTrTextualTag).HasOverlappingDateTag)
                            {
                                //Debug.Print($"GetExpText: has overlap date");
                                NewContent = (T as clsTrTextualTag).GetOverlappingDateTag().ExpandedDate;
                                // det pågæld. datetag markeres som "resolved", så det ikke bliver benyttet nedenunder (i 2. gennemløb)
                                (T as clsTrTextualTag).GetOverlappingDateTag().Resolved = true;
                            }
                            // men hvis det har et overlappende, behandles det helt normalt
                            else
                            {
                                //Debug.Print($"GetExpText: has NOT overlap date");
                                // afhængigt af type, sættes NewContent

                                if (T.GetType() == typeof(clsTrSicTag))
                                    NewContent = (T as clsTrSicTag).Correction;
                                else if (T.GetType() == typeof(clsTrAbbrevTag))
                                    NewContent = (T as clsTrAbbrevTag).Expansion;
                                else if (T.GetType() == typeof(clsTrStyleTag))
                                    NewContent = ""; // jo, det kan vi godt tillade os, for det eneste styletag, der er med nu, er strikethrough!
                                else if (T.GetType() == typeof(clsTrUnclearTag))
                                {
                                    if (!(T as clsTrUnclearTag).IsEmpty)
                                    {
                                        NewContent = OldContent + " [alt.: " + (T as clsTrUnclearTag).Alternative;
                                        if ((T as clsTrUnclearTag).Reason != "")
                                        {
                                            NewContent = NewContent + " (" + (T as clsTrUnclearTag).Reason + ")";
                                        }
                                        NewContent = NewContent + "] ";
                                    }
                                    else
                                    {
                                        NewContent = OldContent;
                                    }
                                }
                                else if (T.GetType() == typeof(clsTrCommentTag))
                                {
                                    if (!(T as clsTrCommentTag).IsEmpty)
                                    {
                                        NewContent = OldContent + " [note: " + (T as clsTrCommentTag).Comment + "] ";
                                    }
                                }
                            }

                            // hvis det aktuelle IKKE er et Datetag, kan man herefter  
                            // fjerne oprindelig tekst og erstatte med NewContent
                            //Debug.Print($"GetExpText: udfører skift");
                            if (TagLength == this.Length)
                            {
                                temp = NewContent;
                            }
                            else if (Offset >= 0 && TagLength < this.Length)
                            {
                                if (TagLength + Offset > temp.Length)
                                    Debug.Print($"FATAL ERROR: Offset: {Offset}, Taglength: {TagLength}, Textequiv.Length: {TextEquiv.Length}");
                                else
                                {
                                    temp = temp.Remove(Offset, TagLength);
                                    if (NewContent != "" && Offset < this.Length)
                                        temp = temp.Insert(Offset, NewContent);
                                }

                            }


                            // CurrentDelta sættes; den indeholder forskellen i længde på gammelt og nyt indhold
                            CurrentDelta = NewContent.Length - OldContent.Length; // tidl. - taglength
                            //Debug.Print($"GetExpText: Std tag - currentdelta: {CurrentDelta}");
                            //Debug.WriteLine($"GetExpText #1 - Type: {T.Type}, Offset: {Offset}, TagLength: {TagLength}, TextLength: {this.Length}, TempLength: {temp.Length}, OldContent: {OldContent}, NewContent: {NewContent}");

                            // Nu er problemet så, at tags længere ude i strengen har forkerte Offset og Length-værdier
                            // Det ordnes ved at gennemløbe disse og sætte deres Delta-værdi
                            if (CurrentDelta != 0)
                                Tags.Move(Offset, CurrentDelta, false);
                            
                        }

                    }

                    // --- 4 --- KUN DATE! -------------------------------------------------------------------
                    // i eget (andet) gennemløb: 

                    for (int i = Max - 1; i >= 0; i--)
			        {
                        //Debug.Print($"GetExpText: 2. gennemløb");
                        clsTrTag T = TextualTags[i];

                        if ((T as clsTrTextualTag).IsDate)
				        {
                            //Debug.Print($"GetExpText: is date (should be) temp = _{temp}_");
                            Offset = (T as clsTrTextualTag).Offset;
                            TagLength = (T as clsTrTextualTag).Length;

                            // kun de datetags, som ikke allerede i 1. gennemløb er fixet, skal ordnes
                            if (!(T as clsTrDateTag).Resolved)
					        {
                                //Debug.Print($"GetExpText: udfører skift");
                                NewContent = (T as clsTrDateTag).ExpandedDate;
						
                                if (TagLength == this.Length)
                                {
                                    temp = NewContent;
                                }
                                else if (Offset >= 0 && TagLength < this.Length)
                                {
                                    temp = temp.Remove(Offset, TagLength);
                                    if (NewContent != "" && Offset < this.Length)
                                        temp = temp.Insert(Offset, NewContent);
                                }

                            }
                        }
                        //Debug.WriteLine($"GetExpText #2 - Type: {T.Type}, Offset: {Offset}, TagLength: {TagLength}, TextLength: {this.Length}, TempLength: {temp.Length}, OldContent: {OldContent}, NewContent: {NewContent}");
                    }



                }
            }

            // for ikke at forplumre næste kald til GetExp... skal alle Deltaværdier på alle Tags nulstilles!
            foreach (clsTrTag T in Tags)
            {
                T.DeltaOffset = 0;
                T.DeltaLength = 0;
            }

            // uanset øvrigt udfald, skal temp-strengen trimmes og evt. raffineres.
            temp = temp.Trim();

	        if (Refine)
                temp = clsTrLibrary.RefineText(temp, ConvertOtrema);

            //Debug.Print($"RESULT ------------------------------------------------ {temp}");

	        return temp;
        }


        private bool TernaryXor(bool a, bool b, bool c)
        {
            //return ((a && !b && !c) || (!a && b && !c) || (!a && !b && c));

            // taking into account Jim Mischel's comment, a faster solution would be:
            return (!a && (b ^ c)) || (a && !(b || c));
        }


        //private string GetExpandedText(bool Refine, bool ConvertOtrema)
        //{
        //    string temp = TextEquiv;        // Regex.Unescape??
        //    // Debug.Print($"TextEquiv: {TextEquiv}, Temp: {temp}");

        //    if (HasAbbrevTag || HasSicTag || HasStrikeThroughTag || HasDateTag)
        //    {
        //        // NB: Den kan p.t. IKKE håndtere, hvis der er FLERE af denne slags tags i linien - så springer vi den over! Derfor denne sære linie:
        //        // if (TernaryXor(HasAbbrevTag, HasSicTag, HasStrikeThroughTag))
        //        {
        //            clsTrTags TextualTags = new clsTrTags();

        //            foreach (clsTrTag T in Tags)
        //            {
        //                // hvis det er sic eller abbrev, skal der helt klart hentes noget andet frem
        //                if (T.GetType() == typeof(clsTrSicTag) || T.GetType() == typeof(clsTrAbbrevTag))
        //                {
        //                    TextualTags.Add(T);
        //                    // Debug.Print($"Tag: {(T as clsTrTextualTag).Type}, {(T as clsTrTextualTag).Offset}, {(T as clsTrTextualTag).Length}, {(T as clsTrTextualTag).Content}");
        //                }
        //                // men hvis det er gennemstreget tekst, skal der slettes tekst
        //                else if (T.GetType() == typeof(clsTrStyleTag))
        //                {
        //                    // Debug.Print($"Found Styletag!!!");
        //                    if ((T as clsTrStyleTag).Strikethrough)
        //                    {
        //                        // Debug.Print($"Adding strikethrough tag: Offset = {(T as clsTrStyleTag).Offset}");
        //                        TextualTags.Add(T);
        //                    }
        //                }
        //                else if (T.GetType() == typeof(clsTrDateTag))
        //                {
        //                    TextualTags.Add(T);
        //                }
        //            }


        //            TextualTags.Sort();

        //            string NewContent = "";
        //            int Max = TextualTags.Count;
        //            int Offset;
        //            int TagLength;


        //            // kører BAGLÆNS for ikke at få problemer med de øvrige tags undervejs (offset og length ændres ikke på de andre - med mindre de OVERLAPPER)

        //            // 1. gennemløb: KUN tags, som IKKE har fuld længde - øh nej, kun hvis ikke date
        //            for (int i = Max - 1; i >= 0; i--)
        //            {
        //                clsTrTag T = TextualTags[i];
        //                Offset = (T as clsTrTextualTag).Offset;
        //                TagLength = (T as clsTrTextualTag).Length;

        //                //if (TagLength < this.Length)
        //                {
        //                    if (T.GetType() == typeof(clsTrSicTag))
        //                        NewContent = (T as clsTrSicTag).Correction;
        //                    else if (T.GetType() == typeof(clsTrAbbrevTag))
        //                        NewContent = (T as clsTrAbbrevTag).Expansion;
        //                    else if (T.GetType() == typeof(clsTrStyleTag))
        //                        NewContent = ""; // for så er det strikethrough, dvs. der skal slettes.
        //                    else if (T.GetType() == typeof(clsTrDateTag))
        //                        NewContent = (T as clsTrDateTag).ExpandedDate;

        //                    //Debug.WriteLine($"GetExpText ** Type: {T.Type}, Offset: {Offset}, TagLength: {TagLength}, TextLength: {this.Length}, TempLength: {temp.Length}, OldContent: {TextEquiv.Substring(Offset, TagLength)}, NewContent: {NewContent}");

        //                    // if (NewContent != null)
        //                    {
        //                        // i 1. gennemløb bruger vi TAGGETs Offset og Length
        //                        // men kun hvis TagLength er mindre end temp-length (HER ER NOGET GALT)
        //                        if (TagLength <= temp.Length)
        //                        {
        //                            // hvis dato skal det helt udskiftes
        //                            if (T.GetType() == typeof(clsTrDateTag))
        //                            {
        //                                temp = NewContent;
                                            
        //                            }
        //                            else
        //                            {
        //                                if (!HasDateTag)
        //                                {
        //                                    temp = temp.Remove(Offset, TagLength);
        //                                    temp = temp.Insert(Offset, NewContent);
        //                                }
        //                            }

        //                        }
        //                        else
        //                        {
        //                            Debug.WriteLine($"TagLength er for stor!!! Type: {T.Type}, Offset: {Offset}, Length: {Length}, Content: {NewContent}");
        //                        }
        //                    }
        //                }
        //            }

        //            //// 2.gennemløb: KUN tags, som HAR fuld længde - øh nej, kun date
        //            //for (int i = Max - 1; i >= 0; i--)
        //            //{
        //            //    clsTrTag T = TextualTags[i];
        //            //    Offset = (T as clsTrTextualTag).Offset;
        //            //    TagLength = (T as clsTrTextualTag).Length;

        //            //    //if (Offset == 0 && TagLength == this.Length)
        //            //    {
        //            //        //if (T.GetType() == typeof(clsTrSicTag))
        //            //        //    NewContent = (T as clsTrSicTag).Correction;
        //            //        //else if (T.GetType() == typeof(clsTrAbbrevTag))
        //            //        //    NewContent = (T as clsTrAbbrevTag).Expansion;
        //            //        //else if (T.GetType() == typeof(clsTrStyleTag))
        //            //        //    NewContent = ""; // for så er det strikethrough, dvs. der skal slettes.
        //            //        //else 
        //            //        if (T.GetType() == typeof(clsTrDateTag))
        //            //            NewContent = (T as clsTrDateTag).ExpandedDate;

        //            //        // Debug.WriteLine($"Type: {T.Type}, Offset: {Offset}, Length: {Length}, Content: {NewContent}");

        //            //        // if (NewContent != null)
        //            //        {
        //            //            // i 2. gennemløb er det HELE strengen, der skal ændres til NewContent.
        //            //            //temp = NewContent;
        //            //        }
        //            //        if (TagLength <= temp.Length)
        //            //        {
        //            //            temp = temp.Remove(Offset, TagLength);
        //            //            temp = temp.Insert(Offset, NewContent);
        //            //        }
        //            //        else
        //            //        {
        //            //            Debug.WriteLine($"TagLength er for stor!!! Type: {T.Type}, Offset: {Offset}, Length: {Length}, Content: {NewContent}");
        //            //        }

        //            //    }
        //            //}

        //        }
        //    }
        //    temp = temp.Trim();

        //    if (Refine)
        //        temp = clsTrLibrary.RefineText(temp, ConvertOtrema);

        //    return temp;
        //}
               
        // genbrugte

        public contentType ContentType = contentType.undefined;

        // public string[] TagStrings;
               
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        
        // endnu nyere constructor til indlæsning af Xdoc
        public clsTrTextLine(string tID, string tTags, string tLineCoords, string tBaseLineCoords, string tTextEquiv)
        {
            ID = tID;
            TagString = tTags;
            ReadingOrder = clsTrLibrary.GetReadingOrder(TagString);
            CoordsString = tLineCoords;
            BaseLineCoordsString = tBaseLineCoords;
            TextEquiv = tTextEquiv;

            //Debug.WriteLine("#1!");

            Tags.ParentRegion = this.ParentRegion;
            Tags.ParentLine = this;
            //Debug.WriteLine("#2!") ;
            Tags.LoadFromCustomAttribute(tTags);
            //Debug.WriteLine("#3!");

            if (Tags.Count > 0)
            {
                foreach (clsTrTag Tag in Tags)
                {
                    if (Tag.GetType() == typeof(clsTrStructuralTag))
                    {
                        StructuralTag = (clsTrStructuralTag)Tag;
                    }
                }
            }
            // Debug.WriteLine("Textline created!");
        }

        public List<string> GetTextualTags()
        {
            List<string> temp = new List<string>(); 
            StringBuilder sb = new StringBuilder();

            if (HasTags)
            {
                foreach (clsTrTag T in Tags)
                {
                    sb.Clear();

                    if (T.GetType() == typeof(clsTrTextualTag) || T.GetType() == typeof(clsTrAbbrevTag) || T.GetType() == typeof(clsTrSicTag) || T.GetType() == typeof(clsTrUnclearTag) || T.GetType() == typeof(clsTrDateTag))
                    {
                        sb.Append("Type: ");
                        sb.Append(T.Type);

                        sb.Append(", Offset: ");
                        sb.Append((T as clsTrTextualTag).Offset);
                        sb.Append(", Length: ");
                        sb.Append((T as clsTrTextualTag).Length);
                    }

                    if (T.GetType() == typeof(clsTrAbbrevTag))
                    {
                        sb.Append(", Expansion: ");
                        sb.Append((T as clsTrAbbrevTag).Expansion);
                    }
                    else if (T.GetType() == typeof(clsTrSicTag))
                    {
                        sb.Append(", Correction: ");
                        sb.Append((T as clsTrSicTag).Correction);
                    }
                    else if (T.GetType() == typeof(clsTrUnclearTag))
                    {
                        sb.Append(", Alternative: ");
                        sb.Append((T as clsTrUnclearTag).Alternative);
                        sb.Append(", Reason: ");
                        sb.Append((T as clsTrUnclearTag).Reason);
                    }

                    if (T.GetType() == typeof(clsTrDateTag))
                    {
                        sb.Append(", Year: ");
                        sb.Append((T as clsTrDateTag).Year);
                        sb.Append(", Month: ");
                        sb.Append((T as clsTrDateTag).Month);
                        sb.Append(", Day: ");
                        sb.Append((T as clsTrDateTag).Day);
                    }

                    if (T.IsEmpty)
                        sb.Append(" (Empty)");
                    else
                        sb.Append(" (OK)");

                    temp.Add(sb.ToString());

                }
            }
            return temp;
        }

        public List<string> GetAllTags()
        {
            List<string> temp = new List<string>();
            StringBuilder sb = new StringBuilder();

            if (HasTags)
            {
                foreach (clsTrTag T in Tags)
                {
                    sb.Clear();
                    sb.Append("Type: ");
                    sb.Append(T.Type);
                    foreach (clsTrTagProperty TP in T.Properties)
                    {
                        sb.Append(", Name: ");
                        sb.Append(TP.Name);
                        sb.Append(", Value: ");
                        sb.Append(TP.Value);
                    }
                    temp.Add(sb.ToString());
                }
            }
            return temp;
        }

        private bool _hasBaseLine;
        public bool HasBaseLine
        {
            get
            {
                _hasBaseLine = (BaseLineCoordsString != "");
                return _hasBaseLine;
            }
        }

        private bool _hasCoords;
        public bool HasCoords
        {
            get
            {
                _hasCoords = (CoordsString != "");
                return _hasCoords;
            }
        }
        
        private bool _hasTags;
        public bool HasTags
        {
            get
            {
                _hasTags = (Tags.Count > 0);
                return _hasTags;
            }
        }

        private bool _hasStructuralTag;
        public bool HasStructuralTag
        {
            get
            {
                _hasStructuralTag = false;

                foreach (clsTrTag T in Tags)
                {
                    _hasStructuralTag = _hasStructuralTag || (T.GetType() == typeof(clsTrStructuralTag));
                }
                return _hasStructuralTag;

            }
        }
                     
        private string _structuralTagValue;
        public string StructuralTagValue
        {
            get
            {
                if (HasStructuralTag && StructuralTag != null)
                    _structuralTagValue = StructuralTag.SubType;
                else
                    _structuralTagValue = "";
                return _structuralTagValue;
            }
            set
            {
                if (_structuralTagValue != value)
                {
                    _structuralTagValue = value;
                    NotifyPropertyChanged("StructuralTagValue");
                }
            }
        }

        public bool HasSpecificStructuralTag(string TagName)
        {
            if (HasStructuralTag)
            {
                if (StructuralTagValue == TagName)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public void RenameStructuralTag(string OldName, string NewName)
        {
            if (HasStructuralTag)
            {
                if (StructuralTag.SubType == OldName)
                {
                    StructuralTag.SubType = NewName;
                    StructuralTagValue = NewName;
                    HasChanged = true;
                    NotifyPropertyChanged("StructuralTag");
                    NotifyPropertyChanged("StructuralValue");
                }
            }
        }

        public void AddStructuralTag(string TagName, bool OverWrite)
        {
            bool ProcessThis;
            if (OverWrite)
                ProcessThis = true;
            else
                ProcessThis = !HasStructuralTag;

            if (ProcessThis)
            {
                clsTrStructuralTag NewTag = new clsTrStructuralTag(TagName);
                Tags.Add(NewTag);
                StructuralTag = NewTag;
                StructuralTagValue = TagName;
                HasChanged = true;
                NotifyPropertyChanged("StructuralTag");
                NotifyPropertyChanged("StructuralValue");
                NotifyPropertyChanged("HasStructuralTag");
                NotifyPropertyChanged("HasTags");
            }
        }

        public void DeleteStructuralTag()
        {
            bool FoundTag = false;

            if (HasStructuralTag)
            {
                FoundTag = true;
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrStructuralTag))
                        T.MarkToDeletion = true;
                }
                StructuralTag = null;
                StructuralTagValue = "";
            }

            if (FoundTag)
            {
                for (int i = Tags.Count - 1; i >= 0; i--)
                {
                    if (Tags[i].MarkToDeletion)
                        Tags.RemoveAt(i);
                }
                HasChanged = true;
                NotifyPropertyChanged("StructuralTag");
                NotifyPropertyChanged("StructuralValue");
                NotifyPropertyChanged("HasStructuralTag");
                NotifyPropertyChanged("HasTags");
            }
        }
        
        public void AddAbbrevTag(int Offset, int Length, string Expansion)
        {
            // Debug.Print("Tags before: " + Tags.ToString());
            clsTrTextualTag NewTag = new clsTrAbbrevTag(Offset, Length, Expansion);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("HasAbbrevTag");
            NotifyPropertyChanged("ExpandedText"); 
            Debug.Print("Tag added! " + Tags.ToString());
        }

        public void AddSicTag(int Offset, int Length, string Correction)
        {
            clsTrTextualTag NewTag = new clsTrSicTag(Offset, Length, Correction);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("HasSicTag");
            NotifyPropertyChanged("ExpandedText");
        }

        public void AddUnclearTag(int Offset, int Length, string Alternative, string Reason)
        {
            clsTrTextualTag NewTag = new clsTrUnclearTag(Offset, Length, Alternative, Reason);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
        }

        public void AddCustomTextualTag(string TagName, int Offset, int Length)
        {
            clsTrTextualTag NewTag = new clsTrTextualTag(TagName, Offset, Length);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
        }

        public void AddDateTag(int Offset, int Length, DateTime Date)
        {
            clsTrDateTag NewTag = new clsTrDateTag(Offset, Length, Date);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            Debug.Print($"Datetag added as DATE: {NewTag.ExpandedDate}");
        }

        public void AddDateTag(int Offset, int Length, int Day, int Month, int Year)
        {
            clsTrDateTag NewTag = new clsTrDateTag(Offset, Length, Day, Month, Year);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            Debug.Print($"Datetag added as INTs: {NewTag.ExpandedDate}");
        }

        public void AddRomanNumeralTag(int Offset, int Length, string RomanValue)
        {
            clsTrRomanNumeralTag NewTag = new clsTrRomanNumeralTag(Offset, Length, RomanValue);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("ExpandedText");
            Debug.Print("Tag added! " + Tags.ToString());
        }

        public void AddStyleTag(int Offset, int Length, string Type)
        {
            clsTrStyleTag NewTag = new clsTrStyleTag(Offset, Length, Type);
            Tags.Add(NewTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("ExpandedText");
            Debug.Print("Tag added! " + Tags.ToString());
        }

        public void DeleteDateTags()
        {
            bool FoundTag = false;

            if (HasTags)
            {
                FoundTag = true;
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrDateTag))
                    {
                        T.MarkToDeletion = true;
                    }
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
                NotifyPropertyChanged("HasTags");
                NotifyPropertyChanged("ExpandedText");
            }

        }

        public void DeleteSicAndAbbrevTags()
        {
            bool FoundTag = false;

            if (HasTags)
            {
                FoundTag = true;
                foreach (clsTrTag T in Tags)
                {
                    if (T.GetType() == typeof(clsTrSicTag) || T.GetType() == typeof(clsTrAbbrevTag))
                    {
                            T.MarkToDeletion = true;
                    }
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
                NotifyPropertyChanged("HasTags");
                NotifyPropertyChanged("HasAbbrevTags");
                NotifyPropertyChanged("HasSicTags");
                NotifyPropertyChanged("ExpandedText");
            }
        }
        
        public void ExtendRight(int Amount)
        {
            // Debug.WriteLine($"clsTrTextLine : ExtendRight");

            if (HasCoords && HasBaseLine)
            {
                int NewX;

                clsTrCoords C = new clsTrCoords(BaseLineCoordsString);
                int RightMostX = C.GetRightMostXcoord();
                int RightMostY = C.GetRightMostYcoord();

                if (RightMostX + Amount < ParentRegion.ParentTranscript.ParentPage.Width)
                    NewX = RightMostX + Amount;
                else
                    NewX = ParentRegion.ParentTranscript.ParentPage.Width;

                clsTrCoord NewCoord = new clsTrCoord(NewX, RightMostY);
                C.Add(NewCoord);
                BaseLineCoordsString = C.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
            }
        }

        public void ExtendLeft(int Amount)
        {
            // Debug.WriteLine($"clsTrTextLine : ExtendLeft");

            if (HasCoords && HasBaseLine)
            {
                int NewX;

                clsTrCoords C = new clsTrCoords(BaseLineCoordsString);
                int LeftMostX = C.GetLeftMostXcoord();
                int LeftMostY = C.GetLeftMostYcoord();

                if (LeftMostX - Amount > 0)
                    NewX = LeftMostX - Amount;
                else
                    NewX = 0;

                clsTrCoord NewCoord = new clsTrCoord(NewX, LeftMostY);
                C.Add(NewCoord);
                C.Sort();
                BaseLineCoordsString = C.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
            }
        }

        public void Move(int Horizontally, int Vertically)
        {
            if (HasCoords && HasBaseLine)
            {
                // The line itself            
                clsTrCoords L = new clsTrCoords(CoordsString);
                foreach (clsTrCoord CurrentCoord in L)
                {
                    CurrentCoord.X = CurrentCoord.X + Horizontally;
                    CurrentCoord.Y = CurrentCoord.Y + Vertically;
                }
                CoordsString = L.ToString();

                // and then the baseline
                clsTrCoords C = new clsTrCoords(BaseLineCoordsString);
                C.Sort();
                foreach (clsTrCoord CurrentCoord in C)
                {
                    CurrentCoord.X = CurrentCoord.X + Horizontally;
                    CurrentCoord.Y = CurrentCoord.Y + Vertically;
                }
                BaseLineCoordsString = C.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");

            }
        }



        private bool _isCoordinatesPositive;
        public bool IsCoordinatesPositive
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _isCoordinatesPositive = clsTrLibrary.CheckBaseLineCoordinates(BaseLineCoordsString);
                else
                    _isCoordinatesPositive = false;
                return _isCoordinatesPositive;
            }
        }

        private bool _isBaseLineStraight;
        public bool IsBaseLineStraight
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _isBaseLineStraight = clsTrLibrary.CheckBaseLineStraightness(BaseLineCoordsString, MaxAllowedBaseLineAngle);
                else
                    _isBaseLineStraight = false;
                return _isBaseLineStraight;
            }
        }

        private bool _isBaseLineDirectionOK;
        public bool IsBaseLineDirectionOK
        {
            get
            {
                if (HasCoords && HasBaseLine)
                    _isBaseLineDirectionOK = clsTrLibrary.CheckBaseLineDirection(BaseLineCoordsString);
                else
                    _isBaseLineDirectionOK = false;
                return _isBaseLineDirectionOK;
            }
        }

        public void LimitCoordsToPage()
        {
            if (HasCoords && HasBaseLine)
            {
                clsTrCoords NewCoords = new clsTrCoords(BaseLineCoordsString);
                foreach (clsTrCoord C in NewCoords)
                {
                    if (C.X < 0)
                        C.X = 0;
                    if (C.X > ParentRegion.ParentTranscript.ParentPage.Width)
                        C.X = ParentRegion.ParentTranscript.ParentPage.Width;
                }
                BaseLineCoordsString = NewCoords.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
                NotifyPropertyChanged("IsCoordinatesPositive");
            }
        }

        public void FlattenBaseLine()
        {
            if (HasCoords && HasBaseLine)
            {
                int NewY = clsTrLibrary.GetAverageYcoord(BaseLineCoordsString);

                clsTrCoords NewCoords = new clsTrCoords(BaseLineCoordsString);
                foreach (clsTrCoord C in NewCoords)
                {
                    C.Y = NewY;
                }
                BaseLineCoordsString = NewCoords.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
                NotifyPropertyChanged("IsBaseLineStraight");
            }
        }


        public void FixDirection()
        {
            if (HasCoords && HasBaseLine)
            {
                clsTrCoords NewCoords = new clsTrCoords(BaseLineCoordsString);
                NewCoords.Sort();
                BaseLineCoordsString = NewCoords.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
                NotifyPropertyChanged("IsBaseLineDirectionOK");
            }
        }


        public void FixLineCoordinates()
        {
            if (HasCoords)
                CoordsString = FixCoordinates(CoordsString);
        }

        public void FixBaseLineCoordinates()
        {
            if (HasBaseLine)
                BaseLineCoordsString = FixCoordinates(BaseLineCoordsString);
        }

        private string FixCoordinates(string Coords)
        {
            int PageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
            int PageHeigth = ParentRegion.ParentTranscript.ParentPage.Height;

            clsTrCoords CurrentCoords = new clsTrCoords(Coords);

            string Temp = Coords;

            // ændrer punkter med negative koordinater eller større end siden
            foreach (clsTrCoord C in CurrentCoords)
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
                Temp = CurrentCoords.ToString();

            return Temp;
        }

        public void Trim()
        {
            if (!IsEmpty)
            {
                if (TextEquiv.EndsWith(" "))
                {
                    //Debug.Print($"Line ends with space");
                    if (TextEquiv.Length > 0)
                    {
                        int LastRealCharPosition = TextEquiv.Length - 1;
                        while (char.IsWhiteSpace(TextEquiv[LastRealCharPosition]))
                            LastRealCharPosition--;
                        string Temp = TextEquiv.Substring(0, LastRealCharPosition + 1);
                        int Difference = Temp.Length - TextEquiv.Length;
                        //Debug.Print($"Length: {TextEquiv.Length}, lastrealcharpos: {LastRealCharPosition}, difference: {Difference}");
                        if (Difference != 0)
                        {
                            Tags.Move(LastRealCharPosition + 1, Difference, true);
                            TextEquiv = Temp;
                            HasChanged = true;
                        }
                    }
                }

                if (TextEquiv.StartsWith(" "))
                {
                    //Debug.Print($"Line starts with space");
                    if (TextEquiv.Length > 0)
                    {
                        int FirstRealCharPosition = 0;
                        while (char.IsWhiteSpace(TextEquiv[FirstRealCharPosition]))
                            FirstRealCharPosition++;
                        string Temp = TextEquiv.Substring(FirstRealCharPosition);
                        int Difference = Temp.Length - TextEquiv.Length;
                        //Debug.Print($"Length: {TextEquiv.Length}, firstrealcharpos: {FirstRealCharPosition}, difference: {Difference}");
                        if (Difference != 0)
                        {
                            Tags.Move(FirstRealCharPosition - 1, Difference, true);
                            TextEquiv = Temp;
                            HasChanged = true;
                        }
                    }
                }
            }
        }

        public void SimplifyBoundingBox()
        {
            if (HasCoords && HasBaseLine)
                ExecuteSimplifyBoundingBox(TopBorder, BottomBorder, LeftBorder, RightBorder);
            else
                Debug.WriteLine($"ERROR: Doc: {ParentDocTitle}, Page: {ParentPageNr}, Line: {ParentRegionNr}-{Number} has corrupt format!");
        }

        public void SimplifyBoundingBox(int MinimumHeight, int MaximumHeight)
        {
            if (HasCoords && HasBaseLine)
            {
                int ActualHeight = BoundingBoxHeight;

                int TopDelta = (int)((double)MaximumHeight * 0.8);
                int BottomDelta = (int)((double)MaximumHeight * 0.2);

                int TopBorderValue;
                int BottomBorderValue;


                if (ActualHeight < MinimumHeight || ActualHeight > MaximumHeight)
                {
                    // firefemtedele over linien - enfemtedel under
                    TopBorderValue = Vpos - TopDelta;
                    BottomBorderValue = Vpos + BottomDelta;
                }
                else
                {
                    TopBorderValue = TopBorder;
                    BottomBorderValue = BottomBorder;
                }

                ExecuteSimplifyBoundingBox(TopBorderValue, BottomBorderValue, LeftBorder, RightBorder);
            }
            else
                Debug.WriteLine($"ERROR: Doc: {ParentDocTitle}, Page: {ParentPageNr}, Line: {ParentRegionNr}-{Number} has corrupt format!");

        }

        private void ExecuteSimplifyBoundingBox(int Top, int Bottom, int Left, int Right)
        {
            if (Top < 0)
                Top = 0;
            if (Left < 0)
                Left = 0;
            if (Bottom > ParentRegion.ParentTranscript.ParentPage.Height)
                Bottom = ParentRegion.ParentTranscript.ParentPage.Height;
            if (Right > ParentRegion.ParentTranscript.ParentPage.Width)
                Right = ParentRegion.ParentTranscript.ParentPage.Width;

            clsTrCoord LeftTop = new clsTrCoord(Left, Top);
            clsTrCoord LeftBottom = new clsTrCoord(Left, Bottom);
            clsTrCoord RightTop = new clsTrCoord(Right, Top);
            clsTrCoord RightBottom = new clsTrCoord(Right, Bottom);

            clsTrCoords NewCoords = new clsTrCoords();

            NewCoords.Add(LeftBottom);
            NewCoords.Add(RightBottom);
            NewCoords.Add(RightTop);
            NewCoords.Add(LeftTop);

            CoordsString = NewCoords.ToString();
            HasChanged = true;
            NotifyPropertyChanged("CoordsString");
            NotifyPropertyChanged("VisualLineArea");
        }

        public XElement ToXML()
        {
            // string CustomString = "readingOrder {index:" + ReadingOrder.ToString() + ";}";
            string CustomString = Tags.ToString();
            
            XElement xLine = new XElement(clsTrLibrary.xmlns + "TextLine",
                new XAttribute("id", ID),
                new XAttribute("custom", CustomString),
                new XElement(clsTrLibrary.xmlns + "Coords",
                    new XAttribute("points", CoordsString)),
                new XElement(clsTrLibrary.xmlns + "Baseline",
                    new XAttribute("points", BaseLineCoordsString)),
                new XElement(clsTrLibrary.xmlns + "TextEquiv",
                    new XElement(clsTrLibrary.xmlns + "Unicode", TextEquiv)));

            return xLine;
        }
               
        //public XElement ToRefinedXML()
        //{
        //    XElement XLine = new XElement("textLine",
        //        new XElement("pageNumber", ParentRegion.ParentTranscript.ParentPage.PageNr),
        //        new XElement("parentRegion", ParentRegion.Number),
        //        new XElement("lineNumber", Number),
        //        new XElement("lineID", ID),
        //        new XElement("contentType", ContentType.ToString()),
        //        new XElement("text",
        //            new XElement("raw", RawText),
        //            new XElement("refined", RefinedText)),
        //        new XElement("position",
        //            new XElement("horisontal", Hpos),
        //            new XElement("vertical", Vpos)),
        //        new XElement("borders",
        //            new XElement("left", LeftBorder),
        //            new XElement("right", RightBorder),
        //            new XElement("top", TopBorder),
        //            new XElement("bottom", BottomBorder)));
        //    // Debug.WriteLine(XLine.ToString());
        //    return XLine;
        //}
        
        public int CompareTo(object obj)
        {
            var line = obj as clsTrTextLine;
            switch (ParentContainer.SortMethod)
            {
                case clsTrTextLines.SortType.Vertically:
                    return VerticalOrder.CompareTo(line.VerticalOrder);
                case clsTrTextLines.SortType.Horizontally:
                    return HorizontalOrder.CompareTo(line.HorizontalOrder);
                case clsTrTextLines.SortType.Logically:
                    return LogicalOrder.CompareTo(line.LogicalOrder);
                default:
                    return ReadingOrder.CompareTo(line.ReadingOrder);
            }

        }
    }
}

