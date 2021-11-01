// <copyright file="TrDocument.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Linq;
    using DanishNLP;
    using TrClient.Extensions;
    using TrClient.Helpers;
    using TrClient.Libraries;

    public class TrDocument : IComparable, INotifyPropertyChanged
    {
        public string TrpPages = Properties.Resources.TrpServerBaseAddress + Properties.Resources.TrpServerPathPagesList;
            // "https://transkribus.eu/TrpServer/rest/collections/_ColID_/_DocID_/fulldoc.xml";

        public string Folder { get; set; }

        public string ID { get; set; }

        private int nrOfPages = 0;

        public int NrOfPages
        {
            get { return nrOfPages; }
            set { nrOfPages = value; }
        }

        private string title = string.Empty;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (title != value)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private int nrOfTranscriptsLoaded = 0;

        public int NrOfTranscriptsLoaded
        {
            get
            {
                return nrOfTranscriptsLoaded;
            }

            set
            {
                if (nrOfTranscriptsLoaded != value)
                {
                    nrOfTranscriptsLoaded = value;
                    NotifyPropertyChanged("NrOfTranscriptsLoaded");
                }
            }
        }

        private int nrOfTranscriptsChanged = 0;

        public int NrOfTranscriptsChanged
        {
            get
            {
                return nrOfTranscriptsChanged;
            }

            set
            {
                if (nrOfTranscriptsChanged != value)
                {
                    nrOfTranscriptsChanged = value;
                    NotifyPropertyChanged("NrOfTranscriptsChanged");
                }
            }
        }

        private int nrOfTranscriptsUploaded = 0;

        public int NrOfTranscriptsUploaded
        {
            get
            {
                return nrOfTranscriptsUploaded;
            }

            set
            {
                if (nrOfTranscriptsUploaded != value)
                {
                    nrOfTranscriptsUploaded = value;
                    NotifyPropertyChanged("NrOfTranscriptsUploaded");
                }
            }
        }

        private int numberOfRegions = 0;

        public int NumberOfRegions
        {
            get
            {
                int temp = 0;
                foreach (TrPage p in Pages)
                {
                    if (p.HasRegions)
                    {
                        temp = temp + p.NumberOfRegions;
                    }
                }

                numberOfRegions = temp;
                return numberOfRegions;
            }
        }

        private int numberOfLines = 0;

        public int NumberOfLines
        {
            get
            {
                int temp = 0;
                foreach (TrPage p in Pages)
                {
                    if (p.HasRegions)
                    {
                        temp = temp + p.NumberOfLines;
                    }
                }

                numberOfLines = temp;
                Debug.Print($"Document {Title}: Number of lines = {numberOfLines}");
                return numberOfLines;
            }
        }

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

        public XmlDocument PagesAndTranscriptsMetadata = new XmlDocument();

        public TrPages Pages = new TrPages();
        public TrTranscripts Transcripts = new TrTranscripts();

        public TrDocuments ParentContainer;
        public TrCollection ParentCollection;

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

                ParentCollection.HasChanged = value;
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

                ParentCollection.ChangesUploaded = value;
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

        // constructor ONLINE
        public TrDocument(string docTitle, string docID, int docNrOfPages)
        {
            Title = docTitle;
            ID = docID;
            NrOfPages = docNrOfPages;

            Pages.ParentDocument = this;
            IsLoaded = false;

            // Debug.WriteLine($"Document constructed: {Title}");
        }

        // constructor OFFLINE
        //public TrDocument(string docTitle, string docID, int docNrOfPages, string docFolder)
        //{
        //    Title = docTitle;
        //    Folder = docFolder;
        //    ID = docID;
        //    NrOfPages = docNrOfPages;

        //    Pages.ParentDocument = this;
        //    IsLoaded = false;

        //    // Debug.WriteLine($"Document constructed: {Title}, ID: {ID}, NrOfPages: {NrOfPages}, Folder: {Folder}");
        //}

        public int CompareTo(object obj)
        {
            var doc = obj as TrDocument;
            return Title.CompareTo(doc.Title);
        }

        public void Move(int onPage, int horizontally, int vertically)
        {
            foreach (TrPage page in Pages)
            {
                if (page.PageNr == onPage)
                {
                    Debug.WriteLine($"Moving regions on Page nr. {page.PageNr}");
                    page.Move(horizontally, vertically);
                }
            }
        }

        public void WrapSuperAndSubscriptWithSpaces()
        {
            foreach (TrPage page in Pages)
            {
                page.WrapSuperAndSubscriptWithSpaces();
            }
        }

        public void TagEmptyTextLines()
        {
            foreach (TrPage page in Pages)
            {
                page.TagEmptyTextLines();
            }
        }

        public void TagEmptyAbbrevTags()
        {
            foreach (TrPage page in Pages)
            {
                page.TagEmptyAbbrevTags();
            }
        }

        public string KOBACC_GetYear()
        {
            string temp = Title.Substring(0, 4);
            return temp;
        }

        //public XDocument KOBACC_ExportAccessions()
        //{
        //    XDocument xAccessionsDoc = new XDocument(
        //        new XDeclaration("1.0", "UTF-8", "yes"),
        //        new XComment("Created by Transkribus httpClient - The Royal Danish Library"));

        //    XElement xRoot = new XElement("Root");

        //    XElement xAccessions = new XElement(
        //        "Accessions",
        //        new XAttribute("Document", Title));

        //    XElement xSources = new XElement("Sources");

        //    foreach (TrPage page in Pages)
        //    {
        //        if (page.HasRegions)
        //        {
        //            foreach (TrRegion textRegion in page.Transcripts[0].Regions)
        //            {
        //                if (textRegion.GetType() == typeof(TrTextRegion))
        //                {
        //                    foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
        //                    {
        //                        if (textLine.TextEquiv != string.Empty)
        //                        {
        //                            if (textLine.HasSpecificStructuralTag("Acc"))
        //                            {
        //                                // Der kan være een eller flere...
        //                                if (textLine.TextEquiv.Contains(" - "))
        //                                {
        //                                    // der ER flere
        //                                    string[] accessionNumbers = textLine.TextEquiv.Split('-').ToArray();
        //                                    foreach (string aN in accessionNumbers)
        //                                    {
        //                                        if (aN != "n/a")
        //                                        {
        //                                            XElement xAccession = new XElement("Accession", TrLibrary.StripSharpParanthesis(aN),
        //                                                new XAttribute("Page", page.PageNr),
        //                                                new XAttribute("Hpos", textLine.Hpos),
        //                                                new XAttribute("Vpos", textLine.Vpos));
        //                                            xAccessions.Add(xAccession);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    // der er kun een
        //                                    if (textLine.TextEquiv != "n/a")
        //                                    {
        //                                        XElement xAccession = new XElement("Accession", TrLibrary.StripSharpParanthesis(textLine.TextEquiv),
        //                                            new XAttribute("Page", page.PageNr),
        //                                            new XAttribute("Hpos", textLine.Hpos),
        //                                            new XAttribute("Vpos", textLine.Vpos));
        //                                        xAccessions.Add(xAccession);
        //                                    }
        //                                }
        //                            }
        //                            else if (textLine.HasSpecificStructuralTag("caption"))
        //                            {
        //                                XElement xSource = new XElement("Source", textLine.TextEquiv,
        //                                    new XAttribute("Page", page.PageNr),
        //                                    new XAttribute("Hpos", textLine.Hpos),
        //                                    new XAttribute("Vpos", textLine.Vpos));
        //                                xSources.Add(xSource);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    xRoot.Add(xAccessions);
        //    xRoot.Add(xSources);
        //    xAccessionsDoc.Add(xRoot);
        //    return xAccessionsDoc;
        //}

        //public async void CopyFromOtherCollection(TrCollection SourceCollection, HttpClient CurrentClient)
        //{
        //    Debug.WriteLine($"TrDocument:CopyFromOtherCollection");
        //    foreach (TrDocument SourceDocument in SourceCollection.Documents)
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
        //                foreach (TrPage Page in SourceDocument.Pages)
        //                {
        //                    Task<bool> Loaded = Page.Transcripts[0].LoadTranscript(CurrentClient);
        //                    bool OK = await Loaded;
        //                }

        //                CopyFromOtherDocument(SourceDocument);
        //            }
        //        }
        //    }
        //}

        //public void CopyFromOtherDocument(TrDocument SourceDocument)
        //{
        //    Debug.WriteLine($"TrDocument:CopyFromOtherDocument");
        //    foreach (TrPage SourcePage in SourceDocument.Pages)
        //    {
        //        string SourceFileName = SourcePage.ImageFileName;
        //        // Debug.WriteLine($"{SourceFileName}");

        //        foreach (TrPage ActualPage in Pages)
        //        {
        //            string ActualPageFileName = ActualPage.ImageFileName;
        //            // Debug.WriteLine($"{ActualPageFileName}");

        //            if (SourceFileName == ActualPageFileName)
        //            {
        //                Debug.WriteLine($"Filename match!!! (page nr. {ActualPage.PageNr}): {ActualPage.ImageFileName}");
        //                TrRegions SourceRegions = SourcePage.Transcripts[0].TextRegions;

        //                foreach (TrRegion_Text SourceRegion in SourceRegions)
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
        //    foreach (TrPage Page in Pages)
        //    {
        //        Debug.WriteLine($"Setting Column Numbers on Page nr. {Page.PageNr}");
        //        Page.SetColumnNumbers(AssumedNumberOfColumns);
        //    }
        //}

        //public void SetRowNumbers(int AssumedNumberOfRows)
        //{
        //    foreach (TrPage Page in Pages)
        //    {
        //        Debug.WriteLine($"Setting Row Numbers on Page nr. {Page.PageNr}");
        //        Page.SetRowNumbers(AssumedNumberOfRows);
        //    }
        //}
        public void CreateTopLevelRegions()
        {
            string logFileName = "Create Top Level Regions";
            TrLog log = new TrLog(logFileName, ParentCollection.Name, Title);

            string logFileMessage;
            bool oK;

            foreach (TrPage page in Pages)
            {
                oK = page.CreateTopLevelRegion();

                logFileMessage = "Page#" + page.PageNr.ToString().PadLeft(3) + "     Created? " + oK.ToString();
                log.Add(logFileMessage);
            }

            log.Save();
        }

        public void AddHorizontalRegions(int upperPercent, int lowerPercent, int upperPadding, int lowerPadding)
        {
            string logFileName = "Add Horizontal Regions";
            TrLog log = new TrLog(logFileName, ParentCollection.Name, Title);

            string logFileMessage;
            int number;

            foreach (TrPage page in Pages)
            {
                number = page.AddHorizontalRegion(upperPercent, lowerPercent, upperPadding, lowerPadding);

                logFileMessage = "Page#" + page.PageNr.ToString().PadLeft(3) + "     Added region#" + number.ToString().PadLeft(3);
                log.Add(logFileMessage);
            }

            log.Save();
        }

        public TrTextLines FindText(string searchFor)
        {
            TrTextLines tempList = new TrTextLines();
            foreach (TrPage page in Pages)
            {
                TrTranscript transcript = page.Transcripts[0];
                foreach (TrRegion textRegion in transcript.Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.Contains(searchFor))
                            {
                                tempList.Add(textLine);
                                textLine.ParentRegion = (TrTextRegion)textRegion;
                            }

                            // og så tjekker vi om den ender med mellemrum etc.
                            if (searchFor.EndsWith(" "))
                            {
                                if (textLine.TextEquiv.EndsWith(searchFor.Trim()))
                                {
                                    tempList.Add(textLine);
                                    textLine.ParentRegion = (TrTextRegion)textRegion;
                                }
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        public List<string> GetRegionalTags()
        {
            List<string> tempList = new List<string>();
            foreach (TrPage page in Pages)
            {
                TrTranscript transcript = page.Transcripts[0];
                if (transcript.HasRegionalTags)
                {
                    foreach (TrRegion textRegion in transcript.Regions)
                    {
                        if (textRegion.HasStructuralTag)
                        {
                            tempList.Add(textRegion.StructuralTagValue);
                        }
                    }
                }
            }

            List<string> tagList = tempList.Distinct().ToList();
            tagList.Sort();
            return tagList;
        }

        public int GetHighestRegionNumber()
        {
            int temp = 0;
            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    if (page.Transcripts[0].Regions.Count > temp)
                    {
                        temp = page.Transcripts[0].Regions.Count;
                    }
                }
            }

            return temp;
        }

        public int GetHighestLineNumber(int regionNumber)
        {
            int temp = 0;
            foreach (TrPage page in Pages)
            {
                if (page.ExistsRegionNumber(regionNumber))
                {
                    TrRegion textRegion = page.Transcripts[0].GetRegionByNumber(regionNumber);
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        if ((textRegion as TrTextRegion).TextLines.Count > temp)
                        {
                            temp = (textRegion as TrTextRegion).TextLines.Count;
                        }
                    }
                }
            }

            return temp;
        }

        public List<string> GetListOfPages()
        {
            List<string> temp = new List<string>();

            foreach (TrPage page in Pages)
            {
                temp.Add(page.PageNr.ToString());
            }

            return temp;
        }

        public List<string> GetListOfPossibleRegions()
        {
            List<string> temp = new List<string>();

            int i;
            int maxRegion = GetHighestRegionNumber();
            int[] regionsArray = new int[maxRegion + 1];

            foreach (TrPage page in Pages)
            {
                for (i = 1; i <= maxRegion; i++)
                {
                    if (page.ExistsRegionNumber(i))
                    {
                        regionsArray[i]++;
                    }
                }
            }

            for (i = 1; i <= maxRegion; i++)
            {
                if (regionsArray[i] == Pages.Count)
                {
                    temp.Add(i.ToString());
                }
                else
                {
                    temp.Add(i.ToString());
                }

                // temp.Add($"({i.ToString()})");
            }

            return temp;
        }

        public List<string> GetListOfPossibleLinesInRegion(int regionNumber)
        {
            List<string> temp = new List<string>();

            int i;
            int maxLine = GetHighestLineNumber(regionNumber);
            int[] linesArray = new int[maxLine + 1];

            foreach (TrPage page in Pages)
            {
                if (page.ExistsRegionNumber(regionNumber))
                {
                    TrRegion textRegion = page.Transcripts[0].GetRegionByNumber(regionNumber);
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        for (i = 1; i <= maxLine; i++)
                        {
                            if ((textRegion as TrTextRegion).ExistsLineNumber(i))
                            {
                                linesArray[i]++;
                            }
                        }
                    }
                }
            }

            for (i = 1; i <= maxLine; i++)
            {
                if (linesArray[i] == Pages.Count)
                {
                    temp.Add(i.ToString());
                }
                else
                {
                    // temp.Add($"({i.ToString()})");
                    temp.Add(i.ToString());
                }
            }

            return temp;
        }

        private bool hasRegions;

        public bool HasRegions
        {
            get
            {
                hasRegions = NrOfPagesWithRegions() > 0;
                return hasRegions;
            }
        }

        private bool hasLines;

        public bool HasLines
        {
            get
            {
                hasLines = NumberOfLines > 0;
                return hasLines;
            }
        }

        private bool hasTables;

        public bool HasTables
        {
            get
            {
                hasTables = false;
                if (Pages.Count > 0)
                {
                    foreach (TrPage tP in Pages)
                    {
                        hasTables = hasTables || tP.HasTables;
                    }
                }

                return hasTables;
            }
        }

        private bool hasFormerTables;

        public bool HasFormerTables
        {
            get
            {
                hasFormerTables = false;
                if (Pages.Count > 0)
                {
                    foreach (TrPage tP in Pages)
                    {
                        hasFormerTables = hasFormerTables || tP.HasFormerTables;
                    }
                }

                return hasFormerTables;
            }
        }

        public void MergeAllRegionsToTopLevelRegion(int startPage, int endPage)
        {
            if (Pages.Count > 0)
            {
                foreach (TrPage tP in Pages)
                {
                    if (tP.PageNr >= startPage && tP.PageNr <= endPage)
                    {
                        tP.MergeAllRegionsToTopLevelRegion();
                    }
                }
            }
        }

        public void ConvertTablesToRegions(int startPage, int endPage)
        {
            if (Pages.Count > 0)
            {
                foreach (TrPage tP in Pages)
                {
                    if (tP.PageNr >= startPage && tP.PageNr <= endPage)
                    {
                        if (tP.HasTables)
                        {
                            tP.ConvertTablesToRegions();
                            tP.DeleteEmptyRegions();
                            tP.Transcripts[0].Regions.ReNumberVertically();
                        }
                    }
                }
            }
        }

        public void CopyOldTablesToNewestTranscript()
        {
            // NB: KUN hvis der er gamle og IKKE aktuelle tabeller!
            if (Pages.Count > 0)
            {
                foreach (TrPage tP in Pages)
                {
                    if (tP.HasFormerTables && !tP.HasTables)
                    {
                        tP.CopyOldTablesToNewestTranscript();
                    }
                }
            }
        }

        public TrTextLines GetLinesWithNumber(int regionNumber, int lineNumber)
        {
            TrTextLines temp = new TrTextLines();

            foreach (TrPage page in Pages)
            {
                if (page.ExistsRegionNumber(regionNumber))
                {
                    TrRegion textRegion = page.Transcripts[0].GetRegionByNumber(regionNumber);
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            if ((textRegion as TrTextRegion).ExistsLineNumber(lineNumber))
                            {
                                TrTextLine textLine = (textRegion as TrTextRegion).GetLineByNumber(lineNumber);
                                temp.Add(textLine);
                                textLine.ParentRegion = textRegion as TrTextRegion;
                            }
                        }
                    }
                }
            }

            return temp;
        }

        //public TrTextLines GetAllLines()
        //{
        //    TrTextLines tempList = new TrTextLines();

        //    foreach (TrPage page in Pages)
        //    {
        //        if (page.HasRegions)
        //        {
        //            foreach (TrRegion textRegion in page.Transcripts[0].Regions)
        //            {
        //                if (textRegion.GetType() == typeof(TrTextRegion))
        //                {
        //                    foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
        //                    {
        //                        tempList.Add(textLine);
        //                        textLine.ParentRegion = (TrTextRegion)textRegion;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return tempList;
        //}

        public TrTextLines GetLinesWithBaseLineProblems()
        {
            TrTextLines tempList = new TrTextLines();

            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    foreach (TrRegion textRegion in page.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (!textLine.IsBaseLineDirectionOK || !textLine.IsBaseLineStraight || !textLine.IsCoordinatesPositive)
                                {
                                    tempList.Add(textLine);
                                    textLine.ParentRegion = (TrTextRegion)textRegion;
                                }
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        //public TrTextLines GetLines_BaseLineFiltered(TrBaseLineFilter currentFilter)
        //{
        //    TrTextLines tempList = new TrTextLines();

        //    foreach (TrPage page in Pages)
        //    {
        //        if (page.HasRegions)
        //        {
        //            foreach (TrRegion textRegion in page.Transcripts[0].Regions)
        //            {
        //                if (textRegion.GetType() == typeof(TrTextRegion))
        //                {
        //                    foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
        //                    {
        //                        if (textLine.IsOKwithBaseLineFilter(currentFilter))
        //                        {
        //                            tempList.Add(textLine);
        //                            textLine.ParentRegion = (TrTextRegion)textRegion;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return tempList;
        //}

        public TrTextLines GetFilteredLines(TrLineFilterSettings currentFilter)
        {
            TrTextLines tempList = new TrTextLines();

            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    foreach (TrRegion textRegion in page.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (textLine.MeetsFilterConditions(currentFilter))
                                {
                                    tempList.Add(textLine);
                                    textLine.ParentRegion = (TrTextRegion)textRegion;
                                }
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        public TrTextLines GetLines_WindowFiltered(TrLineFilterSettings settings)
        {
            TrTextLines tempList = new TrTextLines();

            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    foreach (TrRegion textRegion in page.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (settings.Inside)
                                {
                                    if (textLine.IsInWindow(settings))
                                    {
                                        tempList.Add(textLine);
                                        textLine.ParentRegion = (TrTextRegion)textRegion;
                                    }
                                }
                                else
                                {
                                    if (!textLine.IsInWindow(settings))
                                    {
                                        tempList.Add(textLine);
                                        textLine.ParentRegion = (TrTextRegion)textRegion;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        public TrTextLines GetLines_RegexFiltered(string regExPattern) // Regex MatchPattern
        {
            // Debug.Print($"TrDocument: Pattern: {MatchPattern.ToString()}");
            TrTextLines tempList = new TrTextLines();

            foreach (TrPage page in Pages)
            {
                //Debug.Print($"Page {Page.PageNr}");
                if (page.HasRegions)
                {
                    foreach (TrRegion textRegion in page.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                //Debug.Print($"Line {TL.Number}");
                                if (textLine.MatchesRegex(regExPattern))
                                {
                                    tempList.Add(textLine);
                                    textLine.ParentRegion = (TrTextRegion)textRegion;
                                }
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        public List<string> CheckElfeltRecordNumbers()
        {
            string currentRecord = string.Empty;

            char delimiter = TrLibrary.CSVDelimiter;
            List<string> tempList = new List<string>();

            foreach (TrPage p in Pages)
            {
                int lineCounter = 0;
                StringBuilder sb = new StringBuilder();

                foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            if (textLine.HasSpecificStructuralTag("RecordName"))
                            {
                                lineCounter += 1;
                                currentRecord = p.PageNr.ToString() + delimiter + lineCounter.ToString() + delimiter + textLine.ExpandedText;
                                tempList.Add(currentRecord);

                                //Debug.Print(CurrentRecord);
                            }
                        }
                    }
                }
            }

            return tempList;
        }

        public List<TrRecord> GetPseudoTableText(int minimumRecordNumber)
        {
            List<TrRecord> tempRecords = new List<TrRecord>();
            string currentDate = string.Empty;
            string currentRecordName = string.Empty;
            string metadata = string.Empty;
            char delimiter = TrLibrary.CSVDelimiter;

            foreach (TrPage p in Pages)
            {
                //Debug.Print($"Page # {PageNr} ----------------------------------------------------");
                int lineCounter = 0;

                int currentHpos = 0;
                int currentVpos = 0;
                int previousHpos = 0;
                int previousVpos = 0;

                StringBuilder sb = new StringBuilder();
                TrRecord newRecord;

                foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            // Debug.Print($"Page # {P.PageNr} - TL number {TL.Number}");
                            if (!textLine.HasSpecificStructuralTag("Year") && !textLine.HasSpecificStructuralTag("Title") && !textLine.HasSpecificStructuralTag("Exclude"))
                            {
                                previousHpos = currentHpos;
                                previousVpos = currentVpos;

                                currentHpos = textLine.Hpos;
                                currentVpos = textLine.Vpos;

                                // test for linieskift = ny record
                                // dog hvis previousHpos og previousVpos begge er 0, er det den første TL på siden - den skal springes over
                                if (previousHpos != 0 && previousVpos != 0)
                                {
                                    // Debug.Print("Not zero");
                                    // hvis vi er rykket ned - eller det er den sidste TL skal der ske noget
                                    // 2.9.2021 ændret i linien herunder: før var den
                                    // ((CurrentHpos < (PreviousHpos - 300) && CurrentVpos > PreviousVpos) )
                                    // men det gav mange fejl i 4'eren, hvor det er meningen, at man godt må hoppe LIDT baglæns...
                                    // værdien øges derfor fra 300 til 1500
                                    if (currentHpos < (previousHpos - 1500) && currentVpos > previousVpos)
                                    {
                                        // Debug.Print("current er rykket til venstre og ned");
                                        // linieskift er indtruffet!
                                        // men har vi data?
                                        lineCounter += 1;
                                        metadata = sb.ToString().Trim();
                                        sb.Clear();

                                        if (currentRecordName != string.Empty && currentRecordName != "00000" && metadata != string.Empty && metadata.Length > 5)
                                        {
                                            // Debug.Print("Vi har data");

                                            // test for om CurrentRecordName indeholder mere end eet negativnummer!
                                            Regex numbers = new Regex(@"\d+\p{L}?");
                                            string stripped = ClsLanguageLibrary.StripAll(currentRecordName);
                                            string recordName = string.Empty;
                                            MatchCollection numberMatches = numbers.Matches(stripped);
                                            if (numberMatches.Count > 0)
                                            {
                                                foreach (Match m in numberMatches)
                                                {
                                                    // vi tjekker, om tallet er stort nok til at være et negativnummer
                                                    Regex digitsOnly = new Regex(@"\d+");
                                                    Match digitsMatch = digitsOnly.Match(m.Value);
                                                    if (digitsMatch.Success)
                                                    {
                                                        // Debug.Print($"Succes! {M.Value} = {DigitsMatch.Value}");
                                                        if (Convert.ToInt32(digitsMatch.Value) > minimumRecordNumber)
                                                        {
                                                            recordName = TrLibrary.ExtractRecordName(m.Value);
                                                            Debug.Print($"Creating record: {recordName}!");
                                                            newRecord = new TrRecord(recordName, currentDate, metadata, Title, p.PageNr, lineCounter);
                                                            tempRecords.Add(newRecord);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (textLine.HasDateTag)
                                {
                                    currentDate = ClsLanguageLibrary.StripPunctuation(textLine.ExpandedText);

                                    // Debug.Print($"Date!");
                                }
                                else if (textLine.HasSpecificStructuralTag("RecordName"))
                                {
                                    currentRecordName = textLine.ExpandedText;

                                    // Debug.Print($"RecordName(s)!");
                                }
                                else
                                {
                                    sb.Append(textLine.ExpandedText);
                                    sb.Append(delimiter);

                                    // Debug.Print($"Metadata!");
                                }
                            }
                        }
                    }
                }

                // sidste linie på siden!
                // Debug.Print("Sidst linie på siden!");
                lineCounter += 1;
                metadata = sb.ToString().Trim();
                sb.Clear();
                if (currentRecordName != string.Empty && currentRecordName != "00000" && metadata != string.Empty && metadata.Length > 5)
                {
                    // Debug.Print("Vi har data");

                    // test for om CurrentRecordName indeholder mere end eet negativnummer!
                    Regex numbers = new Regex(@"\d+\p{L}?");
                    string stripped = ClsLanguageLibrary.StripAll(currentRecordName);
                    string recordName = string.Empty;
                    MatchCollection numberMatches = numbers.Matches(stripped);
                    if (numberMatches.Count > 0)
                    {
                        foreach (Match m in numberMatches)
                        {
                            // vi tjekker, om tallet er stort nok til at være et negativnummer
                            Regex digitsOnly = new Regex(@"\d+");
                            Match digitsMatch = digitsOnly.Match(m.Value);
                            if (digitsMatch.Success)
                            {
                                // Debug.Print($"Succes! {M.Value} = {DigitsMatch.Value}");
                                if (Convert.ToInt32(digitsMatch.Value) > minimumRecordNumber)
                                {
                                    recordName = TrLibrary.ExtractRecordName(m.Value);
                                    Debug.Print($"Creating record: {recordName}!");
                                    newRecord = new TrRecord(recordName, currentDate, metadata, Title, p.PageNr, lineCounter);
                                    tempRecords.Add(newRecord);
                                }
                            }
                        }
                    }
                }
            }

            return tempRecords;
        }

        public void AutoRecordTag(double percentualLeftBorder, double percentualRightBorder)
        {
            Regex numbers = new Regex(@"\d+");

            foreach (TrPage p in Pages)
            {
                if (p.HasRegions)
                {
                    foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (textLine.PercentualHpos >= percentualLeftBorder && textLine.PercentualHendPos <= percentualRightBorder)
                                {
                                    string stripped = ClsLanguageLibrary.StripAll(textLine.ExpandedText);

                                    MatchCollection numberMatches = numbers.Matches(stripped);

                                    if (numberMatches.Count == 1)
                                    {
                                        string recordNumber = numberMatches[0].Value;
                                        if (ClsLanguageLibrary.ConsecutiveDigitCount(recordNumber) >= 4)
                                        {
                                            if (Convert.ToInt32(recordNumber) > 2000)
                                            {
                                                textLine.AddStructuralTag("RecordName", true);
                                            }
                                        }
                                    }
                                    else if (numberMatches.Count > 1)
                                    {
                                        Debug.Print($"Page # {p.PageNr}, Line # {textLine.Number}: Too many matches");
                                        foreach (Match m in numberMatches)
                                        {
                                            Debug.Print($"     Record: {m.Value}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AutoDateTag(double percentualLeftBorder, double percentualRightBorder)
        {
            // sætter datotags i den kolonne, der er defineret ved de to parametre - procentuelt!
            // NB: Kører på DOCUMENT-niveau (og ikke page), fordi "CurrentYear" kan løbe hen over sideskift.
            int currentYear = 0;
            int currentMonth = 0;
            int currentDay = 0;
            int test;
            bool setDate = false;

            double leftYearBorder = 25;
            double rightYearBorder = 75;
            int minYear = 1898;
            int maxYear = 1905;

            Regex numbers = new Regex(@"\d+");

            foreach (TrPage p in Pages)
            {
                if (p.HasRegions)
                {
                    foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                // midlertidigt: vi sletter eksisterende datetags
                                if (textLine.HasDateTag)
                                {
                                    textLine.DeleteDateTags();
                                }

                                setDate = false;

                                // finder et årstal på siden
                                if (textLine.PercentualHpos >= leftYearBorder && textLine.PercentualHendPos <= rightYearBorder)
                                {
                                    string stripped = ClsLanguageLibrary.StripAll(textLine.ExpandedText);

                                    if (ClsLanguageLibrary.IsNumeric(stripped))
                                    {
                                        if (ClsLanguageLibrary.ConsecutiveDigitCount(stripped) == 4)
                                        {
                                            int strippedNumber = Convert.ToInt32(stripped);
                                            if (strippedNumber >= minYear && strippedNumber <= maxYear)
                                            {
                                                currentYear = strippedNumber;

                                                // Debug.Print($"Page no. {P.PageNr}: Current year: {CurrentYear}");
                                            }
                                        }
                                    }
                                }

                                //else
                                if (textLine.PercentualHpos >= percentualLeftBorder && textLine.PercentualHendPos <= percentualRightBorder)
                                {
                                    string stripped = ClsLanguageLibrary.StripAll(textLine.ExpandedText);

                                    if (ClsLanguageLibrary.IsPossibleDate(stripped))
                                    {
                                        // Debug.Print($"Possible date: Stripped = {Stripped}");

                                        // først tester vi, om der indgår et navn på en måned, dvs. formen d-MMM-yyyy / MMM-yyyy
                                        List<string> sourceList = stripped.Split(' ').ToList();
                                        bool testSource = false;
                                        int monthIndex = -1;

                                        if (sourceList.Count > 0)
                                        {
                                            for (int i = 0; i < sourceList.Count; i++)
                                            {
                                                if (!testSource)
                                                {
                                                    testSource = ClsLanguageLibrary.IsMonthName(sourceList[i]) || ClsLanguageLibrary.IsMonthAbbreviation(sourceList[i]);
                                                    if (testSource)
                                                    {
                                                        monthIndex = i;
                                                    }
                                                }
                                            }

                                            // hvis monthindex er 0 eller højere, er der fundet noget:
                                            if (monthIndex >= 0)
                                            {
                                                currentMonth = ClsLanguageLibrary.GetMonthNumber(sourceList[monthIndex]);

                                                // men vi må nulstille CurrentDay, for der kan være en gammel, som nu er forkert:
                                                currentDay = 0;
                                                setDate = true;

                                                // hvis måneden ikke står i den første, kan der være en dato før
                                                if (monthIndex > 0)
                                                {
                                                    if (ClsLanguageLibrary.IsNumeric(sourceList[monthIndex - 1]))
                                                    {
                                                        test = Convert.ToInt32(sourceList[monthIndex - 1]);
                                                        if (test >= 1 && test <= 31)
                                                        {
                                                            currentDay = test;
                                                        }
                                                    }
                                                }

                                                // hvis måneden ikke står i den sidste, kan der være et årstal bagefter
                                                if (monthIndex < sourceList.Count - 2)
                                                {
                                                    if (ClsLanguageLibrary.IsNumeric(sourceList[monthIndex + 1]))
                                                    {
                                                        test = Convert.ToInt32(sourceList[monthIndex + 1]);
                                                        if (test >= 1890 && test <= 1999)
                                                        {
                                                            currentYear = test;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // hvis ikke, må det være en dato med rene tal
                                        if (!setDate)
                                        {
                                            MatchCollection numberMatches = numbers.Matches(stripped);

                                            // Debug.Print($"Matches: {NumberMatches.Count}");
                                            if (numberMatches.Count == 3)
                                            {
                                                // dag, måned og år?
                                                test = Convert.ToInt16(numberMatches[0].Value); // dag?
                                                if (test >= 1 && test <= 31)
                                                {
                                                    currentDay = test;
                                                }

                                                test = Convert.ToInt16(numberMatches[1].Value); // måned?
                                                if (test >= 1 && test <= 12)
                                                {
                                                    currentMonth = test;
                                                }

                                                test = Convert.ToInt16(numberMatches[2].Value); // måned?
                                                if (test >= 1890 && test <= 1999)
                                                {
                                                    currentYear = test;
                                                }

                                                setDate = true;
                                            }
                                            else
                                            if (numberMatches.Count == 2)
                                            {
                                                // dag og måned?
                                                test = Convert.ToInt16(numberMatches[0].Value); // dag?
                                                if (test >= 1 && test <= 31)
                                                {
                                                    currentDay = test;
                                                }

                                                test = Convert.ToInt16(numberMatches[1].Value); // måned?
                                                if (test >= 1 && test <= 12)
                                                {
                                                    currentMonth = test;
                                                }

                                                setDate = true;
                                            }
                                            else
                                            if (numberMatches.Count == 1)
                                            {
                                                // kun dag?
                                                test = Convert.ToInt16(numberMatches[0].Value); // dag?
                                                if (test >= 1 && test <= 31)
                                                {
                                                    currentDay = test;
                                                }

                                                setDate = true;
                                            }
                                        }

                                        if (setDate)
                                        {
                                            // komplet dato?
                                            if (currentDay > 0 && currentMonth > 0)
                                            {
                                                int offset = textLine.TextEquiv.IndexOf(stripped);
                                                int length = stripped.Length;

                                                // af sære grunde kan man nogle gange ikke finde den - så bliver offset -1, og det går ikke.
                                                // update: det er sandsynligvis ved abbrev-dates, hvor man i sagens natur ikke KAN finde den ...
                                                if (offset < 0)
                                                {
                                                    offset = 0;
                                                    length = 1;
                                                }

                                                if (TrLibrary.IsValidDate(currentYear, currentMonth, currentDay))
                                                {
                                                    DateTime newDate = new DateTime(currentYear, currentMonth, currentDay);
                                                    textLine.AddDateTag(offset, length, newDate);

                                                    // Debug.Print($"Date found: Page no. {P.PageNr}: Date: {NewDate.ToShortDateString()}");
                                                }
                                                else
                                                {
                                                    textLine.AddDateTag(offset, length, currentDay, currentMonth, currentYear);
                                                    textLine.AddStructuralTag("InvalidDate", true);

                                                    // Debug.Print($"Invalid date found: Page no. {P.PageNr}: Date: {CurrentDay}-{CurrentMonth}-{CurrentYear}");
                                                }
                                            }
                                            else if (currentMonth > 0)
                                            {
                                                // inkomplet dato: kun måned; sætter tag på hele TL
                                                textLine.AddDateTag(0, textLine.TextEquiv.Length, currentDay, currentMonth, currentYear);
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
            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    temp++;
                }
            }

            return temp;
        }

        public string GetListOfPagesWithRegions()
        {
            string temp = string.Empty;
            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    temp = temp + page.PageNr.ToString() + ", ";
                }
            }

            if (temp.Length > 2)
            {
                return temp.Substring(0, temp.Length - 2);
            }
            else
            {
                return temp;
            }
        }

        public string GetListOfPagesWithOverlappingRegions()
        {
            string temp = string.Empty;
            bool pageOK;
            bool borderOK;

            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    pageOK = true;
                    TrTranscript transcript = page.Transcripts[0];
                    if (transcript.Regions.Count > 3) // do. - ellers bør det være 1
                    {
                        // specifikt for at se om 3-4-5 etc. i MASTER har problemer (ellers bør det være int i = 0)
                        for (int i = 2; i <= transcript.Regions.Count - 2; i++) // mindre end -2, fordi den sidste (-1) bestiles som i+1
                        {
                            if (transcript.Regions[i].BottomBorder < (transcript.Regions[i + 1].TopBorder + 5))
                            {
                                borderOK = true;
                            }
                            else
                            {
                                borderOK = false;
                            }

                            pageOK = pageOK && borderOK; // det sidste +5 er for at få falske positive, hvor en region er klippet over - så har de samme t/b-værdi
                            if (!borderOK)
                            {
                                Debug.WriteLine($"Page {page.PageNr}: Region {transcript.Regions[i].Number}, bottom: {transcript.Regions[i].BottomBorder} - Region {transcript.Regions[i + 1].Number}, top: {transcript.Regions[i + 1].TopBorder}");
                            }
                        }

                        if (!pageOK)
                        {
                            temp = temp + page.PageNr.ToString() + ", ";
                        }
                    }
                }
            }

            if (temp.Length > 2)
            {
                return temp.Substring(0, temp.Length - 2);
            }
            else
            {
                return temp;
            }
        }

        public string GetListOfPagesWithoutRegionalTags()
        {
            string temp = string.Empty;
            foreach (TrPage page in Pages)
            {
                TrTranscript transcript = page.Transcripts[0];
                if (!transcript.HasRegionalTags)
                {
                    temp = temp + page.PageNr.ToString() + ", ";
                }
            }

            if (temp.Length > 2)
            {
                return temp.Substring(0, temp.Length - 2);
            }
            else
            {
                return temp;
            }
        }

        public void DeleteLinesWithTag(string structuralTagValue)
        {
            foreach (TrPage page in Pages)
            {
                page.DeleteLinesWithTag(structuralTagValue);
            }
        }

        public void DeleteLinesOtherThan(string structuralTagValue)
        {
            foreach (TrPage page in Pages)
            {
                page.DeleteLinesOtherThan(structuralTagValue);
            }
        }

        public void DeleteRegionsWithTag(string regionalTagValue)
        {
            foreach (TrPage page in Pages)
            {
                page.DeleteRegionsWithTag(regionalTagValue);
            }
        }

        public void DeleteRegionsOtherThan(string regionalTagValue)
        {
            foreach (TrPage page in Pages)
            {
                page.DeleteRegionsOtherThan(regionalTagValue);
            }
        }

        public void DeleteEmptyRegions()
        {
            foreach (TrPage page in Pages)
            {
                page.DeleteEmptyRegions();
            }
        }

        public void SimplifyBoundingBoxes()
        {
            foreach (TrPage page in Pages)
            {
                page.SimplifyBoundingBoxes();
            }
        }

        public void SimplifyBoundingBoxes(int minimumHeight, int maximumHeight)
        {
            foreach (TrPage page in Pages)
            {
                page.SimplifyBoundingBoxes(minimumHeight, maximumHeight);
            }
        }

        public void DeleteShortBaseLines(TrDialogTransferSettings settings)
        {
            bool processThis;
            string logFileName = "Delete Short BaseLines";
            TrLog log = new TrLog(logFileName, ParentCollection.Name, Title);

            foreach (TrPage page in Pages)
            {
                if (settings.AllPages)
                {
                    processThis = true;
                }
                else if (page.PageNr >= settings.PagesFrom && page.PageNr <= settings.PagesTo)
                {
                    processThis = true;
                }
                else
                {
                    processThis = false;
                }

                if (processThis)
                {
                    log.AddLine();
                    page.DeleteShortBaseLines(settings, log);
                }
            }

            // Log.Show();
            log.Save();
        }

        public void ExtendBaseLines(TrDialogTransferSettings settings)
        {
            // Debug.WriteLine($"TrDocument : ExtendBaseLines");
            bool processThis;
            string logFileName = "Extend BaseLines";
            TrLog log = new TrLog(logFileName, ParentCollection.Name, Title);

            foreach (TrPage page in Pages)
            {
                if (settings.AllPages)
                {
                    processThis = true;
                }
                else if (page.PageNr >= settings.PagesFrom && page.PageNr <= settings.PagesTo)
                {
                    processThis = true;
                }
                else
                {
                    processThis = false;
                }

                if (processThis)
                {
                    log.AddLine();
                    page.ExtendBaseLines(settings, log);
                }
            }

            // Log.Show();
            log.Save();
        }

        //public void RepairBaseLines()
        //{
        //    string LogFileName = "Repair BaseLines";
        //    TrLog Log = new TrLog(LogFileName, ParentCollection.Name, Title);

        //    foreach (TrPage Page in Pages)
        //    {
        //        Log.AddLine();
        //        Page.RepairBaseLines(Log);
        //    }
        //    Log.Show();
        //    Log.Save();
        //}
        public List<string> GetExpandedText(bool refine, bool convertOtrema)
        {
            List<string> tempList = new List<string>();
            List<string> pageList;

            foreach (TrPage p in Pages)
            {
                pageList = p.GetExpandedText(refine, convertOtrema);
                foreach (string s in pageList)
                {
                    tempList.Add(s);
                }
            }

            return tempList;
        }

        public List<string> GetExpandedWords(bool refine, bool convertOtrema)
        {
            List<string> lineList = GetExpandedText(refine, convertOtrema);
            List<string> tempList = new List<string>();

            foreach (string l in lineList)
            {
                var wordArray = l.Split(' ').ToArray();
                int wordCount = wordArray.Length;
                for (int i = 0; i < wordCount; i++)
                {
                    tempList.Add(wordArray[i].ToString());
                }
            }

            List<string> sortedList = tempList.Distinct().ToList();
            sortedList.Sort();

            return sortedList;
        }

        private TrWords words = new TrWords();

        public TrWords Words
        {
            get
            {
                foreach (TrPage p in Pages)
                {
                    foreach (TrWord w in p.Words)
                    {
                        words.Add(w);
                    }
                }

                return words;
            }
        }

        private TrLemmas lemmas = new TrLemmas();

        public TrLemmas Lemmas
        {
            get
            {
                if (lemmas.Count == 0)
                {
                    lemmas.Clear();

                    foreach (TrWord word in Words)
                    {
                        lemmas.AddWord(word);
                    }
                }

                return lemmas;
            }
        }

        public List<string> GetTextualTags()
        {
            List<string> tempList = new List<string>();
            List<string> lineTags;

            foreach (TrPage page in Pages)
            {
                if (page.HasRegions)
                {
                    TrTranscript transcript = page.Transcripts[0];
                    foreach (TrRegion textRegion in transcript.Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (textLine.HasTags)
                                {
                                    lineTags = textLine.GetTextualTags();
                                    foreach (string tagString in lineTags)
                                    {
                                        tempList.Add(tagString);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            List<string> tagList = tempList.Distinct().ToList();
            tagList.Sort();

            foreach (string s in tagList)
            {
                Debug.WriteLine(s);
            }

            return tagList;
        }

        public List<string> GetStructuralTags()
        {
            List<string> tempList = new List<string>();

            foreach (TrPage page in Pages)
            {
                TrTranscript transcript = page.Transcripts[0];
                if (transcript.HasStructuralTags)
                {
                    foreach (TrRegion textRegion in transcript.Regions)
                    {
                        List<string> tagsInRegion = textRegion.GetStructuralTags();
                        foreach (string s in tagsInRegion)
                        {
                            tempList.Add(s);
                        }
                    }
                }
            }

            List<string> tagList = tempList.Distinct().ToList();
            tagList.Sort();
            return tagList;
        }

        public void RenameStructuralTags(string oldName, string newName)
        {
            foreach (TrPage page in Pages)
            {
                TrTranscript transcript = page.Transcripts[0];
                if (transcript.HasStructuralTags)
                {
                    foreach (TrRegion textRegion in transcript.Regions)
                    {
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                if (textLine.HasStructuralTag)
                                {
                                    if (textLine.StructuralTagValue == oldName)
                                    {
                                        textLine.RenameStructuralTag(oldName, newName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //public void OpenPages()
        //{
        //    // Debug.WriteLine($"OpenPages entred");
        //    if (!IsLoaded)
        //    {
        //        // bruges kun OFFLINE
        //        string metsFileName = Folder + "mets.xml";

        //        // Debug.WriteLine($"OpenPages: Metsfile = {MetsFileName}");
        //        XmlDocument metsDocument = new XmlDocument();
        //        metsDocument.Load(metsFileName);

        //        // Filer i dokument
        //        XmlNodeList fileNodes = metsDocument.DocumentElement.SelectNodes("//*[name()='ns3:FLocat']");

        //        // siderne behandles
        //        foreach (XmlNode xn in fileNodes)
        //        {
        //            string fn = xn.Attributes[3].Value;

        //            // Debug.WriteLine($"OpenPages: fn = {fn}");
        //            if (fn.Length > 20)
        //            {
        //                // så er vi inde på den enkelte side
        //                // ------------------------------------------------------------------------------------------------------------------
        //                string pageID = string.Empty;
        //                int pageNr = 0;
        //                string imageFileName = string.Empty;
        //                string imageURL = string.Empty;
        //                int width = 0;
        //                int height = 0;

        //                string documentFileName = Folder + xn.Attributes[3].Value.Replace("/", "\\");

        //                //Debug.WriteLine($"OpenPages: DocFileName = {DocumentFileName}");
        //                XmlDocument pageDocument = new XmlDocument();
        //                pageDocument.Load(documentFileName);

        //                XmlNamespaceManager nsmgr = new XmlNamespaceManager(pageDocument.NameTable);
        //                nsmgr.AddNamespace("tr", "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15");

        //                // XmlNodeList PageNodes = PageDocument.DocumentElement.SelectNodes("//tr:Page", nsmgr);

        //                // Debug.WriteLine($"OpenPages: Første foreach - leder efter TranskribusMetadata");
        //                XmlNodeList pageMetaData = pageDocument.DocumentElement.SelectNodes("//tr:TranskribusMetadata", nsmgr);
        //                foreach (XmlNode px in pageMetaData)
        //                {
        //                    //Debug.WriteLine($"OpenPages: pxløkke");
        //                    //Debug.WriteLine($"px.name = {px.Name}");
        //                    if (px.Name == "TranskribusMetadata")
        //                    {
        //                        //Debug.WriteLine($"OpenPages: px er fundet trmeta");
        //                        foreach (XmlAttribute pxa in px.Attributes)
        //                        {
        //                            string name = pxa.Name;
        //                            string value = pxa.Value;

        //                            switch (name)
        //                            {
        //                                case "pageId":
        //                                    pageID = value;
        //                                    break;
        //                                case "pageNr":
        //                                    pageNr = Int32.Parse(value);
        //                                    break;
        //                                case "imgUrl":
        //                                    imageURL = value;
        //                                    break;
        //                            }
        //                        }
        //                    }
        //                }

        //                //Debug.WriteLine($"OpenPages: Anden foreach - leder efter PcGts");
        //                XmlNodeList pageContent = pageDocument.DocumentElement.SelectNodes("//tr:Page", nsmgr);
        //                foreach (XmlNode px in pageContent)
        //                {
        //                    //Debug.WriteLine($"OpenPages: pxløkke2");
        //                    //Debug.WriteLine($"px.name = {px.Name}");
        //                    if (px.Name == "Page")
        //                    {
        //                        //Debug.WriteLine($"OpenPages: px = page");
        //                        foreach (XmlAttribute pxa in px.Attributes)
        //                        {
        //                            string name = pxa.Name;
        //                            string value = pxa.Value;

        //                            switch (name)
        //                            {
        //                                case "imageFilename":
        //                                    imageFileName = value;
        //                                    break;
        //                                case "imageWidth":
        //                                    width = Int32.Parse(value);
        //                                    break;
        //                                case "imageHeight":
        //                                    height = Int32.Parse(value);
        //                                    break;
        //                            }
        //                        }
        //                    }
        //                }

        //                //Debug.WriteLine($"OpenPages: burde nu være klar til at danne en ny side");
        //                //Debug.WriteLine($"PageID, PageNr, ImageFileName, Width, Height = {PageID}, {PageNr}, {ImageFileName}, {Width}, {Height}");
        //                TrPage page = new TrPage(pageID, pageNr, imageFileName, imageURL, width, height, documentFileName);
        //                Pages.Add(page);

        //                TrTranscript t = new TrTranscript(documentFileName);
        //                page.Transcripts.Add(t);

        //                t.LoadPageXML();
        //                IsLoaded = true;
        //            }
        //        }

        //        Pages.Sort();
        //    }
        //}

        public async Task<bool> CheckNewTranscripts(HttpClient currentClient)
        {

            // indledende check
            for (int i = 0; i < Pages.Count; i++)
            {
                Debug.Print($"i = {i}; pagenr = {Pages[i].PageNr}");

                for (int j = 0; j < Pages[i].Transcripts.Count; j++)
                {

                    Debug.Print($"j = {j}; transcript date = {TrLibrary.ConvertUnixTimeStamp(Pages[i].Transcripts[j].Timestamp)}");
                }

            }


            int currentPage;
            int pageIndex;
            TrTranscript latestTranscript;
            DateTime latestTranscriptDateTime;
            
            DateTime newTranscriptDateTime;

            // checker, om der er nye transcripts
            TrpPages = TrpPages.Replace("_ColID_", ParentCollection.ID);
            TrpPages = TrpPages.Replace("_DocID_", ID);
          
            // Henter de relevante pages ind i et XMLdoc
            HttpResponseMessage pagesResponseMessage = await currentClient.GetAsync(TrpPages);
            string pagesResponse = await pagesResponseMessage.Content.ReadAsStringAsync();
            PagesAndTranscriptsMetadata.LoadXml(pagesResponse);

            // Udtrækker de enkelte transcripts
            XmlNodeList transcriptNodes = PagesAndTranscriptsMetadata.DocumentElement.SelectNodes("//transcripts");
            foreach (XmlNode xnTranscript in transcriptNodes)
            {
                XmlNodeList transcriptMetaData = xnTranscript.ChildNodes;

                string transcriptID = string.Empty;
                string transcriptKey = string.Empty;
                int pageNr = 0;
                string transcriptStatus = string.Empty;
                string transcriptUser = string.Empty;
                string timestamp = string.Empty;
                int numberOfRegions = 0;
                int numberOfTranscribedRegions = 0;
                int numberOfLines = 0;
                int numberOfTranscribedLines = 0;

                // Debug.WriteLine("Henter transcripts.");
                int temp = 0;

                foreach (XmlNode xnTranscriptMetaData in transcriptMetaData)
                {
                    string name = xnTranscriptMetaData.Name;
                    string value = xnTranscriptMetaData.InnerText;
                    temp++;

                    // Debug.WriteLine($"trsc. {temp}:\t{Name}\t{Value}");
                    switch (name)
                    {
                        case "tsId":
                            transcriptID = value;
                            break;
                        case "key":
                            transcriptKey = value;
                            break;
                        case "pageNr":
                            pageNr = Int32.Parse(value);
                            break;
                        case "status":
                            transcriptStatus = value;
                            break;
                        case "userName":
                            transcriptUser = value;
                            break;
                        case "timestamp":
                            timestamp = value;
                            break;
                        case "nrOfRegions":
                            numberOfRegions = Int32.Parse(value);
                            break;
                        case "nrOfTranscribedRegions":
                            numberOfTranscribedRegions = Int32.Parse(value);
                            break;
                        case "nrOfLines":
                            numberOfLines = Int32.Parse(value);
                            break;
                        case "nrOfTranscribedLines":
                            numberOfTranscribedLines = Int32.Parse(value);
                            break;
                    }
                }

                TrTranscript newTranscript = new TrTranscript(transcriptID, transcriptKey, pageNr, transcriptStatus,
                    transcriptUser, timestamp);
                
                newTranscriptDateTime = TrLibrary.ConvertUnixTimeStamp(newTranscript.Timestamp);

                currentPage = pageNr;
                pageIndex = currentPage - 1;
                latestTranscript = Pages.GetPageFromPageNr(currentPage).Transcripts[0];
                latestTranscriptDateTime = TrLibrary.ConvertUnixTimeStamp(latestTranscript.Timestamp);



                if (newTranscriptDateTime > latestTranscriptDateTime)
                {
                    Debug.Print($"                               ");
                    Debug.Print($"Page nr. {currentPage}");
                    Debug.Print($"-------------------------------");
                    Debug.Print($"Latest: {latestTranscriptDateTime}    Newest on server: {newTranscriptDateTime}");

                    newTranscript.ParentPage = Pages.GetPageFromPageNr(currentPage);
                    newTranscript.LoadTranscript(currentClient);
                    Pages.GetPageFromPageNr(currentPage).Transcripts.Insert(newTranscript);
                    // Pages[pageIndex].Transcripts.Insert(newTranscript);
                    // Pages.GetPageFromPageNr(currentPage).HasChanged = true;
                    // Pages[pageIndex].HasChanged = true;
                    
                }

                // Transcripts.Add(transcript);
            }

            // SKIDTET VIRKER!!! Blot opdateres lstPages ikke med antal Transcripts - men den tvinger ikke upload igennem, så den VED, at den nyeste er inde.

            // Debug.WriteLine($"antal transcripts = {Transcripts.Count}");

            //// så nu fordeler vi dem på siderne
            //foreach (TrTranscript tra in Transcripts)
            //{
            //    Pages.GetPageFromPageNr(tra.PageNr).Transcripts.Add(tra);
            //}

            //// og sorterer dem - og vender dem rundt, så nyeste er først
            //foreach (TrPage page in Pages)
            //{
            //    page.Transcripts.Sort();
            //    page.Transcripts.Reverse();

            //    // Page.IsLoaded = true;
            //    // Debug.WriteLine($"Page # {Page.PageNr}: {Page.TranscriptCount} transcripts.");
            //}


            return true;
        }

        public async Task<bool> LoadPages(HttpClient currentClient)
        {
            // bruges kun ONLINE
            if (!IsLoaded)
            {
                TrpPages = TrpPages.Replace("_ColID_", ParentCollection.ID);
                TrpPages = TrpPages.Replace("_DocID_", ID);

                // Debug.WriteLine(TrpPages);

                // Henter de relevante pages ind i et XMLdoc
                HttpResponseMessage pagesResponseMessage = await currentClient.GetAsync(TrpPages);
                string pagesResponse = await pagesResponseMessage.Content.ReadAsStringAsync();
                PagesAndTranscriptsMetadata.LoadXml(pagesResponse);

                // Debug.WriteLine($"henter sider i dokumentet {Title}, {ID}");
                // Debug.WriteLine($"antal sider 1 = {Pages.Count}");

                // Og gemmer - i udviklingsfasen - xml-filen.
                // Det har samtidig den fordel, at det forsinker lidt....
                //string XMLFileName = TrLibrary.ExportFolder + ParentCollection.Name + "_" + Title + ".xml";
                //PagesAndTranscriptsMetadata.Save(XMLFileName);

                // Udtrækker de enkelte pages
                XmlNodeList pageNodes = PagesAndTranscriptsMetadata.DocumentElement.SelectNodes("//pages");
                foreach (XmlNode xnPage in pageNodes)
                {
                    XmlNodeList pageMetaData = xnPage.ChildNodes;
                    string pageID = string.Empty;
                    int pageNr = 0;
                    string imageFileName = string.Empty;
                    string imageURL = string.Empty;
                    int width = 0;
                    int height = 0;

                    foreach (XmlNode xnPageMetaData in pageMetaData)
                    {
                        string name = xnPageMetaData.Name;
                        string value = xnPageMetaData.InnerText;

                        switch (name)
                        {
                            case "pageId":
                                pageID = value;
                                break;
                            case "pageNr":
                                pageNr = Int32.Parse(value);
                                break;
                            case "imgFileName":
                                imageFileName = value;
                                break;
                            case "url":
                                imageURL = value;
                                break;
                            case "width":
                                width = Int32.Parse(value);
                                break;
                            case "height":
                                height = Int32.Parse(value);
                                break;
                        }
                    }

                    TrPage page = new TrPage(pageID, pageNr, imageFileName, imageURL, width, height);
                    Pages.Add(page);
                }

                // Debug.WriteLine($"antal sider 2 = {Pages.Count}");
                Pages.Sort();

                // Udtrækker de enkelte transcripts
                XmlNodeList transcriptNodes = PagesAndTranscriptsMetadata.DocumentElement.SelectNodes("//transcripts");
                foreach (XmlNode xnTranscript in transcriptNodes)
                {
                    XmlNodeList transcriptMetaData = xnTranscript.ChildNodes;

                    string transcriptID = string.Empty;
                    string transcriptKey = string.Empty;
                    int pageNr = 0;
                    string transcriptStatus = string.Empty;
                    string transcriptUser = string.Empty;
                    string timestamp = string.Empty;
                    int numberOfRegions = 0;
                    int numberOfTranscribedRegions = 0;
                    int numberOfLines = 0;
                    int numberOfTranscribedLines = 0;

                    // Debug.WriteLine("Henter transcripts.");
                    int temp = 0;

                    foreach (XmlNode xnTranscriptMetaData in transcriptMetaData)
                    {
                        string name = xnTranscriptMetaData.Name;
                        string value = xnTranscriptMetaData.InnerText;
                        temp++;

                        // Debug.WriteLine($"trsc. {temp}:\t{Name}\t{Value}");
                        switch (name)
                        {
                            case "tsId":
                                transcriptID = value;
                                break;
                            case "key":
                                transcriptKey = value;
                                break;
                            case "pageNr":
                                pageNr = Int32.Parse(value);
                                break;
                            case "status":
                                transcriptStatus = value;
                                break;
                            case "userName":
                                transcriptUser = value;
                                break;
                            case "timestamp":
                                timestamp = value;
                                break;
                            case "nrOfRegions":
                                numberOfRegions = Int32.Parse(value);
                                break;
                            case "nrOfTranscribedRegions":
                                numberOfTranscribedRegions = Int32.Parse(value);
                                break;
                            case "nrOfLines":
                                numberOfLines = Int32.Parse(value);
                                break;
                            case "nrOfTranscribedLines":
                                numberOfTranscribedLines = Int32.Parse(value);
                                break;
                        }
                    }

                    TrTranscript transcript = new TrTranscript(transcriptID, transcriptKey, pageNr, transcriptStatus,
                        transcriptUser, timestamp);

                    Transcripts.Add(transcript);
                }

                // Debug.WriteLine($"antal transcripts = {Transcripts.Count}");

                // så nu fordeler vi dem på siderne
                foreach (TrTranscript tra in Transcripts)
                {
                    Pages.GetPageFromPageNr(tra.PageNr).Transcripts.Add(tra);
                }

                // og sorterer dem - og vender dem rundt, så nyeste er først
                foreach (TrPage page in Pages)
                {
                    page.Transcripts.Sort();
                    page.Transcripts.Reverse();

                    // Page.IsLoaded = true;
                    // Debug.WriteLine($"Page # {Page.PageNr}: {Page.TranscriptCount} transcripts.");
                }

                IsLoaded = true;
                ParentCollection.NrOfDocsLoaded++;
                Debug.WriteLine($"Document {ParentCollection.Name} / {Title} loaded!");
            }

            return true;
        }

        public void Upload(HttpClient currentClient)
        {
            foreach (TrPage page in Pages)
            {
                if (page.HasChanged)
                {
                    page.Transcripts[0].Upload(currentClient);
                }
            }

            Debug.Print($"***** RESULT: Transcripts changed: {NrOfTranscriptsChanged} - Transcripts uploaded: {NrOfTranscriptsUploaded}");
        }

        public void Save()
        {
            foreach (TrPage page in Pages)
            {
                if (page.HasChanged)
                {
                    page.Transcripts[0].Save();
                }
            }
        }

        public bool PostLoadTestOK()
        {
            // udfører standard test efter load af doc.
            // det undersøges:
            // 1. om regioner, liner og baselines har ulovlige koordinater (mindre end 0, større end siden)
            // 2. om nogle textlines skal trimmes for leading og trailing spaces
            bool regionsOK = true;
            bool linesOK = true;
            bool baseLinesOK = true;
            bool spacesOK = true;

            bool regionOK;
            bool lineOK;
            bool baseLineOK;
            bool spaceOK;

            bool documentOK;

            foreach (TrPage p in Pages)
            {
                //Debug.Print($"Page# {P.PageNr} ----------------------------------------------------");
                if (p.HasRegions)
                {
                    foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                    {
                        regionOK = TrLibrary.CheckCoordinates(textRegion.CoordsString, p.Width, p.Height);
                        regionsOK = regionsOK && regionOK;

                        //Debug.Print($"Reg.# {TR.Number.ToString("000")}: Area {RegionOK}");
                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                lineOK = TrLibrary.CheckCoordinates(textLine.CoordsString, p.Width, p.Height);
                                linesOK = linesOK && lineOK;

                                baseLineOK = TrLibrary.CheckCoordinates(textLine.BaseLineCoordsString, p.Width, p.Height);
                                baseLinesOK = baseLinesOK && baseLineOK;

                                spaceOK = textLine.TextEquiv == textLine.TextEquiv.Trim();
                                spacesOK = spacesOK && spaceOK;

                                //Debug.Print($"Line# {TL.Number.ToString("000")}: Line {LineOK} - BaseLine {BaseLineOK} - Spaces {SpaceOK}");
                            }
                        }
                    }
                }
            }

            documentOK = regionsOK && linesOK && baseLinesOK && spacesOK;
            return documentOK;
        }

        public void PostLoadFix()
        {
            // fixer de ting, som PostLoadTest finder:
            // 1. regioner, liner og baselines: ulovlige koordinater (mindre end 0, større end siden) fixes
            // 2. textlines trimmes for leading og trailing spaces
            bool regionOK;
            bool lineOK;
            bool baseLineOK;
            bool spaceOK;

            foreach (TrPage p in Pages)
            {
                Debug.Print($"Page# {p.PageNr} ----------------------------------------------------");

                if (p.HasRegions)
                {
                    foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                    {
                        regionOK = TrLibrary.CheckCoordinates(textRegion.CoordsString, p.Width, p.Height);

                        if (!regionOK)
                        {
                            Debug.Print($"Region# {textRegion.Number.ToString("000")}: Fixing coordinates");
                            textRegion.FixCoordinates();
                        }

                        if (textRegion.GetType() == typeof(TrTextRegion))
                        {
                            foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                            {
                                lineOK = TrLibrary.CheckCoordinates(textLine.CoordsString, p.Width, p.Height);
                                if (!lineOK)
                                {
                                    Debug.Print($"Line# {textLine.Number.ToString("000")}: Fixing line area coordinates");
                                    textLine.FixLineCoordinates();
                                }

                                baseLineOK = TrLibrary.CheckCoordinates(textLine.BaseLineCoordsString, p.Width, p.Height);
                                if (!baseLineOK)
                                {
                                    Debug.Print($"Line# {textLine.Number.ToString("000")}: Fixing baseline coordinates");
                                    textLine.FixBaseLineCoordinates();
                                }

                                spaceOK = textLine.TextEquiv == textLine.TextEquiv.Trim();
                                if (!spaceOK)
                                {
                                    Debug.Print($"Line# {textLine.Number.ToString("000")}: Trimming line");
                                    textLine.Trim();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
