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

namespace TrClient
{
    public class clsTrCollection : IComparable, INotifyPropertyChanged
    {
        public string TrpDocuments = "https://transkribus.eu/TrpServer/rest/collections/_ColID_/list.xml";

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }

        }


        public string Folder { get; set; }

        public string ID { get; set; }

        private int _nrOfDocs = 0;
        public int NrOfDocs
        {
            get { return _nrOfDocs; }
            set
            {
                if (_nrOfDocs != value)
                {
                    _nrOfDocs = value;
                    NotifyPropertyChanged("NrOfDocs");
                }
            }
        }

        private int _nrOfDocsLoaded = 0;
        public int NrOfDocsLoaded
        {
            get { return _nrOfDocsLoaded; }
            set
            {
                if (_nrOfDocsLoaded != value)
                {
                    _nrOfDocsLoaded = value;
                    NotifyPropertyChanged("NrOfDocsLoaded");
                }
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
                ParentContainer.HasChanged = value;        
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
                ParentContainer.ChangesUploaded = value;
            }
        }


        public XmlDocument DocumentsMetadata = new XmlDocument();
        public clsTrDocuments Documents = new clsTrDocuments();

        public clsTrCollections ParentContainer;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        // constructor ONLINE
        public clsTrCollection(string CollName, string CollID, int CollNrOfDocs)
        {
            Name = CollName;
            ID = CollID;
            NrOfDocs = CollNrOfDocs;
            // Debug.WriteLine($"Collection created! Name: {Name}, ID: {ID}, NrOfDocs: {NrOfDocs}");

            Documents.ParentCollection = this;
            IsLoaded = false;
        }

        // constuctor OFFLINE
        public clsTrCollection(string CollName, string CollID, int CollNrOfDocs, string CollFolder) // , string CollID, int CollNrOfDocs
        {
            Name = CollName;
            Folder = CollFolder;
            ID = CollID;
            NrOfDocs = CollNrOfDocs;
            // Debug.WriteLine($"Collection created! Name: {Name}, ID: {ID}, NrOfDocs: {NrOfDocs}, Folder: {Folder}");

            Documents.ParentCollection = this;
            IsLoaded = false;
        }



        public int CompareTo(object obj)
        {
            var coll = obj as clsTrCollection;
            return Name.CompareTo(coll.Name);
        }

        public async Task<bool> LoadDocuments(HttpClient CurrentClient)
        {
            // bruges kun ONLINE
            if (!IsLoaded)
            {
                TrpDocuments = TrpDocuments.Replace("_ColID_", ID);
                // Debug.WriteLine(TrpDocuments);

                // Henter de relevante documents ind i et XMLdoc
                HttpResponseMessage DocumentsResponseMessage = await CurrentClient.GetAsync(TrpDocuments);
                string DocumentsResponse = await DocumentsResponseMessage.Content.ReadAsStringAsync();
                DocumentsMetadata.LoadXml(DocumentsResponse);
                // Debug.WriteLine($"henter doks i samlingen {Name}, {ID}");
                // Debug.WriteLine($"antal doks 1 = {Documents.Count}");

                // Og gemmer - i udviklingsfasen - xml-filen.
                // Det har samtidig den fordel, at det forsinker lidt....
                // string XMLFileName = clsTrLibrary.ExportFolder + Name + ".xml";
                // DocumentsMetadata.Save(XMLFileName);

                // Udtrækker de enkelte documents
                XmlNodeList DocumentNodes = DocumentsMetadata.DocumentElement.SelectNodes("//trpDocMetadata");
                foreach (XmlNode xnDocument in DocumentNodes)
                {
                    XmlNodeList DocumentMetaData = xnDocument.ChildNodes;
                    string DocID = "";
                    string DocTitle = "";
                    int NrOfPages = 0;

                    foreach (XmlNode xnDocumentMetaData in DocumentMetaData)
                    {
                        string Name = xnDocumentMetaData.Name;
                        string Value = xnDocumentMetaData.InnerText;

                        switch (Name)
                        {
                            case "docId":
                                DocID = Value;
                                break;
                            case "title":
                                DocTitle = Value;
                                break;
                            case "nrOfPages":
                                NrOfPages = Int32.Parse(Value);
                                break;
                        }
                    }
                    if (DocTitle.Substring(0, 4) != "TRAI")
                    {
                        clsTrDocument Doc = new clsTrDocument(DocTitle, DocID, NrOfPages);
                        Documents.Add(Doc);
                    }

                }
                // Debug.WriteLine($"antal doks 2 = {Documents.Count}");
                Documents.Sort();
                NrOfDocs = Documents.Count;
                IsLoaded = true;
            }
            return true;
        }

        public void OpenDocuments()
        {
            if (!IsLoaded)
            {
                DirectoryInfo diDocumentIDs = new DirectoryInfo(Folder);
                DirectoryInfo[] diDocumentIDsArr = diDocumentIDs.GetDirectories();

                foreach (DirectoryInfo diDocumentID in diDocumentIDsArr)
                {
                    // så er vi inde i det enkelte dokument
                    // ------------------------------------------------------------------------------------------------------------------
                    string DocumentID = diDocumentID.Name;
                    string DocumentIDFolder = Folder + "\\" + DocumentID + "\\";

                    DirectoryInfo diDocumentTitles = new DirectoryInfo(DocumentIDFolder);
                    DirectoryInfo[] diDocumentTitlesArr = diDocumentTitles.GetDirectories();
                    DirectoryInfo diDocumentTitle = diDocumentTitlesArr.First();

                    string DocumentTitle = diDocumentTitle.Name;
                    string DocumentFolder = Folder + "\\" + DocumentID + "\\" + DocumentTitle + "\\";

                    string MetsFileName = DocumentFolder + "mets.xml";
                    XmlDocument MetsDocument = new XmlDocument();
                    MetsDocument.Load(MetsFileName);

                    XmlNodeList DocumentNodes = MetsDocument.DocumentElement.SelectNodes("//trpDocMetadata"); // 

                    foreach (XmlNode xnDocument in DocumentNodes)
                    {
                        XmlNodeList DocumentMetaData = xnDocument.ChildNodes;
                        string DocID = "";
                        string DocTitle = "";
                        int NrOfPages = 0;

                        foreach (XmlNode xnDocumentMetaData in DocumentMetaData)
                        {
                            string Name = xnDocumentMetaData.Name;
                            string Value = xnDocumentMetaData.InnerText;

                            switch (Name)
                            {
                                case "docId":
                                    DocID = Value;
                                    break;
                                case "title":
                                    DocTitle = Value;
                                    break;
                                case "nrOfPages":
                                    NrOfPages = Int32.Parse(Value);
                                    break;
                            }
                        }
                        clsTrDocument Doc = new clsTrDocument(DocTitle, DocID, NrOfPages, DocumentFolder);
                        Documents.Add(Doc);
                        NrOfDocsLoaded++;

                    }
                    // Debug.WriteLine($"antal doks 2 = {Documents.Count}");
                    Documents.Sort();
                    NrOfDocs = Documents.Count;
                    IsLoaded = true;

                }

            }
        }


        public void Upload(HttpClient CurrentClient)
        {
            // bruges kun ONLINE

            foreach (clsTrDocument Doc in Documents)
            {
                if (Doc.HasChanged)
                    Doc.Upload(CurrentClient);
            }
        }

        public XDocument KOBACC_ExportAccessions()
        {
            XDocument xAccessionsDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XComment("Created by Transkribus Client - The Royal Danish Library"));

            XElement xRoot = new XElement("Root");

            XElement xAccessions = new XElement("Accessions",
                new XAttribute("Document", Name));

            XElement xSources = new XElement("Sources");

            foreach (clsTrDocument Doc in Documents)
            {
                Doc.OpenPages();

                foreach (clsTrPage Page in Doc.Pages)
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
                                                            new XAttribute("Doc", Doc.Title),
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
                                                        new XAttribute("Doc", Doc.Title),
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
                                                new XAttribute("Doc", Doc.Title),
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
                    
            }


            xRoot.Add(xAccessions);
            // xRoot.Add(xSources);
            xAccessionsDoc.Add(xRoot);
            return xAccessionsDoc;
        }


    }
}
