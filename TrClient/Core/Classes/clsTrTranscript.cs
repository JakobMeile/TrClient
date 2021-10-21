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
using System.Globalization;
using System.ComponentModel;
using System.Windows.Media;
using System.Text.RegularExpressions;


namespace TrClient
{
    public class clsTrTranscript : IComparable, INotifyPropertyChanged
    {
        //public string BaseURL = "https://dbis-thure.uibk.ac.at/f/Get?id=";
        public string BaseURL = "https://files.transkribus.eu/Get?id=";
        public string UploadURL = "https://transkribus.eu/TrpServer/rest/collections/_ColID_/_DocID_/_PageNr_/text?overwrite=true";

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


        public XDocument TranscriptionDocument;

        public clsTrRegions Regions = new clsTrRegions();

        public clsTrTranscripts ParentContainer;
        public clsTrPage ParentPage;

        private bool _hasChanged = false;
        public bool HasChanged
        {
            get { return _hasChanged;  }
            set
            {
                _hasChanged = value;
                if (_hasChanged)
                    StatusColor = Brushes.Orange;
                NotifyPropertyChanged("HasChanged");
                ParentPage.HasChanged = value;
                if (_hasChanged)
                    ParentPage.ParentDocument.NrOfTranscriptsChanged++;
                else
                    ParentPage.ParentDocument.NrOfTranscriptsChanged--;
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
                ParentPage.ChangesUploaded = value;
                Regions.ChangesUploaded = value;
                if (_changesUploaded)
                    ParentPage.ParentDocument.NrOfTranscriptsUploaded++;
                else
                    ParentPage.ParentDocument.NrOfTranscriptsUploaded--;
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


        private bool _hasRegionalTags = false;
        public bool HasRegionalTags
        {
            get
            {
                foreach (clsTrRegion TR in Regions)
                {
                    _hasRegionalTags = _hasRegionalTags || TR.HasStructuralTag;
                }
                return _hasRegionalTags;
            }
        }

        private bool _hasStructuralTags = false;
        public bool HasStructuralTags
        {
            get
            {
                foreach (clsTrRegion TR in Regions)
                {
                    if (TR.GetType() == typeof(clsTrTextRegion))
                    {
                        foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        {
                            _hasStructuralTags = _hasStructuralTags || TL.HasStructuralTag;
                        }
                    }
                    else if (TR.GetType() == typeof(clsTrTableRegion))
                    {
                        //foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                        //{
                        //    _hasStructuralTags = _hasStructuralTags || TL.HasStructuralTag;
                        //}
                    }
                }
                return _hasStructuralTags;
            }
        }

        private bool _hasRegions;
        public bool HasRegions
        {
            get
            {
                _hasRegions = (Regions.Count > 0);
                return _hasRegions;
            }
        }


        private bool _hasTables;
        public bool HasTables
        {
            get
            {
                _hasTables = false;
                if (HasRegions)
                {
                    foreach (clsTrRegion Region in Regions)
                        _hasTables = _hasTables || (Region.GetType() == typeof(clsTrTableRegion));
                }
                return _hasTables;
            }
        }


        public void ConvertTablesToRegions()
        {
            if (HasTables)
            {
                Debug.WriteLine($"Transcript {ID}: Entering convert tables");
                clsTrRegions NewRegions = new clsTrRegions();

                foreach (clsTrRegion TR in Regions)
                {
                    if (TR.GetType() == typeof(clsTrTableRegion))
                    {
                        Debug.WriteLine($"Table found!");
                        clsTrTextRegion NewTextRegion = new clsTrTextRegion(TR.ReadingOrder, TR.Orientation, TR.CoordsString);
                        NewRegions.Add(NewTextRegion);

                        foreach (clsTrCell Cell in (TR as clsTrTableRegion).Cells)
                            foreach (clsTrTextLine TL in Cell.TextLines)
                                NewTextRegion.TextLines.Add(TL);

                        TR.MarkToDeletion = true;
                    }
                }

                // så kopierer vi de nye regioner ind i de gamle
                foreach (clsTrRegion NewRegion in NewRegions)
                    Regions.Add(NewRegion);
                

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
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        // constructor ONLINE
        public clsTrTranscript(string tID, string tKey, int iPageNr, string tStatus, string tUser, string tTimestamp)
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

        // constructor OFFLINE
        public clsTrTranscript(string PageFile)
        {
            // bruges kun OFFLINE
            PageFileName = PageFile;
            Regions.ParentTranscript = this;
            IsLoaded = false;
        }

        public void LoadPageXML()
        {
            // BRUGES KUN OFFLINE
            if (!IsLoaded)
            {
                Regions.ParentTranscript = this;
                // IsLoaded = false;
                // Debug.WriteLine("Transcript starting to be created!");

                // Henter transcript ind i et XMLdoc
                try
                {
                    //HttpResponseMessage TranscriptResponseMessage = await CurrentClient.GetAsync(URL);
                    //string TranscriptResponse = await TranscriptResponseMessage.Content.ReadAsStringAsync();

                    // Debug.WriteLine($"Loading ...: {PageFileName}");
                    TranscriptionDocument = XDocument.Load(PageFileName);

                    XNamespace tr = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2013-07-15";

                    foreach (var metadata in TranscriptionDocument.Descendants(tr + "Metadata"))
                    {
                        string xCreator = metadata.Element(tr + "Creator") == null 
                                ? String.Empty 
                                : metadata.Element(tr + "Creator").Value;
                        string xCreated = metadata.Element(tr + "Created") == null 
                                ? String.Empty 
                                : metadata.Element(tr + "Created").Value;
                        string xLastChange = metadata.Element(tr + "LastChange") == null 
                                ? String.Empty 
                                : metadata.Element(tr + "LastChange").Value;

                        Creator = xCreator;
                        TempCreationDate = xCreated;
                        TempLastChangeDate = xLastChange;
                        // Debug.WriteLine($"Creator = {Creator}, TempCreationDate = {TempCreationDate}, TempLastChangeDate = {TempLastChangeDate}");


                        ID = metadata.Element(tr + "TranskribusMetadata") == null 
                                ? String.Empty 
                                : metadata.Element(tr + "TranskribusMetadata").Attribute("pageId").Value;

                        string xPageNr = metadata.Element(tr + "TranskribusMetadata") == null
                                ? String.Empty
                                : metadata.Element(tr + "TranskribusMetadata").Attribute("pageNr").Value;

                        PageNr = int.Parse(xPageNr);

                        Timestamp = metadata.Element(tr + "TranskribusMetadata") == null
                                ? String.Empty
                                : metadata.Element(tr + "TranskribusMetadata").Attribute("tsid").Value;

                        Status = metadata.Element(tr + "TranskribusMetadata") == null
                                ? String.Empty
                                : metadata.Element(tr + "TranskribusMetadata").Attribute("status").Value;

                        // Debug.WriteLine($"ID = {ID}, PageNr = {PageNr}, Timestamp = {Timestamp}, Status = {Status}");

                    }

                    foreach (var page in TranscriptionDocument.Descendants(tr + "Page"))
                    {
                        Type = page.Attribute("type") == null ? String.Empty : page.Attribute("type").Value;
                    }


                    foreach (var group in TranscriptionDocument.Descendants(tr + "OrderedGroup"))
                    {
                        string xRO = group.Element(tr + "OrderedGroup") == null ? "ro_" + clsTrLibrary.GetNewTimeStamp() : group.Attribute("id").Value;
                        // Debug.WriteLine($"Order ID {xRO}");
                        Regions.OrderedGroupID = xRO;
                    }


                    foreach (var region in TranscriptionDocument.Descendants(tr + "TextRegion"))
                    {
                        string RegionType = region.Attribute("type") == null ? String.Empty : region.Attribute("type").Value;
                        string RegionID = region.Attribute("id") == null ? String.Empty : region.Attribute("id").Value;
                        string RegionTagString = region.Attribute("custom") == null ? String.Empty : region.Attribute("custom").Value;
                        float Orientation = region.Attribute("orientation") == null ? 0 : float.Parse(region.Attribute("orientation").Value);
                        string RegionCoords = region.Element(tr + "Coords") == null ? String.Empty : region.Element(tr + "Coords").Attribute("points").Value;

                        clsTrTextRegion NewTextRegion = new clsTrTextRegion(RegionType, RegionID, RegionTagString, Orientation, RegionCoords);
                        Regions.Add(NewTextRegion);

                        foreach (var line in region.Descendants(tr + "TextLine"))
                        {
                            string LineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
                            string LineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
                            string LineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
                            string BaseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
                            string TextEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

                            // Debug.WriteLine($"{LineID}, {LineTagString}, {LineCoords}, {BaseLineCoords}, {TextEquiv}");

                            clsTrTextLine NewTextLine = new clsTrTextLine(LineID, LineTagString, LineCoords, BaseLineCoords, TextEquiv);
                            NewTextRegion.TextLines.Add(NewTextLine);
                        }
                    }

                    foreach (var table in TranscriptionDocument.Descendants(tr + "TableRegion"))
                    {
                        string RegionType = table.Attribute("type") == null ? String.Empty : table.Attribute("type").Value;
                        string RegionID = table.Attribute("id") == null ? String.Empty : table.Attribute("id").Value;
                        string RegionTagString = table.Attribute("custom") == null ? String.Empty : table.Attribute("custom").Value;
                        float Orientation = table.Attribute("orientation") == null ? 0 : float.Parse(table.Attribute("orientation").Value);
                        string RegionCoords = table.Element(tr + "Coords") == null ? String.Empty : table.Element(tr + "Coords").Attribute("points").Value;

                        clsTrTableRegion NewTable = new clsTrTableRegion(RegionType, RegionID, RegionTagString, Orientation, RegionCoords);
                        Regions.Add(NewTable);

                        // Debug.WriteLine($"New table at page # {PageNr} !!!");

                        foreach (var cell in table.Descendants(tr + "TableCell"))
                        {
                            string CellID = cell.Attribute("id") == null ? String.Empty : cell.Attribute("id").Value;
                            string CellRow = cell.Attribute("row") == null ? String.Empty : cell.Attribute("row").Value;
                            string CellCol = cell.Attribute("col") == null ? String.Empty : cell.Attribute("col").Value;
                            string CellCoords = cell.Element(tr + "Coords") == null ? String.Empty : cell.Element(tr + "Coords").Attribute("points").Value;
                            string CellCornerPoints = cell.Element(tr + "CornerPts") == null ? String.Empty : cell.Element(tr + "CornerPts").Value;

                            clsTrCell NewTableCell = new clsTrCell(CellID, CellRow, CellCol, CellCoords, CellCornerPoints);
                            NewTable.AddCell(NewTableCell);

                            Debug.WriteLine($"New cell in table!");

                            foreach (var line in cell.Descendants(tr + "TextLine"))
                            {
                                string LineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
                                string LineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
                                string LineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
                                string BaseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
                                string TextEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

                                clsTrTextLine NewTextLine = new clsTrTextLine(LineID, LineTagString, LineCoords, BaseLineCoords, TextEquiv);
                                NewTableCell.TextLines.Add(NewTextLine);

                                Debug.WriteLine($"New textline in cell!");
                            }
                        }
                    }


                    IsLoaded = true;
                    ParentPage.IsLoaded = true;
                    ParentPage.ParentDocument.NrOfTranscriptsLoaded++;

                    // Debug.WriteLine($"Transcript loaded: {ParentPage.ParentDocument.ParentCollection.Name} / {ParentPage.ParentDocument.Title} / {ParentPage.PageNr}");

                }
                catch (TaskCanceledException e)
                {
                    // MessageBox.Show("Exception occured!", clsTrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Debug.WriteLine($"Task canceled! Exception message when loading page nr {PageNr.ToString()}: {PageFileName}: {e.Message}");
                }
                catch (OperationCanceledException e)
                {
                    Debug.WriteLine($"Operation canceled! Exception message when loading page nr {PageNr.ToString()}: {PageFileName}: {e.Message}");
                }
                //catch (Exception e)
                //{
                //    Debug.WriteLine($"General error! Exception message when loading page nr {PageNr.ToString()}: {PageFileName}: {e.Message}");
                //}

            }
        }


        private DateTime _dateTimeOfTranscript;
        public DateTime DateTimeOfTranscript
        {
            get
            {
                _dateTimeOfTranscript = clsTrLibrary.ConvertUnixTimeStamp(Timestamp);
                return _dateTimeOfTranscript;
            }
        }

        private string _url;
        private string URL
        {
            get
            {
                _url = BaseURL + Key;
                return _url;
            }
        }

        public int CompareTo(object obj)
        {
            var transcript = obj as clsTrTranscript;
            return Timestamp.CompareTo(transcript.Timestamp);
        }

        public void SetRegionalTagsOnNonTaggedRegions(string TagValue)
        {
            foreach (clsTrRegion TR in Regions)
            {
                if (!TR.HasStructuralTag)
                {
                    clsTrStructuralTag structTag = new clsTrStructuralTag(TagValue);
                    TR.StructuralTag = structTag;
                    TR.Tags.Add(structTag);
                    TR.HasChanged = true;
                }
            }
        }


        public clsTrTags GetRegionalTags()
        {
            clsTrTags TempTags = new clsTrTags();

            foreach (clsTrRegion TR in Regions)
            {
                if (TR.HasStructuralTag)
                {
                    TempTags.Add(TR.StructuralTag);
                }
            }
            return TempTags;
        }

        public clsTrTags GetStructuralTags()
        {
            clsTrTags StructuralTags = new clsTrTags();

            foreach (clsTrRegion TR in Regions)
            {
                if (TR.GetType() == typeof(clsTrTextRegion))
                {
                    foreach (clsTrTextLine TL in (TR as clsTrTextRegion).TextLines)
                    {
                        foreach (clsTrTag Tag in TL.Tags)
                        {
                            if (Tag.GetType() == typeof(clsTrStructuralTag))
                            {
                                StructuralTags.Add(Tag);
                                Tag.ParentLine = TL;
                            }
                        }
                    }
                }
                    
            }
            return StructuralTags;
        }

        public bool AddStructuralTag(int Region, int Line, string TagName, bool OverWrite)
        {
            bool TagAdded = false;

            clsTrRegion TR = GetRegionByNumber(Region);
            if (TR != null)
            {
                if (TR.GetType() == typeof(clsTrTextRegion))
                {
                    clsTrTextLine TL = (TR as clsTrTextRegion).GetLineByNumber(Line);
                    if (TL != null)
                    {
                        if (!TL.HasStructuralTag)
                        {
                            TL.AddStructuralTag(TagName, OverWrite);
                            Debug.WriteLine($"Page# {ParentPage.PageNr}: Tag {TagName} tilføjet linie {Region}-{Line}!");
                            TagAdded = true;
                        }
                        else
                            Debug.WriteLine($"Page# {ParentPage.PageNr}: Linie {Region}-{Line} har allerede et structural tag!");
                    }
                    else
                        Debug.WriteLine($"Page# {ParentPage.PageNr}: Linie {Region}-{Line} findes ikke!");

                }
            }
            else
                Debug.WriteLine($"Page# {ParentPage.PageNr}: Region {Region} findes ikke!");

            return TagAdded;
        }

        public void DeleteStructuralTags(int Region, int Line)
        {
            clsTrRegion TR = GetRegionByNumber(Region);
            if (TR != null)
            {
                if (TR.GetType() == typeof(clsTrTextRegion))
                {
                    clsTrTextLine TL = (TR as clsTrTextRegion).GetLineByNumber(Line);
                    if (TL != null)
                    {
                        if (!TL.HasStructuralTag)
                        {
                            TL.DeleteStructuralTag();
                        }
                    }
                }
            }
        }

        public clsTrRegion GetRegionByNumber(int RegionNumber)
        {
            if (Regions != null)
            {
                if (RegionNumber >= 1 && RegionNumber <= Regions.Count)
                {
                    return Regions[RegionNumber - 1];
                }
                else
                {
                    Debug.WriteLine($"GetRegionByNumber: Region nr. {RegionNumber} eksisterer ikke!");
                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"GetRegionByNumber: Dette transcript indeholder ikke regions!");
                return null;
            }
        }



        public async Task<bool> LoadTranscript(HttpClient CurrentClient)
        {
            // bruges kun ONLINE
            if (!IsLoaded)
            {

                // Henter transcript ind i et XMLdoc
                try
                {
                    HttpResponseMessage TranscriptResponseMessage = await CurrentClient.GetAsync(URL);
                    string TranscriptResponse = await TranscriptResponseMessage.Content.ReadAsStringAsync();
                    TranscriptionDocument = XDocument.Parse(TranscriptResponse);
                    // Og gemmer - i udviklingsfasen - xml-filen.
                    string XMLFileName = clsTrLibrary.ExportFolder + ParentPage.ParentDocument.Title + "_" + ParentPage.PageNr.ToString("0000") + ".xml";
                    TranscriptionDocument.Save(XMLFileName);

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
                        string xRO = group.Element(tr + "OrderedGroup") == null ? "ro_" + clsTrLibrary.GetNewTimeStamp() : group.Attribute("id").Value;
                        // Debug.WriteLine($"Order ID {xRO}");
                        Regions.OrderedGroupID = xRO;
                    }


                    foreach (var region in TranscriptionDocument.Descendants(tr + "TextRegion"))
                    {
                        string RegionType = region.Attribute("type") == null ? String.Empty : region.Attribute("type").Value;
                        string RegionID = region.Attribute("id") == null ? String.Empty : region.Attribute("id").Value;
                        string RegionTagString = region.Attribute("custom") == null ? String.Empty : region.Attribute("custom").Value;
                        float Orientation = region.Attribute("orientation") == null ? 0 : float.Parse(region.Attribute("orientation").Value);
                        string RegionCoords = region.Element(tr + "Coords") == null ? String.Empty : region.Element(tr + "Coords").Attribute("points").Value;

                        clsTrTextRegion NewTextRegion = new clsTrTextRegion(RegionType, RegionID, RegionTagString, Orientation, RegionCoords);
                        Regions.Add(NewTextRegion);

                        foreach (var line in region.Descendants(tr + "TextLine"))
                        {
                            string LineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
                            string LineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
                            string LineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
                            string BaseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
                            string TextEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;
                            //string TextEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : Regex.Unescape(line.Element(tr + "TextEquiv").Value);
                            // REGEX.UNESCAPE forsøg 14.11.2020
                            clsTrTextLine NewTextLine = new clsTrTextLine(LineID, LineTagString, LineCoords, BaseLineCoords, TextEquiv);
                            NewTextRegion.TextLines.Add(NewTextLine);
                        }
                    }

                    foreach (var table in TranscriptionDocument.Descendants(tr + "TableRegion"))
                    {
                        string RegionType = table.Attribute("type") == null ? String.Empty : table.Attribute("type").Value;
                        string RegionID = table.Attribute("id") == null ? String.Empty : table.Attribute("id").Value;
                        string RegionTagString = table.Attribute("custom") == null ? String.Empty : table.Attribute("custom").Value;
                        float Orientation = table.Attribute("orientation") == null ? 0 : float.Parse(table.Attribute("orientation").Value);
                        string RegionCoords = table.Element(tr + "Coords") == null ? String.Empty : table.Element(tr + "Coords").Attribute("points").Value;

                        clsTrTableRegion NewTable = new clsTrTableRegion(RegionType, RegionID, RegionTagString, Orientation, RegionCoords);
                        Regions.Add(NewTable);

                        //Debug.WriteLine($"New table at page # {PageNr} !!!");

                        foreach (var cell in table.Descendants(tr + "TableCell"))
                        {
                            string CellID = cell.Attribute("id") == null ? String.Empty : cell.Attribute("id").Value;
                            string CellRow = cell.Attribute("row") == null ? String.Empty : cell.Attribute("row").Value;
                            string CellCol = cell.Attribute("col") == null ? String.Empty : cell.Attribute("col").Value;
                            string CellCoords = cell.Element(tr + "Coords") == null ? String.Empty : cell.Element(tr + "Coords").Attribute("points").Value;
                            string CellCornerPoints = cell.Element(tr + "CornerPts") == null ? String.Empty : cell.Element(tr + "CornerPts").Value;

                            clsTrCell NewTableCell = new clsTrCell(CellID, CellRow, CellCol, CellCoords, CellCornerPoints);
                            NewTable.AddCell(NewTableCell);

                            //Debug.WriteLine($"New cell in table!");

                            foreach (var line in cell.Descendants(tr + "TextLine"))
                            {
                                string LineID = line.Attribute("id") == null ? String.Empty : line.Attribute("id").Value;
                                string LineTagString = line.Attribute("custom") == null ? String.Empty : line.Attribute("custom").Value;
                                string LineCoords = line.Element(tr + "Coords") == null ? String.Empty : line.Element(tr + "Coords").Attribute("points").Value;
                                string BaseLineCoords = line.Element(tr + "Baseline") == null ? String.Empty : line.Element(tr + "Baseline").Attribute("points").Value;
                                string TextEquiv = line.Element(tr + "TextEquiv") == null ? String.Empty : line.Element(tr + "TextEquiv").Value;

                                clsTrTextLine NewTextLine = new clsTrTextLine(LineID, LineTagString, LineCoords, BaseLineCoords, TextEquiv);
                                NewTableCell.TextLines.Add(NewTextLine);

                                //Debug.WriteLine($"New textline in cell!");
                            }
                        }
                    }



                    IsLoaded = true;
                    ParentPage.IsLoaded = true;
                    ParentPage.ParentDocument.NrOfTranscriptsLoaded++;

                    // Debug.WriteLine($"Transcript loaded: {ParentPage.ParentDocument.ParentCollection.Name} / {ParentPage.ParentDocument.Title} / {ParentPage.PageNr}");

                }
                catch (TaskCanceledException e)
                {
                    // MessageBox.Show("Exception occured!", clsTrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
            Debug.WriteLine($"clsTrTranscript: Saving {PageFileName}");

            XDocument xDoc = this.ToXML();
            xDoc.Save(PageFileName);
            HasChanged = false;
            ChangesUploaded = true;
        }

        public XDocument ToXML()
        {
            if (Regions.OrderedGroupID == null)
            {
                // i det særlige tilfælde, atder kun var en TABEL region på en side - findes der ikke nogen orderedgroup. Den skaber vi så
                Regions.OrderedGroupID = "ro_" + clsTrLibrary.GetNewTimeStamp();
            }


            XDocument xTranscript = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                //new XComment("Created by Transkribus Client - The Royal Danish Library"),
                new XElement(clsTrLibrary.xmlns + "PcGts",
                new XAttribute("xmlns", clsTrLibrary.xmlns),
                new XAttribute(XNamespace.Xmlns + "xsi", clsTrLibrary.xsi),
                new XAttribute(clsTrLibrary.xsi + "schemaLocation", clsTrLibrary.schemaLocation)));

            XElement xMetadata = new XElement(clsTrLibrary.xmlns + "Metadata",
                new XElement(clsTrLibrary.xmlns + "Creator", Creator),
                new XElement(clsTrLibrary.xmlns + "Created", TempCreationDate),     
                new XElement(clsTrLibrary.xmlns + "LastChange", TempLastChangeDate),
                new XElement(clsTrLibrary.xmlns + "TranskribusMetadata", 
                    new XAttribute("pageId", ID),
                    new XAttribute("pageNr", PageNr),
                    new XAttribute("tsid", Timestamp),
                    new XAttribute("status", Status)));

            XElement xPage = new XElement(clsTrLibrary.xmlns + "Page",
                new XAttribute("imageFilename", ParentPage.ImageFileName),
                new XAttribute("imageWidth", ParentPage.Width.ToString()),
                new XAttribute("imageHeight", ParentPage.Height.ToString()),
                new XAttribute("type", Type));

            XElement xOrderedGroup = new XElement(clsTrLibrary.xmlns + "OrderedGroup",
                    new XAttribute("id", Regions.OrderedGroupID),
                    new XAttribute("caption", "Regions reading order"));

            foreach (clsTrRegion Region in Regions)
            {
                XElement xRegionRef = Region.RegionRef;
                xOrderedGroup.Add(xRegionRef);
            }

            XElement xReadingOrder = new XElement(clsTrLibrary.xmlns + "ReadingOrder");
                xReadingOrder.Add(xOrderedGroup);
                xPage.Add(xReadingOrder);

            foreach (clsTrRegion Region in Regions)
            {
                xPage.Add(Region.ToXML());
            }

            xTranscript.Root.Add(xMetadata);
            xTranscript.Root.Add(xPage);
                      
            return xTranscript;
        }


        public async void Upload(HttpClient CurrentClient)
        {
            // POSTing
            UploadURL = UploadURL.Replace("_ColID_", ParentPage.ParentDocument.ParentCollection.ID);
            UploadURL = UploadURL.Replace("_DocID_", ParentPage.ParentDocument.ID);
            UploadURL = UploadURL.Replace("_PageNr_", PageNr.ToString());
            // Debug.WriteLine(UploadURL);

            var Parameters = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("status", "FINAL"),
                    new KeyValuePair<string, string>("overwrite", "FALSE"),
                });

            var UploadHttpContent = new StringContent(ToXML().ToString(), Encoding.UTF8, "application/xml");

            try
            {
                HttpResponseMessage UploadResponseMessage = await CurrentClient.PostAsync(UploadURL, UploadHttpContent);
                string UploadResponse = UploadResponseMessage.StatusCode.ToString();

                Debug.WriteLine($"Uploading page nr... {PageNr.ToString()} ... {UploadResponse}");
                // Debug.WriteLine(UploadResponse);
                HasChanged = false;
                ChangesUploaded = true;
                Debug.Print($"Transcripts changed: {ParentPage.ParentDocument.NrOfTranscriptsChanged} - Transcripts uploaded: {ParentPage.ParentDocument.NrOfTranscriptsUploaded}");

            }
            catch (TaskCanceledException e)
            {
                // MessageBox.Show("Exception occured!", clsTrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
