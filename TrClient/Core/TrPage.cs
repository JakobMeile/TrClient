// <copyright file="TrPage.cs" company="Kyrillos">
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
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using DanishNLP;
    using TrClient.Extensions;
    using TrClient.Helpers;
    using TrClient.Libraries;

    public class TrPage : IComparable, INotifyPropertyChanged
    {
        public string ID { get; set; }

        public int PageNr { get; set; }

        public string ImageFileName { get; set; }

        public string ImageURL { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        //public string PageFileName { get; set; }

        public int AssumedRowCount { get; set; }

        //public double ActualRowHeight { get; set; }

        public TrTranscripts Transcripts = new TrTranscripts();

        public TrPages ParentContainer;
        public TrDocument ParentDocument;

        public BitmapImage PageImage { get; set; }

        private int transcriptCount;

        public int TranscriptCount
        {
            get
            {
                transcriptCount = Transcripts.Count;
                return transcriptCount;
            }
            set
            {
                NotifyPropertyChanged("TranscriptCount");
            }
        }

        private bool hasTables;

        public bool HasTables
        {
            get
            {
                hasTables = Transcripts[0].HasTables;
                return hasTables;
            }
        }

        private bool hasFormerTables;

        public bool HasFormerTables
        {
            get
            {
                hasFormerTables = false;
                if (TranscriptCount > 1)
                {
                    TrTranscript transcript;
                    for (int i = 0; i < TranscriptCount; i++)
                    {
                        // Debug.WriteLine($"Page {PageNr}, Transcript {i}:");
                        transcript = Transcripts[i];
                        foreach (TrRegion region in transcript.Regions)
                        {
                            hasFormerTables = hasFormerTables || (region.GetType() == typeof(TrTableRegion));

                            // Debug.WriteLine($"Region {Region.Number}, Type {Region.GetType().ToString()}");
                        }

                        //if (Tra.Regions.Count > 0)
                        //{

                        //}
                    }
                }

                return hasFormerTables;
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
                bool tablesFound = false;
                int transcriptNumberWithTables = 0;

                TrTranscript newestTranscript = Transcripts[0];
                TrTranscript formerTranscript = null;

                // find latest transcript with tables
                if (TranscriptCount > 1)
                {
                    for (int i = 1; i < TranscriptCount; i++)
                    {
                        formerTranscript = Transcripts[i];
                        if (formerTranscript.HasTables)
                        {
                            tablesFound = true;
                            transcriptNumberWithTables = i;
                            Debug.WriteLine($"Page {PageNr}: Found former table in transcript # {transcriptNumberWithTables}");
                            break;
                        }
                    }
                }

                if (tablesFound)
                {
                    // FormerTranscript er nu det seneste transcript med tables
                    foreach (TrRegion formerRegion in formerTranscript.Regions)
                    {
                        if (formerRegion.GetType() == typeof(TrTableRegion))
                        {
                            Debug.WriteLine($"Page {PageNr}: Copying former table from transcript # {transcriptNumberWithTables}");
                            newestTranscript.Regions.Add(formerRegion);
                            newestTranscript.HasChanged = true;
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

        private bool isPageImageLoaded = false;

        public bool IsPageImageLoaded
        {
            get
            {
                return isPageImageLoaded;
            }

            set
            {
                if (isPageImageLoaded != value)
                {
                    isPageImageLoaded = value;
                    NotifyPropertyChanged("IsPageImageLoaded");
                }
            }
        }

        private double assumedRowHeight;

        //public double AssumedRowHeight
        //{
        //    get
        //    {
        //        if (AssumedRowCount != 0)
        //        {
        //            assumedRowHeight = (double)Height / (double)AssumedRowCount;
        //        }
        //        else
        //        {
        //            assumedRowHeight = 0;
        //        }

        //        return assumedRowHeight;
        //    }
        //}

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
                NotifyPropertyChanged("HasChanged");
                if (hasChanged)
                {
                    StatusColor = Brushes.Orange;
                }

                ParentDocument.HasChanged = value;
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
                NotifyPropertyChanged("ChangesUploaded");
                if (changesUploaded)
                {
                    StatusColor = Brushes.DarkViolet;
                }

                ParentDocument.ChangesUploaded = value;
            }
        }

        private bool hasRegions;

        public bool HasRegions
        {
            get
            {
                hasRegions = Transcripts[0].HasRegions;
                return hasRegions;
            }
        }

        private bool hasLines;

        //public bool HasLines
        //{
        //    get
        //    {
        //        hasLines = GetLines().Count > 0;
        //        return hasLines;
        //    }
        //}

        public bool ExistsRegionNumber(int regionNumber)
        {
            bool temp = false;
            if (HasRegions)
            {
                if (regionNumber <= Transcripts[0].Regions.Count)
                {
                    temp = true;
                }
            }

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
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public int CompareTo(object obj)
        {
            var page = obj as TrPage;
            return PageNr.CompareTo(page.PageNr);
        }

        // constructor ONLINE
        public TrPage(string pageID, int iPageNr, string pageFileName, string imageFileURL, int pageW, int pageH)
        {
            ID = pageID;
            PageNr = iPageNr;
            ImageFileName = pageFileName;
            ImageURL = imageFileURL;
            Width = pageW;
            Height = pageH;

            Transcripts.ParentPage = this;
            IsLoaded = false;
            //Transcripts[0].TestEventHandler += TrPage_TestEventHandler;
        }

        //private void TrPage_TestEventHandler(object sender, EventArgs e)
        //{
        //    Debug.Print("Page: Not Implemented");
        //    throw new NotImplementedException();
        //}

        // constructor OFFLINE
        //public TrPage(string pageID, int iPageNr, string pageFileName, string imageFileURL, int pageW, int pageH, string pageFile)
        //{
        //    ID = pageID;
        //    PageNr = iPageNr;
        //    ImageFileName = pageFileName;
        //    ImageURL = imageFileURL;
        //    Width = pageW;
        //    Height = pageH;
        //    pageFileName = pageFile;

        //    Transcripts.ParentPage = this;
        //    IsLoaded = false;

        //    // Debug.WriteLine("Page created! ");
        //}

        //public void SetColumnNumbers(int AssumedNumberOfColumns)
        //{
        //    if (HasRegions)
        //    {
        //        TrTranscript Transcript = Transcripts[0];
        //        if (Transcript.Regions.Count == 1)
        //        {
        //            TrRegion_Text TR = Transcript.Regions[0];
        //            TrCoords LinePositions = new TrCoords();

        //            foreach (TrTextLine TL in TR.TextLines)
        //            {
        //                int X = TL.Hpos;
        //                int Y = TL.Vpos;
        //                TrCoord Position = new TrCoord(X,Y);
        //                LinePositions.Add(Position);
        //            }

        //            string FileName = TrLibrary.ExportFolder + "LinePositions_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";

        //            using (StreamWriter TextFile = new StreamWriter(FileName, true))
        //            {
        //                TextFile.WriteLine(PageFileName);
        //                foreach (TrCoord Position in LinePositions)
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
        //        TrTranscript Transcript = Transcripts[0];
        //        if (Transcript.Regions.Count == 1)
        //        {
        //            TrRegion_Text TR = Transcript.Regions[0];
        //            List<double> RowsDeltaV = new List<double>();

        //            AssumedRowCount = AssumedNumberOfRows;
        //            Debug.WriteLine($"RowsDeltaV.count = {RowsDeltaV.Count}, Assumed RowHeight = {AssumedRowHeight}");
        //            double LowerBoundary = 0.8 * AssumedRowHeight;
        //            double UpperBoundary = 1.2 * AssumedRowHeight;
        //            Debug.WriteLine($"LowerB = {LowerBoundary}, UpperB = {UpperBoundary}");

        //            int Max = TR.TextLines.Count - 1;
        //            for (int i = 0; i < Max; i++)
        //            {
        //                TrTextLine CurrentLine = TR.TextLines[i];
        //                TrTextLine NextLine = TR.TextLines[i + 1];
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

        //            //foreach (TrTextLine TL in TR.TextLines)
        //            //{
        //            //    TL.RowNumber = (int)(Convert.ToDouble(TL.Vpos) / ActualRowHeight);
        //            //    Debug.WriteLine($"Row = {TL.RowNumber}, Vpos = {TL.Vpos}, Hpos = {TL.Hpos}");
        //            //}

        //        }
        //    }
        //}
        public void DeleteRegionsWithTag(string regionalTagValue)
        {
            TrTranscript transcript = Transcripts[0];
            foreach (TrRegion textRegion in transcript.Regions)
            {
                // if (TR.HasStructuralTag && TR.StructuralTagValue != RegionalTagValue)
                if (textRegion.StructuralTagValue == regionalTagValue)
                {
                    textRegion.MarkToDeletion = true;
                    Debug.WriteLine($"Page# {PageNr}, Region# {textRegion.Number}: Marked to deletion (struct. tag = {textRegion.StructuralTagValue}, chosen tag = {regionalTagValue})");
                }
            }

            for (int i = transcript.Regions.Count - 1; i >= 0; i--)
            {
                if (transcript.Regions[i].MarkToDeletion)
                {
                    transcript.Regions.RemoveAt(i);
                }
            }
        }

        public void DeleteRegionsOtherThan(string regionalTagValue)
        {
            TrTranscript transcript = Transcripts[0];
            foreach (TrRegion textRegion in transcript.Regions)
            {
                // if (TR.HasStructuralTag && TR.StructuralTagValue != RegionalTagValue)
                if (textRegion.StructuralTagValue != regionalTagValue)
                {
                    textRegion.MarkToDeletion = true;
                    Debug.WriteLine($"Page# {PageNr}, Region# {textRegion.Number}: Marked to deletion (struct. tag = {textRegion.StructuralTagValue}, preserve = {regionalTagValue})");
                }
            }

            for (int i = transcript.Regions.Count - 1; i >= 0; i--)
            {
                if (transcript.Regions[i].MarkToDeletion)
                {
                    transcript.Regions.RemoveAt(i);
                }
            }
        }

        public void DeleteEmptyRegions()
        {
            //Debug.WriteLine($"Page # {PageNr}");
            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                foreach (TrRegion textRegion in transcript.Regions)
                {
                    //Debug.WriteLine($"Region# {TR.Number}");
                    if (!textRegion.HasLines)
                    {
                        //Debug.WriteLine($"Hasn't got lines!");
                        textRegion.MarkToDeletion = true;
                    }
                }

                for (int i = transcript.Regions.Count - 1; i >= 0; i--)
                {
                    if (transcript.Regions[i].MarkToDeletion)
                    {
                        transcript.Regions.RemoveAt(i);
                    }
                }
            }
        }

        //public void SortRegions()
        //{
        //    if (HasRegions)
        //    {
        //        Transcripts[0].Regions.Sort();
        //    }
        //}

        public void RenumberRegionsVertically()
        {
            if (HasRegions)
            {
                Transcripts[0].Regions.ReNumberVertically();
            }
        }

        public void RenumberRegionsHorizontally()
        {
            if (HasRegions)
            {
                Transcripts[0].Regions.ReNumberHorizontally();
            }
        }

        public void DeleteLinesWithTag(string structuralTagValue)
        {
            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];

                foreach (TrRegion textRegion in transcript.Regions)
                {
                    if (textRegion.HasLines)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (textLine.StructuralTagValue == structuralTagValue)
                                {
                                    textLine.MarkToDeletion = true;

                                    //Debug.WriteLine($"Page# {PageNr}, Region# {TR.Number}: Marked to deletion (struct. tag = {TR.StructuralTagValue}, chosen tag = {StructuralTagValue})");
                                }
                            }

                            for (int i = (textRegion as TrTextRegion).TextLines.Count - 1; i >= 0; i--)
                            {
                                if ((textRegion as TrTextRegion).TextLines[i].MarkToDeletion)
                                {
                                    (textRegion as TrTextRegion).TextLines.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DeleteLinesOtherThan(string structuralTagValue)
        {
            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];

                foreach (TrRegion textRegion in transcript.Regions)
                {
                    if (textRegion.HasLines)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (textLine.StructuralTagValue != structuralTagValue)
                                {
                                    textLine.MarkToDeletion = true;

                                    //Debug.WriteLine($"Page# {PageNr}, Region# {TR.Number}: Marked to deletion (struct. tag = {TR.StructuralTagValue}, chosen tag = {StructuralTagValue})");
                                }
                            }

                            for (int i = (textRegion as TrTextRegion).TextLines.Count - 1; i >= 0; i--)
                            {
                                if ((textRegion as TrTextRegion).TextLines[i].MarkToDeletion)
                                {
                                    (textRegion as TrTextRegion).TextLines.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<string> GetExpandedText(bool refine, bool convertOtrema)
        {
            List<string> tempList = new List<string>();
            List<string> regionList;

            foreach (TrRegion textRegion in Transcripts[0].Regions)
            {
                regionList = textRegion.GetExpandedText(refine, convertOtrema);
                foreach (string s in regionList)
                {
                    tempList.Add(s);
                }
            }

            return tempList;
        }

        private TrWords words = new TrWords();

        public TrWords Words
        {
            get
            {
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    foreach (TrWord w in textRegion.Words)
                    {
                        words.Add(w);
                    }
                }

                return words;
            }
        }

        private int numberOfRegions = 0;

        public int NumberOfRegions
        {
            get
            {
                if (HasRegions)
                {
                    numberOfRegions = Transcripts[0].Regions.Count;
                }

                return numberOfRegions;
            }
        }

        private int numberOfLines = 0;

        public int NumberOfLines
        {
            get
            {
                int temp = 0;
                if (HasRegions)
                {
                    foreach (TrRegion textRegion in Transcripts[0].Regions)
                    {
                        temp = temp + textRegion.NumberOfLines;
                    }
                }

                numberOfLines = temp;
                Debug.Print($"Page {PageNr}: Number of lines = {numberOfLines}");
                return numberOfLines;
            }
        }

        // public int NumberOfTranscribedRegions { get; }
        // public int NumberOfTranscribedLines { get; }
        public TrTextLines GetLines()
        {
            TrTextLines tempList = new TrTextLines();

            if (HasRegions)
            {
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            tempList.Add(textLine);
                            textLine.ParentRegion = textRegion as TrTextRegion;
                        }
                    }
                }
            }

            return tempList;
        }

        public TrParagraphs GetParagraphs()
        {
            TrParagraphs tempList = new TrParagraphs();
            Debug.WriteLine($"Page:GetP - page: {PageNr}");

            if (HasRegions)
            {
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        Debug.WriteLine($"Page:GetP - region {textRegion.Number}");
                        TrParagraphs tRPS = (textRegion as TrTextRegion).GetParagraphs();
                        if (tRPS != null)
                        {
                            foreach (TrParagraph tRP in tRPS)
                            {
                                Debug.WriteLine($"Page:GetP - paragraph {tRP.Number}");
                                tempList.Add(tRP);
                                tRP.ParentRegion = (TrTextRegion)textRegion;
                                Debug.WriteLine($"Page:GetP - name      {tRP.Name}");
                                Debug.WriteLine($"Page:GetP - content   {tRP.Content}");
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        public bool CreateTopLevelRegion()
        {
            if (!HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                TrRegions newRegions = new TrRegions(true);
                TrTextRegion newRegion = new TrTextRegion(0, 0, GetTopLevelCoords(), newRegions);
                newRegions.Add(newRegion);
                transcript.Regions = newRegions;

                transcript.HasChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MergeAllRegionsToTopLevelRegion()
        {
            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                TrTextRegion newRegion = new TrTextRegion(0, 0, GetTopLevelCoords(), transcript.Regions);

                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            newRegion.TextLines.Add(textLine);
                        }

                        textRegion.MarkToDeletion = true;
                    }
                    else
                    {
                        Debug.WriteLine($"ERROR: Non-Text Region on page {PageNr}");
                    }
                }

                AppendRegion(newRegion);

                for (int i = transcript.Regions.Count - 1; i >= 0; i--)
                {
                    if (transcript.Regions[i].MarkToDeletion)
                    {
                        transcript.Regions.RemoveAt(i);
                    }
                }

                transcript.HasChanged = true;
            }
        }

        public void AppendRegion(TrRegion sourceRegion)
        {
            //Debug.WriteLine($"TrPage:Append...");
            TrTranscript transcript = Transcripts[0];

            //Debug.WriteLine($"Actual region count before: {Transcript.Regions.Count}");
            if (!HasRegions)
            {
                TrRegions newRegions = new TrRegions(true);
                transcript.Regions = newRegions;
            }

            // int NewRegionOrder = Transcript.Regions.Count;
            transcript.Regions.Add(sourceRegion);

            //Debug.WriteLine($"Actual region count after: {Transcript.Regions.Count}");
            HasChanged = true;
        }

        public int AddHorizontalRegion(int upperPercent, int lowerPercent, int upperPadding, int lowerPadding)
        {
            TrTranscript transcript = Transcripts[0];

            if (!HasRegions)
            {
                TrRegions newRegions = new TrRegions(true);
                transcript.Regions = newRegions;
            }

            int newRegionOrder = transcript.Regions.Count;
            float orientation = 0;
            TrTextRegion newRegion = new TrTextRegion(newRegionOrder, orientation,
                GetCoordsForHorizontalRegion(upperPercent, lowerPercent, upperPadding, lowerPadding), transcript.Regions);
            transcript.Regions.Add(newRegion);

            transcript.HasChanged = true;
            return newRegion.Number;
        }

        public void DeleteShortBaseLines(TrDialogTransferSettings settings, TrLog log)
        {
            bool processThis = true;
            bool pageIsOK = true;
            string errorMessage;

            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                foreach (TrRegion region in transcript.Regions)
                {
                    if (settings.AllRegions)
                    {
                        processThis = true;
                    }
                    else if (region.Number >= settings.RegionsFrom && region.Number <= settings.RegionsTo)
                    {
                        processThis = true;
                    }
                    else
                    {
                        processThis = false;
                    }

                    if (processThis)
                    {
                        pageIsOK = pageIsOK && region.DeleteShortBaselines(settings.ShortLimit, log);
                    }
                }

                if (pageIsOK)
                {
                    errorMessage = "Page is OK.";
                    log.Add(this, errorMessage);
                }
            }
            else
            {
                errorMessage = "Page has no regions.";
                log.Add(this, errorMessage);
            }
        }

        public void ExtendBaseLines(TrDialogTransferSettings settings, TrLog log)
        {
            // Debug.WriteLine($"TrPage : ExtendBaseLines");
            bool processThis = true;
            string errorMessage;

            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                foreach (TrRegion region in transcript.Regions)
                {
                    if (settings.AllRegions)
                    {
                        processThis = true;
                    }
                    else if (region.Number >= settings.RegionsFrom && region.Number <= settings.RegionsTo)
                    {
                        processThis = true;
                    }
                    else
                    {
                        processThis = false;
                    }

                    if (processThis)
                    {
                        region.ExtendBaseLines(settings, log);
                    }
                }
            }
            else
            {
                errorMessage = "Page has no regions.";
                log.Add(this, errorMessage);
            }
        }

        //public void RepairBaseLines(TrLog Log)
        //{
        //    bool PageIsOK = true;
        //    string ErrorMessage;

        //    if (HasRegions)
        //    {
        //        TrTranscript Transcript = Transcripts[0];
        //        foreach(TrRegion_Text Region in Transcript.Regions)
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
                TrTranscript transcript = Transcripts[0];
                foreach (TrRegion region in transcript.Regions)
                {
                    region.SimplifyBoundingBoxes();
                }
            }
        }

        public void SimplifyBoundingBoxes(int minimumHeight, int maximumHeight)
        {
            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                foreach (TrRegion region in transcript.Regions)
                {
                    region.SimplifyBoundingBoxes(minimumHeight, maximumHeight);
                }
            }
        }

        public void Move(int horizontally, int vertically)
        {
            if (HasRegions)
            {
                TrTranscript transcript = Transcripts[0];
                foreach (TrRegion region in transcript.Regions)
                {
                    region.Move(horizontally, vertically);
                }
            }
        }

        private string GetTopLevelCoords()
        {
            TrCoord upperLeft = new TrCoord(0, 0);
            TrCoord upperRight = new TrCoord(Width, 0);
            TrCoord lowerRight = new TrCoord(Width, Height);
            TrCoord lowerLeft = new TrCoord(0, Height);

            TrCoords cS = new TrCoords();
            cS.Add(upperLeft);
            cS.Add(upperRight);
            cS.Add(lowerRight);
            cS.Add(lowerLeft);

            return cS.ToString();
        }

        private string GetCoordsForHorizontalRegion(int upperPercent, int lowerPercent, int upperPadding, int lowerPadding)
        {
            int upperBoundary;
            int lowerBoundary;

            if (upperPercent == 0)
            {
                upperBoundary = 0;
            }
            else
            {
                upperBoundary = Height * upperPercent / 100;
            }

            lowerBoundary = Height * lowerPercent / 100;

            upperBoundary = upperBoundary + upperPadding;
            lowerBoundary = lowerBoundary - lowerPadding;

            TrCoord upperLeft = new TrCoord(0, upperBoundary);
            TrCoord upperRight = new TrCoord(Width, upperBoundary);
            TrCoord lowerRight = new TrCoord(Width, lowerBoundary);
            TrCoord lowerLeft = new TrCoord(0, lowerBoundary);

            TrCoords cS = new TrCoords();
            cS.Add(upperLeft);
            cS.Add(upperRight);
            cS.Add(lowerRight);
            cS.Add(lowerLeft);

            return cS.ToString();
        }

        public void TagEmptyTextLines()
        {
            if (HasRegions)
            {
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.IsEmpty)
                            {
                                textLine.AddStructuralTag("Empty", true);
                            }
                        }
                    }
                }
            }
        }

        public void WrapSuperAndSubscriptWithSpaces()
        {
            if (HasRegions)
            {
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            textLine.WrapSuperAndSubscriptWithSpaces();
                        }
                    }
                }
            }
        }

        public void TagEmptyAbbrevTags()
        {
            if (HasRegions)
            {
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.HasEmptyAbbrevTag)
                            {
                                textLine.AddStructuralTag("EmptyAbbrev", true);
                            }
                        }
                    }
                }
            }
        }

        //public void KOBACC_AutoTag()
        //{
        //    // sætter Date og Acc-tags på hhv region 1 og 2, hvis der er 3 eller flere regioner
        //    // hvis der kun er een eller to, sker der ikke noget
        //    string tagValue = string.Empty;

        //    if (HasRegions)
        //    {
        //        if (Transcripts[0].Regions.Count >= 3)
        //        {
        //            foreach (TrRegion textRegion in Transcripts[0].Regions)
        //            {
        //                if (textRegion.GetType() == typeof(TrTextRegion))
        //                {
        //                    if (textRegion.Number == 1)
        //                    {
        //                        tagValue = "Date";
        //                    }
        //                    else if (textRegion.Number == 2)
        //                    {
        //                        tagValue = "Acc";
        //                    }

        //                    if (textRegion.Number < 3)
        //                    {
        //                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
        //                        {
        //                            // det tjekkes, om TextLine primært består af cifre - og kun da sættes tagget
        //                            // desuden tjekkes, om antallet af numre er hhv. 1 eller 2, afh. af tagvalue
        //                            if (ClsLanguageLibrary.DigitCount(textLine.TextEquiv) > ClsLanguageLibrary.LetterCount(textLine.TextEquiv))
        //                            {
        //                                if ((tagValue == "Date" && TrLibrary.UniqueNumbersCount(textLine.TextEquiv) >= 2)
        //                                    || (tagValue == "Acc" && TrLibrary.UniqueNumbersCount(textLine.TextEquiv) == 1))
        //                                {
        //                                    textLine.AddStructuralTag(tagValue, false);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Debug.WriteLine($"{ParentDocument.ParentCollection} / {ParentDocument} / s. {PageNr}: under 3 regioner!");
        //        }
        //    }
        //}

        public void AutoTagFloorNumberSuperScript()
        {
            if (HasRegions)
            {
                Regex floorNumbers = new Regex(@"\b(?<=\s\d{1,3}\.?\s?[A-Z]?\.?\s?)(\d|[IV]+)\.?$");

                // Debug.Print($"Page {PageNr} -------------------------------------------------");
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            MatchCollection floorNumberMatches = floorNumbers.Matches(textLine.TextEquiv);

                            if (floorNumberMatches.Count > 0)
                            {
                                Debug.Print($"Floornumber fundet: page {PageNr}, line {textLine.Number}, text: _{textLine.TextEquiv}_ - floornumber = {floorNumberMatches[0].Value}");

                                foreach (Match m in floorNumberMatches)
                                {
                                    textLine.AddStyleTag(m.Index, m.Value.Length, "superscript");
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
                Regex romanNumbers = new Regex(@"\b[IVXLCDM]+\b");

                // https://stackoverflow.com/questions/39561492/c-sharp-regex-for-match-romanian-number-in-text

                // Debug.Print($"Page {PageNr} -------------------------------------------------");
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            MatchCollection romanNumberMatches = romanNumbers.Matches(textLine.TextEquiv);

                            if (romanNumberMatches.Count > 0)
                            {
                                foreach (Match m in romanNumberMatches)
                                {
                                    bool setTag = false;

                                    // så skal vi lige tjekke, om det i virkeligheden er et initial...
                                    if (m.Value.Length == 1)
                                    {
                                        if (m.Index < textLine.Length - 1)
                                        {
                                            // hvis der IKKE er et punktum efter, er det ok
                                            if (textLine.TextEquiv.Substring(m.Index + 1, 1) != ".")
                                            {
                                                setTag = true;
                                            }
                                        }
                                        else
                                        {
                                            // fundet på sidste plads, uden punktum efter: alt OK
                                            setTag = true;
                                        }
                                    }
                                    else
                                    {
                                        setTag = true;
                                    }

                                    if (setTag)
                                    {
                                        // Debug.Print($"Roman numbers fundet: page {PageNr}, line {TL.Number}, text: _{TL.TextEquiv}_");
                                        textLine.AddRomanNumeralTag(m.Index, m.Value.Length, m.Value);
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
                int offset = 0;
                int length = 0;

                Regex repeatSigns = new Regex(@"(\s*(""|do\.)\s*)+");       // tidl.: med bindestreg - hvilket går helt galt! (@"(\s*(""|-|do\.)\s*)+")
                Regex nonRepeatSigns = new Regex(@"[^\s""(do\.]");          // tidl.: do.             (@"[^\s""-(do\.]")

                // Debug.Print($"Page {PageNr} -------------------------------------------------");
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.Number > 1)
                            {
                                string repeatSign = string.Empty;
                                int repeatSignPos;

                                MatchCollection repeatSignMatches = repeatSigns.Matches(textLine.TextEquiv);
                                MatchCollection nonRepeatSignMatches = nonRepeatSigns.Matches(textLine.TextEquiv);

                                if (repeatSignMatches.Count > 0)
                                {
                                    // Debug.Print($"Repeatsign fundet: page {PageNr}, line {TL.Number}, text: _{TL.TextEquiv}_");
                                    // string TempLine = TL.TextEquiv.Trim();
                                    TrTextLine lineAbove = textLine.GetTextLineAbove();
                                    if (lineAbove != null)
                                    {
                                        string currentContent = textLine.ExpandedText;
                                        string contentAbove = lineAbove.ExpandedText;

                                        var wordsCurrentArray = textLine.TextEquiv.Trim().Split(' ').ToArray();
                                        int wordsCurrentCount = wordsCurrentArray.Length;

                                        //var WordsCurrentTempArray = TempLine.Trim().Split(' ').ToArray();
                                        //int WordsCurrentTempCount = WordsCurrentTempArray.Length;
                                        var wordsAboveArray = contentAbove.Trim().Split(' ').ToArray();
                                        int wordsAboveCount = wordsAboveArray.Length;

                                        //foreach (Match M in RepeatSignMatches)
                                        //    TempLine = TempLine.Replace(M.Value.Trim(), "*");
                                        string expansion = string.Empty;

                                        // vi prøver først med eet match - det er nemmere
                                        // (og betyder, at der godt kan være flere repeatsigns, men de står SAMMEN - ikke på hver sin side af et rigtigt ord!)

                                        //  Debug.Print($"Antal Repeatsign fundet: {RepeatSignMatches.Count}, antal non-repeat: {NonRepeatSignMatches.Count}");
                                        if (repeatSignMatches.Count > 0)
                                        {
                                            foreach (Match m in repeatSignMatches)
                                            {
                                                // hvilken type repeat? " - eller do.?
                                                if (m.Value.Contains('"'))
                                                {
                                                    repeatSign = @"""";
                                                }
                                                else if (m.Value.Contains("do."))
                                                {
                                                    repeatSign = "do.";
                                                }

                                                repeatSignPos = m.Value.IndexOf(repeatSign);

                                                // hvis M.Value begynder med et \s skal der kompenseres for:
                                                if (m.Value.StartsWith(" "))
                                                {
                                                    offset = m.Index + repeatSignPos;
                                                }
                                                else
                                                {
                                                    offset = m.Index;
                                                }

                                                length = m.Value.Trim().Length;

                                                // kun hvis der er EET repeatsign, sættes indhold i tagget - ellers tomme tags!
                                                if (repeatSignMatches.Count == 1)
                                                {
                                                    // Nu kommer så problemet; er hele eller dele af teksten en gentagelse?

                                                    // 1. Hvis aktuel linie udelukkende består af gentagelsestegn, er det enkelt.
                                                    if (nonRepeatSignMatches.Count == 0)
                                                    {
                                                        expansion = contentAbove;

                                                        // Debug.Print($"NRSM.C = 0: Expansion: {Expansion}");
                                                    }

                                                    // 2. Ellers: Hvis der er lige mange ord på denne linie og linien over, er det også enkelt.
                                                    else if (wordsAboveCount == wordsCurrentCount)
                                                    {
                                                        if (wordsAboveCount == 1)
                                                        {
                                                            expansion = contentAbove;

                                                            // Debug.Print($"WAC = WCC, WAC = 1: Expansion: {Expansion}");
                                                        }
                                                        else // if (WordsAboveCount == 2)
                                                        {
                                                            // så gennemløber vi begge arrays
                                                            for (int i = 0; i < wordsCurrentCount; i++)
                                                            {
                                                                if (ClsLanguageLibrary.StripPunctuation(wordsCurrentArray[i]) == repeatSign)
                                                                {
                                                                    expansion = expansion + wordsAboveArray[i] + " ";
                                                                }
                                                            }

                                                            expansion = expansion.Trim();

                                                            // Debug.Print($"WAC = WCC, WAC = 2: Expansion: {Expansion}");
                                                        }
                                                    }
                                                    else

                                                    // der er et ulige antal ord!
                                                    {
                                                        // Debug.Print($"WAC <> WCC: Expansion: {Expansion}");
                                                        expansion = string.Empty;
                                                    }

                                                    if (!textLine.HasAbbrevTag)
                                                    {
                                                        textLine.AddAbbrevTag(offset, length, expansion);
                                                    }

                                                    //else
                                                    //    Debug.Print($"TL had already abbrev-tag: none added.");
                                                }
                                                else

                                                // der er matches på BEGGE sider af et ord sætter tomme tags
                                                {
                                                    // Debug.Print("Sætter tomt abbrev-tag.");
                                                    textLine.AddAbbrevTag(offset, length, string.Empty);
                                                }
                                            }
                                        }
                                    }
                                    else

                                    // kunne ikke finde en linie ovenover
                                    {
                                        if (!textLine.HasAbbrevTag)
                                        {
                                            // Debug.Print("Sætter tomt abbrev-tag.");
                                            textLine.AddAbbrevTag(offset, length, string.Empty);
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

        public void AutoAbbrevTagNumericIntervals(string structuralTagName)
        {
            if (HasRegions)
            {
                // Debug.Print($"Page {PageNr} -------------------------------------------------");
                Regex numericInterval = new Regex(@"\d+(\s*-\s*\d+)*");

                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.HasSpecificStructuralTag(structuralTagName))
                            {
                                MatchCollection numericIntervalMatches = numericInterval.Matches(textLine.TextEquiv);
                                if (numericIntervalMatches.Count > 0)
                                {
                                    StringBuilder matchString = new StringBuilder();
                                    foreach (Match m in numericIntervalMatches)
                                    {
                                        matchString.Append(m.Value);
                                        matchString.Append(" ");
                                    }

                                    if (TrLibrary.UniqueNumbersCount(matchString.ToString()) > 1)
                                    {
                                        // PROBLEM: Adresser som fx "Gl. Torv 10-12" bliver medtaget - og det er sjældent meningen
                                        // Derfor sættes et minimum: det første tal skal være over 1000!
                                        int firstNumber = TrLibrary.GetNumbers(matchString.ToString())[0];
                                        if (firstNumber >= 1000)
                                        {
                                            // vi tester lige, om der kommer snask retur - "n/a" kan ikke bruges
                                            string newString = TrLibrary.ExpandStringWithNumericInterval(matchString.ToString());
                                            if (newString != "n/a")
                                            {
                                                textLine.AddAbbrevTag(numericIntervalMatches[0].Index, numericIntervalMatches[0].Value.Length, newString);
                                                Debug.Print($"Page {textLine.ParentPageNr.ToString("000")}, Line {textLine.Number.ToString("000")}: Interval {textLine.TextEquiv} expanded to {newString}");
                                            }
                                            else
                                            {
                                                Debug.Print($"Page {textLine.ParentPageNr.ToString("000")}, Line {textLine.Number.ToString("000")}: No expansion! Rubbish: {textLine.TextEquiv}");
                                            }
                                        }
                                        else
                                        {
                                            Debug.Print($"Page {textLine.ParentPageNr.ToString("000")}, Line {textLine.Number.ToString("000")}: No expansion! FirstNumber: {firstNumber}");
                                        }

                                        // Debug.Print($"       Expansion: {TrLibrary.ExpandStringWithNumericInterval(MatchString.ToString())}");
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
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            // Ender på g
                            Regex placeName = new Regex(@"\p{Lu}\p{Ll}+[g][\.\:]?");

                            MatchCollection placeNameMatches = placeName.Matches(textLine.TextEquiv);
                            if (placeNameMatches.Count > 0)
                            {
                                int position = placeNameMatches[0].Value.LastIndexOf('g');
                                string expansion = placeNameMatches[0].Value.Substring(0, position) + "gade";

                                //TL.AddAbbrevTag(PlaceNameMatches[0].Index, PlaceNameMatches[0].Value.Length, Expansion);
                                Debug.Print($"Page {textLine.ParentPageNr.ToString("000")}, Line {textLine.Number.ToString("000")}: {placeNameMatches[0].Value} expanded to {expansion}");
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
                foreach (TrRegion textRegion in Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.TextEquiv != string.Empty)                             // det går galt på en tom streng
                            {
                                if (textLine.HasSpecificStructuralTag("Date"))
                                {
                                    if (textLine.TextEquiv.Substring(0, 1) != "[")     // så den ikke gør det igen!!
                                    {
                                        string expandedDate = TrLibrary.GetDate(textLine.TextEquiv, ParentDocument.KOBACC_GetYear());
                                        Debug.WriteLine(expandedDate);
                                        if (expandedDate != "n/a")
                                        {
                                            textLine.TextEquiv = expandedDate;
                                            textLine.HasChanged = true;
                                        }
                                    }
                                }
                                else if (textLine.HasSpecificStructuralTag("Acc"))
                                {
                                    if (textLine.TextEquiv.Substring(0, 1) != "[")     // så den ikke gør det igen!!
                                    {
                                        string expandedAcc = TrLibrary.GetAccNo(textLine.TextEquiv, ParentDocument.KOBACC_GetYear());
                                        Debug.WriteLine(expandedAcc);
                                        if (expandedAcc != "n/a")
                                        {
                                            textLine.TextEquiv = expandedAcc;
                                            textLine.HasChanged = true;
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine(textLine.ExpandedText);
                                    textLine.DeleteSicAndAbbrevTags();
                                    textLine.TextEquiv = textLine.ExpandedText;
                                    textLine.HasChanged = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
