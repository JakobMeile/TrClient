// <copyright file="TrCollection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Linq;
    using TrClient.Libraries;

    public class TrCollection : IComparable, INotifyPropertyChanged
    {
        public string TrpDocuments = "https://transkribus.eu/TrpServer/rest/collections/_ColID_/list.xml";

        private string name = string.Empty;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Folder { get; set; }

        public string ID { get; set; }

        private int nrOfDocs = 0;

        public int NrOfDocs
        {
            get
            {
                return nrOfDocs;
            }

            set
            {
                if (nrOfDocs != value)
                {
                    nrOfDocs = value;
                    NotifyPropertyChanged("NrOfDocs");
                }
            }
        }

        private int nrOfDocsLoaded = 0;

        public int NrOfDocsLoaded
        {
            get
            {
                return nrOfDocsLoaded;
            }

            set
            {
                if (nrOfDocsLoaded != value)
                {
                    nrOfDocsLoaded = value;
                    NotifyPropertyChanged("NrOfDocsLoaded");
                }
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

                ParentContainer.HasChanged = value;
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

                ParentContainer.ChangesUploaded = value;
            }
        }

        public XmlDocument DocumentsMetadata = new XmlDocument();
        public TrDocuments Documents = new TrDocuments();

        public TrCollections ParentContainer;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // constructor ONLINE
        public TrCollection(string collName, string collID, int collNrOfDocs)
        {
            Name = collName;
            ID = collID;
            NrOfDocs = collNrOfDocs;

            // Debug.WriteLine($"Collection created! Name: {Name}, ID: {ID}, NrOfDocs: {NrOfDocs}");
            Documents.ParentCollection = this;
            IsLoaded = false;
        }

        // constuctor OFFLINE
        //public TrCollection(string collName, string collID, int collNrOfDocs, string collFolder) // , string CollID, int CollNrOfDocs
        //{
        //    Name = collName;
        //    Folder = collFolder;
        //    ID = collID;
        //    NrOfDocs = collNrOfDocs;

        //    // Debug.WriteLine($"Collection created! Name: {Name}, ID: {ID}, NrOfDocs: {NrOfDocs}, Folder: {Folder}");
        //    Documents.ParentCollection = this;
        //    IsLoaded = false;
        //}

        public int CompareTo(object obj)
        {
            var coll = obj as TrCollection;
            return Name.CompareTo(coll.Name);
        }

        public async Task<bool> LoadDocuments(HttpClient currentClient)
        {
            // bruges kun ONLINE
            if (!IsLoaded)
            {
                TrpDocuments = TrpDocuments.Replace("_ColID_", ID);

                // Debug.WriteLine(TrpDocuments);

                // Henter de relevante documents ind i et XMLdoc
                HttpResponseMessage documentsResponseMessage = await currentClient.GetAsync(TrpDocuments);
                string documentsResponse = await documentsResponseMessage.Content.ReadAsStringAsync();
                DocumentsMetadata.LoadXml(documentsResponse);

                // Debug.WriteLine($"henter doks i samlingen {Name}, {ID}");
                // Debug.WriteLine($"antal doks 1 = {Documents.Count}");

                // Og gemmer - i udviklingsfasen - xml-filen.
                // Det har samtidig den fordel, at det forsinker lidt....
                // string XMLFileName = TrLibrary.ExportFolder + Name + ".xml";
                // DocumentsMetadata.Save(XMLFileName);

                // Udtrækker de enkelte documents
                XmlNodeList documentNodes = DocumentsMetadata.DocumentElement.SelectNodes("//trpDocMetadata");
                foreach (XmlNode xnDocument in documentNodes)
                {
                    XmlNodeList documentMetaData = xnDocument.ChildNodes;
                    string docID = string.Empty;
                    string docTitle = string.Empty;
                    int nrOfPages = 0;

                    foreach (XmlNode xnDocumentMetaData in documentMetaData)
                    {
                        string name = xnDocumentMetaData.Name;
                        string value = xnDocumentMetaData.InnerText;

                        switch (name)
                        {
                            case "docId":
                                docID = value;
                                break;
                            case "title":
                                docTitle = value;
                                break;
                            case "nrOfPages":
                                nrOfPages = Int32.Parse(value);
                                break;
                        }
                    }

                    if (docTitle.Substring(0, 4) != "TRAI")
                    {
                        TrDocument doc = new TrDocument(docTitle, docID, nrOfPages);
                        Documents.Add(doc);
                    }
                }

                // Debug.WriteLine($"antal doks 2 = {Documents.Count}");
                Documents.Sort();
                NrOfDocs = Documents.Count;
                IsLoaded = true;
            }

            return true;
        }

        //public void OpenDocuments()
        //{
        //    if (!IsLoaded)
        //    {
        //        DirectoryInfo diDocumentIDs = new DirectoryInfo(Folder);
        //        DirectoryInfo[] diDocumentIDsArr = diDocumentIDs.GetDirectories();

        //        foreach (DirectoryInfo diDocumentID in diDocumentIDsArr)
        //        {
        //            // så er vi inde i det enkelte dokument
        //            // ------------------------------------------------------------------------------------------------------------------
        //            string documentID = diDocumentID.Name;
        //            string documentIDFolder = Folder + "\\" + documentID + "\\";

        //            DirectoryInfo diDocumentTitles = new DirectoryInfo(documentIDFolder);
        //            DirectoryInfo[] diDocumentTitlesArr = diDocumentTitles.GetDirectories();
        //            DirectoryInfo diDocumentTitle = diDocumentTitlesArr.First();

        //            string documentTitle = diDocumentTitle.Name;
        //            string documentFolder = Folder + "\\" + documentID + "\\" + documentTitle + "\\";

        //            string metsFileName = documentFolder + "mets.xml";
        //            XmlDocument metsDocument = new XmlDocument();
        //            metsDocument.Load(metsFileName);

        //            XmlNodeList documentNodes = metsDocument.DocumentElement.SelectNodes("//trpDocMetadata");

        //            foreach (XmlNode xnDocument in documentNodes)
        //            {
        //                XmlNodeList documentMetaData = xnDocument.ChildNodes;
        //                string docID = string.Empty;
        //                string docTitle = string.Empty;
        //                int nrOfPages = 0;

        //                foreach (XmlNode xnDocumentMetaData in documentMetaData)
        //                {
        //                    string name = xnDocumentMetaData.Name;
        //                    string value = xnDocumentMetaData.InnerText;

        //                    switch (name)
        //                    {
        //                        case "docId":
        //                            docID = value;
        //                            break;
        //                        case "title":
        //                            docTitle = value;
        //                            break;
        //                        case "nrOfPages":
        //                            nrOfPages = Int32.Parse(value);
        //                            break;
        //                    }
        //                }

        //                TrDocument doc = new TrDocument(docTitle, docID, nrOfPages, documentFolder);
        //                Documents.Add(doc);
        //                NrOfDocsLoaded++;
        //            }

        //            // Debug.WriteLine($"antal doks 2 = {Documents.Count}");
        //            Documents.Sort();
        //            NrOfDocs = Documents.Count;
        //            IsLoaded = true;
        //        }
        //    }
        //}

        public void Upload(HttpClient currentClient)
        {
            // bruges kun ONLINE
            foreach (TrDocument doc in Documents)
            {
                if (doc.HasChanged)
                {
                    doc.Upload(currentClient);
                }
            }
        }

        //public XDocument KOBACC_ExportAccessions()
        //{
        //    XDocument xAccessionsDoc = new XDocument(
        //        new XDeclaration("1.0", "UTF-8", "yes"),
        //        new XComment("Created by Transkribus Client - The Royal Danish Library"));

        //    XElement xRoot = new XElement("Root");

        //    XElement xAccessions = new XElement(
        //        "Accessions",
        //        new XAttribute("Document", Name));

        //    XElement xSources = new XElement("Sources");

        //    foreach (TrDocument doc in Documents)
        //    {
        //        doc.OpenPages();

        //        foreach (TrPage page in doc.Pages)
        //        {
        //            if (page.HasRegions)
        //            {
        //                foreach (TrRegion textRegion in page.Transcripts[0].Regions)
        //                {
        //                    if (textRegion.GetType() == typeof(TrTextRegion))
        //                    {
        //                        foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
        //                        {
        //                            if (textLine.TextEquiv != string.Empty)
        //                            {
        //                                if (textLine.HasSpecificStructuralTag("Acc"))
        //                                {
        //                                    // Der kan være een eller flere...
        //                                    if (textLine.TextEquiv.Contains(" - "))
        //                                    {
        //                                        // der ER flere
        //                                        string[] accessionNumbers = textLine.TextEquiv.Split('-').ToArray();
        //                                        foreach (string aN in accessionNumbers)
        //                                        {
        //                                            if (aN != "n/a")
        //                                            {
        //                                                XElement xAccession = new XElement("Accession", TrLibrary.StripSharpParanthesis(aN),
        //                                                    new XAttribute("Doc", doc.Title),
        //                                                    new XAttribute("Page", page.PageNr),
        //                                                    new XAttribute("Hpos", textLine.Hpos),
        //                                                    new XAttribute("Vpos", textLine.Vpos));
        //                                                xAccessions.Add(xAccession);
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        // der er kun een
        //                                        if (textLine.TextEquiv != "n/a")
        //                                        {
        //                                            XElement xAccession = new XElement("Accession", TrLibrary.StripSharpParanthesis(textLine.TextEquiv),
        //                                                new XAttribute("Doc", doc.Title),
        //                                                new XAttribute("Page", page.PageNr),
        //                                                new XAttribute("Hpos", textLine.Hpos),
        //                                                new XAttribute("Vpos", textLine.Vpos));
        //                                            xAccessions.Add(xAccession);
        //                                        }
        //                                    }
        //                                }
        //                                else if (textLine.HasSpecificStructuralTag("caption"))
        //                                {
        //                                    XElement xSource = new XElement("Source", textLine.TextEquiv,
        //                                        new XAttribute("Doc", doc.Title),
        //                                        new XAttribute("Page", page.PageNr),
        //                                        new XAttribute("Hpos", textLine.Hpos),
        //                                        new XAttribute("Vpos", textLine.Vpos));
        //                                    xSources.Add(xSource);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    xRoot.Add(xAccessions);

        //    // xRoot.Add(xSources);
        //    xAccessionsDoc.Add(xRoot);
        //    return xAccessionsDoc;
        //}
    }
}
