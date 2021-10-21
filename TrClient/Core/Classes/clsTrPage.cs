using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Http;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using TrClient;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using DanishNLP;

namespace TrClient
{
    public class clsTrPage : IComparable, INotifyPropertyChanged
    {
        public string ID { get; set; }
        public int PageNr { get; set; }
        public string ImageFileName { get; set; }

        public string ImageURL { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string PageFileName { get; set; }
        public int AssumedRowCount { get; set; }
        public double ActualRowHeight { get; set; }
        public clsTrTranscripts Transcripts = new clsTrTranscripts();

        public clsTrPages ParentContainer;
        public clsTrDocument ParentDocument;

        public BitmapImage PageImage { get; set; }

        private int _transcriptCount;
        public int TranscriptCount
        {
            get
            {
                _transcriptCount = Transcripts.Count;
                return _transcriptCount;
            }
        }

        private bool _hasTables;
        public bool HasTables
        {
            get
            {
                _hasTables = Transcripts[0].HasTables;
                return _hasTables;
            }
        }

        private bool _hasFormerTables;
        public bool HasFormerTables
        {
            get
            {
                _hasFormerTables = false;
                if (TranscriptCount > 1)
                {
                    clsTrTranscript Transcript;
                    for (int i = 0; i < TranscriptCount; i++)
                    {
                        // Debug.WriteLine($"Page {PageNr}, Transcript {i}:");
                        Transcript = Transcripts[i];
                        foreach (clsTrRegion Region in Transcript.Regions)
                        {
                            _hasFormerTables = _hasFormerTables || (Region.GetType() == typeof(clsTrTableRegion));
                            // Debug.WriteLine($"Region {Region.Number}, Type {Region.GetType().ToString()}");
                        }
                        //if (Tra.Regions.Count > 0)
                        //{

                        //}
                       
                    }
                }
                return _hasFormerTables;
            }
        }

        public void ConvertTablesToRegions()
        {
            if (HasRegions)
            {
                Debug.WriteLine($"Page {PageNr}: Calling convert tables");
                Transcripts[0].ConvertTablesToRegions();
            }
                
        }

        public void CopyOldTablesToNewestTranscript()
        {
            // NB: Kopierer også, selv om der er tabeller i SENESTE transcript - derfor er dette tjek lagt ud i DOCUMENT

            if (HasRegions && HasFormerTables)
            {
                bool TablesFound = false;
                int TranscriptNumberWithTables = 0;

                clsTrTranscript NewestTranscript = Transcripts[0];
                clsTrTranscript FormerTranscript = null;

                // find latest transcript with tables
                if (TranscriptCount > 1)
                {
                    for (int i = 1; i < TranscriptCount; i++)
                    {
                        FormerTranscript = Transcripts[i];
                        if (FormerTranscript.HasTables)
                        {
                            TablesFound = true;
                            TranscriptNumberWithTables = i;
                            Debug.WriteLine($"Page {PageNr}: Found former table in transcript # {TranscriptNumberWithTables}");
                            break;
                        }
                    }
                }
                
                if (TablesFound)
                {
                    // FormerTranscript er nu det seneste transcript med tables
                    foreach (clsTrRegion FormerRegion in FormerTranscript.Regions)
                    {
                        if (FormerRegion.GetType() == typeof(clsTrTableRegion))
                        {
                            Debug.WriteLine($"Page {PageNr}: Copying former table from transcript # {TranscriptNumberWithTables}");
                            NewestTranscript.Regions.Add(FormerRegion);
                            NewestTranscript.HasChanged = true;
                        }
                    }
                }
                else
                {
                    // så er noget gået galt
                    Debug.WriteLine($"Page {PageNr}: Something went wrong: This page should have had former tables, but they were not found!");
                }
            }
        }



        private bool _isPageImageLoaded = false;
        public bool IsPageImageLoaded
        {
            get
            { return _isPageImageLoaded; }
            set
            {
                if (_isPageImageLoaded != value)
                {
                    _isPageImageLoaded = value;
                    NotifyPropertyChanged("IsPageImageLoaded");
                }
            }
        }

        private double _assumedRowHeight;
        public double AssumedRowHeight
        {
            get
            {
                if (AssumedRowCount != 0)
                    _assumedRowHeight = (double)Height / (double)AssumedRowCount;
                else
                    _assumedRowHeight = 0;
                return _assumedRowHeight;
            }
        }

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
                NotifyPropertyChanged("HasChanged");
                if (_hasChanged)
                    StatusColor = Brushes.Orange;
                ParentDocument.HasChanged = value;
            }
        }

        private bool _changesUploaded = false;
        public bool ChangesUploaded
        {
            get { return _changesUploaded; }
            set
            {
                _changesUploaded = value;
                NotifyPropertyChanged("ChangesUploaded");
                if (_changesUploaded)
                    StatusColor = Brushes.DarkViolet;
                ParentDocument.ChangesUploaded = value;
            }
        }

        private bool _hasRegions;
        public bool HasRegions
        {
            get
            {
                _hasRegions = Transcripts[0].HasRegions;
                return _hasRegions;
            }
        }

