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
using System.Xml.Serialization;
using System.IO;
using TrClient;
using System.ComponentModel;
using System.Windows.Media;
using System.Text.RegularExpressions;
using DanishNLP;



namespace TrClient
{
    public class clsTrDocument : IComparable, INotifyPropertyChanged
    {
        public string TrpPages = "https://transkribus.eu/TrpServer/rest/collections/_ColID_/_DocID_/fulldoc.xml";


        public string Folder { get; set; }
        public string ID { get; set; }

        private int _nrOfPages = 0;
        public int NrOfPages
        {
            get { return _nrOfPages; }
            set { _nrOfPages = value; }
        }


        private string _title = "";
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private int _nrOfTranscriptsLoaded = 0;
        public int NrOfTranscriptsLoaded
        {
            get { return _nrOfTranscriptsLoaded; }
            set
            {
                if (_nrOfTranscriptsLoaded != value)
                {
                    _nrOfTranscriptsLoaded = value;
                    NotifyPropertyChanged("NrOfTranscriptsLoaded");
                }
            }
        }

        private int _nrOfTranscriptsChanged = 0;
        public int NrOfTranscriptsChanged
        {
            get { return _nrOfTranscriptsChanged; }
            set
            {
                if (_nrOfTranscriptsChanged != value)
                {
                    _nrOfTranscriptsChanged = value;
                    NotifyPropertyChanged("NrOfTranscriptsChanged");
                }
            }
        }

        private int _nrOfTranscriptsUploaded = 0;
        public int NrOfTranscriptsUploaded
        {
            get { return _nrOfTranscriptsUploaded; }
            set
            {
                if (_nrOfTranscriptsUploaded != value)
                {
                    _nrOfTranscriptsUploaded = value;
                    NotifyPropertyChanged("NrOfTranscriptsUploaded");
                }
            }
        }

        private int _numberOfRegions = 0;
        public int NumberOfRegions
        {
            get
            {
                int temp = 0;
                foreach (clsTrPage P in Pages)
                {
                    if (P.HasRegions)
                    {
                        temp = temp + P.NumberOfRegions;
                    }
                }
                _numberOfRegions = temp;
                return _numberOfRegions;
            }
        }

        private int _numberOfLines = 0;
        public int NumberOfLines
        {
            get
            {
                int temp = 0;
                foreach (clsTrPage P in Pages)
                {
                    if (P.HasRegions)
                    {
                        temp = temp + P.NumberOfLines;
                    }
                }
                _numberOfLines = temp;
                return _numberOfLines;
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


        public XmlDocument PagesAndTranscriptsMetadata = new XmlDocument();

        public clsTrPages Pages = new clsTrPages();
        public clsTrTranscripts Transcripts = new clsTrTranscripts();

        public clsTrDocuments ParentContainer;
        public clsTrCollection ParentCollection;

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
                ParentCollection.HasChanged = value;
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
                ParentCollection.ChangesUploaded = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }




        // constructor ONLINE
        public clsTrDocument(string DocTitle, string DocID, int DocNrOfPages)
        {
            Title = DocTitle;
            ID = DocID;
            NrOfPages = DocNrOfPages;

            Pages.ParentDocument = this;
            IsLoaded = false;
            // Debug.WriteLine($"Document constructed: {Title}");
        }

        // constructor OFFLINE
        public clsTrDocument(string DocTitle, string DocID, int DocNrOfPages, string DocFolder)
        {
            Title = DocTitle;
            Folder = DocFolder;
            ID = DocID;
            NrOfPages = DocNrOfPages;

            Pages.ParentDocument = this;
            IsLoaded = false;
            // Debug.WriteLine($"Document constructed: {Title}, ID: {ID}, NrOfPages: {NrOfPages}, Folder: {Folder}");
        }


        public int CompareTo(object obj)
        {
            var doc = obj as clsTrDocument;
            return Title.CompareTo(doc.Title);
        }

        public void Move(int OnPage, int Horizontally, int Vertically)
        {
            foreach (clsTrPage Page in Pages)
            {
                if (Page.PageNr == OnPage)
                {
                    Debug.WriteLine($"Moving regions on Page nr. {Page.PageNr}");
                    Page.Move(Horizontally, Vertically);
                }
            }
        }

        public void WrapSuperAndSubscriptWithSpaces()
        {
            foreach (clsTrPage Page in Pages)
                Page.WrapSuperAndSubscriptWithSpaces();
        }

        public void TagEmptyTextLines()
        {
            foreach (clsTrPage Page in Pages)
                Page.TagEmptyTextLines();
        }

        public void TagEmptyAbbrevTags()
        {
            foreach (clsTrPage Page in Pages)
                Page.TagEmptyAbbrevTags();
        }

        public string KOBACC_GetYear()
        {
            string Temp = Title.Substring(0, 4);
            return Temp;
        }


        public XDocument KOBACC_ExportAccessions()
        {
            XDocument xAccessionsDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XComment("Created by Transkribus Client - The Royal Danish Library"));

            XElement xRoot = new XElement("Root");

            XElement xAccessions = new XElement("Accessions",
                new XAttribute("Document", Title));

            XElement xSources = new XElement("Sources");

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (TL.TextEquiv != "")
                                {
                                    if (TL.HasSpecificStructuralTag("Acc"))
                                    {
                                        // Der kan være een eller flere...
                                        if (TL.TextEquiv.Contains(" - "))
                                        {
                                            // der ER flere
                                            string[] AccessionNumbers = TL.TextEquiv.Split('-').ToArray();
                                            foreach (string AN in AccessionNumbers)
                                            {
                                                if (AN != "n/a")
                                                {
                                                    XElement xAccession = new XElement("Accession", clsTrLibrary.StripSharpParanthesis(AN),
                                                        new XAttribute("Page", Page.PageNr),
                                                        new XAttribute("Hpos", TL.Hpos),
                                                        new XAttribute("Vpos", TL.Vpos));
                                                    xAccessions.Add(xAccession);
                                                }
                                            }

                                        }
                                        else
                                        {
                                            // der er kun een
                                            if (TL.TextEquiv != "n/a")
                                            {
                                                XElement xAccession = new XElement("Accession", clsTrLibrary.StripSharpParanthesis(TL.TextEquiv),
                                                    new XAttribute("Page", Page.PageNr),
                                                    new XAttribute("Hpos", TL.Hpos),
                                                    new XAttribute("Vpos", TL.Vpos));
                                                xAccessions.Add(xAccession);
                                            }
                                        }
                                    }
                                    else if (TL.HasSpecificStructuralTag("caption"))
                                    {
                                        XElement xSource = new XElement("Source", TL.TextEquiv,
                                            new XAttribute("Page", Page.PageNr),
                                            new XAttribute("Hpos", TL.Hpos),
                                            new XAttribute("Vpos", TL.Vpos));
                                        xSources.Add(xSource);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            xRoot.Add(xAccessions);
            xRoot.Add(xSources);
            xAccessionsDoc.Add(xRoot);
            return xAccessionsDoc;
        }


        //public async void CopyFromOtherCollection(clsTrCollection SourceCollection, HttpClient CurrentClient)
        //{
        //    Debug.WriteLine($"clsTrDocument:CopyFromOtherCollection");
        //    foreach (clsTrDocument SourceDocument in SourceCollection.Documents)
        //    {
        //        string SourceDocumentName = SourceDocument.Title;
        //        // Debug.WriteLine($"{SourceDocumentName}");
        //        {
        //            string ActualDocumentName = Title;
        //            // Debug.WriteLine($"{ActualDocumentName}");

        //            if (SourceDocumentName == ActualDocumentName)
        //            {
        //                Debug.WriteLine($"Document match!!! Title: {Title}");

        //                // henter nyeste transcript for hver side i Source ind 
        //                foreach (clsTrPage Page in SourceDocument.Pages)
        //                {
        //                    Task<bool> Loaded = Page.Transcripts[0].LoadTranscript(CurrentClient);
        //                    bool OK = await Loaded;
        //                }


        //                CopyFromOtherDocument(SourceDocument);
        //            }
        //        }
        //    }
        //}


        //public void CopyFromOtherDocument(clsTrDocument SourceDocument)
        //{
        //    Debug.WriteLine($"clsTrDocument:CopyFromOtherDocument");
        //    foreach (clsTrPage SourcePage in SourceDocument.Pages)
        //    {
        //        string SourceFileName = SourcePage.ImageFileName;
        //        // Debug.WriteLine($"{SourceFileName}");

        //        foreach (clsTrPage ActualPage in Pages)
        //        {
        //            string ActualPageFileName = ActualPage.ImageFileName;
        //            // Debug.WriteLine($"{ActualPageFileName}");

        //            if (SourceFileName == ActualPageFileName)
        //            {
        //                Debug.WriteLine($"Filename match!!! (page nr. {ActualPage.PageNr}): {ActualPage.ImageFileName}");
        //                clsTrRegions SourceRegions = SourcePage.Transcripts[0].TextRegions;

        //                foreach (clsTrTextRegion SourceRegion in SourceRegions)
        //                {
        //                    Debug.WriteLine($"Sourceregion # {SourceRegion.Number}");
        //                    ActualPage.AppendRegion(SourceRegion);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void SetColumnNumbers(int AssumedNumberOfColumns)
        //{
        //    foreach (clsTrPage Page in Pages)
        //    {
        //        Debug.WriteLine($"Setting Column Numbers on Page nr. {Page.PageNr}");
        //        Page.SetColumnNumbers(AssumedNumberOfColumns);
        //    }
        //}



        //public void SetRowNumbers(int AssumedNumberOfRows)
        //{
        //    foreach (clsTrPage Page in Pages)
        //    {
        //        Debug.WriteLine($"Setting Row Numbers on Page nr. {Page.PageNr}");
        //        Page.SetRowNumbers(AssumedNumberOfRows);    
        //    }
        //}

        public void CreateTopLevelRegions()
        {
            string LogFileName = "Create Top Level Regions";
            clsTrLog Log = new clsTrLog(LogFileName, ParentCollection.Name, Title);

            string LogFileMessage;
            bool OK;

            foreach (clsTrPage Page in Pages)
            {
                OK = Page.CreateTopLevelRegion();

                LogFileMessage = "Page#" + Page.PageNr.ToString().PadLeft(3) + "     Created? " + OK.ToString();
                Log.Add(LogFileMessage);
            }
            Log.Save();
        }


        public void AddHorizontalRegions(int UpperPercent, int LowerPercent, int UpperPadding, int LowerPadding)
        {
            string LogFileName = "Add Horizontal Regions";
            clsTrLog Log = new clsTrLog(LogFileName, ParentCollection.Name, Title);

            string LogFileMessage;
            int Number;

            foreach (clsTrPage Page in Pages)
            {
                Number = Page.AddHorizontalRegion(UpperPercent, LowerPercent, UpperPadding, LowerPadding);

                LogFileMessage = "Page#" + Page.PageNr.ToString().PadLeft(3) + "     Added region#" + Number.ToString().PadLeft(3);
                Log.Add(LogFileMessage);
            }
            Log.Save();
        }

        public clsTrTextLines FindText(string SearchFor)
        {
            clsTrTextLines TempList = new clsTrTextLines();
            foreach (clsTrPage Page in Pages)
            {
                clsTrTranscript Transcript = Page.Transcripts[0];
                foreach (clsTrRegion TR in Transcript.Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.Contains(SearchFor))
                            {
                                TempList.Add(TL);
                                TL.ParentRegion = (clsTrTextRegion)TR;
                            }

                            // og så tjekker vi om den ender med mellemrum etc.
                            if (SearchFor.EndsWith(" "))
                            {
                                if (TL.TextEquiv.EndsWith(SearchFor.Trim()))
                                {
                                    TempList.Add(TL);
                                    TL.ParentRegion = (clsTrTextRegion)TR;
                                }
                            }
                        }
                    }

                }
            }
            return TempList;
        }

