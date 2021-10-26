// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using DanishNLP;
    using TrClient.Core;
    using TrClient.Extensions;
    using TrClient.Helpers;
    using TrClient.Libraries;
    using TrClient.Settings;
    using TrClient.Views;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum DeleteAction
        {
            Delete = 0,
            Preserve = 1,
        }

        public static TrUser User;
        public static readonly HttpClient Client = new HttpClient();
        public static XmlDocument MyCollectionsDocument = new XmlDocument();
        public static TrCollections MyCollections = new TrCollections();
        public static TrCurrent Current = new TrCurrent();
        public static TrCurrent Secondary = new TrCurrent();    // bruges til at kopiere FRA

        // MAIN
        // --------------------------------------------------------------------------------------------------------------------
        public MainWindow()
        {
            if (User == null)
            {
                if (File.Exists("User.xml"))
                {
                    using (var stream = File.OpenRead("User.xml"))
                    {
                        var serializer = new XmlSerializer(typeof(TrUser));
                        User = serializer.Deserialize(stream) as TrUser;
                    }
                }
                else
                {
                    User = new TrUser();
                }
            }

            Client.BaseAddress = new Uri(TrLibrary.TrpBaseAdress);
            Client.DefaultRequestHeaders.Accept.Clear();

            InitializeComponent();

            txtUserName.Text = User.Username;
            txtPassword.Password = User.Password;
            StatusLight.DataContext = MyCollections;
        }

        // LOGIN
        // --------------------------------------------------------------------------------------------------------------------
        public async void RunLoginAndGetMyCollections(string username, string password)
        {
            // Kaldes KUN i online-mode!!
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            var credentials = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", username),
                    new KeyValuePair<string, string>("pw", password),
                });

            try
            {
                HttpResponseMessage loginResponseMessage = await Client.PostAsync(TrLibrary.TrpLogin, credentials);

                string loginResponse = loginResponseMessage.StatusCode.ToString();

                bool status = loginResponse == "OK";
                if (status)
                {
                    // Henter brugerens collections ind i et XMLdoc
                    HttpResponseMessage collectionsResponseMessage = await Client.GetAsync(TrLibrary.TrpCollections);
                    string collectionsResponse = await collectionsResponseMessage.Content.ReadAsStringAsync();
                    MyCollectionsDocument.LoadXml(collectionsResponse);

                    // Udtrækker de enkelte collections
                    XmlNodeList collectionNodes = MyCollectionsDocument.DocumentElement.SelectNodes("//trpCollection");
                    foreach (XmlNode xnCollection in collectionNodes)
                    {
                        XmlNodeList collectionMetaData = xnCollection.ChildNodes;
                        string colID = string.Empty;
                        string colName = string.Empty;
                        int nrOfDocs = 0;

                        foreach (XmlNode xnCollectionMetaData in collectionMetaData)
                        {
                            string name = xnCollectionMetaData.Name;
                            string value = xnCollectionMetaData.InnerText;

                            switch (name)
                            {
                                case "colId":
                                    colID = value;
                                    break;
                                case "colName":
                                    colName = value;
                                    break;
                                case "nrOfDocuments":
                                    nrOfDocs = Int32.Parse(value);
                                    break;
                            }
                        }

                        TrCollection coll = new TrCollection(colName, colID, nrOfDocs);
                        MyCollections.Add(coll);
                    }
                }

                MyCollections.Sort();

                // fylder box op
                lstCollections.ItemsSource = MyCollections;
                MyCollections.IsLoaded = true;
                Mouse.OverrideCursor = null;
            }
            catch (TaskCanceledException e)
            {
                // MessageBox.Show("Exception occured!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Debug.WriteLine($"Task canceled! Exception message when logging in: {e.Message}");
            }
            catch (OperationCanceledException e)
            {
                Debug.WriteLine($"Operation canceled! Exception message when logging in: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"General error! Exception message when logging in: {e.Message}");
            }
        }

        //public void OpenCollections()
        //{
        //    // * åbn en fildims og find basis folder - load collections fra mappestruktur
        //    DirectoryInfo diCollections = new DirectoryInfo(TrLibrary.OfflineBaseFolder);
        //    DirectoryInfo[] diCollectionsArr = diCollections.GetDirectories();

        //    foreach (DirectoryInfo diCollection in diCollectionsArr)
        //    {
        //        // så er vi inde i den enkelte collection
        //        // ------------------------------------------------------------------------------------------------------------------
        //        string CollectionName = diCollection.Name;
        //        string CollectionFolder = TrLibrary.OfflineBaseFolder + CollectionName;
        //        string ColID = "";

        //        // Debug.WriteLine($"Name: {CollectionName}, Folder: {CollectionFolder}");

        //        DirectoryInfo diDocuments = new DirectoryInfo(CollectionFolder);
        //        DirectoryInfo[] diDocumentsArr = diDocuments.GetDirectories();
        //        int NrOfDocs = diDocumentsArr.Length;

        //        // hvis collection ikke er tom..:
        //        if (NrOfDocs >= 1)
        //        {
        //            DirectoryInfo diFirstDocument = diDocumentsArr.First();
        //            string FirstDocumentID = diFirstDocument.Name;          // VED export af hel collection er strukturen ID/Title

        //            string FirstDocumentIDFolder = CollectionFolder + "\\" + FirstDocumentID + "\\";

        //            DirectoryInfo diDocTitles = new DirectoryInfo(FirstDocumentIDFolder);
        //            DirectoryInfo[] diDocTitlesArr = diDocTitles.GetDirectories();
        //            DirectoryInfo diFirstDocTitle = diDocTitlesArr.First();
        //            string FirstDocumentTitle = diFirstDocTitle.Name;

        //            string FirstDocumentFolder = CollectionFolder + "\\" + FirstDocumentID + "\\" + FirstDocumentTitle + "\\";

        //            string MetsFileName = FirstDocumentFolder + "mets.xml";
        //            XmlDocument MetsDocument = new XmlDocument();
        //            MetsDocument.Load(MetsFileName);

        //            XmlNodeList DocumentNodes = MetsDocument.DocumentElement.SelectNodes("//colList"); // //trpDocMetadata
        //            foreach (XmlNode xnDocument in DocumentNodes)
        //            {
        //                XmlNodeList DocumentMetaData = xnDocument.ChildNodes;
        //                string TempID = "";

        //                foreach (XmlNode xnDocumentMetaData in DocumentMetaData)
        //                {
        //                    string Name = xnDocumentMetaData.Name;
        //                    string Value = xnDocumentMetaData.InnerText;

        //                    if (Name == "colId")
        //                    {
        //                        TempID = Value;
        //                        // Debug.WriteLine($"TempID: {TempID}");
        //                    }

        //                    // den næste linie er meget meget mærkelig *************************************************************
        //                    if (TempID != "54183" && TempID != "")
        //                    {
        //                        ColID = TempID;
        //                        break;
        //                    }
        //                }
        //            }
        //            // Debug.WriteLine($"ID: {ColID}, NrOfDocs: {NrOfDocs}");

        //            TrCollection Coll = new TrCollection(CollectionName, ColID, NrOfDocs, CollectionFolder);
        //            MyCollections.Add(Coll);

        //        }

        //    }
        //    MyCollections.Sort();

        //    lstCollections.ItemsSource = MyCollections;
        //    lstSecondaryCollections.ItemsSource = MyCollections;
        //    MyCollections.IsLoaded = true;
        //}

        // EVENTS
        // --------------------------------------------------------------------------------------------------------------------

        // LOGIN-knap
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TrLibrary.OfflineMode)
            {
                // Offline
                //OpenCollections();
            }
            else
            {
                // Online
                if (!string.IsNullOrWhiteSpace(txtUserName.Text) && !string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    User.Username = txtUserName.Text;
                    User.Password = txtPassword.Password;
                    using (var stream = File.Open("User.xml", FileMode.Create))
                    {
                        var serializer = new XmlSerializer(typeof(TrUser));
                        serializer.Serialize(stream, User);
                    }

                    RunLoginAndGetMyCollections(txtUserName.Text, txtPassword.Password);
                }
            }
        }

        // Collection valgt
        private async void LstCollections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstCollections.SelectedItem != null)
            {
                if (Current.Collection != null && Current.Collection.HasChanged)
                {
                    string question = $"{Current.Collection.Name} has changed. Upload changes?";
                    MessageBoxResult result = AskUser(question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (TrLibrary.OfflineMode)
                        {
                            // Current.Collection.Save();
                        }
                        else
                        {
                            Current.Collection.Upload(Client);
                        }
                    }
                }

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                // rydder op
                lstDocuments.ItemsSource = null;
                lstPages.ItemsSource = null;
                Current.Document = null;

                Current.Collection = lstCollections.SelectedItem as TrCollection;
                txtCurrentCollection.DataContext = Current.Collection;
                txtCurrentDocument.DataContext = Current.Document;

                ProgressLoadDocs progress = new ProgressLoadDocs(Current.Collection.NrOfDocs);
                progress.Owner = this;
                progress.DataContext = Current.Collection;
                progress.Show();

                if (TrLibrary.OfflineMode)
                {
                    // Current.Collection.OpenDocuments();
                }
                else
                {
                    Task<bool> loaded = Current.Collection.LoadDocuments(Client);
                    bool oK = await loaded;
                }

                // fylder box op
                lstDocuments.ItemsSource = Current.Collection.Documents;

                if (!TrLibrary.OfflineMode)
                {
                    // henter sider
                    foreach (TrDocument doc in Current.Collection.Documents)
                    {
                        try
                        {
                            Task<bool> pagesLoaded = doc.LoadPages(Client);
                            bool pagesOK = await pagesLoaded;
                        }
                        catch (System.Threading.Tasks.TaskCanceledException eDocLoaded)
                        {
                            Debug.WriteLine($"Exception message: {eDocLoaded.Message}");
                        }
                    }
                }

                progress.Hide();

                Mouse.OverrideCursor = null;
            }
        }

        //private async void LstSecondaryCollections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    {
        //        ListBox lb = sender as ListBox;
        //        if (lstSecondaryCollections.SelectedItem != null)
        //        {
        //            if (Secondary.Collection != null && Secondary.Collection.HasChanged)
        //            {
        //                string question = $"{Secondary.Collection.Name} has changed. Upload changes?";
        //                MessageBoxResult result = AskUser(question);

        //                if (result == MessageBoxResult.Yes)
        //                {
        //                    if (TrLibrary.OfflineMode)
        //                    {
        //                    }
        //                    else
        //                    {
        //                        Secondary.Collection.Upload(Client);
        //                    }
        //                }
        //            }

        //            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

        //            // rydder op
        //            lstSecondaryDocuments.ItemsSource = null;
        //            lstSecondaryPages.ItemsSource = null;
        //            Secondary.Document = null;

        //            Secondary.Collection = lstSecondaryCollections.SelectedItem as TrCollection;
        //            txtSecondaryCollection.DataContext = Secondary.Collection;
        //            txtSecondaryDocument.DataContext = Secondary.Document;

        //            ProgressLoadDocs progress = new ProgressLoadDocs(Secondary.Collection.NrOfDocs);
        //            progress.Owner = this;
        //            progress.DataContext = Secondary.Collection;
        //            progress.Show();

        //            if (TrLibrary.OfflineMode)
        //            {
        //                // Secondary.Collection.OpenDocuments();
        //            }
        //            else
        //            {
        //                Task<bool> loaded = Secondary.Collection.LoadDocuments(Client);
        //                bool oK = await loaded;
        //            }

        //            // fylder box op
        //            lstSecondaryDocuments.ItemsSource = Secondary.Collection.Documents;

        //            if (!TrLibrary.OfflineMode)
        //            {
        //                // henter sider
        //                foreach (TrDocument doc in Secondary.Collection.Documents)
        //                {
        //                    try
        //                    {
        //                        Task<bool> pagesLoaded = doc.LoadPages(Client);
        //                        bool pagesOK = await pagesLoaded;
        //                    }
        //                    catch (System.Threading.Tasks.TaskCanceledException eDocLoaded)
        //                    {
        //                        Debug.WriteLine($"Exception message: {eDocLoaded.Message}");
        //                    }
        //                }
        //            }

        //            progress.Hide();

        //            Mouse.OverrideCursor = null;
        //        }
        //    }
        //}

        // Dokument valgt
        private async void LstDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstDocuments.SelectedItem != null)
            {
                if (Current.Document != null && Current.Document.HasChanged)
                {
                    string question = $"{Current.Document.Title} has changed. Upload changes?";
                    MessageBoxResult result = AskUser(question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (TrLibrary.OfflineMode)
                        {
                            // Current.Document.Save();
                        }
                        else
                        {
                            Current.Document.Upload(Client);
                        }
                    }
                }

                // rydder op
                lstPages.ItemsSource = null;

                Current.Document = lstDocuments.SelectedItem as TrDocument;
                Debug.WriteLine($"Current.Document er valgt: ID = {Current.Document.ID}, Title = {Current.Document.Title}, Pages = {Current.Document.NrOfPages}");

                txtCurrentDocument.DataContext = Current.Document;

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                ProgressLoadPages progress = new ProgressLoadPages(Current.Document.NrOfPages);
                progress.Owner = this;
                progress.DataContext = Current.Document;
                progress.Show();

                if (TrLibrary.OfflineMode)
                {
                    // OFFLINE MODE
                    // Current.Document.OpenPages();

                    //Debug.WriteLine($"OpenPages er kaldt! Current.Document.Title = {Current.Document.Title}");

                    // fylder box op
                    // lstPages.ItemsSource = Current.Document.Pages;
                }
                else
                {   // ONLINE MODE
                    // fylder box op
                    lstPages.ItemsSource = Current.Document.Pages;

                    // henter transcripts for hver side - NB: ALLE transcripts (vha. i = 0 to transcript.count - 1)
                    foreach (TrPage page in Current.Document.Pages)
                    {
                        // TrTranscript Tra;
                        if (TrLibrary.LoadOnlyNewestTranscript)
                        {
                            // Henter kun det NYESTE transcript
                            Task<bool> loaded = page.Transcripts[0].LoadTranscript(Client);
                            bool oK = await loaded;
                        }
                        else
                        {
                            // Henter ALLE transcripts - hvis man har brug for at reverte eller hente gamle tabeller
                            for (int i = 0; i < page.TranscriptCount; i++)
                            {
                                Task<bool> loaded = page.Transcripts[i].LoadTranscript(Client);
                                bool oK = await loaded;
                            }
                        }
                    }
                }

                progress.Hide();
                Mouse.OverrideCursor = null;

                if (Current.Document.HasFormerTables)
                {
                    string question = $"WARNING! {Current.Document.Title} has possibly 'forgotten' tables in old transcripts. You should copy tables to newest transcript and convert them to ordinary regions!";
                    TellUser(question);
                }

                if (!Current.Document.PostLoadTestOK())
                {
                    string question = $"WARNING! {Current.Document.Title} has problems with coordinates and/or non-trimmed strings! Do you want to fix it?";
                    MessageBoxResult result = AskUser(question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Current.Document.PostLoadFix();
                    }
                }
            }
        }

        //private async void LstSecondaryDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ListBox lb = sender as ListBox;
        //    if (lstSecondaryDocuments.SelectedItem != null)
        //    {
        //        if (Secondary.Document != null && Secondary.Document.HasChanged)
        //        {
        //            string question = $"{Secondary.Document.Title} has changed. Upload changes?";
        //            MessageBoxResult result = AskUser(question);

        //            if (result == MessageBoxResult.Yes)
        //            {
        //                if (TrLibrary.OfflineMode)
        //                {
        //                }
        //                else
        //                {
        //                    Secondary.Document.Upload(Client);
        //                }
        //            }
        //        }

        //        // rydder op
        //        lstSecondaryPages.ItemsSource = null;

        //        Secondary.Document = lstSecondaryDocuments.SelectedItem as TrDocument;
        //        Debug.WriteLine($"Secondary.Document er valgt: Title = {Secondary.Document.Title}, Pages = {Secondary.Document.NrOfPages}");

        //        txtSecondaryDocument.DataContext = Secondary.Document;

        //        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

        //        ProgressLoadPages progress = new ProgressLoadPages(Secondary.Document.NrOfPages);
        //        progress.Owner = this;
        //        progress.DataContext = Secondary.Document;
        //        progress.Show();

        //        if (TrLibrary.OfflineMode)
        //        {
        //            // Secondary.Document.OpenPages();

        //            //Debug.WriteLine($"OpenPages er kaldt! Current.Document.Title = {Current.Document.Title}");

        //            // fylder box op
        //            // lstSecondaryPages.ItemsSource = Secondary.Document.Pages;
        //        }
        //        else
        //        {
        //            // fylder box op
        //            lstSecondaryPages.ItemsSource = Secondary.Document.Pages;

        //            // henter nyeste transcript for hver side - nb: kun NYESTE
        //            foreach (TrPage page in Secondary.Document.Pages)
        //            {
        //                Task<bool> loaded = page.Transcripts[0].LoadTranscript(Client);
        //                bool oK = await loaded;
        //            }
        //        }

        //        progress.Hide();
        //        Mouse.OverrideCursor = null;
        //    }
        //}

        private void MenuItem_UploadCollection_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Current.Collection.Upload(Client);
                Mouse.OverrideCursor = null;
            }
        }

        private void MenuItem_UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                Current.Document.Upload(Client);
                Mouse.OverrideCursor = null;
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MyCollections.HasChanged)
            {
                string question = "Your collections has changed. Upload changes?";
                MessageBoxResult result = AskUser(question);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrCollection coll in MyCollections)
                    {
                        coll.Upload(Client);
                    }
                }
            }

            Close();
        }

        private void MenuItem_CreateTopLevelRegion_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Create Top-Level Region on all pages in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    Current.Document.CreateTopLevelRegions();
                    Mouse.OverrideCursor = null;
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void CreateHorizontalRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Create three horizontal regions on all pages in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);

                if (result == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                    Current.Document.AddHorizontalRegions(0, 6, 0, 10);
                    Current.Document.AddHorizontalRegions(6, 36, 10, 10);
                    Current.Document.AddHorizontalRegions(36, 100, 10, 0);

                    Mouse.OverrideCursor = null;
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void CreateVerticalRegions_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_ShowRegionalTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowTags showTags = new ShowTags("Regional Tags in Document:");
                showTags.Owner = this;
                showTags.lstTags.ItemsSource = Current.Document.GetRegionalTags();
                showTags.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_DeleteRegions_Click(object sender, RoutedEventArgs e)
        {
            string chosenRegionalTag;
            DeleteAction deleteAction;
            if (Current.Collection != null && Current.Document != null)
            {
                DeleteRegions deleteRegions = new DeleteRegions();
                deleteRegions.Owner = this;
                deleteRegions.txtRegionalTag.ItemsSource = Current.Document.GetRegionalTags();
                deleteRegions.ShowDialog();
                if (deleteRegions.DialogResult == true)
                {
                    chosenRegionalTag = deleteRegions.TagName;
                    deleteAction = deleteRegions.DeleteAction;

                    if (deleteAction == DeleteAction.Preserve)
                    {
                        string question = $"Delete Other Regions Than \"{chosenRegionalTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult result = AskUser(question);

                        if (result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Preserve chosen to be: {ChosenRegionalTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                            Current.Document.DeleteRegionsOtherThan(chosenRegionalTag);
                            Mouse.OverrideCursor = null;
                        }
                    }
                    else if (deleteAction == DeleteAction.Delete)
                    {
                        string question = $"Delete Regions With Tag \"{chosenRegionalTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult result = AskUser(question);

                        if (result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Deleten chosen to be: {ChosenRegionalTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                            Current.Document.DeleteRegionsWithTag(chosenRegionalTag);
                            Mouse.OverrideCursor = null;
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_RepairBaseLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                RepairBaseLines repairBaseLines = new RepairBaseLines(Current.Document, Client);
                repairBaseLines.Owner = this;
                repairBaseLines.ShowDialog();

                // GAMMEL IMPLEMENTERING
                //string Question = $"Repair all Base Lines in {Current.Collection.Name} / {Current.Document.Title}?";
                //MessageBoxResult Result = AskUser(Question);

                //if (Result == MessageBoxResult.Yes)
                //{
                //    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                //    Current.Document.RepairBaseLines();
                //    Mouse.OverrideCursor = null;
                //}
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_EditBaseLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                TrDialogTransferSettings settings = new TrDialogTransferSettings();
                EditBaseLines DlgBaseLines = new EditBaseLines(Current.Document, Client, settings);
                DlgBaseLines.Owner = this;
                DlgBaseLines.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ShowStructuralTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowTags showTags = new ShowTags("Structural Tags in Document:");
                showTags.Owner = this;
                showTags.lstTags.ItemsSource = Current.Document.GetStructuralTags();
                showTags.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_EditStructuralTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                if (Current.Document.HasRegions)
                {
                    EditStructuralTags editTags = new EditStructuralTags(Current.Document, Client);

                    // EditTags.cmbRegion.ItemsSource = Current.Document.GetListOfPossibleRegions();
                    editTags.Owner = this;
                    editTags.ShowDialog();
                }
                else
                {
                    TellUser("The current document has no text regions!");
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_TagLinesByPosition_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                FilterLinesByLocation filterLines = new FilterLinesByLocation(Current.Document, Client);
                filterLines.Owner = this;
                filterLines.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ExportWords_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {   // TrLibrary.ExportFolder +
                string fileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                + "Words_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";

                using (StreamWriter textFile = new StreamWriter(fileName, true))
                {
                    textFile.WriteLine("Words from " + Current.Collection.Name + " - " + Current.Document.Title);
                    List<string> docWords = Current.Document.GetExpandedWords(false, false);

                    foreach (string s in docWords)
                    {
                        textFile.WriteLine(s);
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_CountPagesWithRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                int count = Current.Document.NrOfPagesWithRegions();
                string message = $"Document {Current.Document.Title} has {count} pages (out of {Current.Document.NrOfPages}) with regions: \n\n"
                    + Current.Document.GetListOfPagesWithRegions();
                MessageBox.Show(message, TrLibrary.AppName, MessageBoxButton.OK);
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        public MessageBoxResult AskUser(string question)
        {
            MessageBoxResult result = MessageBox.Show(question, TrLibrary.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result;
        }

        public void TellUser(string information)
        {
            MessageBox.Show(information, TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void MenuItem_ListPagesWORegionalTags_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Current.Document.Title + ": pages " + Current.Document.GetListOfPagesWithoutRegionalTags() + " are missing regional tags.");
        }

        private void MenuItem_ListPagesWithOverlappingRegions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Current.Document.Title + ": pages " + Current.Document.GetListOfPagesWithOverlappingRegions() + " have overlapping regions.");
        }

        private void MenuItem_ShowTextualTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowTags showTags = new ShowTags("ALL Tags in Document:");
                showTags.Owner = this;
                showTags.lstTags.ItemsSource = Current.Document.GetTextualTags();
                showTags.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ShowExpandedText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowTags showTags = new ShowTags("Expanded text:");
                showTags.Owner = this;
                showTags.lstTags.ItemsSource = Current.Document.GetExpandedText(true, false);
                showTags.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_FindAndReplaceText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                FindReplace findReplace = new FindReplace(Current.Document, Client);
                findReplace.Owner = this;
                findReplace.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ShowParagraphs_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowTags showTags = new ShowTags("Paragraphs:");
                showTags.Owner = this;
                showTags.lstTags.ItemsSource = Current.Document.GetExpandedText(true, false);
                showTags.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void LstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstPages.SelectedItem != null)
            {
                Current.Page = lstPages.SelectedItem as TrPage;
                ShowParagraphs showParagraphs = new ShowParagraphs(Current.Page);
                showParagraphs.Owner = this;
                showParagraphs.ShowDialog();
            }
        }

        //private void LstSecondaryPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ListBox lb = sender as ListBox;
        //    if (lstSecondaryPages.SelectedItem != null)
        //    {
        //        Secondary.Page = lstSecondaryPages.SelectedItem as TrPage;
        //        ShowParagraphs showParagraphs = new ShowParagraphs(Secondary.Page);
        //        showParagraphs.Owner = this;
        //        showParagraphs.ShowDialog();
        //    }
        //}

        private void MenuItem_ShowAndExportPages_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowAndExportPages showExportPages = new ShowAndExportPages(Current.Document, Client);
                showExportPages.Owner = this;
                showExportPages.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ExportAsPlainText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {   // TrLibrary.ExportFolder +
                string fileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                    + "PlainText_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";
                using (StreamWriter textFile = new StreamWriter(fileName, true))
                {
                    textFile.WriteLine("Plain text from " + Current.Collection.Name + " - " + Current.Document.Title);

                    List<string> lines = new List<string>();
                    foreach (TrPage p in Current.Document.Pages)
                    {
                        textFile.WriteLine("------------------------------------------------------------------------------------");
                        textFile.WriteLine("Page nr. " + p.PageNr.ToString());

                        lines = p.GetExpandedText(true, false);
                        foreach (string s in lines)
                        {
                            textFile.WriteLine(s);
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private async void MenuItem_LoadAllDocsInCurrentCollection_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                foreach (TrDocument doc in Current.Collection.Documents)
                {
                    // Current.Document = Doc;
                    ProgressLoadPages progress = new ProgressLoadPages(doc.NrOfPages);
                    progress.Owner = this;
                    progress.DataContext = doc;
                    progress.Show();

                    // fylder box op
                    // lstPages.ItemsSource = Current.Document.Pages;

                    // henter nyeste transcript for hver side
                    if (TrLibrary.OfflineMode)
                    {
                        // doc.OpenPages();
                    }
                    else
                    {
                        foreach (TrPage page in doc.Pages)
                        {
                            Task<bool> loaded = page.Transcripts[0].LoadTranscript(Client);
                            bool oK = await loaded;
                        }
                    }

                    progress.Hide();
                    Mouse.OverrideCursor = null;
                }
            }
            else
            {
                TellUser("You have to choose a collection !");
            }
        }

        //private void MenuItem_SetRowNumbers_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Current.Collection != null && Current.Document != null)
        //    {
        //        Current.Document.SetRowNumbers(46);
        //    }
        //    else
        //        TellUser("You have to choose a collection AND a document!");

        //}

        //private void MenuItem_SetColumnNumbers_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Current.Collection != null && Current.Document != null)
        //    {
        //        Current.Document.SetColumnNumbers(4);
        //    }
        //    else
        //        TellUser("You have to choose a collection AND a document!");

        //}
        private void MenuItem_MoveRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                MoveRegions moveRegions = new MoveRegions(Current.Document, Client);
                moveRegions.Owner = this;
                moveRegions.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void BtnCopyDocument_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null && Secondary.Collection != null && Secondary.Document != null)
            {
                string question = $"Copy content from \n{Secondary.Collection.Name} / {Secondary.Document.Title} \nto\n{Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    CopyFromSecondaryDocument(Secondary.Document, Current.Document);
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document in both primary and secondary!");
            }
        }

        private void BtnCopyCollection_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Secondary.Collection != null)
            {
                string question = $"Copy content from ALL documents in \n{Secondary.Collection.Name} \nto\n{Current.Collection.Name}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrDocument currentDoc in Current.Collection.Documents)
                    {
                        // currentDoc.OpenPages();
                    }

                    foreach (TrDocument secondaryDoc in Secondary.Collection.Documents)
                    {
                        // secondaryDoc.OpenPages();
                    }

                    CopyFromSecondaryCollection(Secondary.Collection, Current.Collection);
                }
            }
            else
            {
                TellUser("You have to choose a collection in both primary and secondary!");
            }
        }

        public void CopyFromSecondaryDocument(TrDocument sourceDocument, TrDocument destinationDocument)
        {
            Debug.WriteLine($"*** CopyFromSecondaryDocument ***");
            foreach (TrPage sourcePage in sourceDocument.Pages)
            {
                string sourceFileName = sourcePage.ImageFileName;

                // Debug.WriteLine($"SourceFileName: {SourceFileName}");
                foreach (TrPage destinationPage in destinationDocument.Pages)
                {
                    string destinationFileName = destinationPage.ImageFileName;

                    // Debug.WriteLine($"DestinationFileName: {DestinationFileName}");
                    if (sourceFileName == destinationFileName)
                    {
                        Debug.WriteLine($"Filename match!!! (page nr. {destinationPage.PageNr}): {destinationPage.ImageFileName}");
                        TrRegions sourceRegions = sourcePage.Transcripts[0].Regions;

                        foreach (TrRegion sourceRegion in sourceRegions)
                        {
                            // Debug.WriteLine($"Sourceregion # {SourceRegion.Number}");
                            destinationPage.AppendRegion(sourceRegion);
                        }
                    }
                }
            }
        }

        public void CopyFromSecondaryCollection(TrCollection sourceColl, TrCollection destinationColl)
        {
            Debug.WriteLine($"*** CopyFromSecondaryCollection ***");
            foreach (TrDocument sourceDocument in sourceColl.Documents)
            {
                string sourceDocumentName = sourceDocument.Title;

                // Debug.WriteLine($"SourceDocumentName: {SourceDocumentName}");
                {
                    foreach (TrDocument destinationDocument in destinationColl.Documents)
                    {
                        string destinationDocumentName = destinationDocument.Title;

                        // Debug.WriteLine($"DestinationDocumentName: {DestinationDocumentName}");
                        if (sourceDocumentName == destinationDocumentName)
                        {
                            Debug.WriteLine($"Document match!!! Title: {destinationDocumentName}");

                            CopyFromSecondaryDocument(sourceDocument, destinationDocument);
                        }
                    }
                }
            }
        }

        private void MenuItem_SaveCurrentDocument_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                Current.Document.Save();
            }
        }

        private void MenuItem_SaveCurrentCollection_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null)
            {
                foreach (TrDocument doc in Current.Collection.Documents)
                {
                    doc.Save();
                }
            }
        }

        private void MenuItem_ExpandAllText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                foreach (TrPage p in Current.Document.Pages)
                {
                    p.KOBACC_ExpandText();
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoTagging_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                foreach (TrPage p in Current.Document.Pages)
                {
                    p.Elfelt_AutoTag();
                }

                // P.KOBACC_AutoTag();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void AddRegionalTagsOnEmpty_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Add <_NoTag> tag to all non-tagged regions in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage tP in Current.Document.Pages)
                    {
                        tP.Transcripts[0].SetRegionalTagsOnNonTaggedRegions("_NoTag");
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_RenumberRegionsHorizontally_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Renumber all regions horizontally in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage tP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        tP.RenumberRegionsHorizontally();
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_RenumberRegionsVertically_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Renumber all regions vertically in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage tP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        tP.RenumberRegionsVertically();
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        //private void MenuItem_GetAllAccNos_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Current.Collection != null) // && Current.Document != null)
        //    {
        //        string question = $"Get (export) all ACC-Nos in {Current.Collection.Name} ?"; // / {Current.Document.Title}
        //        MessageBoxResult result = AskUser(question);
        //        if (result == MessageBoxResult.Yes)
        //        {
        //            XDocument xAccessions = Current.Collection.KOBACC_ExportAccessions(); // Current.Document.KOBACC_ExportAccessions();

        //            // TrLibrary.ExportFolder +
        //            string fileName = @"AccNos\" + "Accessions_" + Current.Collection.Name + ".xml";

        //            xAccessions.Save(fileName);
        //        }
        //    }
        //    else
        //    {
        //        TellUser("You have to choose a collection!"); // AND a document
        //    }
        //}

        private void MenuItem_SimplifyBoundingBoxes_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                SimplifyBoundingBoxes simplifyBoundingBoxes = new SimplifyBoundingBoxes(Current.Document, Client);
                simplifyBoundingBoxes.Owner = this;
                simplifyBoundingBoxes.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_RenumberLinesHorizontally_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Renumber all lines horizontally in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage tP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        foreach (TrTextRegion textRegion in tP.Transcripts[0].Regions)
                        {
                            if (textRegion.GetType() == typeof(TrTextRegion))
                            {
                                (textRegion as TrTextRegion).TextLines.ReNumberHorizontally();
                            }
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_RenumberLinesVertically_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Renumber all lines vertically in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage tP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        foreach (TrRegion textRegion in tP.Transcripts[0].Regions)
                        {
                            if (textRegion.GetType() == typeof(TrTextRegion))
                            {
                                (textRegion as TrTextRegion).TextLines.ReNumberVertically();
                            }
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_RenumberLinesLogically_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Renumber all lines logically in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    ChoosePageRange choosePageRange = new ChoosePageRange(Current.Document);
                    choosePageRange.Owner = this;
                    choosePageRange.ShowDialog();

                    if (choosePageRange.DialogResult == true)
                    {
                        int startPage = choosePageRange.StartPage;
                        int endPage = choosePageRange.EndPage;
                        int linePixelLimit = 40;    // 60 er bedst til Elfelt - måske til håndskrift generelt. Og mindre til maskinskrift?
                        foreach (TrPage tP in Current.Document.Pages)
                        {
                            if (tP.PageNr >= startPage && tP.PageNr <= endPage)
                            {
                                Debug.WriteLine($"Page nr. {tP.PageNr}");
                                foreach (TrRegion textRegion in tP.Transcripts[0].Regions)
                                {
                                    if (textRegion.GetType() == typeof(TrTextRegion))
                                    {
                                        (textRegion as TrTextRegion).TextLines.ReNumberLogically(linePixelLimit);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ShowImages_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                TrPage testPage = Current.Document.Pages[1];

                ShowPage showPage = new ShowPage(testPage, Client);
                showPage.Owner = this;
                showPage.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ShowLemmas_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowLemmas showLemmas = new ShowLemmas(Current.Document);
                showLemmas.Owner = this;
                showLemmas.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_DeleteLines_Click(object sender, RoutedEventArgs e)
        {
            string chosenStructuralTag;
            DeleteAction deleteAction;
            if (Current.Collection != null && Current.Document != null)
            {
                DeleteLines deleteLines = new DeleteLines();
                deleteLines.Owner = this;
                deleteLines.txtStructuralTag.ItemsSource = Current.Document.GetStructuralTags();
                deleteLines.ShowDialog();
                if (deleteLines.DialogResult == true)
                {
                    chosenStructuralTag = deleteLines.TagName;
                    deleteAction = deleteLines.DeleteAction;

                    if (deleteAction == DeleteAction.Preserve)
                    {
                        string question = $"Delete Other Lines Than \"{chosenStructuralTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult result = AskUser(question);

                        if (result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Preserve chosen to be: {ChosenStructuralTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                            Current.Document.DeleteLinesOtherThan(chosenStructuralTag);

                            Mouse.OverrideCursor = null;
                        }
                    }
                    else if (deleteAction == DeleteAction.Delete)
                    {
                        string question = $"Delete Lines With Tag \"{chosenStructuralTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult result = AskUser(question);

                        if (result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Deleten chosen to be: {ChosenStructuralTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                            Current.Document.DeleteLinesWithTag(chosenStructuralTag);

                            Mouse.OverrideCursor = null;
                        }
                    }

                    // nu kan nogle regioner være bleet tomme - derfor kalder vi DeleteEmptyRegions:
                    Current.Document.DeleteEmptyRegions();
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_DeleteEmptyRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Delete Empty Regions in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    Current.Document.DeleteEmptyRegions();
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_CheckTableSituation_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                foreach (TrPage tP in Current.Document.Pages)
                {
                    Debug.Print($"Page # {tP.PageNr} : Current tables? {tP.HasTables.ToString()} - Former tables? {tP.HasFormerTables.ToString()}");
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ConvertTablesToTextRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                if (Current.Document.HasTables)
                {
                    string question = $"Convert tables to text regions in {Current.Collection.Name} / {Current.Document.Title}?";
                    MessageBoxResult result = AskUser(question);
                    if (result == MessageBoxResult.Yes)
                    {
                        ChoosePageRange choosePageRange = new ChoosePageRange(Current.Document);
                        choosePageRange.Owner = this;
                        choosePageRange.ShowDialog();

                        if (choosePageRange.DialogResult == true)
                        {
                            Current.Document.ConvertTablesToRegions(choosePageRange.StartPage, choosePageRange.EndPage);
                        }
                    }
                }
                else
                {
                    TellUser("The current document has no tables!");
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_CopyForgottenTables_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                if (Current.Document.HasFormerTables)
                {
                    string question = $"Copy tables from former transcripts in {Current.Collection.Name} / {Current.Document.Title}?";
                    MessageBoxResult result = AskUser(question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Current.Document.CopyOldTablesToNewestTranscript();
                    }
                }
                else
                {
                    TellUser("The current document has no old tables!");
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_MergeAllToTopLevel_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Merge All Regions to Top Level Region in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    ChoosePageRange choosePageRange = new ChoosePageRange(Current.Document);
                    choosePageRange.Owner = this;
                    choosePageRange.ShowDialog();

                    if (choosePageRange.DialogResult == true)
                    {
                        Current.Document.MergeAllRegionsToTopLevelRegion(choosePageRange.StartPage, choosePageRange.EndPage);
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoAddAbbrevTags_Repetitions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag with <abbrev> at Repetitions in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage p in Current.Document.Pages)
                    {
                        p.AutoAbbrevTagRepetitions();
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoTagRomanNumerals_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag roman numerals in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage p in Current.Document.Pages)
                    {
                        p.AutoTagRomanNumerals();
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ExportPseudoTables_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                int minimumRecordNumber = 0;

                ChooseMinMaxNumbers choose = new ChooseMinMaxNumbers();
                choose.Owner = this;
                choose.ShowDialog();

                if (choose.DialogResult == true)
                {
                    if (choose.Minimum != 0)
                    {
                        minimumRecordNumber = choose.Minimum;
                    }

                    // TrLibrary.ExportFolder +
                    string fileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                        + "PseudoTableRecords_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".csv";
                    using (StreamWriter textFile = new StreamWriter(fileName, true, Encoding.UTF8))
                    {
                        Debug.Print($"Exporting pseudo table text from {Current.Collection.Name} - {Current.Document.Title} - Minimum = {minimumRecordNumber}");

                        List<TrRecord> pageRecords = Current.Document.GetPseudoTableText(minimumRecordNumber);

                        Debug.Print($"Result: {pageRecords.Count} records in this doc! Writing to file.");

                        foreach (TrRecord pageRec in pageRecords)
                        {
                            textFile.WriteLine(pageRec.ToString());
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoAddDateTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag with <date> in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    int left = 0;
                    int right = 30;
                    Current.Document.AutoDateTag(left, right);
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_TagEmptyTextLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Tag empty textlines in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    Current.Document.TagEmptyTextLines();
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_MarkEmptyAbbrevTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Mark (tag) empty abbrev-tags in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    Current.Document.TagEmptyAbbrevTags();
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoAddRecordTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag with structural tag <RecordName> in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    double left = 70;
                    double right = 100;
                    Current.Document.AutoRecordTag(left, right);
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_ElfeltRecordCheck_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Check lines with structural tag <RecordName> in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    // TrLibrary.ExportFolder +
                    string fileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                        + "ElfeltRecordCheck_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".csv";
                    using (StreamWriter textFile = new StreamWriter(fileName, true, Encoding.UTF8))
                    {
                        // TextFile.WriteLine("Pseudo table text from " + Current.Collection.Name + " - " + Current.Document.Title);
                        List<string> records = Current.Document.CheckElfeltRecordNumbers();

                        foreach (string record in records)
                        {
                            textFile.WriteLine(record);
                        }
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_WrapSuperAndSubscriptWithSpaces_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Wrap super- and subscript with spaces in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    Current.Document.WrapSuperAndSubscriptWithSpaces();
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_CurrentTestModule_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Check what-ever :) ... in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    Regex numbers = new Regex(@"\d{4,}\p{L}?");
                    Regex yearOnly = new Regex(@"^\d{4}(?!(\-|\d))|^\d{4}$");

                    foreach (TrPage p in Current.Document.Pages)
                    {
                        //if (P.PageNr >= 3 && P.PageNr <= 10)
                        {
                            Debug.Print($"Page# {p.PageNr} ----------------------------------------------------");

                            //bool RegionOK;
                            //int i = 0;
                            foreach (TrRegion textRegion in p.Transcripts[0].Regions)
                            {
                                if (textRegion.GetType() == typeof(TrTextRegion))
                                {
                                    foreach (TrTextLine textLine in (textRegion as TrTextRegion).TextLines)
                                    {
                                        //MatchCollection NumberMatches = Numbers.Matches(TL.ExpandedText);
                                        MatchCollection yearOnlyMatches = yearOnly.Matches(textLine.ExpandedText);
                                        if (yearOnlyMatches.Count > 0)

                                        //if (NumberMatches.Count > 0)
                                        {
                                            int i = 0;
                                            int digitValue = 0;
                                            foreach (Match m in yearOnlyMatches)

                                            //foreach (Match M in NumberMatches)
                                            {
                                                string content = ClsLanguageLibrary.StripPunctuation(m.Value);
                                                if (ClsLanguageLibrary.IsNumeric(content))
                                                {
                                                    digitValue = Convert.ToInt32(content);
                                                }

                                                //char LastChar = M.Value[M.Value.Length - 1];
                                                //int DigitValue;

                                                //if (!char.IsLetter(LastChar))
                                                //{
                                                //    // der er IKKE et bogstav i enden
                                                //    DigitValue = Convert.ToInt32(M.Value);
                                                //}
                                                //else
                                                //{
                                                //    // der ER et bogstav i enden
                                                //    //string Letter = NumberMatches[0].Value.Last().ToString();
                                                //    int DigitCount = M.Value.Length - 1;
                                                //    DigitValue = Convert.ToInt32(M.Value.Substring(0, DigitCount));

                                                //}
                                                if (digitValue < 2000)
                                                {
                                                    i++;
                                                    Debug.WriteLine($"Line# {textLine.Number}: Match# {i}: found {content}");
                                                }
                                            }

                                            //if (Found)
                                            //{
                                            //    if (!TL.HasSpecificStructuralTag("RecordName"))
                                            //        Debug.WriteLine($" NO TAG!");
                                            //    else
                                            //        Debug.WriteLine($"");

                                            //}
                                        }
                                    }

                                    //do
                                    //{
                                    //    (TR as TrRegion_Text).TextLines.TestSort();
                                    //    i++;
                                    //    Debug.Print($"Iteration # {i} --------------------------");
                                    //    RegionOK = true;
                                    //    bool OK = true;

                                    //    foreach (TrTextLine TL in (TR as TrRegion_Text).TextLines)
                                    //    {
                                    //        if (TL.Next != null)
                                    //        {
                                    //            int DeltaX = TL.Next.Hpos - TL.Hpos;
                                    //            int DeltaY = TL.Next.Vpos - TL.Vpos;

                                    //            if (DeltaX < 0 && DeltaY < 50)
                                    //            {
                                    //                Debug.Print($"Looks wrong between line# {TL.Number} and {TL.Next.Number}: DeltaX = {DeltaX}, DeltaY = {DeltaY}, SortDeltaY = {TL.SortDeltaY}, NextSortDeltaY = {TL.Next.SortDeltaY} ");
                                    //                TL.SortDeltaY += 20;
                                    //                TL.Next.SortDeltaY -= 20;
                                    //                OK = false;
                                    //            }
                                    //            else
                                    //            {
                                    //                OK = true;
                                    //            }
                                    //        }
                                    //        RegionOK = RegionOK && OK;

                                    //    }

                                    //}
                                    //while (!RegionOK);
                                }
                            }
                        }

                        //foreach (TrRegion TR in P.Transcripts[0].Regions)
                        //{
                        //    //Debug.Print($"Region# {TR.Number} ----------");
                        //    if (TR.GetType() == typeof(TrRegion_Text))
                        //    {
                        //        (TR as TrRegion_Text).TextLines.TestSort();

                        //        //foreach (TrTextLine TL in (TR as TrRegion_Text).TextLines)
                        //        //{
                        //        //    //if (TL.PercentualHendPos > 75 && !TL.HasSpecificStructuralTag("RecordName"))
                        //        //    //{
                        //        //    //    if (clsLanguageLibrary.ConsecutiveDigitCount(TL.ExpandedText) >= 4)
                        //        //    //        Debug.Print($"Page# {P.PageNr}, Line# {TL.Number}, Content = {TL.ExpandedText} ");
                        //        //    //}
                        //        //    Debug.Print($"Line# {TL.Number} ---------- Angle: {TL.AngleFromOrigo} Content: {TL.TextEquiv} ----- Length: {TL.Length}");
                        //        //    //Debug.Print(TL.Tags.ToString());
                        //        //    //TL.Replace("Frk", "Frk.");
                        //        //    //TL.Tags.Move(3, 1, false);
                        //        //    //TL.WrapSuperAndSubscriptWithSpaces();
                        //        //} // end TEXTLINE

                        //    } // end IF TEXTREGION
                        //} // end REGION
                    } // end PAGE

                    //double averageHpos = SumHpos / (double)LineCount;
                    //double averageHendPos = SumHendPos / (double)LineCount;
                    //Debug.Print($"Lines: {LineCount},  (only counting lines with structural tag!)");
                    //Debug.Print($"avg.Hpos: { averageHpos}, avg.HendPos: { averageHendPos}");
                    //Debug.Print($"min. Hpos {minHpos},  max. Hpos {maxHpos}");
                    //Debug.Print($"min. Hendpos {minHendPos},  max. Hendpos {maxHendPos}");
                } // end DO IT
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_TagLinesByRegex_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                FilterLinesByRegex filterLines = new FilterLinesByRegex(Current.Document, Client);
                filterLines.Owner = this;
                filterLines.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_FilterLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                FilterLines filterLines = new FilterLines(Current.Document);
                filterLines.Owner = this;
                filterLines.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoAddAbbrevTags_NumericIntervals_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag with <abbrev> at numeric intervals in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    string tagName = "RecordName";

                    // vælg hvilket struct-tag, der skal bestemme det
                    foreach (TrPage p in Current.Document.Pages)
                    {
                        p.AutoAbbrevTagNumericIntervals(tagName);
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoAddAbbrevTags_PlaceNames_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag with <abbrev> in place names in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage p in Current.Document.Pages)
                    {
                        p.AutoAbbrevTagPlaceNames();
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        private void MenuItem_AutoTagFloorNumbers_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string question = $"Auto-Tag floor numbers with superscript in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult result = AskUser(question);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (TrPage p in Current.Document.Pages)
                    {
                        p.AutoTagFloorNumberSuperScript();
                    }
                }
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }

        // Histogram
        private void MenuItem_ShowHistogram_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                ShowHistogram histogram = new ShowHistogram(Current.Document);
                histogram.Owner = this;
                histogram.ShowDialog();
            }
            else
            {
                TellUser("You have to choose a collection AND a document!");
            }
        }
    }
}