        public bool ExistsRegionNumber(int RegionNumber)
        {
            bool temp = false;
            if (HasRegions)
                if (RegionNumber <= Transcripts[0].Regions.Count)
                    temp = true;
            return temp;
        }

        public void LoadImage()
        {
            if (!IsPageImageLoaded)
            {
                PageImage = new BitmapImage();
                PageImage.BeginInit();
                PageImage.CacheOption = BitmapCacheOption.OnLoad;
                PageImage.UriSource = new Uri(ImageURL);
                PageImage.EndInit();

                PageImage.DownloadCompleted += new EventHandler(
                    (object xsender, EventArgs xe) =>
                    {
                        BitmapImage readySrc = (BitmapImage)xsender;
                        PageImage = readySrc;
                        IsPageImageLoaded = true;
                    });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public int CompareTo(object obj)
        {
            var page = obj as clsTrPage;
            return PageNr.CompareTo(page.PageNr);
        }

        // constructor ONLINE
        public clsTrPage(string PageID, int iPageNr, string PageFileName, string ImageFileURL, int PageW, int PageH)
        {
            ID = PageID;
            PageNr = iPageNr;
            ImageFileName = PageFileName;
            ImageURL = ImageFileURL;
            Width = PageW;
            Height = PageH;

            Transcripts.ParentPage = this;
            IsLoaded = false;
        }

        // constructor OFFLINE
        public clsTrPage(string PageID, int iPageNr, string PageFileName, string ImageFileURL, int PageW, int PageH, string PageFile)
        {
            ID = PageID;
            PageNr = iPageNr;
            ImageFileName = PageFileName;
            ImageURL = ImageFileURL;
            Width = PageW;
            Height = PageH;
            PageFileName = PageFile;

            Transcripts.ParentPage = this;
            IsLoaded = false;

            // Debug.WriteLine("Page created! ");





        }


        //public void SetColumnNumbers(int AssumedNumberOfColumns)
        //{
        //    if (HasRegions)
        //    {
        //        clsTrTranscript Transcript = Transcripts[0];
        //        if (Transcript.Regions.Count == 1)
        //        {
        //            clsTrTextRegion TR = Transcript.Regions[0];
        //            clsTrCoords LinePositions = new clsTrCoords();

        //            foreach (clsTrTextLine TL in TR.TextLines)
        //            {
        //                int X = TL.Hpos;
        //                int Y = TL.Vpos;
        //                clsTrCoord Position = new clsTrCoord(X,Y);
        //                LinePositions.Add(Position);
        //            }

        //            string FileName = clsTrLibrary.ExportFolder + "LinePositions_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";

        //            using (StreamWriter TextFile = new StreamWriter(FileName, true))
        //            {
        //                TextFile.WriteLine(PageFileName);
        //                foreach (clsTrCoord Position in LinePositions)
        //                {
        //                    TextFile.WriteLine(Position.ToString());
        //                }
        //            }
        //        }
        //    }
        //}

        //public void SetRowNumbers(int AssumedNumberOfRows)
        //{
        //    if (HasRegions)
        //    {
        //        clsTrTranscript Transcript = Transcripts[0];
        //        if (Transcript.Regions.Count == 1)
        //        {
        //            clsTrTextRegion TR = Transcript.Regions[0];
        //            List<double> RowsDeltaV = new List<double>();

        //            AssumedRowCount = AssumedNumberOfRows;
        //            Debug.WriteLine($"RowsDeltaV.count = {RowsDeltaV.Count}, Assumed RowHeight = {AssumedRowHeight}");
        //            double LowerBoundary = 0.8 * AssumedRowHeight;
        //            double UpperBoundary = 1.2 * AssumedRowHeight;
        //            Debug.WriteLine($"LowerB = {LowerBoundary}, UpperB = {UpperBoundary}");

        //            int Max = TR.TextLines.Count - 1;
        //            for (int i = 0; i < Max; i++)
        //            {
        //                clsTrTextLine CurrentLine = TR.TextLines[i];
        //                clsTrTextLine NextLine = TR.TextLines[i + 1];
        //                double DeltaV = NextLine.Vpos - CurrentLine.Vpos;
        //                Debug.WriteLine($"CurrentLineVpos = {CurrentLine.Vpos}, DeltaV = {DeltaV}");

        //                if (DeltaV > LowerBoundary && DeltaV < UpperBoundary)
        //                {
        //                    RowsDeltaV.Add(DeltaV);
        //                }
        //            }
        //            if (RowsDeltaV.Count > 0)
        //                ActualRowHeight = Convert.ToInt32(RowsDeltaV.Average());
        //            else
        //                ActualRowHeight = AssumedRowHeight;

        //            Debug.WriteLine($"RowsDeltaV.count = {RowsDeltaV.Count}, Actual RowHeight = {ActualRowHeight}");

        //            //foreach (clsTrTextLine TL in TR.TextLines)
        //            //{
        //            //    TL.RowNumber = (int)(Convert.ToDouble(TL.Vpos) / ActualRowHeight);
        //            //    Debug.WriteLine($"Row = {TL.RowNumber}, Vpos = {TL.Vpos}, Hpos = {TL.Hpos}");
        //            //}
                      



        //        }
        //    }
        //}

        public void DeleteRegionsWithTag(string RegionalTagValue)
        {
            clsTrTranscript Transcript = Transcripts[0];
            foreach (clsTrRegion TR in Transcript.Regions)
            {
                // if (TR.HasStructuralTag && TR.StructuralTagValue != RegionalTagValue)

                if (TR.StructuralTagValue == RegionalTagValue)
                {
                    TR.MarkToDeletion = true;
                    Debug.WriteLine($"Page# {PageNr}, Region# {TR.Number}: Marked to deletion (struct. tag = {TR.StructuralTagValue}, chosen tag = {RegionalTagValue})");
                }
            }
            for (int i = Transcript.Regions.Count - 1; i >= 0; i--)
            {
                if (Transcript.Regions[i].MarkToDeletion)
                    Transcript.Regions.RemoveAt(i);
            }

        }


        public void DeleteRegionsOtherThan(string RegionalTagValue)
        {
            clsTrTranscript Transcript = Transcripts[0];
            foreach (clsTrRegion TR in Transcript.Regions)
            {
                // if (TR.HasStructuralTag && TR.StructuralTagValue != RegionalTagValue)

                if (TR.StructuralTagValue != RegionalTagValue)
                {
                    TR.MarkToDeletion = true;
                    Debug.WriteLine($"Page# {PageNr}, Region# {TR.Number}: Marked to deletion (struct. tag = {TR.StructuralTagValue}, preserve = {RegionalTagValue})");
                }
            }
            for (int i = Transcript.Regions.Count - 1; i >= 0; i--)
            {
                if (Transcript.Regions[i].MarkToDeletion)
                    Transcript.Regions.RemoveAt(i);
            }

        }

        public void DeleteEmptyRegions()
        {
            //Debug.WriteLine($"Page # {PageNr}");
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                foreach (clsTrRegion TR in Transcript.Regions)
                {
                    //Debug.WriteLine($"Region# {TR.Number}");
                    if (!TR.HasLines)
                    {
                        //Debug.WriteLine($"Hasn't got lines!");
                        TR.MarkToDeletion = true;
                    }
                }
                for (int i = Transcript.Regions.Count - 1; i >= 0; i--)
                {
                    if (Transcript.Regions[i].MarkToDeletion)
                        Transcript.Regions.RemoveAt(i);
                }
            }
        }

        public void SortRegions()
        {
            if (HasRegions)
            {
                Transcripts[0].Regions.Sort();
            }

        }

        public void RenumberRegionsVertically()
        {
            if (HasRegions)
                Transcripts[0].Regions.ReNumberVertically();
        }

        public void RenumberRegionsHorizontally()
        {
            if (HasRegions)
                Transcripts[0].Regions.ReNumberHorizontally();
        }



        public void DeleteLinesWithTag(string StructuralTagValue)
        {
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];

                foreach (clsTrRegion TR in Transcript.Regions)
                {
                    if (TR.HasLines)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (TL.StructuralTagValue == StructuralTagValue)
                                {
                                    TL.MarkToDeletion = true;
                                    //Debug.WriteLine($"Page# {PageNr}, Region# {TR.Number}: Marked to deletion (struct. tag = {TR.StructuralTagValue}, chosen tag = {StructuralTagValue})");
                                }
                            }

                            for (int i = (TR as clsTrTextRegion).TextLines.Count - 1; i >= 0; i--)
                            {
                                if ((TR as clsTrTextRegion).TextLines[i].MarkToDeletion)
                                    (TR as clsTrTextRegion).TextLines.RemoveAt(i);
                            }

                        }

                    }
                }
            }
        }


        public void DeleteLinesOtherThan(string StructuralTagValue)
        {
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];

                foreach (clsTrRegion TR in Transcript.Regions)
                {
                    if (TR.HasLines)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (TL.StructuralTagValue != StructuralTagValue)
                                {
                                    TL.MarkToDeletion = true;
                                    //Debug.WriteLine($"Page# {PageNr}, Region# {TR.Number}: Marked to deletion (struct. tag = {TR.StructuralTagValue}, chosen tag = {StructuralTagValue})");
                                }
                            }

                            for (int i = (TR as clsTrTextRegion).TextLines.Count - 1; i >= 0; i--)
                            {
                                if ((TR as clsTrTextRegion).TextLines[i].MarkToDeletion)
                                    (TR as clsTrTextRegion).TextLines.RemoveAt(i);
                            }

                        }


                    }
                }
            }
        }


        public List<string> GetExpandedText(bool Refine, bool ConvertOtrema)
        {
            List<string> TempList = new List<string>();
            List<string> RegionList;

            foreach (clsTrRegion TR in Transcripts[0].Regions)
            {
                RegionList = TR.GetExpandedText(Refine, ConvertOtrema);
                foreach (string s in RegionList)
                    TempList.Add(s);
            }
            return TempList;
        }

        private clsTrWords _words = new clsTrWords();
        public clsTrWords Words
        {
            get
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    foreach (clsTrWord W in TR.Words)
                        _words.Add(W);
                }
                return _words;
            }
        }

        private int _numberOfRegions = 0;
        public int NumberOfRegions
        {
            get
            {
                if (HasRegions)
                {
                    _numberOfRegions = Transcripts[0].Regions.Count;
                }
                return _numberOfRegions;
            }
        }

        private int _numberOfLines = 0;
        public int NumberOfLines
        {
            get
            {
                int temp = 0;
                if (HasRegions)
                {
                    foreach (clsTrRegion TR in Transcripts[0].Regions)
                    {
                        temp = temp + TR.NumberOfLines;
                    }
                }
                _numberOfLines = temp;
                return _numberOfLines;
            }
        }

        // public int NumberOfTranscribedRegions { get; }
        // public int NumberOfTranscribedLines { get; }



        public clsTrTextLines GetLines()
        {
            clsTrTextLines TempList = new clsTrTextLines();

            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            TempList.Add(TL);
                            TL.ParentRegion = (TR as clsTrTextRegion);
                        }

                    }

                }

            }
            return TempList;
        }



        public clsTrParagraphs GetParagraphs()
        {
            clsTrParagraphs TempList = new clsTrParagraphs();
            Debug.WriteLine($"Page:GetP - page: {PageNr}");

            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        Debug.WriteLine($"Page:GetP - region {TR.Number}");
                        clsTrParagraphs TRPS = (TR as clsTrTextRegion).GetParagraphs();
                        if (TRPS != null)
                        {
                            foreach (clsTrParagraph TRP in TRPS)
                            {
                                Debug.WriteLine($"Page:GetP - paragraph {TRP.Number}");
                                TempList.Add(TRP);
                                TRP.ParentRegion = (clsTrTextRegion)TR;
                                Debug.WriteLine($"Page:GetP - name      {TRP.Name}");
                                Debug.WriteLine($"Page:GetP - content   {TRP.Content}");

                            }
                        }

                    }
                }
            }
            return TempList;
        }

        public bool CreateTopLevelRegion()
        {
            if (!HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];

                clsTrTextRegion NewRegion = new clsTrTextRegion(0, 0, GetTopLevelCoords());
                clsTrRegions NewRegions = new clsTrRegions(true);
                NewRegions.Add(NewRegion);
                Transcript.Regions = NewRegions;

                Transcript.HasChanged = true;
                return true;
            }
            else
                return false;
        }

        public void MergeAllRegionsToTopLevelRegion()
        {
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                clsTrTextRegion NewRegion = new clsTrTextRegion(0, 0, GetTopLevelCoords());

                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            NewRegion.TextLines.Add(TL);
                        }
                        TR.MarkToDeletion = true;
                    }
                    else
                    {
                        Debug.WriteLine($"ERROR: Non-Text Region on page {PageNr}");
                    }

                }

                AppendRegion(NewRegion);

                for (int i = Transcript.Regions.Count - 1; i >= 0; i--)
                {
                    if (Transcript.Regions[i].MarkToDeletion)
                        Transcript.Regions.RemoveAt(i);
                }

                Transcript.HasChanged = true;
            }
        }

        public void AppendRegion(clsTrRegion SourceRegion)
        {
            //Debug.WriteLine($"clsTrPage:Append...");

            clsTrTranscript Transcript = Transcripts[0];
            //Debug.WriteLine($"Actual region count before: {Transcript.Regions.Count}");

            if (!HasRegions)
            {
                clsTrRegions NewRegions = new clsTrRegions(true);
                Transcript.Regions = NewRegions;
            }

            // int NewRegionOrder = Transcript.Regions.Count;

            Transcript.Regions.Add(SourceRegion);

            //Debug.WriteLine($"Actual region count after: {Transcript.Regions.Count}");
            HasChanged = true;
        }

        public int AddHorizontalRegion(int UpperPercent, int LowerPercent, int UpperPadding, int LowerPadding)
        {
            clsTrTranscript Transcript = Transcripts[0];

            if (!HasRegions)
            {
                clsTrRegions NewRegions = new clsTrRegions(true);
                Transcript.Regions = NewRegions;
            }

            int NewRegionOrder = Transcript.Regions.Count;
            float Orientation = 0;
            clsTrTextRegion NewRegion = new clsTrTextRegion(NewRegionOrder, Orientation,
                GetCoordsForHorizontalRegion(UpperPercent, LowerPercent, UpperPadding, LowerPadding));
            Transcript.Regions.Add(NewRegion);

            Transcript.HasChanged = true;
            return NewRegion.Number;
        }

        public void DeleteShortBaseLines(clsTrDialogTransferSettings Settings, clsTrLog Log)
        {
            bool ProcessThis = true;
            bool PageIsOK = true;
            string ErrorMessage;

            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                foreach (clsTrRegion Region in Transcript.Regions)
                {
                    if (Settings.AllRegions)
                        ProcessThis = true;
                    else if (Region.Number >= Settings.RegionsFrom && Region.Number <= Settings.RegionsTo)
                        ProcessThis = true;
                    else
                        ProcessThis = false;

                    if (ProcessThis)
                        PageIsOK = PageIsOK && Region.DeleteShortBaselines(Settings.ShortLimit, Log);
                }
                if (PageIsOK)
                {
                    ErrorMessage = "Page is OK.";
                    Log.Add(this, ErrorMessage);
                }
            }
            else
            {
                ErrorMessage = "Page has no regions.";
                Log.Add(this, ErrorMessage);
            }
        }

        public void ExtendBaseLines(clsTrDialogTransferSettings Settings, clsTrLog Log)
        {
            // Debug.WriteLine($"clsTrPage : ExtendBaseLines");

            bool ProcessThis = true;
            string ErrorMessage;

            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                foreach (clsTrRegion Region in Transcript.Regions)
                {
                    if (Settings.AllRegions)
                        ProcessThis = true;
                    else if (Region.Number >= Settings.RegionsFrom && Region.Number <= Settings.RegionsTo)
                        ProcessThis = true;
                    else
                        ProcessThis = false;

                    if (ProcessThis)
                        Region.ExtendBaseLines(Settings, Log);
                }
            }
            else
            {
                ErrorMessage = "Page has no regions.";
                Log.Add(this, ErrorMessage);
            }
        }

        //public void RepairBaseLines(clsTrLog Log)
        //{
        //    bool PageIsOK = true;
        //    string ErrorMessage;

        //    if (HasRegions)
        //    {
        //        clsTrTranscript Transcript = Transcripts[0];
        //        foreach(clsTrTextRegion Region in Transcript.Regions)
        //        {
        //            PageIsOK = PageIsOK && Region.RepairBaseLines(Log);
        //        }
        //        if (PageIsOK)
        //        {
        //            ErrorMessage = "Page is OK.";
        //            Log.Add(this, ErrorMessage);
        //        }
        //    }
        //    else
        //    {
        //        ErrorMessage = "Page has no regions.";
        //        Log.Add(this, ErrorMessage);
        //    }
        //}

        public void SimplifyBoundingBoxes()
        {
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                foreach (clsTrRegion Region in Transcript.Regions)
                    Region.SimplifyBoundingBoxes();
            }
        }

        public void SimplifyBoundingBoxes(int MinimumHeight, int MaximumHeight)
        {
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                foreach (clsTrRegion Region in Transcript.Regions)
                    Region.SimplifyBoundingBoxes(MinimumHeight, MaximumHeight);
            }
        }

        

        public void Move(int Horizontally, int Vertically)
        {
            if (HasRegions)
            {
                clsTrTranscript Transcript = Transcripts[0];
                foreach (clsTrRegion Region in Transcript.Regions)
                {
                    Region.Move(Horizontally, Vertically);
                }
            }
        }



        private string GetTopLevelCoords()
        {
            clsTrCoord UpperLeft = new clsTrCoord(0, 0);
            clsTrCoord UpperRight = new clsTrCoord(Width, 0);
            clsTrCoord LowerRight = new clsTrCoord(Width, Height);
            clsTrCoord LowerLeft = new clsTrCoord(0, Height);

            clsTrCoords CS = new clsTrCoords();
            CS.Add(UpperLeft);
            CS.Add(UpperRight);
            CS.Add(LowerRight);
            CS.Add(LowerLeft);
         
            return CS.ToString();
        }

        private string GetCoordsForHorizontalRegion(int UpperPercent, int LowerPercent, int UpperPadding, int LowerPadding)
        {
            int UpperBoundary;
            int LowerBoundary;

            if (UpperPercent == 0)
                UpperBoundary = 0;
            else
                UpperBoundary = (Height * UpperPercent / 100);

            LowerBoundary = (Height * LowerPercent / 100);

            UpperBoundary = UpperBoundary + UpperPadding;
            LowerBoundary = LowerBoundary - LowerPadding;

            clsTrCoord UpperLeft = new clsTrCoord(0, UpperBoundary);
            clsTrCoord UpperRight = new clsTrCoord(Width, UpperBoundary);
            clsTrCoord LowerRight = new clsTrCoord(Width, LowerBoundary);
            clsTrCoord LowerLeft = new clsTrCoord(0, LowerBoundary);

            clsTrCoords CS = new clsTrCoords();
            CS.Add(UpperLeft);
            CS.Add(UpperRight);
            CS.Add(LowerRight);
            CS.Add(LowerLeft);

            return CS.ToString();
        }

        public void TagEmptyTextLines()
        {
            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.IsEmpty)
                                TL.AddStructuralTag("Empty", true);
                        }
                    }
                }
            }
        }


        public void WrapSuperAndSubscriptWithSpaces()
        {
            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            TL.WrapSuperAndSubscriptWithSpaces();
                        }
                    }
                }
            }
        }

        public void TagEmptyAbbrevTags()
        {
            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.HasEmptyAbbrevTag)
                                TL.AddStructuralTag("EmptyAbbrev", true);
                        }
                    }
                }
            }
        }

        public void KOBACC_AutoTag()
        {
            // sætter Date og Acc-tags på hhv region 1 og 2, hvis der er 3 eller flere regioner
            // hvis der kun er een eller to, sker der ikke noget
            string TagValue = "";

            if (HasRegions)
            {
                if (Transcripts[0].Regions.Count >= 3)
                {
                    foreach (clsTrRegion TR in Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            if (TR.Number == 1)
                                TagValue = "Date";
                            else if (TR.Number == 2)
                                TagValue = "Acc";

                            if (TR.Number < 3)
                            {
                                foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                                {
                                    // det tjekkes, om TextLine primært består af cifre - og kun da sættes tagget
                                    // desuden tjekkes, om antallet af numre er hhv. 1 eller 2, afh. af tagvalue

                                    if (clsLanguageLibrary.DigitCount(TL.TextEquiv) > clsLanguageLibrary.LetterCount(TL.TextEquiv))
                                    {
                                        if ((TagValue == "Date" && clsTrLibrary.UniqueNumbersCount(TL.TextEquiv) >= 2)
                                            || (TagValue == "Acc" && clsTrLibrary.UniqueNumbersCount(TL.TextEquiv) == 1))
                                            TL.AddStructuralTag(TagValue, false);
                                    }
                                }
                            }

                        }

                    }
                }
                else
                    Debug.WriteLine($"{ParentDocument.ParentCollection} / {ParentDocument} / s. {PageNr}: under 3 regioner!");

            }

        }

        public void AutoTagFloorNumberSuperScript()
        {
            if (HasRegions)
            {
                int Offset = 0;
                int Length = 0;

                Regex FloorNumbers = new Regex(@"\b(?<=\s\d{1,3}\.?\s?[A-Z]?\.?\s?)(\d|[IV]+)\.?$");

                // Debug.Print($"Page {PageNr} -------------------------------------------------");

                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            MatchCollection FloorNumberMatches = FloorNumbers.Matches(TL.TextEquiv);

                            if (FloorNumberMatches.Count > 0)
                            {
                                Debug.Print($"Floornumber fundet: page {PageNr}, line {TL.Number}, text: _{TL.TextEquiv}_ - floornumber = {FloorNumberMatches[0].Value}");

                                foreach (Match M in FloorNumberMatches)
                                {
                                    TL.AddStyleTag(M.Index, M.Value.Length, "superscript");
                                }

                            }
                        }
                    }
                }

            }
        }

        public void AutoTagRomanNumerals()
        {
            if (HasRegions)
            {
                int Offset = 0;
                int Length = 0;

                Regex RomanNumbers = new Regex(@"\b[IVXLCDM]+\b");
                // https://stackoverflow.com/questions/39561492/c-sharp-regex-for-match-romanian-number-in-text

                // Debug.Print($"Page {PageNr} -------------------------------------------------");

                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            MatchCollection RomanNumberMatches = RomanNumbers.Matches(TL.TextEquiv);

                            if (RomanNumberMatches.Count > 0)
                            {
                                foreach (Match M in RomanNumberMatches)
                                {
                                    bool SetTag = false;

                                    // så skal vi lige tjekke, om det i virkeligheden er et initial...
                                    if (M.Value.Length == 1)
                                    {
                                        if (M.Index < TL.Length - 1)
                                        {
                                            // hvis der IKKE er et punktum efter, er det ok
                                            if (TL.TextEquiv.Substring(M.Index + 1, 1) != ".")
                                                SetTag = true;
                                        }
                                        else
                                        {
                                            // fundet på sidste plads, uden punktum efter: alt OK
                                            SetTag = true;
                                        }
                                    }
                                    else
                                    {
                                        SetTag = true;
                                    }

                                    if (SetTag)
                                    {
                                        // Debug.Print($"Roman numbers fundet: page {PageNr}, line {TL.Number}, text: _{TL.TextEquiv}_");
                                        TL.AddRomanNumeralTag(M.Index, M.Value.Length, M.Value);
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }


        public void AutoAbbrevTagRepetitions()
        {
            if (HasRegions)
            {
                int Offset = 0;
                int Length = 0;

                Regex RepeatSigns = new Regex(@"(\s*(""|do\.)\s*)+");       // tidl.: med bindestreg - hvilket går helt galt! (@"(\s*(""|-|do\.)\s*)+")
                Regex NonRepeatSigns = new Regex(@"[^\s""(do\.]");          // tidl.: do.             (@"[^\s""-(do\.]")

                // Debug.Print($"Page {PageNr} -------------------------------------------------");

                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.Number > 1)
                            {
                                string RepeatSign = "";
                                int RepeatSignPos;

                                MatchCollection RepeatSignMatches = RepeatSigns.Matches(TL.TextEquiv);
                                MatchCollection NonRepeatSignMatches = NonRepeatSigns.Matches(TL.TextEquiv);

                                if (RepeatSignMatches.Count > 0)
                                {
                                    // Debug.Print($"Repeatsign fundet: page {PageNr}, line {TL.Number}, text: _{TL.TextEquiv}_");
                                    // string TempLine = TL.TextEquiv.Trim();

                                    clsTrTextLine LineAbove = TL.GetTextLineAbove();
                                    if (LineAbove != null)
                                    {
                                        string CurrentContent = TL.ExpandedText;
                                        string ContentAbove = LineAbove.ExpandedText;
                                        
                                        var WordsCurrentArray = TL.TextEquiv.Trim().Split(' ').ToArray();
                                        int WordsCurrentCount = WordsCurrentArray.Length;

                                        //var WordsCurrentTempArray = TempLine.Trim().Split(' ').ToArray();
                                        //int WordsCurrentTempCount = WordsCurrentTempArray.Length;

                                        var WordsAboveArray = ContentAbove.Trim().Split(' ').ToArray();
                                        int WordsAboveCount = WordsAboveArray.Length;

                                        //foreach (Match M in RepeatSignMatches)
                                        //    TempLine = TempLine.Replace(M.Value.Trim(), "*");

                                        string Expansion = "";

                                        // vi prøver først med eet match - det er nemmere
                                        // (og betyder, at der godt kan være flere repeatsigns, men de står SAMMEN - ikke på hver sin side af et rigtigt ord!)

                                        //  Debug.Print($"Antal Repeatsign fundet: {RepeatSignMatches.Count}, antal non-repeat: {NonRepeatSignMatches.Count}");

                                        if (RepeatSignMatches.Count > 0)
                                        {
                                            foreach (Match M in RepeatSignMatches)
                                            {
                                                // hvilken type repeat? " - eller do.?
                                                if (M.Value.Contains('"'))
                                                    RepeatSign = @"""";
                                                else if (M.Value.Contains("do."))
                                                    RepeatSign = "do.";
                                                RepeatSignPos = M.Value.IndexOf(RepeatSign);

                                                // hvis M.Value begynder med et \s skal der kompenseres for:
                                                if (M.Value.StartsWith(" "))
                                                    Offset = M.Index + RepeatSignPos;
                                                else
                                                    Offset = M.Index;
                                                Length = M.Value.Trim().Length;

                                                // kun hvis der er EET repeatsign, sættes indhold i tagget - ellers tomme tags!
                                                if (RepeatSignMatches.Count == 1)
                                                {
                                                    // Nu kommer så problemet; er hele eller dele af teksten en gentagelse?

                                                    // 1. Hvis aktuel linie udelukkende består af gentagelsestegn, er det enkelt.
                                                    if (NonRepeatSignMatches.Count == 0)
                                                    {
                                                        Expansion = ContentAbove;
                                                        // Debug.Print($"NRSM.C = 0: Expansion: {Expansion}");
                                                    }
                                                    // 2. Ellers: Hvis der er lige mange ord på denne linie og linien over, er det også enkelt.
                                                    else if (WordsAboveCount == WordsCurrentCount)
                                                    {
                                                        if (WordsAboveCount == 1)
                                                        {
                                                            Expansion = ContentAbove;
                                                            // Debug.Print($"WAC = WCC, WAC = 1: Expansion: {Expansion}");
                                                        }
                                                        else // if (WordsAboveCount == 2)
                                                        {
                                                            // så gennemløber vi begge arrays
                                                            for (int i = 0; i < WordsCurrentCount; i++)
                                                            {
                                                                if (clsLanguageLibrary.StripPunctuation(WordsCurrentArray[i]) == RepeatSign)
                                                                {
                                                                    Expansion = Expansion + WordsAboveArray[i] + " ";
                                                                }
                                                            }
                                                            Expansion = Expansion.Trim();
                                                            // Debug.Print($"WAC = WCC, WAC = 2: Expansion: {Expansion}");
                                                        }
                                                    }
                                                    else
                                                    // der er et ulige antal ord!
                                                    {
                                                        // Debug.Print($"WAC <> WCC: Expansion: {Expansion}");
                                                        Expansion = "";
                                                    }

                                                    if (!TL.HasAbbrevTag)
                                                    {
                                                        TL.AddAbbrevTag(Offset, Length, Expansion);
                                                    }
                                                    //else
                                                    //    Debug.Print($"TL had already abbrev-tag: none added.");
                                                }
                                                else
                                                // der er matches på BEGGE sider af et ord sætter tomme tags
                                                {
                                                    // Debug.Print("Sætter tomt abbrev-tag.");
                                                    TL.AddAbbrevTag(Offset, Length, "");
                                                }

                                            }
                                        }
                                    }
                                    else
                                    // kunne ikke finde en linie ovenover
                                    {
                                        if (!TL.HasAbbrevTag)
                                        {
                                            // Debug.Print("Sætter tomt abbrev-tag.");
                                            TL.AddAbbrevTag(Offset, Length, "");
                                        }
                                        // else
                                        // Debug.Print($"TL had already abbrev-tag: none added.");
                                    }
                                }
                                else
                                {
                                    // har ikke fundet repeatsigns
                                    // Debug.Print($"Kunne ikke finde repeatsign: page {PageNr}, line {TL.Number}, text: _{TL.TextEquiv}_");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AutoAbbrevTagNumericIntervals(string StructuralTagName)
        {
            if (HasRegions)
            {
                // Debug.Print($"Page {PageNr} -------------------------------------------------");

                Regex NumericInterval = new Regex(@"\d+(\s*-\s*\d+)*");          

                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.HasSpecificStructuralTag(StructuralTagName))
                            {
                                MatchCollection NumericIntervalMatches = NumericInterval.Matches(TL.TextEquiv);
                                if (NumericIntervalMatches.Count > 0)
                                {
                                    StringBuilder MatchString = new StringBuilder();
                                    foreach (Match M in NumericIntervalMatches)
                                    {
                                        MatchString.Append(M.Value);
                                        MatchString.Append(" ");
                                    }
                                    if (clsTrLibrary.UniqueNumbersCount(MatchString.ToString()) > 1)
                                    {
                                        // PROBLEM: Adresser som fx "Gl. Torv 10-12" bliver medtaget - og det er sjældent meningen
                                        // Derfor sættes et minimum: det første tal skal være over 1000!

                                        int FirstNumber = clsTrLibrary.GetNumbers(MatchString.ToString())[0];
                                        if (FirstNumber >= 1000)
                                        {
                                            // vi tester lige, om der kommer snask retur - "n/a" kan ikke bruges
                                            string NewString = clsTrLibrary.ExpandStringWithNumericInterval(MatchString.ToString());
                                            if (NewString != "n/a")
                                            {
                                                TL.AddAbbrevTag(NumericIntervalMatches[0].Index, NumericIntervalMatches[0].Value.Length, NewString);
                                                Debug.Print($"Page {TL.ParentPageNr.ToString("000")}, Line {TL.Number.ToString("000")}: Interval {TL.TextEquiv} expanded to {NewString}");

                                            }
                                            else
                                                Debug.Print($"Page {TL.ParentPageNr.ToString("000")}, Line {TL.Number.ToString("000")}: No expansion! Rubbish: {TL.TextEquiv}");

                                        }
                                        else
                                        {
                                            Debug.Print($"Page {TL.ParentPageNr.ToString("000")}, Line {TL.Number.ToString("000")}: No expansion! FirstNumber: {FirstNumber}");
                                        }


                                        // Debug.Print($"       Expansion: {clsTrLibrary.ExpandStringWithNumericInterval(MatchString.ToString())}");

                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        public void AutoAbbrevTagPlaceNames()
        {
            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            // Ender på g
                            Regex PlaceName = new Regex(@"\p{Lu}\p{Ll}+[g][\.\:]?");

                            MatchCollection PlaceNameMatches = PlaceName.Matches(TL.TextEquiv);
                            if (PlaceNameMatches.Count > 0)
                            {
                                int Position = PlaceNameMatches[0].Value.LastIndexOf('g');
                                string Expansion = PlaceNameMatches[0].Value.Substring(0, Position) + "gade";

                                //TL.AddAbbrevTag(PlaceNameMatches[0].Index, PlaceNameMatches[0].Value.Length, Expansion);
                                Debug.Print($"Page {TL.ParentPageNr.ToString("000")}, Line {TL.Number.ToString("000")}: {PlaceNameMatches[0].Value} expanded to {Expansion}");
                            }
                        }
                    }
                }
            }

        }

        public void Elfelt_AutoTag()
        {
            
        }

        public void KOBACC_ExpandText()
        {
            // ADVARSEL: "Ødelægger" teksten - gør det KUN på en kopi!!!
            if (HasRegions)
            {
                foreach (clsTrRegion TR in Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.TextEquiv != "")                             // det går galt på en tom streng
                            {
                                if (TL.HasSpecificStructuralTag("Date"))
                                {
                                    if (TL.TextEquiv.Substring(0, 1) != "[")     // så den ikke gør det igen!!
                                    {
                                        string ExpandedDate = clsTrLibrary.GetDate(TL.TextEquiv, ParentDocument.KOBACC_GetYear());
                                        Debug.WriteLine(ExpandedDate);
                                        if (ExpandedDate != "n/a")
                                        {
                                            TL.TextEquiv = ExpandedDate;
                                            TL.HasChanged = true;
                                        }
                                    }
                                }
                                else if (TL.HasSpecificStructuralTag("Acc"))
                                {
                                    if (TL.TextEquiv.Substring(0, 1) != "[")     // så den ikke gør det igen!!
                                    {
                                        string ExpandedAcc = clsTrLibrary.GetAccNo(TL.TextEquiv, ParentDocument.KOBACC_GetYear());
                                        Debug.WriteLine(ExpandedAcc);
                                        if (ExpandedAcc != "n/a")
                                        {
                                            TL.TextEquiv = ExpandedAcc;
                                            TL.HasChanged = true;
                                        }
                                    }
                                }
                                else
                                {
                                    
                                    Debug.WriteLine(TL.ExpandedText);
                                    TL.DeleteSicAndAbbrevTags();
                                    TL.TextEquiv = TL.ExpandedText;
                                    TL.HasChanged = true;
                                }

                            }
                        }

                    }

                }

            }

        }




    }
}