        public List<string> GetRegionalTags()
        {
            List<string> TempList = new List<string>();
            foreach (clsTrPage Page in Pages)
            {
                clsTrTranscript Transcript = Page.Transcripts[0];
                if (Transcript.HasRegionalTags)
                {
                    foreach (clsTrRegion TR in Transcript.Regions)
                        if (TR.HasStructuralTag)
                            TempList.Add(TR.StructuralTagValue);
                }
            }
            List<string> TagList = TempList.Distinct().ToList();
            TagList.Sort();
            return TagList;
        }

        public int GetHighestRegionNumber()
        {
            int temp = 0;
            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    if (Page.Transcripts[0].Regions.Count > temp)
                        temp = Page.Transcripts[0].Regions.Count;
                }
            }
            return temp;
        }

        public int GetHighestLineNumber(int RegionNumber)
        {
            int temp = 0;
            foreach (clsTrPage Page in Pages)
            {
                if (Page.ExistsRegionNumber(RegionNumber))
                {
                    clsTrRegion TR = Page.Transcripts[0].GetRegionByNumber(RegionNumber);
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        if ((TR as clsTrTextRegion).TextLines.Count > temp)
                            temp = (TR as clsTrTextRegion).TextLines.Count;
                    }
                }
            }
            return temp;
        }

        public List<string> GetListOfPages()
        {
            List<string> temp = new List<string>();

            foreach (clsTrPage Page in Pages)
            {
                temp.Add(Page.PageNr.ToString());
            }
            return temp;
        }

        public List<string> GetListOfPossibleRegions()
        {
            List<string> temp = new List<string>();

            int i;
            int MaxRegion = GetHighestRegionNumber();
            int[] RegionsArray = new int[MaxRegion + 1];

            foreach (clsTrPage Page in Pages)
            {
                for (i = 1; i <= MaxRegion; i++)
                {
                    if (Page.ExistsRegionNumber(i))
                        RegionsArray[i]++;
                }
            }

            for (i = 1; i <= MaxRegion; i++)
            {
                if (RegionsArray[i] == Pages.Count)
                    temp.Add(i.ToString());
                else
                    temp.Add(i.ToString());
                // temp.Add($"({i.ToString()})");
            }

            return temp;
        }

        public List<string> GetListOfPossibleLinesInRegion(int RegionNumber)
        {
            List<string> temp = new List<string>();

            int i;
            int MaxLine = GetHighestLineNumber(RegionNumber);
            int[] LinesArray = new int[MaxLine + 1];

            foreach (clsTrPage Page in Pages)
            {
                if (Page.ExistsRegionNumber(RegionNumber))
                {
                    clsTrRegion TR = Page.Transcripts[0].GetRegionByNumber(RegionNumber);
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        for (i = 1; i <= MaxLine; i++)
                        {
                            if ((TR as clsTrTextRegion).ExistsLineNumber(i))
                                LinesArray[i]++;
                        }
                    }

                }
            }

            for (i = 1; i <= MaxLine; i++)
            {
                if (LinesArray[i] == Pages.Count)
                    temp.Add(i.ToString());
                else
                    // temp.Add($"({i.ToString()})");
                    temp.Add(i.ToString());
            }

            return temp;
        }

        private bool _hasRegions;
        public bool HasRegions
        {
            get
            {
                _hasRegions = (NrOfPagesWithRegions() > 0);
                return _hasRegions;
            }
        }

        private bool _hasTables;
        public bool HasTables
        {
            get
            {
                _hasTables = false;
                if (Pages.Count > 0)
                {
                    foreach (clsTrPage TP in Pages)
                        _hasTables = _hasTables || TP.HasTables;
                }
                return _hasTables;
            }
        }

        private bool _hasFormerTables;
        public bool HasFormerTables
        {
            get
            {
                _hasFormerTables = false;
                if (Pages.Count > 0)
                {
                    foreach (clsTrPage TP in Pages)
                        _hasFormerTables = _hasFormerTables || TP.HasFormerTables;
                }
                return _hasFormerTables;

            }
        }

        public void MergeAllRegionsToTopLevelRegion(int StartPage, int EndPage)
        {
            if (Pages.Count > 0)
            {
                foreach (clsTrPage TP in Pages)
                    if (TP.PageNr >= StartPage && TP.PageNr <= EndPage)
                    {
                        TP.MergeAllRegionsToTopLevelRegion();
                    }

            }
        }

        public void ConvertTablesToRegions(int StartPage, int EndPage)
        {
            if (Pages.Count > 0)
            {
                foreach (clsTrPage TP in Pages)
                    if (TP.PageNr >= StartPage && TP.PageNr <= EndPage)
                    {
                        if (TP.HasTables)
                        {
                            TP.ConvertTablesToRegions();
                            TP.DeleteEmptyRegions();
                            TP.Transcripts[0].Regions.ReNumberVertically();
                        }
                    }
            }
        }

        public void CopyOldTablesToNewestTranscript()
        {
            // NB: KUN hvis der er gamle og IKKE aktuelle tabeller!
            if (Pages.Count > 0)
            {
                foreach (clsTrPage TP in Pages)
                    if (TP.HasFormerTables && !TP.HasTables)
                    {
                        TP.CopyOldTablesToNewestTranscript();
                    }
            }
        }


        public clsTrTextLines GetLinesWithNumber(int RegionNumber, int LineNumber)
        {
            clsTrTextLines temp = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                if (Page.ExistsRegionNumber(RegionNumber))
                {
                    clsTrRegion TR = Page.Transcripts[0].GetRegionByNumber(RegionNumber);
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            if ((TR as clsTrTextRegion).ExistsLineNumber(LineNumber))
                            {
                                clsTrTextLine TL = (TR as clsTrTextRegion).GetLineByNumber(LineNumber);
                                temp.Add(TL);
                                TL.ParentRegion = (TR as clsTrTextRegion);
                            }

                        }
                    }
                }
            }
            return temp;
        }


        public clsTrTextLines GetAllLines()
        {
            clsTrTextLines TempList = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                TempList.Add(TL);
                                TL.ParentRegion = (clsTrTextRegion)TR;
                            }

                        }
                    }
                }
            }
            return TempList;
        }

        public clsTrTextLines GetLinesWithBaseLineProblems()
        {
            clsTrTextLines TempList = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (!TL.IsBaseLineDirectionOK || !TL.IsBaseLineStraight || !TL.IsCoordinatesPositive)
                                {
                                    TempList.Add(TL);
                                    TL.ParentRegion = (clsTrTextRegion)TR;
                                }
                            }

                        }
                    }
                }

            }
            return TempList;

        }

        public clsTrTextLines GetLines_BaseLineFiltered(clsTrBaseLineFilter CurrentFilter)
        {
            clsTrTextLines TempList = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (TL.IsOKwithBaseLineFilter(CurrentFilter))
                                {
                                    TempList.Add(TL);
                                    TL.ParentRegion = (clsTrTextRegion)TR;
                                }
                            }

                        }
                    }
                }

            }
            return TempList;
        }

        public clsTrTextLines GetFilteredLines(clsTrLineFilterSettings CurrentFilter)
        {
            clsTrTextLines TempList = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (TL.MeetsFilterConditions(CurrentFilter))
                                {
                                    TempList.Add(TL);
                                    TL.ParentRegion = (clsTrTextRegion)TR;
                                }

                            }

                        }


                    }
                }

            }
            return TempList;

        }

        public clsTrTextLines GetLines_WindowFiltered(clsTrLineFilterSettings Settings)
        {
            clsTrTextLines TempList = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (Settings.Inside)
                                {
                                    if (TL.IsInWindow(Settings))
                                    {
                                        TempList.Add(TL);
                                        TL.ParentRegion = (clsTrTextRegion)TR;
                                    }
                                }
                                else
                                {
                                    if (!TL.IsInWindow(Settings))
                                    {
                                        TempList.Add(TL);
                                        TL.ParentRegion = (clsTrTextRegion)TR;
                                    }
                                }
                            }

                        }


                    }
                }

            }
            return TempList;
        }

        public clsTrTextLines GetLines_RegexFiltered(string RegExPattern) // Regex MatchPattern
        {
            // Debug.Print($"clsTrDocument: Pattern: {MatchPattern.ToString()}");

            clsTrTextLines TempList = new clsTrTextLines();

            foreach (clsTrPage Page in Pages)
            {
                //Debug.Print($"Page {Page.PageNr}");

                if (Page.HasRegions)
                {
                    foreach (clsTrRegion TR in Page.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                //Debug.Print($"Line {TL.Number}");

                                if (TL.MatchesRegex(RegExPattern))
                                {
                                    TempList.Add(TL);
                                    TL.ParentRegion = (clsTrTextRegion)TR;
                                }
                            }
                        }
                    }
                }
            }
            return TempList;

        }

        public List<string> CheckElfeltRecordNumbers()
        {
            string CurrentRecord = "";

            char Delimiter = clsTrLibrary.CSV_Delimiter;
            List<string> TempList = new List<string>();

            foreach (clsTrPage P in Pages)
            {

                int LineCounter = 0;
                StringBuilder sb = new StringBuilder();

                foreach (clsTrRegion TR in P.Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {

                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            if (TL.HasSpecificStructuralTag("RecordName"))
                            {
                                LineCounter += 1;
                                CurrentRecord = P.PageNr.ToString() + Delimiter + LineCounter.ToString() + Delimiter + TL.ExpandedText;
                                TempList.Add(CurrentRecord);
                                //Debug.Print(CurrentRecord);
                            }
                        }
                    }
                }

            }
            return TempList;

        }

        public List<clsTrRecord> GetPseudoTableText(int MinimumRecordNumber)
        {
            List<clsTrRecord> TempRecords = new List<clsTrRecord>();
            string CurrentDate = "";
            string CurrentRecordName = "";
            string Metadata = "";
            char Delimiter = clsTrLibrary.CSV_Delimiter;

            foreach (clsTrPage P in Pages)
            {
                //Debug.Print($"Page # {PageNr} ----------------------------------------------------");

                int LineCounter = 0;

                int CurrentHpos = 0;
                int CurrentVpos = 0;
                int PreviousHpos = 0;
                int PreviousVpos = 0;

                StringBuilder sb = new StringBuilder();
                clsTrRecord NewRecord;

                foreach (clsTrRegion TR in P.Transcripts[0].Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {

                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            // Debug.Print($"Page # {P.PageNr} - TL number {TL.Number}");   
                            if (!TL.HasSpecificStructuralTag("Year") && !TL.HasSpecificStructuralTag("Title") && !TL.HasSpecificStructuralTag("Exclude"))
                            {
                                PreviousHpos = CurrentHpos;
                                PreviousVpos = CurrentVpos;

                                CurrentHpos = TL.Hpos;
                                CurrentVpos = TL.Vpos;

                                // test for linieskift = ny record
                                // dog hvis previousHpos og previousVpos begge er 0, er det den første TL på siden - den skal springes over
                                if (PreviousHpos != 0 && PreviousVpos != 0)
                                {
                                    // Debug.Print("Not zero");
                                    // hvis vi er rykket ned - eller det er den sidste TL skal der ske noget
                                    // 2.9.2021 ændret i linien herunder: før var den
                                    // ((CurrentHpos < (PreviousHpos - 300) && CurrentVpos > PreviousVpos) )
                                    // men det gav mange fejl i 4'eren, hvor det er meningen, at man godt må hoppe LIDT baglæns...
                                    // værdien øges derfor fra 300 til 1500

                                    if ((CurrentHpos < (PreviousHpos - 1500) && CurrentVpos > PreviousVpos))
                                    {
                                        // Debug.Print("current er rykket til venstre og ned");
                                        // linieskift er indtruffet!
                                        // men har vi data?
                                        LineCounter += 1;
                                        Metadata = sb.ToString().Trim();
                                        sb.Clear();

                                        if (CurrentRecordName != "" && CurrentRecordName != "00000" && Metadata != "" && Metadata.Length > 5)
                                        {
                                            // Debug.Print("Vi har data");


                                            // test for om CurrentRecordName indeholder mere end eet negativnummer!
                                            Regex Numbers = new Regex(@"\d+\p{L}?");
                                            string Stripped = clsLanguageLibrary.StripAll(CurrentRecordName);
                                            string RecordName = "";
                                            MatchCollection NumberMatches = Numbers.Matches(Stripped);
                                            if (NumberMatches.Count > 0)
                                            {
                                                foreach (Match M in NumberMatches)
                                                {
                                                    // vi tjekker, om tallet er stort nok til at være et negativnummer
                                                    Regex DigitsOnly = new Regex(@"\d+");
                                                    Match DigitsMatch = DigitsOnly.Match(M.Value);
                                                    if (DigitsMatch.Success)
                                                    {
                                                        // Debug.Print($"Succes! {M.Value} = {DigitsMatch.Value}");
                                                        if (Convert.ToInt32(DigitsMatch.Value) > MinimumRecordNumber)
                                                        {

                                                            RecordName = clsTrLibrary.ExtractRecordName(M.Value);
                                                            Debug.Print($"Creating record: {RecordName}!");
                                                            NewRecord = new clsTrRecord(RecordName, CurrentDate, Metadata, Title, P.PageNr, LineCounter);
                                                            TempRecords.Add(NewRecord);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                if (TL.HasDateTag)
                                {
                                    CurrentDate = clsLanguageLibrary.StripPunctuation(TL.ExpandedText);
                                    // Debug.Print($"Date!");
                                }
                                else if (TL.HasSpecificStructuralTag("RecordName"))
                                {
                                    CurrentRecordName = TL.ExpandedText;
                                    // Debug.Print($"RecordName(s)!");
                                }
                                else
                                {
                                    sb.Append(TL.ExpandedText);
                                    sb.Append(Delimiter);
                                    // Debug.Print($"Metadata!");
                                }
                            }
                        }
                    }
                }
                // sidste linie på siden!
                // Debug.Print("Sidst linie på siden!");

                LineCounter += 1;
                Metadata = sb.ToString().Trim();
                sb.Clear();
                if (CurrentRecordName != "" && CurrentRecordName != "00000" && Metadata != "" && Metadata.Length > 5)
                {
                    // Debug.Print("Vi har data");

                    // test for om CurrentRecordName indeholder mere end eet negativnummer!
                    Regex Numbers = new Regex(@"\d+\p{L}?");
                    string Stripped = clsLanguageLibrary.StripAll(CurrentRecordName);
                    string RecordName = "";
                    MatchCollection NumberMatches = Numbers.Matches(Stripped);
                    if (NumberMatches.Count > 0)
                    {
                        foreach (Match M in NumberMatches)
                        {
                            // vi tjekker, om tallet er stort nok til at være et negativnummer
                            Regex DigitsOnly = new Regex(@"\d+");
                            Match DigitsMatch = DigitsOnly.Match(M.Value);
                            if (DigitsMatch.Success)
                            {
                                // Debug.Print($"Succes! {M.Value} = {DigitsMatch.Value}");
                                if (Convert.ToInt32(DigitsMatch.Value) > MinimumRecordNumber)
                                {

                                    RecordName = clsTrLibrary.ExtractRecordName(M.Value);
                                    Debug.Print($"Creating record: {RecordName}!");
                                    NewRecord = new clsTrRecord(RecordName, CurrentDate, Metadata, Title, P.PageNr, LineCounter);
                                    TempRecords.Add(NewRecord);
                                }
                            }
                        }
                    }
                }

            }
            return TempRecords;
        }




        public void AutoRecordTag(double PercentualLeftBorder, double PercentualRightBorder)
        {
            Regex Numbers = new Regex(@"\d+");

            foreach (clsTrPage P in Pages)
            {
                if (P.HasRegions)
                {
                    foreach (clsTrRegion TR in P.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                if (TL.PercentualHpos >= PercentualLeftBorder && TL.PercentualHendPos <= PercentualRightBorder)
                                {
                                    string Stripped = clsLanguageLibrary.StripAll(TL.ExpandedText);

                                    MatchCollection NumberMatches = Numbers.Matches(Stripped);

                                    if (NumberMatches.Count == 1)
                                    {
                                        string RecordNumber = NumberMatches[0].Value;
                                        if (clsLanguageLibrary.ConsecutiveDigitCount(RecordNumber) >= 4)
                                            if (Convert.ToInt32(RecordNumber) > 2000)
                                                TL.AddStructuralTag("RecordName", true);
                                    }
                                    else if (NumberMatches.Count > 1)
                                    {
                                        Debug.Print($"Page # {P.PageNr}, Line # {TL.Number}: Too many matches");
                                        foreach (Match M in NumberMatches)
                                        {
                                            Debug.Print($"     Record: {M.Value}");
                                        }

                                    }

                                }
                            }
                        }
                    }
                }

            }
        }


        public void AutoDateTag(double PercentualLeftBorder, double PercentualRightBorder)
        {
            // sætter datotags i den kolonne, der er defineret ved de to parametre - procentuelt!
            // NB: Kører på DOCUMENT-niveau (og ikke page), fordi "CurrentYear" kan løbe hen over sideskift.
            int CurrentYear = 0;
            int CurrentMonth = 0;
            int CurrentDay = 0;
            int Test;
            bool SetDate = false;

            double LeftYearBorder = 25;
            double RightYearBorder = 75;
            int MinYear = 1898;
            int MaxYear = 1905;

            Regex Numbers = new Regex(@"\d+");

            foreach (clsTrPage P in Pages)
            {
                if (P.HasRegions)
                {
                    foreach (clsTrRegion TR in P.Transcripts[0].Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                // midlertidigt: vi sletter eksisterende datetags
                                if (TL.HasDateTag)
                                    TL.DeleteDateTags();

                                SetDate = false;
                                // finder et årstal på siden
                                if (TL.PercentualHpos >= LeftYearBorder && TL.PercentualHendPos <= RightYearBorder)
                                {
                                    string Stripped = clsLanguageLibrary.StripAll(TL.ExpandedText);

                                    if (clsLanguageLibrary.IsNumeric(Stripped))
                                    {
                                        if (clsLanguageLibrary.ConsecutiveDigitCount(Stripped) == 4)
                                        {
                                            int StrippedNumber = Convert.ToInt32(Stripped);
                                            if (StrippedNumber >= MinYear && StrippedNumber <= MaxYear)
                                            {
                                                CurrentYear = StrippedNumber;
                                                // Debug.Print($"Page no. {P.PageNr}: Current year: {CurrentYear}");
                                            }
                                        }
                                    }
                                }
                                //else 
                                if (TL.PercentualHpos >= PercentualLeftBorder && TL.PercentualHendPos <= PercentualRightBorder)
                                {
                                    string Stripped = clsLanguageLibrary.StripAll(TL.ExpandedText);

                                    if (clsLanguageLibrary.IsPossibleDate(Stripped))
                                    {
                                        // Debug.Print($"Possible date: Stripped = {Stripped}");

                                        // først tester vi, om der indgår et navn på en måned, dvs. formen d-MMM-yyyy / MMM-yyyy
                                        List<string> SourceList = Stripped.Split(' ').ToList();
                                        bool TestSource = false;
                                        int MonthIndex = -1;

                                        if (SourceList.Count > 0)
                                        {
                                            for (int i = 0; i < SourceList.Count; i++)
                                            {
                                                if (!TestSource)
                                                {
                                                    TestSource = clsLanguageLibrary.IsMonthName(SourceList[i]) || clsLanguageLibrary.IsMonthAbbreviation(SourceList[i]);
                                                    if (TestSource)
                                                        MonthIndex = i;
                                                }
                                            }

                                            // hvis monthindex er 0 eller højere, er der fundet noget:
                                            if (MonthIndex >= 0)
                                            {
                                                CurrentMonth = clsLanguageLibrary.GetMonthNumber(SourceList[MonthIndex]);

                                                // men vi må nulstille CurrentDay, for der kan være en gammel, som nu er forkert:
                                                CurrentDay = 0;
                                                SetDate = true;

                                                // hvis måneden ikke står i den første, kan der være en dato før
                                                if (MonthIndex > 0)
                                                {
                                                    if (clsLanguageLibrary.IsNumeric(SourceList[MonthIndex - 1]))
                                                    {
                                                        Test = Convert.ToInt32(SourceList[MonthIndex - 1]);
                                                        if (Test >= 1 && Test <= 31)
                                                            CurrentDay = Test;
                                                    }
                                                }

                                                // hvis måneden ikke står i den sidste, kan der være et årstal bagefter
                                                if (MonthIndex < SourceList.Count - 2)
                                                {
                                                    if (clsLanguageLibrary.IsNumeric(SourceList[MonthIndex + 1]))
                                                    {
                                                        Test = Convert.ToInt32(SourceList[MonthIndex + 1]);
                                                        if (Test >= 1890 && Test <= 1999)
                                                            CurrentYear = Test;
                                                    }
                                                }
                                            }
                                        }


                                        // hvis ikke, må det være en dato med rene tal
                                        if (!SetDate)
                                        {
                                            MatchCollection NumberMatches = Numbers.Matches(Stripped);
                                            // Debug.Print($"Matches: {NumberMatches.Count}");

                                            if (NumberMatches.Count == 3)
                                            {
                                                // dag, måned og år?
                                                Test = Convert.ToInt16(NumberMatches[0].Value); // dag?
                                                if (Test >= 1 && Test <= 31)
                                                    CurrentDay = Test;

                                                Test = Convert.ToInt16(NumberMatches[1].Value); // måned?
                                                if (Test >= 1 && Test <= 12)
                                                    CurrentMonth = Test;

                                                Test = Convert.ToInt16(NumberMatches[2].Value); // måned?
                                                if (Test >= 1890 && Test <= 1999)
                                                    CurrentYear = Test;

                                                SetDate = true;
                                            }
                                            else
                                            if (NumberMatches.Count == 2)
                                            {
                                                // dag og måned?
                                                Test = Convert.ToInt16(NumberMatches[0].Value); // dag?
                                                if (Test >= 1 && Test <= 31)
                                                    CurrentDay = Test;

                                                Test = Convert.ToInt16(NumberMatches[1].Value); // måned?
                                                if (Test >= 1 && Test <= 12)
                                                    CurrentMonth = Test;

                                                SetDate = true;

                                            }
                                            else
                                            if (NumberMatches.Count == 1)
                                            {
                                                // kun dag?
                                                Test = Convert.ToInt16(NumberMatches[0].Value); // dag?
                                                if (Test >= 1 && Test <= 31)
                                                    CurrentDay = Test;

                                                SetDate = true;
                                            }
                                        }



                                        if (SetDate)
                                        {
                                            // komplet dato?
                                            if (CurrentDay > 0 && CurrentMonth > 0)
                                            {
                                                int Offset = TL.TextEquiv.IndexOf(Stripped);
                                                int Length = Stripped.Length;

                                                // af sære grunde kan man nogle gange ikke finde den - så bliver offset -1, og det går ikke.
                                                // update: det er sandsynligvis ved abbrev-dates, hvor man i sagens natur ikke KAN finde den ...
                                                if (Offset < 0)
                                                {
                                                    Offset = 0;
                                                    Length = 1;
                                                }

                                                if (clsTrLibrary.IsValidDate(CurrentYear, CurrentMonth, CurrentDay))
                                                {
                                                    DateTime NewDate = new DateTime(CurrentYear, CurrentMonth, CurrentDay);
                                                    TL.AddDateTag(Offset, Length, NewDate);
                                                    // Debug.Print($"Date found: Page no. {P.PageNr}: Date: {NewDate.ToShortDateString()}");
                                                }
                                                else
                                                {
                                                    TL.AddDateTag(Offset, Length, CurrentDay, CurrentMonth, CurrentYear);
                                                    TL.AddStructuralTag("InvalidDate", true);
                                                    // Debug.Print($"Invalid date found: Page no. {P.PageNr}: Date: {CurrentDay}-{CurrentMonth}-{CurrentYear}");
                                                }

                                            }
                                            else if (CurrentMonth > 0)
                                            {
                                                // inkomplet dato: kun måned; sætter tag på hele TL
                                                TL.AddDateTag(0, TL.TextEquiv.Length, CurrentDay, CurrentMonth, CurrentYear);

                                            }
                                        }

                                    }


                                }
                            }
                        }
                    }
                }

            }

        }




        public int NrOfPagesWithRegions()
        {
            int temp = 0;
            foreach (clsTrPage Page in Pages)
                if (Page.HasRegions)
                    temp++;
            return temp;
        }

        public string GetListOfPagesWithRegions()
        {
            string temp = "";
            foreach (clsTrPage Page in Pages)
                if (Page.HasRegions)
                    temp = temp + Page.PageNr.ToString() + ", ";
            if (temp.Length > 2)
                return temp.Substring(0, temp.Length - 2);
            else
                return temp;
        }

        public string GetListOfPagesWithOverlappingRegions()
        {
            string temp = "";
            bool PageOK;
            bool BorderOK;

            foreach (clsTrPage Page in Pages)
                if (Page.HasRegions)
                {
                    PageOK = true;
                    clsTrTranscript Transcript = Page.Transcripts[0];
                    if (Transcript.Regions.Count > 3) // do. - ellers bør det være 1   
                    {
                        // specifikt for at se om 3-4-5 etc. i MASTER har problemer (ellers bør det være int i = 0)
                        for (int i = 2; i <= Transcript.Regions.Count - 2; i++) // mindre end -2, fordi den sidste (-1) bestiles som i+1
                        {
                            if (Transcript.Regions[i].BottomBorder < (Transcript.Regions[i + 1].TopBorder + 5))
                                BorderOK = true;
                            else
                                BorderOK = false;
                            PageOK = PageOK && BorderOK; // det sidste +5 er for at få falske positive, hvor en region er klippet over - så har de samme t/b-værdi 
                            if (!BorderOK)
                                Debug.WriteLine($"Page {Page.PageNr}: Region {Transcript.Regions[i].Number}, bottom: {Transcript.Regions[i].BottomBorder} - Region {Transcript.Regions[i + 1].Number}, top: {Transcript.Regions[i + 1].TopBorder}");
                        }
                        if (!PageOK)
                        {
                            temp = temp + Page.PageNr.ToString() + ", ";
                        }
                    }
                }
            if (temp.Length > 2)
                return temp.Substring(0, temp.Length - 2);
            else
                return temp;
        }

        public string GetListOfPagesWithoutRegionalTags()
        {
            string temp = "";
            foreach (clsTrPage Page in Pages)
            {
                clsTrTranscript Transcript = Page.Transcripts[0];
                if (!Transcript.HasRegionalTags)
                    temp = temp + Page.PageNr.ToString() + ", ";
            }
            if (temp.Length > 2)
                return temp.Substring(0, temp.Length - 2);
            else
                return temp;
        }

        public void DeleteLinesWithTag(string StructuralTagValue)
        {
            foreach (clsTrPage Page in Pages)
                Page.DeleteLinesWithTag(StructuralTagValue);
        }

        public void DeleteLinesOtherThan(string StructuralTagValue)
        {
            foreach (clsTrPage Page in Pages)
                Page.DeleteLinesOtherThan(StructuralTagValue);
        }

        public void DeleteRegionsWithTag(string RegionalTagValue)
        {
            foreach (clsTrPage Page in Pages)
                Page.DeleteRegionsWithTag(RegionalTagValue);
        }


        public void DeleteRegionsOtherThan(string RegionalTagValue)
        {
            foreach (clsTrPage Page in Pages)
                Page.DeleteRegionsOtherThan(RegionalTagValue);
        }

        public void DeleteEmptyRegions()
        {
            foreach (clsTrPage Page in Pages)
                Page.DeleteEmptyRegions();
        }

        public void SimplifyBoundingBoxes()
        {
            foreach (clsTrPage Page in Pages)
                Page.SimplifyBoundingBoxes();
        }

        public void SimplifyBoundingBoxes(int MinimumHeight, int MaximumHeight)
        {
            foreach (clsTrPage Page in Pages)
                Page.SimplifyBoundingBoxes(MinimumHeight, MaximumHeight);
        }


        public void DeleteShortBaseLines(clsTrDialogTransferSettings Settings)
        {
            bool ProcessThis;
            string LogFileName = "Delete Short BaseLines";
            clsTrLog Log = new clsTrLog(LogFileName, ParentCollection.Name, Title);

            foreach (clsTrPage Page in Pages)
            {
                if (Settings.AllPages)
                    ProcessThis = true;
                else if (Page.PageNr >= Settings.PagesFrom && Page.PageNr <= Settings.PagesTo)
                    ProcessThis = true;
                else
                    ProcessThis = false;

                if (ProcessThis)
                {
                    Log.AddLine();
                    Page.DeleteShortBaseLines(Settings, Log);
                }
            }
            // Log.Show();
            Log.Save();
        }

        public void ExtendBaseLines(clsTrDialogTransferSettings Settings)
        {
            // Debug.WriteLine($"clsTrDocument : ExtendBaseLines");

            bool ProcessThis;
            string LogFileName = "Extend BaseLines";
            clsTrLog Log = new clsTrLog(LogFileName, ParentCollection.Name, Title);

            foreach (clsTrPage Page in Pages)
            {
                if (Settings.AllPages)
                    ProcessThis = true;
                else if (Page.PageNr >= Settings.PagesFrom && Page.PageNr <= Settings.PagesTo)
                    ProcessThis = true;
                else
                    ProcessThis = false;

                if (ProcessThis)
                {
                    Log.AddLine();
                    Page.ExtendBaseLines(Settings, Log);
                }
            }
            // Log.Show();
            Log.Save();
        }


        //public void RepairBaseLines()
        //{
        //    string LogFileName = "Repair BaseLines";
        //    clsTrLog Log = new clsTrLog(LogFileName, ParentCollection.Name, Title);

        //    foreach (clsTrPage Page in Pages)
        //    {
        //        Log.AddLine();
        //        Page.RepairBaseLines(Log);
        //    }
        //    Log.Show();
        //    Log.Save();
        //}

        public List<string> GetExpandedText(bool Refine, bool ConvertOtrema)
        {
            List<string> TempList = new List<string>();
            List<string> PageList;

            foreach (clsTrPage P in Pages)
            {
                PageList = P.GetExpandedText(Refine, ConvertOtrema);
                foreach (string s in PageList)
                    TempList.Add(s);
            }
            return TempList;
        }

        public List<string> GetExpandedWords(bool Refine, bool ConvertOtrema)
        {
            List<string> LineList = GetExpandedText(Refine, ConvertOtrema);
            List<string> TempList = new List<string>();

            foreach (string L in LineList)
            {
                var WordArray = L.Split(' ').ToArray();
                int WordCount = WordArray.Length;
                for (int i = 0; i < WordCount; i++)
                    TempList.Add(WordArray[i].ToString());
            }
            List<string> SortedList = TempList.Distinct().ToList();
            SortedList.Sort();

            return SortedList;
        }

        private clsTrWords _words = new clsTrWords();
        public clsTrWords Words
        {
            get
            {
                foreach (clsTrPage P in Pages)
                {
                    foreach (clsTrWord W in P.Words)
                        _words.Add(W);
                }
                return _words;
            }
        }


        private clsTrLemmas _lemmas = new clsTrLemmas();
        public clsTrLemmas Lemmas
        {
            get
            {
                if (_lemmas.Count == 0)
                {
                    _lemmas.Clear();

                    foreach (clsTrWord Word in Words)
                        _lemmas.AddWord(Word);
                }
                return _lemmas;
            }
        }


        public List<string> GetTextualTags()
        {
            List<string> TempList = new List<string>();
            List<string> LineTags;

            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasRegions)
                {
                    clsTrTranscript Transcript = Page.Transcripts[0];
                    foreach (clsTrRegion TR in Transcript.Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                                if (TL.HasTags)
                                {
                                    LineTags = TL.GetTextualTags();
                                    foreach (string TagString in LineTags)
                                        TempList.Add(TagString);
                                }

                        }
                    }
                }
            }
            List<string> TagList = TempList.Distinct().ToList();
            TagList.Sort();

            foreach (string s in TagList)
                Debug.WriteLine(s);

            return TagList;
        }

        public List<string> GetStructuralTags()
        {
            List<string> TempList = new List<string>();

            foreach (clsTrPage Page in Pages)
            {
                clsTrTranscript Transcript = Page.Transcripts[0];
                if (Transcript.HasStructuralTags)
                {
                    foreach (clsTrRegion TR in Transcript.Regions)
                    {
                        List<string> TagsInRegion = TR.GetStructuralTags();
                        foreach (string s in TagsInRegion)
                            TempList.Add(s);
                    }
                }
            }
            List<string> TagList = TempList.Distinct().ToList();
            TagList.Sort();
            return TagList;
        }

        public void RenameStructuralTags(string OldName, string NewName)
        {
            foreach (clsTrPage Page in Pages)
            {
                clsTrTranscript Transcript = Page.Transcripts[0];
                if (Transcript.HasStructuralTags)
                {
                    foreach (clsTrRegion TR in Transcript.Regions)
                    {
                        if (TR.GetType() == typeof(clsTrTextRegion))
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                                if (TL.HasStructuralTag)
                                {
                                    if (TL.StructuralTagValue == OldName)
                                        TL.RenameStructuralTag(OldName, NewName);
                                }
                    }
                }
            }
        }

        public void OpenPages()
        {
            // Debug.WriteLine($"OpenPages entred");
            if (!IsLoaded)
            {
                // bruges kun OFFLINE
                string MetsFileName = Folder + "mets.xml";
                // Debug.WriteLine($"OpenPages: Metsfile = {MetsFileName}");

                XmlDocument MetsDocument = new XmlDocument();
                MetsDocument.Load(MetsFileName);

                // Filer i dokument
                XmlNodeList FileNodes = MetsDocument.DocumentElement.SelectNodes("//*[name()='ns3:FLocat']");

                // siderne behandles
                foreach (XmlNode xn in FileNodes)
                {
                    string fn = xn.Attributes[3].Value;
                    // Debug.WriteLine($"OpenPages: fn = {fn}");
                    if (fn.Length > 20)
                    {
                        // så er vi inde på den enkelte side
                        // ------------------------------------------------------------------------------------------------------------------
                        string PageID = "";
                        int PageNr = 0;
                        string ImageFileName = "";
                        string ImageURL = "";
                        int Width = 0;
                        int Height = 0;

                        string DocumentFileName = Folder + xn.Attributes[3].Value.Replace("/", "\\");
                        //Debug.WriteLine($"OpenPages: DocFileName = {DocumentFileName}");

                        XmlDocument PageDocument = new XmlDocument();
                        PageDocument.Load(DocumentFileName);

                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(PageDocument.NameTable);
                        nsmgr.AddNamespace("tr", "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15");

                        // XmlNodeList PageNodes = PageDocument.DocumentElement.SelectNodes("//tr:Page", nsmgr);


                        // Debug.WriteLine($"OpenPages: Første foreach - leder efter TranskribusMetadata");
                        XmlNodeList PageMetaData = PageDocument.DocumentElement.SelectNodes("//tr:TranskribusMetadata", nsmgr); // 
                        foreach (XmlNode px in PageMetaData)
                        {
                            //Debug.WriteLine($"OpenPages: pxløkke");
                            //Debug.WriteLine($"px.name = {px.Name}");

                            if (px.Name == "TranskribusMetadata")
                            {
                                //Debug.WriteLine($"OpenPages: px er fundet trmeta");
                                foreach (XmlAttribute pxa in px.Attributes)
                                {
                                    string Name = pxa.Name;
                                    string Value = pxa.Value;

                                    switch (Name)
                                    {
                                        case "pageId":
                                            PageID = Value;
                                            break;
                                        case "pageNr":
                                            PageNr = Int32.Parse(Value);
                                            break;
                                        case "imgUrl":
                                            ImageURL = Value;
                                            break;
                                    }
                                }
                            }
                        }

                        //Debug.WriteLine($"OpenPages: Anden foreach - leder efter PcGts");
                        XmlNodeList PageContent = PageDocument.DocumentElement.SelectNodes("//tr:Page", nsmgr);
                        foreach (XmlNode px in PageContent)
                        {
                            //Debug.WriteLine($"OpenPages: pxløkke2");
                            //Debug.WriteLine($"px.name = {px.Name}");
                            if (px.Name == "Page")
                            {
                                //Debug.WriteLine($"OpenPages: px = page");

                                foreach (XmlAttribute pxa in px.Attributes)
                                {
                                    string Name = pxa.Name;
                                    string Value = pxa.Value;

                                    switch (Name)
                                    {
                                        case "imageFilename":
                                            ImageFileName = Value;
                                            break;
                                        case "imageWidth":
                                            Width = Int32.Parse(Value);
                                            break;
                                        case "imageHeight":
                                            Height = Int32.Parse(Value);
                                            break;
                                    }
                                }
                            }
                        }

                        //Debug.WriteLine($"OpenPages: burde nu være klar til at danne en ny side");
                        //Debug.WriteLine($"PageID, PageNr, ImageFileName, Width, Height = {PageID}, {PageNr}, {ImageFileName}, {Width}, {Height}");


                        clsTrPage Page = new clsTrPage(PageID, PageNr, ImageFileName, ImageURL, Width, Height, DocumentFileName);
                        Pages.Add(Page);

                        clsTrTranscript T = new clsTrTranscript(DocumentFileName);
                        Page.Transcripts.Add(T);

                        T.LoadPageXML();
                        IsLoaded = true;

                    }
                }
                Pages.Sort();
            }
        }

        public async Task<bool> LoadPages(HttpClient CurrentClient)
        {
            // bruges kun ONLINE
            if (!IsLoaded)
            {
                TrpPages = TrpPages.Replace("_ColID_", ParentCollection.ID);
                TrpPages = TrpPages.Replace("_DocID_", ID);
                // Debug.WriteLine(TrpPages);

                // Henter de relevante pages ind i et XMLdoc
                HttpResponseMessage PagesResponseMessage = await CurrentClient.GetAsync(TrpPages);
                string PagesResponse = await PagesResponseMessage.Content.ReadAsStringAsync();
                PagesAndTranscriptsMetadata.LoadXml(PagesResponse);
                // Debug.WriteLine($"henter sider i dokumentet {Title}, {ID}");
                // Debug.WriteLine($"antal sider 1 = {Pages.Count}");

                // Og gemmer - i udviklingsfasen - xml-filen.
                // Det har samtidig den fordel, at det forsinker lidt....
                //string XMLFileName = clsTrLibrary.ExportFolder + ParentCollection.Name + "_" + Title + ".xml";
                //PagesAndTranscriptsMetadata.Save(XMLFileName);


                // Udtrækker de enkelte pages
                XmlNodeList PageNodes = PagesAndTranscriptsMetadata.DocumentElement.SelectNodes("//pages");
                foreach (XmlNode xnPage in PageNodes)
                {
                    XmlNodeList PageMetaData = xnPage.ChildNodes;
                    string PageID = "";
                    int PageNr = 0;
                    string ImageFileName = "";
                    string ImageURL = "";
                    int Width = 0;
                    int Height = 0;

                    foreach (XmlNode xnPageMetaData in PageMetaData)
                    {
                        string Name = xnPageMetaData.Name;
                        string Value = xnPageMetaData.InnerText;

                        switch (Name)
                        {
                            case "pageId":
                                PageID = Value;
                                break;
                            case "pageNr":
                                PageNr = Int32.Parse(Value);
                                break;
                            case "imgFileName":
                                ImageFileName = Value;
                                break;
                            case "url":
                                ImageURL = Value;
                                break;
                            case "width":
                                Width = Int32.Parse(Value);
                                break;
                            case "height":
                                Height = Int32.Parse(Value);
                                break;
                        }
                    }
                    clsTrPage Page = new clsTrPage(PageID, PageNr, ImageFileName, ImageURL, Width, Height);
                    Pages.Add(Page);

                }
                // Debug.WriteLine($"antal sider 2 = {Pages.Count}");
                Pages.Sort();

                // Udtrækker de enkelte transcripts
                XmlNodeList TranscriptNodes = PagesAndTranscriptsMetadata.DocumentElement.SelectNodes("//transcripts");
                foreach (XmlNode xnTranscript in TranscriptNodes)
                {
                    XmlNodeList TranscriptMetaData = xnTranscript.ChildNodes;

                    string TranscriptID = "";
                    string TranscriptKey = "";
                    int PageNr = 0;
                    string TranscriptStatus = "";
                    string TranscriptUser = "";
                    string Timestamp = "";
                    int NumberOfRegions = 0;
                    int NumberOfTranscribedRegions = 0;
                    int NumberOfLines = 0;
                    int NumberOfTranscribedLines = 0;

                    // Debug.WriteLine("Henter transcripts.");
                    int temp = 0;

                    foreach (XmlNode xnTranscriptMetaData in TranscriptMetaData)
                    {
                        string Name = xnTranscriptMetaData.Name;
                        string Value = xnTranscriptMetaData.InnerText;
                        temp++;
                        // Debug.WriteLine($"trsc. {temp}:\t{Name}\t{Value}");

                        switch (Name)
                        {
                            case "tsId":
                                TranscriptID = Value;
                                break;
                            case "key":
                                TranscriptKey = Value;
                                break;
                            case "pageNr":
                                PageNr = Int32.Parse(Value);
                                break;
                            case "status":
                                TranscriptStatus = Value;
                                break;
                            case "userName":
                                TranscriptUser = Value;
                                break;
                            case "timestamp":
                                Timestamp = Value;
                                break;
                            case "nrOfRegions":
                                NumberOfRegions = Int32.Parse(Value);
                                break;
                            case "nrOfTranscribedRegions":
                                NumberOfTranscribedRegions = Int32.Parse(Value);
                                break;
                            case "nrOfLines":
                                NumberOfLines = Int32.Parse(Value);
                                break;
                            case "nrOfTranscribedLines":
                                NumberOfTranscribedLines = Int32.Parse(Value);
                                break;
                        }
                    }

                    clsTrTranscript Transcript = new clsTrTranscript(TranscriptID, TranscriptKey, PageNr, TranscriptStatus,
                        TranscriptUser, Timestamp);

                    Transcripts.Add(Transcript);

                }

                // Debug.WriteLine($"antal transcripts = {Transcripts.Count}");

                // så nu fordeler vi dem på siderne
                foreach (clsTrTranscript Tra in Transcripts)
                {
                    Pages.GetPageFromPageNr(Tra.PageNr).Transcripts.Add(Tra);

                }
                // og sorterer dem - og vender dem rundt, så nyeste er først
                foreach (clsTrPage Page in Pages)
                {
                    Page.Transcripts.Sort();
                    Page.Transcripts.Reverse();
                    // Page.IsLoaded = true;
                    // Debug.WriteLine($"Page # {Page.PageNr}: {Page.TranscriptCount} transcripts.");
                }
                IsLoaded = true;
                ParentCollection.NrOfDocsLoaded++;
                Debug.WriteLine($"Document {ParentCollection.Name} / {Title} loaded!");
            }

            return true;

        }

        public void Upload(HttpClient CurrentClient)
        {
            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasChanged)
                    Page.Transcripts[0].Upload(CurrentClient);
            }

            Debug.Print($"***** RESULT: Transcripts changed: {NrOfTranscriptsChanged} - Transcripts uploaded: {NrOfTranscriptsUploaded}");

        }

        public void Save()
        {
            foreach (clsTrPage Page in Pages)
            {
                if (Page.HasChanged)
                {
                    Page.Transcripts[0].Save();

                }
            }
        }


        public bool PostLoadTestOK()
        {
            // udfører standard test efter load af doc.
            // det undersøges:
            // 1. om regioner, liner og baselines har ulovlige koordinater (mindre end 0, større end siden)
            // 2. om nogle textlines skal trimmes for leading og trailing spaces

            bool RegionsOK = true;
            bool LinesOK = true;
            bool BaseLinesOK = true;
            bool SpacesOK = true;

            bool RegionOK;
            bool LineOK;
            bool BaseLineOK;
            bool SpaceOK;

            bool DocumentOK;

            foreach (clsTrPage P in Pages)
            {
                //Debug.Print($"Page# {P.PageNr} ----------------------------------------------------");

                if (P.HasRegions)
                {
                    foreach (clsTrRegion TR in P.Transcripts[0].Regions)
                    {
                        RegionOK = clsTrLibrary.CheckCoordinates(TR.CoordsString, P.Width, P.Height);
                        RegionsOK = RegionsOK && RegionOK;
                        //Debug.Print($"Reg.# {TR.Number.ToString("000")}: Area {RegionOK}");

                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                LineOK = clsTrLibrary.CheckCoordinates(TL.CoordsString, P.Width, P.Height);
                                LinesOK = LinesOK && LineOK;

                                BaseLineOK = clsTrLibrary.CheckCoordinates(TL.BaseLineCoordsString, P.Width, P.Height);
                                BaseLinesOK = BaseLinesOK && BaseLineOK;

                                SpaceOK = TL.TextEquiv == TL.TextEquiv.Trim();
                                SpacesOK = SpacesOK && SpaceOK;

                                //Debug.Print($"Line# {TL.Number.ToString("000")}: Line {LineOK} - BaseLine {BaseLineOK} - Spaces {SpaceOK}");

                            }
                        }
                    }
                }
            }

            DocumentOK = RegionsOK && LinesOK && BaseLinesOK && SpacesOK;
            return DocumentOK;
        }

        public void PostLoadFix()
        {
            // fixer de ting, som PostLoadTest finder:
            // 1. regioner, liner og baselines: ulovlige koordinater (mindre end 0, større end siden) fixes
            // 2. textlines trimmes for leading og trailing spaces

            bool RegionOK;
            bool LineOK;
            bool BaseLineOK;
            bool SpaceOK;

            foreach (clsTrPage P in Pages)
            {
                Debug.Print($"Page# {P.PageNr} ----------------------------------------------------");

                if (P.HasRegions)
                {
                    foreach (clsTrRegion TR in P.Transcripts[0].Regions)
                    {
                        RegionOK = clsTrLibrary.CheckCoordinates(TR.CoordsString, P.Width, P.Height);

                        if (!RegionOK)
                        {
                            Debug.Print($"Region# {TR.Number.ToString("000")}: Fixing coordinates");
                            TR.FixCoordinates();
                        }

                        if (TR.GetType() == typeof(clsTrTextRegion))
                        {
                            foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                            {
                                LineOK = clsTrLibrary.CheckCoordinates(TL.CoordsString, P.Width, P.Height);
                                if (!LineOK)
                                {
                                    Debug.Print($"Line# {TL.Number.ToString("000")}: Fixing line area coordinates");
                                    TL.FixLineCoordinates();
                                }

                                BaseLineOK = clsTrLibrary.CheckCoordinates(TL.BaseLineCoordsString, P.Width, P.Height);
                                if (!BaseLineOK)
                                {
                                    Debug.Print($"Line# {TL.Number.ToString("000")}: Fixing baseline coordinates");
                                    TL.FixBaseLineCoordinates();
                                }

                                SpaceOK = TL.TextEquiv == TL.TextEquiv.Trim();
                                if (!SpaceOK)
                                {
                                    Debug.Print($"Line# {TL.Number.ToString("000")}: Trimming line");
                                    TL.Trim();
                                }
                            }
                        }
                    }
                }
            }

        }




    }
}
