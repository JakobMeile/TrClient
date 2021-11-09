// <copyright file="TrTranscript.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Xml.Linq;
    using TranskribusClient.Core.Tags;
    using TranskribusClient.Libraries;
    using System.Resources;

    public class TrTranscript : IComparable, INotifyPropertyChanged
    {

        //public string BaseURL = "https://dbis-thure.uibk.ac.at/f/Get?id=";
        public string BaseURL = Properties.Resources.TrpServerTranscriptURL; // "https://files.transkribus.eu/Get?id=";
        
        //public string UploadURL = "https://transkribus.eu/TrpServer/rest/collections/_ColID_/_DocID_/_PageNr_/text?overwrite=true";
        public string UploadURL = Properties.Resources.TrpServerBaseAddress + Properties.Resources.TrpServerPathUpload;

        public string PageFileName { get; set; }            // bruges offline

        public string ID { get; set; }                      // tsId

        public string Key { get; set; }                     // key - sættes efter BaseURL for at få filen

        public int PageNr { get; set; }                     // pageNr

        public string Status { get; set; }                  // status - fx. NEW eller GT

        public string User { get; set; }                    // userName - den ansvarlige user for netop dette transkript

        public string Timestamp { get; set; }               // timestamp - noget UTC-std. - må kunne brugs til sortering

        public string Creator { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastChangeDate { get; set; }

        public string Type { get; set; }

        public string TempCreationDate { get; set; }

        public string TempLastChangeDate { get; set; }

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

        //public event EventHandler TestEventHandler;

        //static void Transcripts_TestEventHandler(object sender, EventArgs e)
        //{
        //    Debug.Print($"TestEventHandler: {e.ToString()}");
        //}

        public XDocument TranscriptionDocument;

        public TrRegions Regions = new TrRegions();

        public TrTranscripts ParentContainer;
        public TrPage ParentPage;

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

                //TestEventHandler?.Invoke(this, EventArgs.Empty);

                NotifyPropertyChanged("HasChanged");
                ParentPage.HasChanged = value;
                if (hasChanged)
                {
                    ParentPage.ParentDocument.NrOfTranscriptsChanged++;
                }
                else
                {
                    ParentPage.ParentDocument.NrOfTranscriptsChanged--;
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
                ParentPage.ChangesUploaded = value;
                Regions.ChangesUploaded = value;
                if (changesUploaded)
                {
                    ParentPage.ParentDocument.NrOfTranscriptsUploaded++;
                }
                else
                {
                    ParentPage.ParentDocument.NrOfTranscriptsUploaded--;
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

        private bool hasRegionalTags = false;

        public bool HasRegionalTags
        {
            get
            {
                foreach (TrRegion textRegion in Regions)
                {
                    hasRegionalTags = hasRegionalTags || textRegion.HasStructuralTag;
                }

                return hasRegionalTags;
            }
        }

        private bool hasStructuralTags = false;

        public bool HasStructuralTags
        {
            get
            {
                foreach (TrRegion textRegion in Regions)
                {
                    if (textRegion.GetType() == typeof(TrTextRegion))
                    {
                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                        {
                            hasStructuralTags = hasStructuralTags || textLine.HasStructuralTag;
                        }
                    }
                    else if (textRegion.GetType() == typeof(TrTableRegion))
                    {
                        //foreach (TrTextLine TL in (TR as TrRegion_Text).TextLines)
                        //{
                        //    _hasStructuralTags = _hasStructuralTags || TL.HasStructuralTag;
                        //}
                    }
                }

                return hasStructuralTags;
            }
        }

        private bool hasRegions;

        public bool HasRegions
        {
            get
            {
                hasRegions = Regions.Count > 0;
                return hasRegions;
            }
        }

        private bool hasTables;

        public bool HasTables
        {
            get
            {
                hasTables = false;
                if (HasRegions)
                {
                    foreach (TrRegion region in Regions)
                    {
                        hasTables = hasTables || (region.GetType() == typeof(TrTableRegion));
                    }
                }

                return hasTables;
            }
        }

        public void ConvertTablesToRegions()
        {
            if (HasTables)
            {
                Debug.WriteLine($"Transcript {ID}: Entering convert tables");
                TrRegions newRegions = new TrRegions();

                foreach (TrRegion textRegion in Regions)
                {
                    if (textRegion.GetType() == typeof(TrTableRegion))
                    {
                        Debug.WriteLine($"Table found!");
                        TrTextRegion newTextRegion = new TrTextRegion(textRegion.ReadingOrder, textRegion.Orientation, textRegion.CoordsString, Regions);
                        newRegions.Add(newTextRegion);

                        foreach (TrCell cell in (textRegion as TrTableRegion).Cells)
                        {
                            foreach (TrTextLine textLine in cell.TextLines)
                            {
                                newTextRegion.TextLines.Add(textLine);
                            }
                        }

                        textRegion.MarkToDeletion = true;
                    }
                }

                // så kopierer vi de nye regioner ind i de gamle
                foreach (TrRegion newRegion in newRegions)
                {
                    Regions.Add(newRegion);
                }

                // og så sletter vi de gamle tabelregioner
                for (int i = Regions.Count - 1; i >= 0; i--)
                {
                    if (Regions[i].MarkToDeletion)
                    {
                        Debug.WriteLine($"Deleting region nr. {i}");
                        Regions.RemoveAt(i);
                    }
                }
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
        public TrTranscript(string tID, string tKey, int iPageNr, string tStatus, string tUser, string tTimestamp)
        {
            TranscriptionDocument = new XDocument();

            ID = tID;
            Key = tKey;
            PageNr = iPageNr;
            Status = tStatus;
            User = tUser;
            Timestamp = tTimestamp;

            Regions.ParentTranscript = this;
            IsLoaded = false;
        }

        //// constructor OFFLINE
        //public TrTranscript(string pageFile)
        //{
        //    // bruges kun OFFLINE
        //    PageFileName = pageFile;
        //    Regions.ParentTranscript = this;
        //    IsLoaded = false;
        //}

        //public void LoadPageXML()
        //{
        //    // BRUGES KUN OFFLINE
        //    if (!IsLoaded)
        //    {
        //        Regions.ParentTranscript = this;

        //        // IsLoaded = false;
        //        // Debug.WriteLine("Transcript starting to be created!");

        //        // Henter transcript ind i et XMLdoc
        //        try
        //        {
        //            //HttpResponseMessage TranscriptResponseMessage = await CurrentClient.GetAsync(URL);
        //            //string TranscriptResponse = await TranscriptResponseMessage.Content.ReadAsStringAsync();

        //            // Debug.WriteLine($"Loading ...: {PageFileName}");
        //            TranscriptionDocument = XDocument.Load(PageFileName);

        //            XNamespace tr = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15";

        //            foreach (var metadata in TranscriptionDocument.Descendants(tr + "Metadata"))
        //            {
        //                string xCreator = metadata.Element(tr + "Creator") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "Creator").Value;
        //                string xCreated = metadata.Element(tr + "Created") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "Created").Value;
        //                string xLastChange = metadata.Element(tr + "LastChange") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "LastChange").Value;

        //                Creator = xCreator;
        //                TempCreationDate = xCreated;
        //                TempLastChangeDate = xLastChange;

        //                // Debug.WriteLine($"Creator = {Creator}, TempCreationDate = {TempCreationDate}, TempLastChangeDate = {TempLastChangeDate}");
        //                ID = metadata.Element(tr + "TranskribusMetadata") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "TranskribusMetadata").Attribute("pageId").Value;

        //                string xPageNr = metadata.Element(tr + "TranskribusMetadata") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "TranskribusMetadata").Attribute("pageNr").Value;

        //                PageNr = int.Parse(xPageNr);

        //                Timestamp = metadata.Element(tr + "TranskribusMetadata") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "TranskribusMetadata").Attribute("tsid").Value;

        //                Status = metadata.Element(tr + "TranskribusMetadata") == null
        //                        ? String.Empty
        //                        : metadata.Element(tr + "TranskribusMetadata").Attribute("status").Value;

        //                // Debug.WriteLine($"ID = {ID}, PageNr = {PageNr}, Timestamp = {Timestamp}, Status = {Status}");
        //            }

        //            foreach (var page in TranscriptionDocument.Descendants(tr + "Page"))
        //            {
        //                Type = page.Attribute("type") == null ? String.Empty : page.Attribute("type").Value;
        //            }

        //            foreach (var group in TranscriptionDocument.Descendants(tr + "OrderedGroup"))
        //            {
        //                string xRO = group.Element(tr + "OrderedGroup") == null ? "ro_" + TrLibrary.GetNewTimeStamp() : group.Attribute("id").Value;

        //                // Debug.WriteLine($"Order ID {xRO}");
        //                Regions.OrderedGroupID = xRO;
        //            }

        //            foreach (var region in TranscriptionDocument.Descendants(tr + "TextRegion"))
        //            {
        //                string regionType = region.Attribute("type") == null ? String.Empty : region.Attribute("type").Value;
        //                string regionID = region.Attribute("id") == null ? String.Empty : region.Attribute("id").Value;
        //                string regionTagString = region.Attribute("custom") == null ? String.Empty : region.Attribute("custom").Value;
        //                float orientation = region.Attribute("orientation") == null ? 0 : float.Parse(region.Attribute("orientation").Value);
        //                string regionCoords = region.Element(tr + "Coords") == null ? String.Empty : region.Element(tr + "Coords").Attribute("points").Value;

        //                TrTextRegion newTextRegion = new TrTextRegion(regionType, regionID, regionTagString, orientation, regionCoords);
        //                Regions.Add(newTextRegion);

        //                foreach (var line in region.Descendants(tr + "TextLine"))
        //                {
        //                    string lineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
        //                    string lineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
        //                    string lineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
        //                    string baseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
        //                    string textEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

        //                    // Debug.WriteLine($"{LineID}, {LineTagString}, {LineCoords}, {BaseLineCoords}, {TextEquiv}");
        //                    TrTextLine newTextLine = new TrTextLine(lineID, lineTagString, lineCoords, baseLineCoords, textEquiv);
        //                    newTextRegion.TextLines.Add(newTextLine);
        //                }
        //            }

        //            foreach (var table in TranscriptionDocument.Descendants(tr + "TableRegion"))
        //            {
        //                string regionType = table.Attribute("type") == null ? String.Empty : table.Attribute("type").Value;
        //                string regionID = table.Attribute("id") == null ? String.Empty : table.Attribute("id").Value;
        //                string regionTagString = table.Attribute("custom") == null ? String.Empty : table.Attribute("custom").Value;
        //                float orientation = table.Attribute("orientation") == null ? 0 : float.Parse(table.Attribute("orientation").Value);
        //                string regionCoords = table.Element(tr + "Coords") == null ? String.Empty : table.Element(tr + "Coords").Attribute("points").Value;

        //                TrTableRegion newTable = new TrTableRegion(regionType, regionID, regionTagString, orientation, regionCoords);
        //                Regions.Add(newTable);

        //                // Debug.WriteLine($"New table at page # {PageNr} !!!");
        //                foreach (var cell in table.Descendants(tr + "TableCell"))
        //                {
        //                    string cellID = cell.Attribute("id") == null ? String.Empty : cell.Attribute("id").Value;
        //                    string cellRow = cell.Attribute("row") == null ? String.Empty : cell.Attribute("row").Value;
        //                    string cellCol = cell.Attribute("col") == null ? String.Empty : cell.Attribute("col").Value;
        //                    string cellCoords = cell.Element(tr + "Coords") == null ? String.Empty : cell.Element(tr + "Coords").Attribute("points").Value;
        //                    string cellCornerPoints = cell.Element(tr + "CornerPts") == null ? String.Empty : cell.Element(tr + "CornerPts").Value;

        //                    TrCell newTableCell = new TrCell(cellID, cellRow, cellCol, cellCoords, cellCornerPoints);
        //                    newTable.AddCell(newTableCell);

        //                    Debug.WriteLine($"New cell in table!");

        //                    foreach (var line in cell.Descendants(tr + "TextLine"))
        //                    {
        //                        string lineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
        //                        string lineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
        //                        string lineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
        //                        string baseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
        //                        string textEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

        //                        TrTextLine newTextLine = new TrTextLine(lineID, lineTagString, lineCoords, baseLineCoords, textEquiv);
        //                        newTableCell.TextLines.Add(newTextLine);

        //                        Debug.WriteLine($"New textline in cell!");
        //                    }
        //                }
        //            }

        //            IsLoaded = true;
        //            ParentPage.IsLoaded = true;
        //            ParentPage.ParentDocument.NrOfTranscriptsLoaded++;

        //            // Debug.WriteLine($"Transcript loaded: {ParentPage.ParentDocument.ParentCollection.Name} / {ParentPage.ParentDocument.Title} / {ParentPage.PageNr}");
        //        }
        //        catch (TaskCanceledException e)
        //        {
        //            // MessageBox.Show("Exception occured!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //            Debug.WriteLine($"Task canceled! Exception message when loading page nr {PageNr.ToString()}: {PageFileName}: {e.Message}");
        //        }
        //        catch (OperationCanceledException e)
        //        {
        //            Debug.WriteLine($"Operation canceled! Exception message when loading page nr {PageNr.ToString()}: {PageFileName}: {e.Message}");
        //        }

        //        //catch (Exception e)
        //        //{
        //        //    Debug.WriteLine($"General error! Exception message when loading page nr {PageNr.ToString()}: {PageFileName}: {e.Message}");
        //        //}
        //    }
        //}

        private DateTime dateTimeOfTranscript;

        public DateTime DateTimeOfTranscript
        {
            get
            {
                dateTimeOfTranscript = TrLibrary.ConvertUnixTimeStamp(Timestamp);
                return dateTimeOfTranscript;
            }
        }

        private string url;

        private string URL
        {
            get
            {
                url = BaseURL + Key;
                return url;
            }
        }

        public int CompareTo(object obj)
        {
            var transcript = obj as TrTranscript;
            return Timestamp.CompareTo(transcript.Timestamp);
        }

        public void SetRegionalTagsOnNonTaggedRegions(string tagValue)
        {
            foreach (TrRegion textRegion in Regions)
            {
                if (!textRegion.HasStructuralTag)
                {
                    TrTagStructural structTag = new TrTagStructural(tagValue);
                    textRegion.StructuralTag = structTag;
                    textRegion.Tags.Add(structTag);
                    textRegion.HasChanged = true;
                }
            }
        }

        public TrTags GetRegionalTags()
        {
            TrTags tempTags = new TrTags();

            foreach (TrRegion textRegion in Regions)
            {
                if (textRegion.HasStructuralTag)
                {
                    tempTags.Add(textRegion.StructuralTag);
                }
            }

            return tempTags;
        }

        public TrTags GetStructuralTags()
        {
            TrTags structuralTags = new TrTags();

            foreach (TrRegion textRegion in Regions)
            {
                if (textRegion.GetType() == typeof(TrTextRegion))
                {
                    foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                    {
                        foreach (TrTag tag in textLine.Tags)
                        {
                            if (tag.GetType() == typeof(TrTagStructural))
                            {
                                structuralTags.Add(tag);
                                tag.ParentLine = textLine;
                            }
                        }
                    }
                }
            }

            return structuralTags;
        }

        public bool AddStructuralTag(int region, int line, string tagName, bool overWrite)
        {
            bool tagAdded = false;

            TrRegion textRegion = GetRegionByNumber(region);
            if (textRegion != null)
            {
                if (textRegion.GetType() == typeof(TrTextRegion))
                {
                    TrTextLine textLine = (textRegion as TrTextRegion).GetLineByNumber(line);
                    if (textLine != null)
                    {
                        if (!textLine.HasStructuralTag)
                        {
                            textLine.AddStructuralTag(tagName, overWrite);
                            Debug.WriteLine($"Page# {ParentPage.PageNr}: Tag {tagName} tilføjet linie {region}-{line}!");
                            tagAdded = true;
                        }
                        else
                        {
                            Debug.WriteLine($"Page# {ParentPage.PageNr}: Linie {region}-{line} har allerede et structural tag!");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Page# {ParentPage.PageNr}: Linie {region}-{line} findes ikke!");
                    }
                }
            }
            else
            {
                Debug.WriteLine($"Page# {ParentPage.PageNr}: Region {region} findes ikke!");
            }

            return tagAdded;
        }

        public void DeleteStructuralTags(int region, int line)
        {
            TrRegion textRegion = GetRegionByNumber(region);
            if (textRegion != null)
            {
                if (textRegion.GetType() == typeof(TrTextRegion))
                {
                    TrTextLine textLine = (textRegion as TrTextRegion).GetLineByNumber(line);
                    if (textLine != null)
                    {
                        if (!textLine.HasStructuralTag)
                        {
                            textLine.DeleteStructuralTag();
                        }
                    }
                }
            }
        }

        public TrRegion GetRegionByNumber(int regionNumber)
        {
            if (Regions != null)
            {
                if (regionNumber >= 1 && regionNumber <= Regions.Count)
                {
                    return Regions[regionNumber - 1];
                }
                else
                {
                    Debug.WriteLine($"GetRegionByNumber: Region nr. {regionNumber} eksisterer ikke!");
                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"GetRegionByNumber: Dette transcript indeholder ikke regions!");
                return null;
            }
        }

        public async Task<bool> LoadTranscript(HttpClient currentClient)
        {
            // bruges kun ONLINE
            if (!IsLoaded)
            {
                // Henter transcript ind i et XMLdoc
                try
                {
                    HttpResponseMessage transcriptResponseMessage = await currentClient.GetAsync(URL);
                    string transcriptResponse = await transcriptResponseMessage.Content.ReadAsStringAsync();
                    TranscriptionDocument = XDocument.Parse(transcriptResponse);

                    // Og gemmer - i udviklingsfasen - xml-filen.
                    //string XMLFileName = TrLibrary.ExportFolder + ParentPage.ParentDocument.Title + "_" + ParentPage.PageNr.ToString("0000") + ".xml";
                    //TranscriptionDocument.Save(XMLFileName);
                    XNamespace tr = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15";

                    foreach (var metadata in TranscriptionDocument.Descendants(tr + "Metadata"))
                    {
                        string xCreator = metadata.Element(tr + "Creator") == null ? String.Empty : metadata.Element(tr + "Creator").Value;
                        string xCreated = metadata.Element(tr + "Created") == null ? String.Empty : metadata.Element(tr + "Created").Value;
                        string xLastChange = metadata.Element(tr + "LastChange") == null ? String.Empty : metadata.Element(tr + "LastChange").Value;

                        Creator = xCreator;
                        TempCreationDate = xCreated;
                        TempLastChangeDate = xLastChange;
                    }

                    foreach (var page in TranscriptionDocument.Descendants(tr + "Page"))
                    {
                        Type = page.Attribute("type") == null ? String.Empty : page.Attribute("type").Value;
                    }

                    foreach (var group in TranscriptionDocument.Descendants(tr + "OrderedGroup"))
                    {
                        string xRO = group.Element(tr + "OrderedGroup") == null ? "ro_" + TrLibrary.GetNewTimeStamp() : group.Attribute("id").Value;

                        // Debug.WriteLine($"Order ID {xRO}");
                        Regions.OrderedGroupID = xRO;
                    }

                    foreach (var region in TranscriptionDocument.Descendants(tr + "TextRegion"))
                    {
                        string regionType = region.Attribute("type") == null ? String.Empty : region.Attribute("type").Value;
                        string regionID = region.Attribute("id") == null ? String.Empty : region.Attribute("id").Value;
                        string regionTagString = region.Attribute("custom") == null ? String.Empty : region.Attribute("custom").Value;
                        float orientation = region.Attribute("orientation") == null ? 0 : float.Parse(region.Attribute("orientation").Value);
                        string regionCoords = region.Element(tr + "Coords") == null ? String.Empty : region.Element(tr + "Coords").Attribute("points").Value;

                        TrTextRegion newTextRegion = new TrTextRegion(regionType, regionID, regionTagString, orientation, regionCoords, Regions);
                        Regions.Add(newTextRegion);

                        foreach (var line in region.Descendants(tr + "TextLine"))
                        {
                            string lineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
                            string lineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
                            string lineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
                            string baseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
                            string textEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

                            //string TextEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : Regex.Unescape(line.Element(tr + "TextEquiv").Value);
                            // REGEX.UNESCAPE forsøg 14.11.2020
                            TrTextLine newTextLine = new TrTextLine(lineID, lineTagString, lineCoords, baseLineCoords, textEquiv, newTextRegion.TextLines);
                            newTextRegion.TextLines.Add(newTextLine);
                        }
                    }

                    foreach (var table in TranscriptionDocument.Descendants(tr + "TableRegion"))
                    {
                        string regionType = table.Attribute("type") == null ? String.Empty : table.Attribute("type").Value;
                        string regionID = table.Attribute("id") == null ? String.Empty : table.Attribute("id").Value;
                        string regionTagString = table.Attribute("custom") == null ? String.Empty : table.Attribute("custom").Value;
                        float orientation = table.Attribute("orientation") == null ? 0 : float.Parse(table.Attribute("orientation").Value);
                        string regionCoords = table.Element(tr + "Coords") == null ? String.Empty : table.Element(tr + "Coords").Attribute("points").Value;

                        TrTableRegion newTable = new TrTableRegion(regionType, regionID, regionTagString, orientation, regionCoords, Regions);
                        Regions.Add(newTable);

                        //Debug.WriteLine($"New table at page # {PageNr} !!!");
                        foreach (var cell in table.Descendants(tr + "TableCell"))
                        {
                            string cellID = cell.Attribute("id") == null ? String.Empty : cell.Attribute("id").Value;
                            string cellRow = cell.Attribute("row") == null ? String.Empty : cell.Attribute("row").Value;
                            string cellCol = cell.Attribute("col") == null ? String.Empty : cell.Attribute("col").Value;
                            string cellCoords = cell.Element(tr + "Coords") == null ? String.Empty : cell.Element(tr + "Coords").Attribute("points").Value;
                            string cellCornerPoints = cell.Element(tr + "CornerPts") == null ? String.Empty : cell.Element(tr + "CornerPts").Value;

                            TrCell newTableCell = new TrCell(cellID, cellRow, cellCol, cellCoords, cellCornerPoints);
                            newTable.AddCell(newTableCell);

                            //Debug.WriteLine($"New cell in table!");
                            foreach (var line in cell.Descendants(tr + "TextLine"))
                            {
                                string lineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
                                string lineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
                                string lineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
                                string baseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
                                string textEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

                                TrTextLine newTextLine = new TrTextLine(lineID, lineTagString, lineCoords, baseLineCoords, textEquiv, newTableCell.TextLines);
                                newTableCell.TextLines.Add(newTextLine);

                                //Debug.WriteLine($"New textline in cell!");
                            }
                        }
                    }

                    IsLoaded = true;
                    ParentPage.IsLoaded = true;
                    ParentPage.ParentDocument.NrOfTranscriptsLoaded += 1;
                    ParentPage.TranscriptCount += 1;

                    // Debug.WriteLine($"Transcript loaded: {ParentPage.ParentDocument.ParentCollection.Name} / {ParentPage.ParentDocument.Title} / {ParentPage.PageNr}");
                }
                catch (TaskCanceledException e)
                {
                    // MessageBox.Show("Exception occured!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Debug.WriteLine($"Task canceled! Exception message when loading page nr {PageNr.ToString()}: {e.Message}");
                }
                catch (OperationCanceledException e)
                {
                    Debug.WriteLine($"Operation canceled! Exception message when loading page nr {PageNr.ToString()}: {e.Message}");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"General error! Exception message when loading page nr {PageNr.ToString()}: {e.Message}");
                }
            }

            return true;
        }

        public void Save()
        {
            Debug.WriteLine($"TrTranscript: Saving {PageFileName}");

            XDocument xDoc = ToXML();
            xDoc.Save(PageFileName);
            HasChanged = false;
            ChangesUploaded = true;
        }

        public XDocument ToXML()
        {
            if (Regions.OrderedGroupID == null)
            {
                // i det særlige tilfælde, atder kun var en TABEL region på en side - findes der ikke nogen orderedgroup. Den skaber vi så
                Regions.OrderedGroupID = "ro_" + TrLibrary.GetNewTimeStamp();
            }

            // new XComment("Created by Transkribus httpClient - The Royal Danish Library"),
            XDocument xTranscript = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(
                    TrLibrary.Xmlns + "PcGts",
                    new XAttribute("xmlns", TrLibrary.Xmlns),
                    new XAttribute(XNamespace.Xmlns + "xsi", TrLibrary.Xsi),
                    new XAttribute(TrLibrary.Xsi + "schemaLocation", TrLibrary.SchemaLocation)));

            XElement xMetadata = new XElement(
                TrLibrary.Xmlns + "Metadata",
                new XElement(TrLibrary.Xmlns + "Creator", Creator),
                new XElement(TrLibrary.Xmlns + "Created", TempCreationDate),
                new XElement(TrLibrary.Xmlns + "LastChange", TempLastChangeDate),
                new XElement(
                    TrLibrary.Xmlns + "TranskribusMetadata",
                    new XAttribute("pageId", ID),
                    new XAttribute("pageNr", PageNr),
                    new XAttribute("tsid", Timestamp),
                    new XAttribute("status", Status)));

            XElement xPage = new XElement(
                TrLibrary.Xmlns + "Page",
                new XAttribute("imageFilename", ParentPage.ImageFileName),
                new XAttribute("imageWidth", ParentPage.Width.ToString()),
                new XAttribute("imageHeight", ParentPage.Height.ToString()),
                new XAttribute("type", Type));

            XElement xOrderedGroup = new XElement(
                TrLibrary.Xmlns + "OrderedGroup",
                new XAttribute("id", Regions.OrderedGroupID),
                new XAttribute("caption", "Regions reading order"));

            foreach (TrRegion region in Regions)
            {
                XElement xRegionRef = region.RegionRef;
                xOrderedGroup.Add(xRegionRef);
            }

            XElement xReadingOrder = new XElement(TrLibrary.Xmlns + "ReadingOrder");
            xReadingOrder.Add(xOrderedGroup);
            xPage.Add(xReadingOrder);

            foreach (TrRegion region in Regions)
            {
                xPage.Add(region.ToXML());
            }

            xTranscript.Root.Add(xMetadata);
            xTranscript.Root.Add(xPage);

            return xTranscript;
        }

        public async void Upload(HttpClient currentClient)
        {
            // POSTing
            UploadURL = UploadURL.Replace("_ColID_", ParentPage.ParentDocument.ParentCollection.ID);
            UploadURL = UploadURL.Replace("_DocID_", ParentPage.ParentDocument.ID);
            UploadURL = UploadURL.Replace("_PageNr_", PageNr.ToString());

            // Debug.WriteLine(UploadURL);
            var parameters = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("status", "FINAL"),
                    new KeyValuePair<string, string>("overwrite", "FALSE"),
                });

            Timestamp = TrLibrary.GetNewTimeStamp().ToString();

            var uploadHttpContent = new StringContent(ToXML().ToString(), Encoding.UTF8, "application/xml");

            try
            {
                HttpResponseMessage uploadResponseMessage = await currentClient.PostAsync(UploadURL, uploadHttpContent);
                string uploadResponse = uploadResponseMessage.StatusCode.ToString();

                Debug.WriteLine($"Uploading page nr... {PageNr.ToString()} ... {uploadResponse}");

                // Debug.WriteLine(UploadResponse);
                HasChanged = false;
                ChangesUploaded = true;
                Debug.Print($"Transcripts changed: {ParentPage.ParentDocument.NrOfTranscriptsChanged} - Transcripts uploaded: {ParentPage.ParentDocument.NrOfTranscriptsUploaded}");
            }
            catch (TaskCanceledException e)
            {
                // MessageBox.Show("Exception occured!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Debug.WriteLine($"Task canceled! Exception message when uploading page nr {PageNr.ToString()}: {e.Message}");
            }
            catch (OperationCanceledException e)
            {
                Debug.WriteLine($"Operation canceled! Exception message when uploading page nr {PageNr.ToString()}: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"General error! Exception message when uploading page nr {PageNr.ToString()}: {e.Message}");
            }
        }
    }
}
