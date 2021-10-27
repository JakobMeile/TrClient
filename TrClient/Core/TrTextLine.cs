// <copyright file="TrTextLine.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Xml.Linq;
    using TrClient.Core.Tags;
    using TrClient.Extensions;
    using TrClient.Helpers;
    using TrClient.Libraries;

    public enum ContentType
    {
        Undefined = 0,
        Date = 1,
        AccNumber = 2,
        Plain = 3,
        Caption = 100,
    }

    public enum VisualOffsetType
    {
        None = 0,
        SmallBoundingBox = 1,
        LargeBoundingBox = 2,
    }

    public class TrTextLine : IComparable, INotifyPropertyChanged
    {
        public string ID { get; set; }

        public string TagString { get; set; }

        public string CoordsString { get; set; }

        public string BaseLineCoordsString { get; set; }

        public bool MarkToDeletion = false;

        public int RowNumber { get; set; }

        //public int ColumnNumber { get; set; }

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
                    //Debug.Print($"TrTextLine: set RO: HAS RO-tag: Delete it! Count before: {Tags.Count}");
                    DeleteReadingOrderTag();

                    //Debug.Print($"Count after: {Tags.Count}");
                }

                readingOrder = value;
                TrTagReadingOrder rOTag = new TrTagReadingOrder(readingOrder);
                Tags.Add(rOTag);
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

        private TrTextLine previous;

        public TrTextLine Previous
        {
            get
            {
                if (Number == 1)
                {
                    previous = null;
                }
                else
                {
                    try
                    {
                        previous = ParentRegion.GetLineByNumber(Number - 1);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Error! Exception message when getting previous in TL {Number.ToString()}: {e.Message}");
                    }
                }

                return previous;
            }

            set
            {
                if (previous != value)
                {
                    previous = value;
                }
            }
        }

        private TrTextLine next;

        public TrTextLine Next
        {
            get
            {
                if (Number == ParentContainer.Count)
                {
                    next = null;
                }
                else
                {
                    next = ParentRegion.GetLineByNumber(Number + 1);
                }

                return next;
            }

            set
            {
                if (next != value)
                {
                    next = value;
                }
            }
        }

        private TrWords words = new TrWords();

        public TrWords Words
        {
            get
            {
                var wordsArray = TextEquiv.Split(' ').ToArray();
                int wordsCount = wordsArray.Length;

                //int PositionPointer = 0;
                for (int i = 0; i < wordsCount; i++)
                {
                    if (wordsArray[i].ToString().Trim() != string.Empty)
                    {
                        // NEJ ikke mere: vi fjerner lige tallene
                        //if (!clsLanguageLibrary.IsNumeric(WordsArray[i]))
                        {
                            TrWord word = new TrWord(wordsArray[i].ToString(), this);
                            words.Add(word);
                        }
                    }
                }

                return words;
            }
        }

        private int amount = 50; // "delta" for BoundingBoxLarge

        private Polygon visualLineArea;

        public Polygon VisualLineArea
        {
            get
            {
                VisualOffsetType offset = VisualOffsetType.LargeBoundingBox;
                Polygon temp = new Polygon();
                PointCollection linePoints = new PointCollection();
                TrCoords newCoords = new TrCoords(CoordsString);

                foreach (TrCoord c in newCoords)
                {
                    TrCoord tempC;
                    if (offset != VisualOffsetType.None)
                    {
                        if (offset == VisualOffsetType.SmallBoundingBox)
                        {
                            tempC = c.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetSmall);
                        }
                        else
                        {
                            tempC = c.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetLarge);
                        }
                    }
                    else
                    {
                        tempC = c;
                    }

                    // SCALING
                    double scalingFactor = 0.25;
                    double tempX = (double)tempC.X * scalingFactor;
                    double tempY = (double)tempC.Y * scalingFactor;

                    Point currentPoint = new Point(tempX, tempY);
                    linePoints.Add(currentPoint);
                }

                temp.Points = linePoints;
                return temp;
            }
        }

        private Polyline visualBaseLine;

        public Polyline VisualBaseLine
        {
            get
            {
                VisualOffsetType offset = VisualOffsetType.LargeBoundingBox;
                Polyline temp = new Polyline();
                PointCollection baseLinePoints = new PointCollection();
                TrCoords newCoords = new TrCoords(BaseLineCoordsString);

                foreach (TrCoord c in newCoords)
                {
                    TrCoord tempC;
                    if (offset != VisualOffsetType.None)
                    {
                        if (offset == VisualOffsetType.SmallBoundingBox)
                        {
                            tempC = c.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetSmall);
                        }
                        else
                        {
                            tempC = c.SubtractOffset(LeftBorder, TopBorder, BoundingBoxOffsetLarge);
                        }
                    }
                    else
                    {
                        tempC = c;
                    }

                    // SCALING
                    double scalingFactor = 0.25;
                    double tempX = (double)tempC.X * scalingFactor;
                    double tempY = (double)tempC.Y * scalingFactor;

                    Point currentPoint = new Point(tempX, tempY);
                    baseLinePoints.Add(currentPoint);
                }

                temp.Points = baseLinePoints;
                return temp;
            }
        }

        private string textEquiv;

        public string TextEquiv
        {
            get
            {
                return textEquiv;
            }

            set
            {
                if (textEquiv != value)
                {
                    textEquiv = value;
                    NotifyPropertyChanged("TextEquiv");
                }
            }
        }

        private bool endsWithHyphen;

        public bool EndsWithHyphen
        {
            get
            {
                endsWithHyphen = false;
                string temp = GetExpandedText(false, false);
                if (temp.Length > 2)
                {
                    char last = temp[temp.Length - 1];
                    if (last == '-' || last == '=')
                    {
                        endsWithHyphen = true;
                    }
                }

                return endsWithHyphen;
            }
        }

        private bool startsWithSmallLetter;

        public bool StartsWithSmallLetter
        {
            get
            {
                string temp = GetExpandedText(false, false);
                if (temp.Length > 0)
                {
                    char first = temp[0];
                    startsWithSmallLetter = char.IsLower(first);
                }
                else
                {
                    startsWithSmallLetter = false;
                }

                return startsWithSmallLetter;
            }
        }

        private TrTagStructural structuralTag;

        public TrTagStructural StructuralTag
        {
            get
            {
                return structuralTag;
            }

            set
            {
                if (structuralTag != value)
                {
                    structuralTag = value;
                    NotifyPropertyChanged("StructuralTag");

                    // StructuralTagValue = value.SubType;
                }
            }
        }

        public void WrapSuperAndSubscriptWithSpaces()
        {
            if (HasSuperscriptTag || HasSubscriptTag)
            {
                TrTags superAndSubscriptTags = new TrTags();
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualStyle))
                    {
                        if ((t as TrTagTextualStyle).Superscript || (t as TrTagTextualStyle).Subscript)
                        {
                            superAndSubscriptTags.Add(t);
                        }
                    }
                }

                int superAndSubscriptCount = superAndSubscriptTags.Count;

                //Debug.Print($"Super- and Subscripts: {SuperAndSubscriptCount}");
                Regex romanNumbers = new Regex(@"\b[IVXLCDM]+\b");

                for (int i = 0; i < superAndSubscriptCount; i++)
                {
                    TrTag s = superAndSubscriptTags[i];
                    int offset = (s as TrTagTextual).Offset;
                    int endPosition = (s as TrTagTextual).EndPosition;
                    int length = (s as TrTagTextual).Length;
                    string tagContent = TextEquiv.Substring(offset, length);

                    //Debug.Print($"Content: {TagContent}");
                    //Debug.Print($"Offset: {Offset}, Endpos: {EndPosition}");
                    MatchCollection romanNumberMatches = romanNumbers.Matches(tagContent);

                    // det er dog kun, hvis selve tagget er et romertal eller et arabertal, at vi indsætter space
                    if (romanNumberMatches.Count > 0 || TrLibrary.IsNumeric(tagContent))
                    {
                        // da vi går baglæns, tester vi først endposition og EFTER
                        if (endPosition < length - 1)
                        {
                            // så står superscriptet IKKE helt ude for enden
                            // hvad står der EFTER?
                            char after = TextEquiv[endPosition + 1];

                            //Debug.Print($"After: _{After}_");

                            // hvis det IKKE er et mellemrum, og hvis det IKKE er et bogstav, indsætter vi et mellemrum
                            if (!char.IsWhiteSpace(after) && !char.IsLetter(after))
                            {
                                TextEquiv = TextEquiv.Insert(endPosition + 1, " ");
                                Tags.Move(endPosition + 1, 1, true);
                                HasChanged = true;

                                //Debug.Print($"Space added!");
                            }
                        }

                        // dernæst tester vi INDEN, dvs. via offset
                        if (offset > 0)
                        {
                            // hvis ikke begyndelsen af linien
                            // hvad står der før?
                            char before = TextEquiv[offset - 1];

                            //Debug.Print($"Before: _{Before}_");

                            // hvis det IKKE er et mellemrum, og hvis det IKKE er et bogstav, indsætter vi et mellemrum
                            if (!char.IsWhiteSpace(before) && !char.IsLetter(before))
                            {
                                TextEquiv = TextEquiv.Insert(offset, " ");
                                Tags.Move(offset - 1, 1, true);
                                HasChanged = true;

                                //Debug.Print($"Space added!");
                            }
                        }
                    }
                }
            }
        }

        public void Replace(string find, string replaceWith)
        {
            int offset;
            int length;
            int difference;

            // MEN hvis man ønsker at finde noget, der ender med mellemrum, skal der tages højde for det:
            if (find.EndsWith(" "))
            {
                length = find.Trim().Length;

                // i dette tilfælde vil man også tit gerne sætte noget ind med et mellemrum: det skal så ædndres
                difference = replaceWith.Trim().Length - find.Trim().Length;

                string searchForTrimmed = Regex.Escape(find.Trim()) + "$";

                //Debug.Print($"SearchForTrimmed = _{SearchForTrimmed}_");
                Regex toFindTrimmed = new Regex(searchForTrimmed);
                MatchCollection trimmedMatches = toFindTrimmed.Matches(TextEquiv);
                if (trimmedMatches.Count > 1)
                {
                    // noget er helt galt!
                    Debug.Print($"FATAL REGEX ERROR: TextEquiv: {TextEquiv}, Mathces: {trimmedMatches.Count}");
                }
                else if (trimmedMatches.Count == 1)
                {
                    Match end = trimmedMatches[0];
                    offset = end.Index;

                    if (offset >= 0)
                    {
                        TextEquiv = TextEquiv.Remove(offset, length);

                        // der indsættes en trimmet udgave
                        TextEquiv = TextEquiv.Insert(offset, replaceWith.Trim());
                        Tags.Move(offset, difference, true);
                        HasChanged = true;
                    }
                }
            }

            // derefter kan vi gøre det hele mere normalt, dvs. inde i strengen
            length = find.Length;
            difference = replaceWith.Length - find.Length;

            string searchForFull = Regex.Escape(find);

            //Debug.Print($"SearchForFull = _{SearchForFull}_");
            Regex toFindFull = new Regex(searchForFull);
            MatchCollection fullMatches = toFindFull.Matches(TextEquiv);

            // så gennemløber vi baglæns
            for (int i = fullMatches.Count - 1; i >= 0; i--)
            {
                // Debug.WriteLine($"TrTextLine : Replace : Find = {Find }, ReplaceWith = {ReplaceWith }, i = {i}");
                Match m = fullMatches[i];
                offset = m.Index;

                if (offset >= 0)
                {
                    TextEquiv = TextEquiv.Remove(offset, length);
                    TextEquiv = TextEquiv.Insert(offset, replaceWith);
                    Tags.Move(offset, difference, true);
                    HasChanged = true;
                }
            }
        }

        private int length = 0;

        public int Length
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    length = TextEquiv.Length;
                }
                else
                {
                    length = 0;
                }

                return length;
            }
        }

        private int number;

        public int Number
        {
            get
            {
                // Debug.WriteLine($"TrTextLine : Number: ReadingOrder = {ReadingOrder}");
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

        private string parentDocTitle;

        public string ParentDocTitle
        {
            get
            {
                parentDocTitle = ParentRegion.ParentTranscript.ParentPage.ParentDocument.Title;
                return parentDocTitle;
            }
        }

        private int parentPageNr;

        public int ParentPageNr
        {
            get
            {
                parentPageNr = ParentRegion.ParentTranscript.ParentPage.PageNr;
                return parentPageNr;
            }
        }

        private int parentRegionNr;

        public int ParentRegionNr
        {
            get
            {
                parentRegionNr = ParentRegion.Number;
                return parentRegionNr;
            }
        }

        private bool isEmpty = false;

        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(TextEquiv))
                {
                    isEmpty = true;
                }
                else if (string.IsNullOrEmpty(TextEquiv.Trim()))
                {
                    isEmpty = true;
                }

                return isEmpty;
            }
        }

        public double MaxAllowedBaseLineAngle = 10.0; // gælder kun FREMAD - hvis linjen går baglæns er den altid gal

        public TrTags Tags = new TrTags();

        public TrTextLines ParentContainer;
        public TrTextRegion ParentRegion;

        private bool isLoaded = false;

        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }

            set
            {
                if (isLoaded != value)
                {
                    isLoaded = value;
                    NotifyPropertyChanged("IsLoaded");
                    switch (isLoaded)
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
                // flg. linie pga en bug
                if (ParentRegion != null)
                {
                    ParentRegion.HasChanged = value;
                }
            }
        }

        private bool changesUploaded = false;

        public bool ChangesUploaded
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
            }
        }

        private Int32Rect boundingBoxSmall;

        //public Int32Rect BoundingBoxSmall
        //{
        //    get
        //    {
        //        boundingBoxSmall = new Int32Rect(LeftBorder, TopBorder, BoundingBoxWidth, BoundingBoxHeight);
        //        return boundingBoxSmall;
        //    }
        //}

        private TrCoord boundingBoxOffsetSmall;

        public TrCoord BoundingBoxOffsetSmall
        {
            get
            {
                TrCoord temp = new TrCoord(LeftBorder, TopBorder);
                return temp;
            }
        }

        private TrCoord boundingBoxOffsetLarge;

        public TrCoord BoundingBoxOffsetLarge
        {
            get
            {
                int tempLeft;
                int tempTop;

                if (LeftBorder - amount > 0)
                {
                    tempLeft = amount;
                }
                else
                {
                    tempLeft = LeftBorder;
                }

                if (TopBorder - amount > 0)
                {
                    tempTop = amount;
                }
                else
                {
                    tempTop = TopBorder;
                }

                TrCoord temp = new TrCoord(tempLeft, tempTop);
                return temp;
            }
        }

        private Int32Rect boundingBoxLarge;

        public Int32Rect BoundingBoxLarge
        {
            get
            {
                int pageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
                int pageHeight = ParentRegion.ParentTranscript.ParentPage.Height;

                int newLeft = LeftBorder - amount;
                if (newLeft < 0)
                {
                    newLeft = 0;
                }

                int newTop = TopBorder - amount;
                if (newTop < 0)
                {
                    newTop = 0;
                }

                int newRight = RightBorder + amount;
                if (newRight > pageWidth)
                {
                    newRight = pageWidth;
                }

                int newBottom = BottomBorder + amount;
                if (newBottom > pageHeight)
                {
                    newBottom = pageHeight;
                }

                int newWidth = newRight - newLeft;
                int newHeight = newBottom - newTop;

                boundingBoxLarge = new Int32Rect(newLeft, newTop, newWidth, newHeight);
                return boundingBoxLarge;
            }
        }

        private int leftBorder;

        public int LeftBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    int boundingBoxValue = TrLibrary.GetLeftMostXcoord(CoordsString);
                    int baseLineValue = TrLibrary.GetLeftMostXcoord(BaseLineCoordsString);

                    if (boundingBoxValue < baseLineValue)
                    {
                        leftBorder = boundingBoxValue;
                    }
                    else
                    {
                        leftBorder = baseLineValue;
                    }
                }
                else
                {
                    leftBorder = -1;
                }

                return leftBorder;
            }
        }

        private int rightBorder;

        public int RightBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    int boundingBoxValue = TrLibrary.GetRightMostXcoord(CoordsString);
                    int baseLineValue = TrLibrary.GetRightMostXcoord(BaseLineCoordsString);

                    if (boundingBoxValue > baseLineValue)
                    {
                        rightBorder = boundingBoxValue;
                    }
                    else
                    {
                        rightBorder = baseLineValue;
                    }
                }
                else
                {
                    rightBorder = -1;
                }

                return rightBorder;
            }
        }

        private int boundingBoxWidth;

        public int BoundingBoxWidth
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    boundingBoxWidth = RightBorder - LeftBorder;
                }
                else
                {
                    boundingBoxWidth = -1;
                }

                return boundingBoxWidth;
            }
        }

        private int width;

        public int Width
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    width = RightBorder - LeftBorder;
                }
                else
                {
                    width = -1;
                }

                return width;
            }
        }

        private int topBorder;

        public int TopBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    topBorder = TrLibrary.GetTopYcoord(CoordsString);
                }
                else
                {
                    topBorder = -1;
                }

                return topBorder;
            }
        }

        private int bottomBorder;

        public int BottomBorder
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    bottomBorder = TrLibrary.GetBottomYcoord(CoordsString);
                }
                else
                {
                    bottomBorder = -1;
                }

                return bottomBorder;
            }
        }

        private int boundingBoxHeight;

        public int BoundingBoxHeight
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    boundingBoxHeight = BottomBorder - TopBorder;
                }
                else
                {
                    boundingBoxHeight = -1;
                }

                return boundingBoxHeight;
            }
        }

        private int height;

        public int Height
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    height = BottomBorder - TopBorder;
                }
                else
                {
                    height = -1;
                }

                return height;
            }
        }

        private int hPos;

        public int Hpos
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    hPos = LeftBorder;
                }
                else
                {
                    hPos = -1;
                }

                return hPos;
            }
        }

        private int vPos = 0;

        public int Vpos
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    // først testes, om Vpos er blevet manuelt sat (dvs. forskellig fra 0); i givet fald returneres den blot; ellers beregnes den først
                    if (vPos == 0)
                    {
                        vPos = TrLibrary.GetAverageYcoord(BaseLineCoordsString);
                    }
                }
                else
                {
                    vPos = -1;
                }

                return vPos;
            }

            set
            {
                // Vpos bør KUN SÆTTES ifm. logisk arrangerering af linierne (kaldes af TrTextLines.RenumberLogically)
                vPos = value;
            }
        }

        private int textSizeFactor;

        public int TextSizeFactor
        {
            get
            {
                if (Length > 0)
                {
                    textSizeFactor = Width / Length;
                }
                else
                {
                    textSizeFactor = 0;
                }

                return textSizeFactor;
            }
        }

        public TrTextLine GetTextLineAbove()
        {
            int current = Number - 1;
            TrTextLine lineAbove = null;

            // Debug.Print($"TrTextLine: GetTextLineAbove: Current = {Current}, TextLineCount: {ParentRegion.TextLines.Count}");
            int middle = Hpos + ((HendPos - Hpos) / 2);

            for (int i = current - 1; i >= 1; i--)
            {
                if (middle > ParentRegion.TextLines[i].Hpos && middle < ParentRegion.TextLines[i].HendPos)
                {
                    lineAbove = ParentRegion.TextLines[i];
                    break;
                }
            }

            return lineAbove;
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

        private int logicalOrder;

        public int LogicalOrder
        {
            get
            {
                logicalOrder = (((Vpos + SortDeltaY) / 100) * 1_000_000) + Hpos + AngleFromOrigo;
                return logicalOrder;
            }
        }

        private int sortDeltaY = 0;

        public int SortDeltaY
        {
            get
            {
                return sortDeltaY;
            }

            set
            {
                if (value != sortDeltaY)
                {
                    sortDeltaY = value;
                }
            }
        }

        private int angleFromOrigo = 0;

        public int AngleFromOrigo
        {
            // returnerer vinklen fra sidens øverste venstre hjørne til beg.punkt
            get
            {
                if (Vpos > 0)
                {
                    double ratio = (double)Hpos / (double)Vpos;
                    double angle = System.Math.Atan(ratio) * (180 / Math.PI);
                    angleFromOrigo = Convert.ToInt32(angle);
                }

                return angleFromOrigo;
            }
        }

        private int hEndPos;

        public int HendPos
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    hEndPos = RightBorder;
                }
                else
                {
                    hEndPos = -1;
                }

                return hEndPos;
            }
        }

        private double percentualVpos;

        public double PercentualVpos
        {
            get
            {
                int pageHeight = ParentRegion.ParentTranscript.ParentPage.Height;
                percentualVpos = (double)Vpos / (double)pageHeight * 100.0;

                // Debug.WriteLine($"Pageheigth = {PageHeight}, Vpos = {Vpos}, PercentualVpos = {_percentualVpos}");
                return percentualVpos;
            }
        }

        private double percentualHpos;

        public double PercentualHpos
        {
            get
            {
                int pageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
                percentualHpos = (double)Hpos / (double)pageWidth * 100.0;

                // Debug.WriteLine($"Pagewidth = {PageWidth}, Hpos = {Hpos}, PercentualHpos = {_percentualHpos}");
                return percentualHpos;
            }
        }

        private double percentualHendPos;

        public double PercentualHendPos
        {
            get
            {
                int pageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
                percentualHendPos = (double)HendPos / (double)pageWidth * 100.0;
                return percentualHendPos;
            }
        }

        //public bool IsOKwithBaseLineFilter(TrBaseLineFilter currentFilter)
        //{
        //    bool temp;

        //    bool positiveCheck;
        //    bool straightCheck;
        //    bool directionCheck;

        //    positiveCheck = IsCoordinatesPositive == currentFilter.CoordinatesPositive;
        //    straightCheck = IsBaseLineStraight == currentFilter.BaseLineStraight;
        //    directionCheck = IsBaseLineDirectionOK == currentFilter.BaseLineDirectionOK;

        //    temp = positiveCheck && straightCheck && directionCheck;

        //    return temp;
        //}

        public bool MeetsFilterConditions(TrLineFilterSettings currentFilter)
        {
            bool temp;

            // sætter alt til true - og hvis de enkelte dele rent faktisk skal testes, kan de blive false
            bool checkPageNumber = true;
            bool checkRegEx = true;
            bool checkStructuralTag = true;
            bool checkTextSizeFactor = true;
            bool checkTextLength = true;
            bool checkPosition = true;

            if (currentFilter.FilterByPageNumber)
            {
                checkPageNumber = (ParentPageNr >= currentFilter.StartPage) && (ParentPageNr <= currentFilter.EndPage);
            }

            if (currentFilter.FilterByRegEx)
            {
                checkRegEx = MatchesRegex(currentFilter.RegExPattern);
            }

            if (currentFilter.FilterByStructuralTag)
            {
                if (currentFilter.StructuralTag != string.Empty)
                {
                    checkStructuralTag = HasSpecificStructuralTag(currentFilter.StructuralTag);
                }
            }

            if (currentFilter.FilterByTextSizeFactor)
            {
                checkTextSizeFactor = MatchesTextSize(currentFilter);
            }

            if (currentFilter.FilterByTextLength)
            {
                checkTextLength = MatchesTextLength(currentFilter);
            }

            if (currentFilter.FilterByPosition)
            {
                checkPosition = MatchesWindow(currentFilter);
            }

            temp = checkPageNumber && checkRegEx && checkStructuralTag && checkTextSizeFactor && checkTextLength && checkPosition;

            return temp;
        }

        public bool MatchesTextSize(TrLineFilterSettings filterSettings)
        {
            bool temp;

            bool lowerOK;
            bool upperOK;

            if (filterSettings.LowerLimitTextSizeFactor == 0)
            {
                lowerOK = true;
            }
            else
            {
                lowerOK = TextSizeFactor >= filterSettings.LowerLimitTextSizeFactor;
            }

            if (filterSettings.UpperLimitTextSizeFactor == 0)
            {
                upperOK = true;
            }
            else
            {
                upperOK = TextSizeFactor <= filterSettings.UpperLimitTextSizeFactor;
            }

            temp = lowerOK && upperOK;

            return temp;
        }

        public bool MatchesTextLength(TrLineFilterSettings filterSettings)
        {
            bool temp;

            bool lowerOK;
            bool upperOK;

            if (filterSettings.LowerLimitTextLength == 0)
            {
                lowerOK = true;
            }
            else
            {
                lowerOK = Length >= filterSettings.LowerLimitTextLength;
            }

            if (filterSettings.UpperLimitTextLength == 0)
            {
                upperOK = true;
            }
            else
            {
                upperOK = Length <= filterSettings.UpperLimitTextLength;
            }

            temp = lowerOK && upperOK;

            return temp;
        }

        public bool MatchesWindow(TrLineFilterSettings filterSettings)
        {
            bool temp;

            if (filterSettings.Inside)
            {
                temp = IsInWindow(filterSettings);
            }
            else
            {
                temp = !IsInWindow(filterSettings);
            }

            return temp;
        }

        public bool IsInWindow(TrLineFilterSettings filterSettings)
        {
            bool temp;

            bool vcheck;
            bool hcheck;
            bool hendCheck;

            vcheck = (PercentualVpos >= filterSettings.TopBorder) && (PercentualVpos <= filterSettings.BottomBorder);
            hcheck = (PercentualHpos >= filterSettings.LeftBorder) && (PercentualHpos <= filterSettings.RightBorder);

            hendCheck = (PercentualHendPos >= filterSettings.LeftBorder) && (PercentualHendPos <= filterSettings.RightBorder);

            temp = vcheck && hcheck;

            if (filterSettings.IncludeEnding)
            {
                temp = temp && hendCheck;
            }

            //Debug.WriteLine($"PVpos = {PercentualVpos}, Top = {CurrentWindow.TopBorder}, Bottom = {CurrentWindow.BottomBorder}, " +
            //    $"PHpos = {PercentualHpos}, Left = {CurrentWindow.LeftBorder}, Right = {CurrentWindow.RightBorder}, OK? {temp}");
            return temp;
        }

        public bool MatchesRegex(string regExPattern) // Regex MatchPattern
        {
            bool temp = false;

            Regex matchPattern = new Regex(regExPattern);
            MatchCollection matches = matchPattern.Matches(ExpandedText);
            if (matches.Count > 0)
            {
                temp = true;
            }
            else
            {
                temp = false;
            }

            return temp;
        }

        private bool hasAbbrevTag;

        public bool HasAbbrevTag
        {
            get
            {
                hasAbbrevTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualAbbrev))
                    {
                        hasAbbrevTag = hasAbbrevTag || true;
                    }
                }

                return hasAbbrevTag;
            }
        }

        private bool hasEmptyAbbrevTag;

        public bool HasEmptyAbbrevTag
        {
            get
            {
                hasEmptyAbbrevTag = false;
                if (HasAbbrevTag)
                {
                    foreach (TrTag t in Tags)
                    {
                        if (t.GetType() == typeof(TrTagTextualAbbrev))
                        {
                            hasEmptyAbbrevTag = hasEmptyAbbrevTag || (t as TrTagTextualAbbrev).IsEmpty;
                        }
                    }
                }

                return hasEmptyAbbrevTag;
            }
        }

        private bool hasSicTag;

        public bool HasSicTag
        {
            get
            {
                hasSicTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualSic))
                    {
                        hasSicTag = hasSicTag || true;
                    }
                }

                return hasSicTag;
            }
        }

        private bool hasCommentTag;

        public bool HasCommentTag
        {
            get
            {
                hasCommentTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualComment))
                    {
                        hasCommentTag = hasCommentTag || true;
                    }
                }

                return hasCommentTag;
            }
        }

        private bool hasUnclearTag;

        public bool HasUnclearTag
        {
            get
            {
                hasUnclearTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualUnclear))
                    {
                        hasUnclearTag = hasUnclearTag || true;
                    }
                }

                return hasUnclearTag;
            }
        }

        private bool hasDateTag;

        public bool HasDateTag
        {
            get
            {
                hasDateTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualDate))
                    {
                        hasDateTag = hasDateTag || true;
                    }
                }

                return hasDateTag;
            }
        }

        private bool hasStrikethroughTag;

        public bool HasStrikethroughTag
        {
            get
            {
                hasStrikethroughTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualStyle))
                    {
                        if ((t as TrTagTextualStyle).Strikethrough)
                        {
                            hasStrikethroughTag = hasStrikethroughTag || true;
                        }
                    }
                }

                return hasStrikethroughTag;
            }
        }

        private bool hasSuperscriptTag;

        public bool HasSuperscriptTag
        {
            get
            {
                hasSuperscriptTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualStyle))
                    {
                        if ((t as TrTagTextualStyle).Superscript)
                        {
                            hasSuperscriptTag = hasSuperscriptTag || true;
                        }
                    }
                }

                return hasSuperscriptTag;
            }
        }

        private bool hasSubscriptTag;

        public bool HasSubscriptTag
        {
            get
            {
                hasSubscriptTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualStyle))
                    {
                        if ((t as TrTagTextualStyle).Subscript)
                        {
                            hasSubscriptTag = hasSubscriptTag || true;
                        }
                    }
                }

                return hasSubscriptTag;
            }
        }

        private bool hasRomanNumeralTag;

        public bool HasRomanNumeralTag
        {
            get
            {
                hasRomanNumeralTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualRomanNumeral))
                    {
                        hasRomanNumeralTag = hasRomanNumeralTag || true;
                    }
                }

                return hasRomanNumeralTag;
            }
        }

        private bool hasFullStrikethroughTag;

        public bool HasFullStrikethroughTag
        {
            get
            {
                hasFullStrikethroughTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextual))
                    {
                        hasFullStrikethroughTag = ((t as TrTagTextual).IsStrikethrough && (t as TrTagTextual).IsFull) || hasFullStrikethroughTag;
                    }
                }

                return hasFullStrikethroughTag;
            }
        }

        private bool hasFullDateTag;

        public bool HasFullDateTag
        {
            get
            {
                hasFullDateTag = false;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextual))
                    {
                        hasFullDateTag = ((t as TrTagTextual).IsDate && (t as TrTagTextual).IsFull) || hasFullDateTag;
                    }
                }

                return hasFullDateTag;
            }
        }

        public bool Contains(string searchFor)
        {
            return TextEquiv.Contains(searchFor);
        }

        private string quickExpandedText;

        public string QuickExpandedText
        {
            get
            {
                quickExpandedText = GetExpandedText(true, true);
                return quickExpandedText;
            }
        }

        private string expandedText;

        public string ExpandedText
        {
            get
            {
                expandedText = GetExpandedText(true, true);
                return expandedText;
            }
        }

        private string GetExpandedText(bool refine, bool convertOtrema)
        {
            //Debug.Print($"GetExpText: Page: {ParentPageNr} TL # {Number}");
            string temp = TextEquiv;
            int offset;
            int tagLength;

            int currentDelta = 0;
            string oldContent = string.Empty;
            string newContent = string.Empty;

            // --- 1 --- konverter roman numbers -------------------------------------------------
            if (HasRomanNumeralTag)
            {
                //Debug.Print($"GetExpText: Roman numbers temp = _{temp}_");
                for (int i = Tags.Count - 1; i >= 0; i--)
                {
                    TrTag testTag = Tags[i];
                    if (testTag.GetType() == typeof(TrTagTextualRomanNumeral))
                    {
                        TrTag r = testTag as TrTagTextualRomanNumeral;
                        offset = (r as TrTagTextual).Offset;
                        tagLength = (r as TrTagTextual).Length;

                        oldContent = TextEquiv.Substring(offset, tagLength);
                        newContent = (r as TrTagTextualRomanNumeral).ArabicEquivalent.ToString();

                        if (tagLength == Length)
                        {
                            temp = newContent;
                        }
                        else if (offset >= 0 && tagLength < Length)
                        {
                            temp = temp.Remove(offset, tagLength);
                            if (newContent != string.Empty && offset < Length)
                            {
                                temp = temp.Insert(offset, newContent);
                            }
                        }

                        // CurrentDelta sættes; den indeholder forskellen i længde på gammelt og nyt indhold
                        currentDelta = newContent.Length - oldContent.Length; // tidl. - taglength

                        // Nu er problemet så, at tags længere ude i strengen har forkerte Offset og Length-værdier
                        // Det ordnes ved at gennemløbe disse og sætte deres Delta-værdi
                        //Debug.Print($"GetExpText: Roman numbers - currentdelta: {CurrentDelta}");
                        if (currentDelta != 0)
                        {
                            Tags.Move(offset, currentDelta, false);
                        }
                    }
                }
            }

            // --- 2 --- konverterer superscript --------------------------------------------------------
            if (HasSuperscriptTag)
            {
                //Debug.Print($"GetExpText: Superscripts: temp = _{temp}_");
                TrTags superscriptTags = new TrTags();
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualStyle))
                    {
                        if ((t as TrTagTextualStyle).Superscript)
                        {
                            superscriptTags.Add(t);
                        }
                    }
                }

                int superscriptCount = superscriptTags.Count;

                string oldNumbers;
                string newNumbers = string.Empty;
                char currentChar;
                char newChar = ' ';
                offset = 0;
                tagLength = 0;

                for (int i = 0; i < superscriptCount; i++)
                {
                    TrTag s = superscriptTags[i];
                    offset = (s as TrTagTextual).Offset;
                    tagLength = (s as TrTagTextual).Length;
                    oldNumbers = temp.Substring(offset, tagLength);

                    if (oldNumbers.Length > 0)
                    {
                        for (int k = 0; k < oldNumbers.Length; k++)
                        {
                            currentChar = oldNumbers[k];
                            if (char.IsNumber(currentChar))
                            {
                                newChar = TrLibrary.GetSuperscriptChar(currentChar);
                            }
                            else
                            {
                                newChar = currentChar;
                            }

                            newNumbers = newNumbers + newChar.ToString();
                        }

                        // erstat
                        temp = temp.Remove(offset, tagLength);
                        temp = temp.Insert(offset, newNumbers);
                    }
                }
            }

            // --- 3 --- ordinære tags! -------------------------------------------------------------------
            // henter alle Tags, som er af disse relevante typer
            if (HasAbbrevTag || HasSicTag || HasUnclearTag || HasStrikethroughTag || HasCommentTag || HasDateTag)
            {
                oldContent = string.Empty;
                TrTags textualTags = new TrTags();
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualSic) || t.GetType() == typeof(TrTagTextualAbbrev) || t.GetType() == typeof(TrTagTextualUnclear) || t.GetType() == typeof(TrTagTextualComment) || t.GetType() == typeof(TrTagTextualDate))
                    {
                        //Debug.Print($"GetExpText: Fundet Sic/Abbrev/Date/Unclear temp = _{temp}_");
                        textualTags.Add(t);
                        t.ParentLine = this;
                    }
                    else if (t.GetType() == typeof(TrTagTextualStyle))
                    {
                        // det eneste styletag, som er relevant, er Strikethrough
                        if ((t as TrTagTextualStyle).Strikethrough)
                        {
                            // Debug.Print($"GetExpText: Fundet Strike");
                            textualTags.Add(t);
                            t.ParentLine = this;
                        }
                    }
                }

                int max = textualTags.Count;

                // TextualTags indeholder nu kun de tags, der skal bruges til at ændre teksten

                // --- 3.1. --- først testes, om der findes et FULL StrikethroughTag
                if (HasFullStrikethroughTag)
                {
                    //Debug.Print($"GetExpText: Has Full Strike");
                    // i givet fald er hele teksten streget ud; der skal returneres en tom streng
                    oldContent = temp;
                    temp = string.Empty;
                }

                // --- 3.2. --- dernæst testes, om der findes et FULL DateTag
                else if (HasFullDateTag)
                {
                    //Debug.Print($"GetExpText: Has Full Date");
                    // i givet fald skal det pågældende datetag findes; dette returneres alene
                    foreach (TrTag t in textualTags)
                    {
                        if (t.GetType() == typeof(TrTagTextualDate))
                        {
                            oldContent = temp;
                            temp = (t as TrTagTextualDate).ExpandedDate;
                        }
                    }
                }

                // --- 3.3. --- hvis hverken full strikethrough eller full date, skal alle gennemgås:
                else
                {
                    //Debug.Print($"GetExpText: Std. tag procedure");
                    // de fundne tags sorteres (efter SortKey)
                    textualTags.Sort();

                    newContent = string.Empty;
                    oldContent = string.Empty;
                    offset = 0;
                    tagLength = 0;
                    currentDelta = 0;

                    // kører BAGLÆNS for ikke at få problemer med de øvrige tags undervejs

                    // 1. gennemløb: ALLE PÅNÆR DATE
                    for (int i = max - 1; i >= 0; i--)
                    {
                        //Debug.Print($"GetExpText: 1. gennemløb");
                        TrTag t = textualTags[i];

                        if (!(t as TrTagTextual).IsDate)
                        {
                            //Debug.Print($"GetExpText: is NOT date");
                            offset = (t as TrTagTextual).Offset;
                            tagLength = (t as TrTagTextual).Length;

                            //Debug.Print($"Ikke date: Number: {Number}, Type: {T.Type}, Offset: {Offset}, Length: {TagLength}, Text: {TextEquiv}");
                            if (tagLength > temp.Length || offset > temp.Length)

                            //if (TagLength > TextEquiv.Length || Offset > TextEquiv.Length)
                            {
                                Debug.Print($"FATAL ERROR: Offset: {offset}, Taglength: {tagLength}, Textequiv.Length: {TextEquiv.Length}");
                            }
                            else
                            {
                                if (tagLength + offset > temp.Length)
                                {
                                    Debug.Print($"FATAL ERROR: Offset: {offset}, Taglength: {tagLength}, Textequiv.Length: {TextEquiv.Length}");
                                }
                                else
                                {
                                    oldContent = temp.Substring(offset, tagLength); // rettet fra textequiv.substring
                                }
                            }

                            // hvis det enkelte tag har et overlappende datetag, erstattes teksten med dette datetags expansion
                            if ((t as TrTagTextual).HasOverlappingDateTag)
                            {
                                //Debug.Print($"GetExpText: has overlap date");
                                newContent = (t as TrTagTextual).GetOverlappingDateTag().ExpandedDate;

                                // det pågæld. datetag markeres som "resolved", så det ikke bliver benyttet nedenunder (i 2. gennemløb)
                                (t as TrTagTextual).GetOverlappingDateTag().Resolved = true;
                            }

                            // men hvis det har et overlappende, behandles det helt normalt
                            else
                            {
                                //Debug.Print($"GetExpText: has NOT overlap date");
                                // afhængigt af type, sættes NewContent
                                if (t.GetType() == typeof(TrTagTextualSic))
                                {
                                    newContent = (t as TrTagTextualSic).Correction;
                                }
                                else if (t.GetType() == typeof(TrTagTextualAbbrev))
                                {
                                    newContent = (t as TrTagTextualAbbrev).Expansion;
                                }
                                else if (t.GetType() == typeof(TrTagTextualStyle))
                                {
                                    newContent = string.Empty; // jo, det kan vi godt tillade os, for det eneste styletag, der er med nu, er strikethrough!
                                }
                                else if (t.GetType() == typeof(TrTagTextualUnclear))
                                {
                                    if (!(t as TrTagTextualUnclear).IsEmpty)
                                    {
                                        newContent = oldContent + " [alt.: " + (t as TrTagTextualUnclear).Alternative;
                                        if ((t as TrTagTextualUnclear).Reason != string.Empty)
                                        {
                                            newContent = newContent + " (" + (t as TrTagTextualUnclear).Reason + ")";
                                        }

                                        newContent = newContent + "] ";
                                    }
                                    else
                                    {
                                        newContent = oldContent;
                                    }
                                }
                                else if (t.GetType() == typeof(TrTagTextualComment))
                                {
                                    if (!(t as TrTagTextualComment).IsEmpty)
                                    {
                                        newContent = oldContent + " [note: " + (t as TrTagTextualComment).Comment + "] ";
                                    }
                                }
                            }

                            // hvis det aktuelle IKKE er et Datetag, kan man herefter
                            // fjerne oprindelig tekst og erstatte med NewContent
                            //Debug.Print($"GetExpText: udfører skift");
                            if (tagLength == Length)
                            {
                                temp = newContent;
                            }
                            else if (offset >= 0 && tagLength < Length)
                            {
                                if (tagLength + offset > temp.Length)
                                {
                                    Debug.Print($"FATAL ERROR: Offset: {offset}, Taglength: {tagLength}, Textequiv.Length: {TextEquiv.Length}");
                                }
                                else
                                {
                                    temp = temp.Remove(offset, tagLength);
                                    if (newContent != string.Empty && offset < Length)
                                    {
                                        temp = temp.Insert(offset, newContent);
                                    }
                                }
                            }

                            // CurrentDelta sættes; den indeholder forskellen i længde på gammelt og nyt indhold
                            currentDelta = newContent.Length - oldContent.Length; // tidl. - taglength

                            //Debug.Print($"GetExpText: Std tag - currentdelta: {CurrentDelta}");
                            //Debug.WriteLine($"GetExpText #1 - Type: {T.Type}, Offset: {Offset}, TagLength: {TagLength}, TextLength: {this.Length}, TempLength: {temp.Length}, OldContent: {OldContent}, NewContent: {NewContent}");

                            // Nu er problemet så, at tags længere ude i strengen har forkerte Offset og Length-værdier
                            // Det ordnes ved at gennemløbe disse og sætte deres Delta-værdi
                            if (currentDelta != 0)
                            {
                                Tags.Move(offset, currentDelta, false);
                            }
                        }
                    }

                    // --- 4 --- KUN DATE! -------------------------------------------------------------------
                    // i eget (andet) gennemløb:
                    for (int i = max - 1; i >= 0; i--)
                    {
                        //Debug.Print($"GetExpText: 2. gennemløb");
                        TrTag t = textualTags[i];

                        if ((t as TrTagTextual).IsDate)
                        {
                            //Debug.Print($"GetExpText: is date (should be) temp = _{temp}_");
                            offset = (t as TrTagTextual).Offset;
                            tagLength = (t as TrTagTextual).Length;

                            // kun de datetags, som ikke allerede i 1. gennemløb er fixet, skal ordnes
                            if (!(t as TrTagTextualDate).Resolved)
                            {
                                //Debug.Print($"GetExpText: udfører skift");
                                newContent = (t as TrTagTextualDate).ExpandedDate;

                                if (tagLength == Length)
                                {
                                    temp = newContent;
                                }
                                else if (offset >= 0 && tagLength < Length)
                                {
                                    temp = temp.Remove(offset, tagLength);
                                    if (newContent != string.Empty && offset < Length)
                                    {
                                        temp = temp.Insert(offset, newContent);
                                    }
                                }
                            }
                        }

                        //Debug.WriteLine($"GetExpText #2 - Type: {T.Type}, Offset: {Offset}, TagLength: {TagLength}, TextLength: {this.Length}, TempLength: {temp.Length}, OldContent: {OldContent}, NewContent: {NewContent}");
                    }
                }
            }

            // for ikke at forplumre næste kald til GetExp... skal alle Deltaværdier på alle Tags nulstilles!
            foreach (TrTag t in Tags)
            {
                t.DeltaOffset = 0;
                t.DeltaLength = 0;
            }

            // uanset øvrigt udfald, skal temp-strengen trimmes og evt. raffineres.
            temp = temp.Trim();

            if (refine)
            {
                temp = TrLibrary.RefineText(temp, convertOtrema);
            }

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
        //            TrTags TextualTags = new TrTags();

        //            foreach (TrTag T in Tags)
        //            {
        //                // hvis det er sic eller abbrev, skal der helt klart hentes noget andet frem
        //                if (T.GetType() == typeof(TrTagTextualSic) || T.GetType() == typeof(TrTagTextualAbbrev))
        //                {
        //                    TextualTags.Add(T);
        //                    // Debug.Print($"Tag: {(T as TrTagTextual).Type}, {(T as TrTagTextual).Offset}, {(T as TrTagTextual).Length}, {(T as TrTagTextual).Content}");
        //                }
        //                // men hvis det er gennemstreget tekst, skal der slettes tekst
        //                else if (T.GetType() == typeof(TrTagTextualStyle))
        //                {
        //                    // Debug.Print($"Found Styletag!!!");
        //                    if ((T as TrTagTextualStyle).Strikethrough)
        //                    {
        //                        // Debug.Print($"Adding strikethrough tag: Offset = {(T as TrTagTextualStyle).Offset}");
        //                        TextualTags.Add(T);
        //                    }
        //                }
        //                else if (T.GetType() == typeof(TrTagTextualDate))
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
        //                TrTag T = TextualTags[i];
        //                Offset = (T as TrTagTextual).Offset;
        //                TagLength = (T as TrTagTextual).Length;

        //                //if (TagLength < this.Length)
        //                {
        //                    if (T.GetType() == typeof(TrTagTextualSic))
        //                        NewContent = (T as TrTagTextualSic).Correction;
        //                    else if (T.GetType() == typeof(TrTagTextualAbbrev))
        //                        NewContent = (T as TrTagTextualAbbrev).Expansion;
        //                    else if (T.GetType() == typeof(TrTagTextualStyle))
        //                        NewContent = ""; // for så er det strikethrough, dvs. der skal slettes.
        //                    else if (T.GetType() == typeof(TrTagTextualDate))
        //                        NewContent = (T as TrTagTextualDate).ExpandedDate;

        //                    //Debug.WriteLine($"GetExpText ** Type: {T.Type}, Offset: {Offset}, TagLength: {TagLength}, TextLength: {this.Length}, TempLength: {temp.Length}, OldContent: {TextEquiv.Substring(Offset, TagLength)}, NewContent: {NewContent}");

        //                    // if (NewContent != null)
        //                    {
        //                        // i 1. gennemløb bruger vi TAGGETs Offset og Length
        //                        // men kun hvis TagLength er mindre end temp-length (HER ER NOGET GALT)
        //                        if (TagLength <= temp.Length)
        //                        {
        //                            // hvis dato skal det helt udskiftes
        //                            if (T.GetType() == typeof(TrTagTextualDate))
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
        //            //    TrTag T = TextualTags[i];
        //            //    Offset = (T as TrTagTextual).Offset;
        //            //    TagLength = (T as TrTagTextual).Length;

        //            //    //if (Offset == 0 && TagLength == this.Length)
        //            //    {
        //            //        //if (T.GetType() == typeof(TrTagTextualSic))
        //            //        //    NewContent = (T as TrTagTextualSic).Correction;
        //            //        //else if (T.GetType() == typeof(TrTagTextualAbbrev))
        //            //        //    NewContent = (T as TrTagTextualAbbrev).Expansion;
        //            //        //else if (T.GetType() == typeof(TrTagTextualStyle))
        //            //        //    NewContent = ""; // for så er det strikethrough, dvs. der skal slettes.
        //            //        //else
        //            //        if (T.GetType() == typeof(TrTagTextualDate))
        //            //            NewContent = (T as TrTagTextualDate).ExpandedDate;

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
        //        temp = TrLibrary.RefineText(temp, ConvertOtrema);

        //    return temp;
        //}

        // genbrugte
        public ContentType ContentType = ContentType.Undefined;

        // public string[] TagStrings;
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // endnu nyere constructor til indlæsning af Xdoc
        public TrTextLine(string tID, string tTags, string tLineCoords, string tBaseLineCoords, string tTextEquiv, TrTextLines parentContainer)
        {
            ID = tID;
            TagString = tTags;
            ReadingOrder = TrLibrary.GetReadingOrder(TagString);
            CoordsString = tLineCoords;
            BaseLineCoordsString = tBaseLineCoords;
            TextEquiv = tTextEquiv;
            ParentContainer = parentContainer;
            ParentRegion = ParentContainer.ParentRegion;
            if (ParentRegion == null)
            {
                Debug.Print($"TextLine constructor: ParentRegion = null");
            }

            //Debug.WriteLine("#1!");
            Tags.ParentRegion = ParentRegion;
            Tags.ParentLine = this;

            //Debug.WriteLine("#2!") ;
            Tags.LoadFromCustomAttribute(tTags);

            //Debug.WriteLine("#3!");
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

            // Debug.WriteLine("Textline created!");
        }

        public List<string> GetTextualTags()
        {
            List<string> temp = new List<string>();
            StringBuilder sb = new StringBuilder();

            if (HasTags)
            {
                foreach (TrTag t in Tags)
                {
                    sb.Clear();

                    if (t.GetType() == typeof(TrTagTextual) || t.GetType() == typeof(TrTagTextualAbbrev) || t.GetType() == typeof(TrTagTextualSic) || t.GetType() == typeof(TrTagTextualUnclear) || t.GetType() == typeof(TrTagTextualDate))
                    {
                        sb.Append("Type: ");
                        sb.Append(t.Type);

                        sb.Append(", Offset: ");
                        sb.Append((t as TrTagTextual).Offset);
                        sb.Append(", Length: ");
                        sb.Append((t as TrTagTextual).Length);
                    }

                    if (t.GetType() == typeof(TrTagTextualAbbrev))
                    {
                        sb.Append(", Expansion: ");
                        sb.Append((t as TrTagTextualAbbrev).Expansion);
                    }
                    else if (t.GetType() == typeof(TrTagTextualSic))
                    {
                        sb.Append(", Correction: ");
                        sb.Append((t as TrTagTextualSic).Correction);
                    }
                    else if (t.GetType() == typeof(TrTagTextualUnclear))
                    {
                        sb.Append(", Alternative: ");
                        sb.Append((t as TrTagTextualUnclear).Alternative);
                        sb.Append(", Reason: ");
                        sb.Append((t as TrTagTextualUnclear).Reason);
                    }

                    if (t.GetType() == typeof(TrTagTextualDate))
                    {
                        sb.Append(", Year: ");
                        sb.Append((t as TrTagTextualDate).Year);
                        sb.Append(", Month: ");
                        sb.Append((t as TrTagTextualDate).Month);
                        sb.Append(", Day: ");
                        sb.Append((t as TrTagTextualDate).Day);
                    }

                    if (t.IsEmpty)
                    {
                        sb.Append(" (Empty)");
                    }
                    else
                    {
                        sb.Append(" (OK)");
                    }

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
                foreach (TrTag t in Tags)
                {
                    sb.Clear();
                    sb.Append("Type: ");
                    sb.Append(t.Type);
                    foreach (TrTagProperty tP in t.Properties)
                    {
                        sb.Append(", Name: ");
                        sb.Append(tP.Name);
                        sb.Append(", Value: ");
                        sb.Append(tP.Value);
                    }

                    temp.Add(sb.ToString());
                }
            }

            return temp;
        }

        private bool hasBaseLine;

        public bool HasBaseLine
        {
            get
            {
                hasBaseLine = BaseLineCoordsString != string.Empty;
                return hasBaseLine;
            }
        }

        private bool hasCoords;

        public bool HasCoords
        {
            get
            {
                hasCoords = CoordsString != string.Empty;
                return hasCoords;
            }
        }

        private bool hasTags;

        public bool HasTags
        {
            get
            {
                hasTags = Tags.Count > 0;
                return hasTags;
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

        private string structuralTagValue;

        public string StructuralTagValue
        {
            get
            {
                if (HasStructuralTag && StructuralTag != null)
                {
                    structuralTagValue = StructuralTag.SubType;
                }
                else
                {
                    structuralTagValue = string.Empty;
                }

                return structuralTagValue;
            }

            set
            {
                if (structuralTagValue != value)
                {
                    structuralTagValue = value;
                    NotifyPropertyChanged("StructuralTagValue");
                }
            }
        }

        public bool HasSpecificStructuralTag(string tagName)
        {
            if (HasStructuralTag)
            {
                if (StructuralTagValue == tagName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void RenameStructuralTag(string oldName, string newName)
        {
            if (HasStructuralTag)
            {
                if (StructuralTag.SubType == oldName)
                {
                    StructuralTag.SubType = newName;
                    StructuralTagValue = newName;
                    HasChanged = true;
                    NotifyPropertyChanged("StructuralTag");
                    NotifyPropertyChanged("StructuralValue");
                }
            }
        }

        public void AddStructuralTag(string tagName, bool overWrite)
        {
            bool processThis;
            if (overWrite)
            {
                processThis = true;
            }
            else
            {
                processThis = !HasStructuralTag;
            }

            if (processThis)
            {
                TrTagStructural newTag = new TrTagStructural(tagName);
                Tags.Add(newTag);
                StructuralTag = newTag;
                StructuralTagValue = tagName;
                HasChanged = true;
                NotifyPropertyChanged("StructuralTag");
                NotifyPropertyChanged("StructuralValue");
                NotifyPropertyChanged("HasStructuralTag");
                NotifyPropertyChanged("HasTags");
            }
        }

        public void DeleteStructuralTag()
        {
            bool foundTag = false;

            if (HasStructuralTag)
            {
                foundTag = true;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagStructural))
                    {
                        t.MarkToDeletion = true;
                    }
                }

                StructuralTag = null;
                StructuralTagValue = string.Empty;
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
                NotifyPropertyChanged("StructuralTag");
                NotifyPropertyChanged("StructuralValue");
                NotifyPropertyChanged("HasStructuralTag");
                NotifyPropertyChanged("HasTags");
            }
        }

        public void AddAbbrevTag(int offset, int length, string expansion)
        {
            // Debug.Print("Tags before: " + Tags.ToString());
            TrTagTextual newTag = new TrTagTextualAbbrev(offset, length, expansion);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("HasAbbrevTag");
            NotifyPropertyChanged("ExpandedText");
            Debug.Print("Tag added! " + Tags.ToString());
        }

        public void AddSicTag(int offset, int length, string correction)
        {
            TrTagTextual newTag = new TrTagTextualSic(offset, length, correction);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("HasSicTag");
            NotifyPropertyChanged("ExpandedText");
        }

        public void AddUnclearTag(int offset, int length, string alternative, string reason)
        {
            TrTagTextual newTag = new TrTagTextualUnclear(offset, length, alternative, reason);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
        }

        public void AddCustomTextualTag(string tagName, int offset, int length)
        {
            TrTagTextual newTag = new TrTagTextual(tagName, offset, length);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
        }

        public void AddDateTag(int offset, int length, DateTime date)
        {
            TrTagTextualDate newTag = new TrTagTextualDate(offset, length, date);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            Debug.Print($"Datetag added as DATE: {newTag.ExpandedDate}");
        }

        public void AddDateTag(int offset, int length, int day, int month, int year)
        {
            TrTagTextualDate newTag = new TrTagTextualDate(offset, length, day, month, year);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            Debug.Print($"Datetag added as INTs: {newTag.ExpandedDate}");
        }

        public void AddRomanNumeralTag(int offset, int length, string romanValue)
        {
            TrTagTextualRomanNumeral newTag = new TrTagTextualRomanNumeral(offset, length, romanValue);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("ExpandedText");
            Debug.Print("Tag added! " + Tags.ToString());
        }

        public void AddStyleTag(int offset, int length, string type)
        {
            TrTagTextualStyle newTag = new TrTagTextualStyle(offset, length, type);
            Tags.Add(newTag);
            HasChanged = true;
            NotifyPropertyChanged("HasTags");
            NotifyPropertyChanged("ExpandedText");
            Debug.Print("Tag added! " + Tags.ToString());
        }

        public void DeleteDateTags()
        {
            bool foundTag = false;

            if (HasTags)
            {
                foundTag = true;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualDate))
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
                NotifyPropertyChanged("HasTags");
                NotifyPropertyChanged("ExpandedText");
            }
        }

        public void DeleteSicAndAbbrevTags()
        {
            bool foundTag = false;

            if (HasTags)
            {
                foundTag = true;
                foreach (TrTag t in Tags)
                {
                    if (t.GetType() == typeof(TrTagTextualSic) || t.GetType() == typeof(TrTagTextualAbbrev))
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
                NotifyPropertyChanged("HasTags");
                NotifyPropertyChanged("HasAbbrevTags");
                NotifyPropertyChanged("HasSicTags");
                NotifyPropertyChanged("ExpandedText");
            }
        }

        public void ExtendRight(int amount)
        {
            // Debug.WriteLine($"TrTextLine : ExtendRight");
            if (HasCoords && HasBaseLine)
            {
                int newX;

                TrCoords c = new TrCoords(BaseLineCoordsString);
                int rightMostX = c.GetRightMostXcoord();
                int rightMostY = c.GetRightMostYcoord();

                if (rightMostX + amount < ParentRegion.ParentTranscript.ParentPage.Width)
                {
                    newX = rightMostX + amount;
                }
                else
                {
                    newX = ParentRegion.ParentTranscript.ParentPage.Width;
                }

                TrCoord newCoord = new TrCoord(newX, rightMostY);
                c.Add(newCoord);
                BaseLineCoordsString = c.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
            }
        }

        public void ExtendLeft(int amount)
        {
            // Debug.WriteLine($"TrTextLine : ExtendLeft");
            if (HasCoords && HasBaseLine)
            {
                int newX;

                TrCoords c = new TrCoords(BaseLineCoordsString);
                int leftMostX = c.GetLeftMostXcoord();
                int leftMostY = c.GetLeftMostYcoord();

                if (leftMostX - amount > 0)
                {
                    newX = leftMostX - amount;
                }
                else
                {
                    newX = 0;
                }

                TrCoord newCoord = new TrCoord(newX, leftMostY);
                c.Add(newCoord);
                c.Sort();
                BaseLineCoordsString = c.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
            }
        }

        public void Move(int horizontally, int vertically)
        {
            if (HasCoords && HasBaseLine)
            {
                // The line itself
                TrCoords l = new TrCoords(CoordsString);
                foreach (TrCoord currentCoord in l)
                {
                    currentCoord.X = currentCoord.X + horizontally;
                    currentCoord.Y = currentCoord.Y + vertically;
                }

                CoordsString = l.ToString();

                // and then the baseline
                TrCoords c = new TrCoords(BaseLineCoordsString);
                c.Sort();
                foreach (TrCoord currentCoord in c)
                {
                    currentCoord.X = currentCoord.X + horizontally;
                    currentCoord.Y = currentCoord.Y + vertically;
                }

                BaseLineCoordsString = c.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
            }
        }

        private bool isCoordinatesPositive;

        public bool IsCoordinatesPositive
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    isCoordinatesPositive = TrLibrary.CheckBaseLineCoordinates(BaseLineCoordsString);
                }
                else
                {
                    isCoordinatesPositive = false;
                }

                return isCoordinatesPositive;
            }
        }

        private bool isBaseLineStraight;

        public bool IsBaseLineStraight
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    isBaseLineStraight = TrLibrary.CheckBaseLineStraightness(BaseLineCoordsString, MaxAllowedBaseLineAngle);
                }
                else
                {
                    isBaseLineStraight = false;
                }

                return isBaseLineStraight;
            }
        }

        private bool isBaseLineDirectionOK;

        public bool IsBaseLineDirectionOK
        {
            get
            {
                if (HasCoords && HasBaseLine)
                {
                    isBaseLineDirectionOK = TrLibrary.CheckBaseLineDirection(BaseLineCoordsString);
                }
                else
                {
                    isBaseLineDirectionOK = false;
                }

                return isBaseLineDirectionOK;
            }
        }

        public void LimitCoordsToPage()
        {
            if (HasCoords && HasBaseLine)
            {
                TrCoords newCoords = new TrCoords(BaseLineCoordsString);
                foreach (TrCoord c in newCoords)
                {
                    if (c.X < 0)
                    {
                        c.X = 0;
                    }

                    if (c.X > ParentRegion.ParentTranscript.ParentPage.Width)
                    {
                        c.X = ParentRegion.ParentTranscript.ParentPage.Width;
                    }
                }

                BaseLineCoordsString = newCoords.ToString();
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
                int newY = TrLibrary.GetAverageYcoord(BaseLineCoordsString);

                TrCoords newCoords = new TrCoords(BaseLineCoordsString);
                foreach (TrCoord c in newCoords)
                {
                    c.Y = newY;
                }

                BaseLineCoordsString = newCoords.ToString();
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
                TrCoords newCoords = new TrCoords(BaseLineCoordsString);
                newCoords.Sort();
                BaseLineCoordsString = newCoords.ToString();
                HasChanged = true;
                NotifyPropertyChanged("BaseLineCoordsString");
                NotifyPropertyChanged("VisualBaseLine");
                NotifyPropertyChanged("IsBaseLineDirectionOK");
            }
        }

        public void FixLineCoordinates()
        {
            if (HasCoords)
            {
                CoordsString = FixCoordinates(CoordsString);
            }
        }

        public void FixBaseLineCoordinates()
        {
            if (HasBaseLine)
            {
                BaseLineCoordsString = FixCoordinates(BaseLineCoordsString);
            }
        }

        private string FixCoordinates(string coords)
        {
            int pageWidth = ParentRegion.ParentTranscript.ParentPage.Width;
            int pageHeigth = ParentRegion.ParentTranscript.ParentPage.Height;

            TrCoords currentCoords = new TrCoords(coords);

            string temp = coords;

            // ændrer punkter med negative koordinater eller større end siden
            foreach (TrCoord c in currentCoords)
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
                temp = currentCoords.ToString();
            }

            return temp;
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
                        int lastRealCharPosition = TextEquiv.Length - 1;
                        while (char.IsWhiteSpace(TextEquiv[lastRealCharPosition]))
                        {
                            lastRealCharPosition--;
                        }

                        string temp = TextEquiv.Substring(0, lastRealCharPosition + 1);
                        int difference = temp.Length - TextEquiv.Length;

                        //Debug.Print($"Length: {TextEquiv.Length}, lastrealcharpos: {LastRealCharPosition}, difference: {Difference}");
                        if (difference != 0)
                        {
                            Tags.Move(lastRealCharPosition + 1, difference, true);
                            TextEquiv = temp;
                            HasChanged = true;
                        }
                    }
                }

                if (TextEquiv.StartsWith(" "))
                {
                    //Debug.Print($"Line starts with space");
                    if (TextEquiv.Length > 0)
                    {
                        int firstRealCharPosition = 0;
                        while (char.IsWhiteSpace(TextEquiv[firstRealCharPosition]))
                        {
                            firstRealCharPosition++;
                        }

                        string temp = TextEquiv.Substring(firstRealCharPosition);
                        int difference = temp.Length - TextEquiv.Length;

                        //Debug.Print($"Length: {TextEquiv.Length}, firstrealcharpos: {FirstRealCharPosition}, difference: {Difference}");
                        if (difference != 0)
                        {
                            Tags.Move(firstRealCharPosition - 1, difference, true);
                            TextEquiv = temp;
                            HasChanged = true;
                        }
                    }
                }
            }
        }

        public void SimplifyBoundingBox()
        {
            if (HasCoords && HasBaseLine)
            {
                ExecuteSimplifyBoundingBox(TopBorder, BottomBorder, LeftBorder, RightBorder);
            }
            else
            {
                Debug.WriteLine($"ERROR: Doc: {ParentDocTitle}, Page: {ParentPageNr}, Line: {ParentRegionNr}-{Number} has corrupt format!");
            }
        }

        public void SimplifyBoundingBox(int minimumHeight, int maximumHeight)
        {
            if (HasCoords && HasBaseLine)
            {
                int actualHeight = BoundingBoxHeight;

                int topDelta = (int)((double)maximumHeight * 0.8);
                int bottomDelta = (int)((double)maximumHeight * 0.2);

                int topBorderValue;
                int bottomBorderValue;

                if (actualHeight < minimumHeight || actualHeight > maximumHeight)
                {
                    // firefemtedele over linien - enfemtedel under
                    topBorderValue = Vpos - topDelta;
                    bottomBorderValue = Vpos + bottomDelta;
                }
                else
                {
                    topBorderValue = TopBorder;
                    bottomBorderValue = BottomBorder;
                }

                ExecuteSimplifyBoundingBox(topBorderValue, bottomBorderValue, LeftBorder, RightBorder);
            }
            else
            {
                Debug.WriteLine($"ERROR: Doc: {ParentDocTitle}, Page: {ParentPageNr}, Line: {ParentRegionNr}-{Number} has corrupt format!");
            }
        }

        private void ExecuteSimplifyBoundingBox(int top, int bottom, int left, int right)
        {
            if (top < 0)
            {
                top = 0;
            }

            if (left < 0)
            {
                left = 0;
            }

            if (bottom > ParentRegion.ParentTranscript.ParentPage.Height)
            {
                bottom = ParentRegion.ParentTranscript.ParentPage.Height;
            }

            if (right > ParentRegion.ParentTranscript.ParentPage.Width)
            {
                right = ParentRegion.ParentTranscript.ParentPage.Width;
            }

            TrCoord leftTop = new TrCoord(left, top);
            TrCoord leftBottom = new TrCoord(left, bottom);
            TrCoord rightTop = new TrCoord(right, top);
            TrCoord rightBottom = new TrCoord(right, bottom);

            TrCoords newCoords = new TrCoords();

            newCoords.Add(leftBottom);
            newCoords.Add(rightBottom);
            newCoords.Add(rightTop);
            newCoords.Add(leftTop);

            CoordsString = newCoords.ToString();
            HasChanged = true;
            NotifyPropertyChanged("CoordsString");
            NotifyPropertyChanged("VisualLineArea");
        }

        public XElement ToXML()
        {
            // string CustomString = "readingOrder {index:" + ReadingOrder.ToString() + ";}";
            string customString = Tags.ToString();

            XElement xLine = new XElement(
                TrLibrary.Xmlns + "TextLine",
                new XAttribute("id", ID),
                new XAttribute("custom", customString),
                new XElement(
                    TrLibrary.Xmlns + "Coords",
                    new XAttribute("points", CoordsString)),
                new XElement(
                    TrLibrary.Xmlns + "Baseline",
                    new XAttribute("points", BaseLineCoordsString)),
                new XElement(
                    TrLibrary.Xmlns + "TextEquiv",
                    new XElement(TrLibrary.Xmlns + "Unicode", TextEquiv)));

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
            var line = obj as TrTextLine;
            switch (ParentContainer.SortMethod)
            {
                case TrTextLines.SortType.Vertically:
                    return VerticalOrder.CompareTo(line.VerticalOrder);
                case TrTextLines.SortType.Horizontally:
                    return HorizontalOrder.CompareTo(line.HorizontalOrder);
                case TrTextLines.SortType.Logically:
                    return LogicalOrder.CompareTo(line.LogicalOrder);
                default:
                    return ReadingOrder.CompareTo(line.ReadingOrder);
            }
        }
    }
}
