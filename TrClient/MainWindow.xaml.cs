using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Timers;
using System.ComponentModel;
using System.Text.RegularExpressions;
using TrClient;
using TrClient.Core;
using TrClient.Dialog;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;
using DanishNLP;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum deleteAction
        {
            delete = 0,
            preserve = 1
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
                    using (var Stream = File.OpenRead("User.xml"))
                    {
                        var Serializer = new XmlSerializer(typeof(TrUser));
                        User = Serializer.Deserialize(Stream) as TrUser;
                    }
                }
                else
                    User = new TrUser();
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
        public async void RunLoginAndGetMyCollections(string Username, string Password)
        {
            // Kaldes KUN i online-mode!!

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            var Credentials = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", Username),
                    new KeyValuePair<string, string>("pw", Password),
                });

            try
            {
                HttpResponseMessage LoginResponseMessage = await Client.PostAsync(TrLibrary.TrpLogin, Credentials);

                string LoginResponse = LoginResponseMessage.StatusCode.ToString();

                bool Status = (LoginResponse == "OK");
                if (Status)
                {
                    // Henter brugerens collections ind i et XMLdoc
                    HttpResponseMessage CollectionsResponseMessage = await Client.GetAsync(TrLibrary.TrpCollections);
                    string CollectionsResponse = await CollectionsResponseMessage.Content.ReadAsStringAsync();
                    MyCollectionsDocument.LoadXml(CollectionsResponse);

                    // Udtrækker de enkelte collections
                    XmlNodeList CollectionNodes = MyCollectionsDocument.DocumentElement.SelectNodes("//trpCollection");
                    foreach (XmlNode xnCollection in CollectionNodes)
                    {
                        XmlNodeList CollectionMetaData = xnCollection.ChildNodes;
                        string ColID = "";
                        string ColName = "";
                        int NrOfDocs = 0;

                        foreach (XmlNode xnCollectionMetaData in CollectionMetaData)
                        {
                            string Name = xnCollectionMetaData.Name;
                            string Value = xnCollectionMetaData.InnerText;

                            switch (Name)
                            {
                                case "colId":
                                    ColID = Value;
                                    break;
                                case "colName":
                                    ColName = Value;
                                    break;
                                case "nrOfDocuments":
                                    NrOfDocs = Int32.Parse(Value);
                                    break;
                            }
                        }
                        TrCollection Coll = new TrCollection(ColName, ColID, NrOfDocs);
                        MyCollections.Add(Coll);
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
                    using (var Stream = File.Open("User.xml", FileMode.Create))
                    {
                        var Serializer = new XmlSerializer(typeof(TrUser));
                        Serializer.Serialize(Stream, User);
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
                    string Question = $"{Current.Collection.Name} has changed. Upload changes?";
                    MessageBoxResult Result = AskUser(Question);

                    if (Result == MessageBoxResult.Yes)
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

                Current.Collection = (lstCollections.SelectedItem as TrCollection);
                txtCurrentCollection.DataContext = Current.Collection;
                txtCurrentDocument.DataContext = Current.Document;

                dlgProgressLoadDocs Progress = new dlgProgressLoadDocs(Current.Collection.NrOfDocs);
                Progress.Owner = this;
                Progress.DataContext = Current.Collection;
                Progress.Show();

                if (TrLibrary.OfflineMode)
                {
                    Current.Collection.OpenDocuments();
                }
                else
                {
                    Task<bool> Loaded = Current.Collection.LoadDocuments(Client);
                    bool OK = await Loaded;
                }

                // fylder box op
                lstDocuments.ItemsSource = Current.Collection.Documents;

                if (!TrLibrary.OfflineMode)
                {
                    // henter sider
                    foreach (TrDocument Doc in Current.Collection.Documents)
                    {
                        try
                        {
                            Task<bool> PagesLoaded = Doc.LoadPages(Client);
                            bool PagesOK = await PagesLoaded;
                        }
                        catch (System.Threading.Tasks.TaskCanceledException eDocLoaded)
                        {
                            Debug.WriteLine($"Exception message: {eDocLoaded.Message}");
                        }
                    }

                }

                Progress.Hide();

                Mouse.OverrideCursor = null;
            }
        }

        private async void LstSecondaryCollections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                ListBox lb = sender as ListBox;
                if (lstSecondaryCollections.SelectedItem != null)
                {
                    if (Secondary.Collection != null && Secondary.Collection.HasChanged)
                    {
                        string Question = $"{Secondary.Collection.Name} has changed. Upload changes?";
                        MessageBoxResult Result = AskUser(Question);

                        if (Result == MessageBoxResult.Yes)
                        {
                            if (TrLibrary.OfflineMode)
                            {

                            }
                            else
                            {
                                Secondary.Collection.Upload(Client);
                            }
                        }
                    }
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;


                    // rydder op
                    lstSecondaryDocuments.ItemsSource = null;
                    lstSecondaryPages.ItemsSource = null;
                    Secondary.Document = null;

                    Secondary.Collection = (lstSecondaryCollections.SelectedItem as TrCollection);
                    txtSecondaryCollection.DataContext = Secondary.Collection;
                    txtSecondaryDocument.DataContext = Secondary.Document;

                    dlgProgressLoadDocs Progress = new dlgProgressLoadDocs(Secondary.Collection.NrOfDocs);
                    Progress.Owner = this;
                    Progress.DataContext = Secondary.Collection;
                    Progress.Show();

                    if (TrLibrary.OfflineMode)
                    {
                        Secondary.Collection.OpenDocuments();
                    }
                    else
                    {
                        Task<bool> Loaded = Secondary.Collection.LoadDocuments(Client);
                        bool OK = await Loaded;
                    }

                    // fylder box op
                    lstSecondaryDocuments.ItemsSource = Secondary.Collection.Documents;

                    if (!TrLibrary.OfflineMode)
                    {
                        // henter sider
                        foreach (TrDocument Doc in Secondary.Collection.Documents)
                        {
                            try
                            {
                                Task<bool> PagesLoaded = Doc.LoadPages(Client);
                                bool PagesOK = await PagesLoaded;
                            }
                            catch (System.Threading.Tasks.TaskCanceledException eDocLoaded)
                            {
                                Debug.WriteLine($"Exception message: {eDocLoaded.Message}");
                            }
                        }

                    }

                    Progress.Hide();

                    Mouse.OverrideCursor = null;
                }

            }

        }

        // Dokument valgt
        private async void LstDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstDocuments.SelectedItem != null)
            {
                if (Current.Document != null && Current.Document.HasChanged)
                {
                    string Question = $"{Current.Document.Title} has changed. Upload changes?";
                    MessageBoxResult Result = AskUser(Question);

                    if (Result == MessageBoxResult.Yes)
                        if (TrLibrary.OfflineMode)
                        {
                            Current.Document.Save();
                        }
                        else
                        {
                            Current.Document.Upload(Client);
                        }
                }

                // rydder op
                lstPages.ItemsSource = null;

                Current.Document = (lstDocuments.SelectedItem as TrDocument);
                Debug.WriteLine($"Current.Document er valgt: ID = {Current.Document.ID}, Title = {Current.Document.Title}, Pages = {Current.Document.NrOfPages}");

                txtCurrentDocument.DataContext = Current.Document;

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                dlgProgressLoadPages Progress = new dlgProgressLoadPages(Current.Document.NrOfPages);
                Progress.Owner = this;
                Progress.DataContext = Current.Document;
                Progress.Show();

                if (TrLibrary.OfflineMode)
                {
                    // OFFLINE MODE
                    Current.Document.OpenPages();
                    //Debug.WriteLine($"OpenPages er kaldt! Current.Document.Title = {Current.Document.Title}");

                    // fylder box op
                    lstPages.ItemsSource = Current.Document.Pages;
                }
                else
                {   // ONLINE MODE
                    // fylder box op
                    lstPages.ItemsSource = Current.Document.Pages;

                    // henter transcripts for hver side - NB: ALLE transcripts (vha. i = 0 to transcript.count - 1)
                    foreach (TrPage Page in Current.Document.Pages)
                    {
                        // TrTranscript Tra;
                        if (TrLibrary.LoadOnlyNewestTranscript)
                        {
                            // Henter kun det NYESTE transcript
                            Task<bool> Loaded = Page.Transcripts[0].LoadTranscript(Client);
                            bool OK = await Loaded;
                        }
                        else
                        {
                            // Henter ALLE transcripts - hvis man har brug for at reverte eller hente gamle tabeller
                            for (int i = 0; i < Page.TranscriptCount; i++)
                            {
                                Task<bool> Loaded = Page.Transcripts[i].LoadTranscript(Client);
                                bool OK = await Loaded;
                            }
                        }


                    }
                }
                Progress.Hide();
                Mouse.OverrideCursor = null;

                if (Current.Document.HasFormerTables)
                {
                    string Question = $"WARNING! {Current.Document.Title} has possibly 'forgotten' tables in old transcripts. You should copy tables to newest transcript and convert them to ordinary regions!";
                    TellUser(Question);
                }

                if (!Current.Document.PostLoadTestOK())
                {
                    string Question = $"WARNING! {Current.Document.Title} has problems with coordinates and/or non-trimmed strings! Do you want to fix it?";
                    MessageBoxResult Result = AskUser(Question);
                    if (Result == MessageBoxResult.Yes)
                        Current.Document.PostLoadFix();
                }
            }
        }

        private async void LstSecondaryDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstSecondaryDocuments.SelectedItem != null)
            {
                if (Secondary.Document != null && Secondary.Document.HasChanged)
                {
                    string Question = $"{Secondary.Document.Title} has changed. Upload changes?";
                    MessageBoxResult Result = AskUser(Question);

                    if (Result == MessageBoxResult.Yes)
                        if (TrLibrary.OfflineMode)
                        {

                        }
                        else
                        {
                            Secondary.Document.Upload(Client);
                        }
                }

                // rydder op
                lstSecondaryPages.ItemsSource = null;

                Secondary.Document = (lstSecondaryDocuments.SelectedItem as TrDocument);
                Debug.WriteLine($"Secondary.Document er valgt: Title = {Secondary.Document.Title}, Pages = {Secondary.Document.NrOfPages}");

                txtSecondaryDocument.DataContext = Secondary.Document;

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                dlgProgressLoadPages Progress = new dlgProgressLoadPages(Secondary.Document.NrOfPages);
                Progress.Owner = this;
                Progress.DataContext = Secondary.Document;
                Progress.Show();

                if (TrLibrary.OfflineMode)
                {
                    Secondary.Document.OpenPages();
                    //Debug.WriteLine($"OpenPages er kaldt! Current.Document.Title = {Current.Document.Title}");

                    // fylder box op
                    lstSecondaryPages.ItemsSource = Secondary.Document.Pages;
                }
                else
                {
                    // fylder box op
                    lstSecondaryPages.ItemsSource = Secondary.Document.Pages;
                    // henter nyeste transcript for hver side - nb: kun NYESTE 
                    foreach (TrPage Page in Secondary.Document.Pages)
                    {
                        Task<bool> Loaded = Page.Transcripts[0].LoadTranscript(Client);
                        bool OK = await Loaded;
                    }
                }
                Progress.Hide();
                Mouse.OverrideCursor = null;
            }

        }

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
                string Question = "Your collections has changed. Upload changes?";
                MessageBoxResult Result = AskUser(Question);

                if (Result == MessageBoxResult.Yes)
                    foreach (TrCollection Coll in MyCollections)
                        Coll.Upload(Client);
            }
            this.Close();
        }


        private void MenuItem_CreateTopLevelRegion_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Create Top-Level Region on all pages in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    Current.Document.CreateTopLevelRegions();
                    Mouse.OverrideCursor = null;
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }


        private void CreateHorizontalRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Create three horizontal regions on all pages in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);

                if (Result == MessageBoxResult.Yes)
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                    Current.Document.AddHorizontalRegions(0, 6, 0, 10);
                    Current.Document.AddHorizontalRegions(6, 36, 10, 10);
                    Current.Document.AddHorizontalRegions(36, 100, 10, 0);

                    Mouse.OverrideCursor = null;
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");


        }

        private void CreateVerticalRegions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ShowRegionalTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowTags ShowTags = new dlgShowTags("Regional Tags in Document:");
                ShowTags.Owner = this;
                ShowTags.lstTags.ItemsSource = Current.Document.GetRegionalTags();
                ShowTags.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }

        private void MenuItem_DeleteRegions_Click(object sender, RoutedEventArgs e)
        {
            string ChosenRegionalTag;
            deleteAction DeleteAction;
            if (Current.Collection != null && Current.Document != null)
            {
                dlgDeleteRegions DeleteRegions = new dlgDeleteRegions();
                DeleteRegions.Owner = this;
                DeleteRegions.txtRegionalTag.ItemsSource = Current.Document.GetRegionalTags();
                DeleteRegions.ShowDialog();
                if (DeleteRegions.DialogResult == true)
                {
                    ChosenRegionalTag = DeleteRegions.TagName;
                    DeleteAction = DeleteRegions.DeleteAction;

                    if (DeleteAction == deleteAction.preserve)
                    {
                        string Question = $"Delete Other Regions Than \"{ChosenRegionalTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult Result = AskUser(Question);

                        if (Result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Preserve chosen to be: {ChosenRegionalTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                            Current.Document.DeleteRegionsOtherThan(ChosenRegionalTag);
                            Mouse.OverrideCursor = null;
                        }

                    }
                    else if (DeleteAction == deleteAction.delete)
                    {
                        string Question = $"Delete Regions With Tag \"{ChosenRegionalTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult Result = AskUser(Question);

                        if (Result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Deleten chosen to be: {ChosenRegionalTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                            Current.Document.DeleteRegionsWithTag(ChosenRegionalTag);
                            Mouse.OverrideCursor = null;
                        }

                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }


        private void MenuItem_RepairBaseLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgRepairBaseLines RepairBaseLines = new dlgRepairBaseLines(Current.Document, Client);
                RepairBaseLines.Owner = this;
                RepairBaseLines.ShowDialog();

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
                TellUser("You have to choose a collection AND a document!");
        }


        private void MenuItem_EditBaseLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                TrDialogTransferSettings Settings = new TrDialogTransferSettings();
                dlgEditBaseLines dlgBaseLines = new dlgEditBaseLines(Current.Document, Client, Settings);
                dlgBaseLines.Owner = this;
                dlgBaseLines.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }


        private void MenuItem_ShowStructuralTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowTags ShowTags = new dlgShowTags("Structural Tags in Document:");
                ShowTags.Owner = this;
                ShowTags.lstTags.ItemsSource = Current.Document.GetStructuralTags();
                ShowTags.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }


        private void MenuItem_EditStructuralTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                if (Current.Document.HasRegions)
                {
                    dlgEditStructuralTags EditTags = new dlgEditStructuralTags(Current.Document, Client);
                    // EditTags.cmbRegion.ItemsSource = Current.Document.GetListOfPossibleRegions();
                    EditTags.Owner = this;
                    EditTags.ShowDialog();
                }
                else
                    TellUser("The current document has no text regions!");
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }

        private void MenuItem_TagLinesByPosition_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgFilterLinesByLocation FilterLines = new dlgFilterLinesByLocation(Current.Document, Client);
                FilterLines.Owner = this;
                FilterLines.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }



        private void MenuItem_ExportWords_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {   // TrLibrary.ExportFolder + 
                string FileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                + "Words_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";


                using (StreamWriter TextFile = new StreamWriter(FileName, true))
                {
                    TextFile.WriteLine("Words from " + Current.Collection.Name + " - " + Current.Document.Title);
                    List<string> DocWords = Current.Document.GetExpandedWords(false, false);

                    foreach (string s in DocWords)
                        TextFile.WriteLine(s);
                }

            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_CountPagesWithRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                int Count = Current.Document.NrOfPagesWithRegions();
                string Message = $"Document {Current.Document.Title} has {Count} pages (out of {Current.Document.NrOfPages}) with regions: \n\n"
                    + Current.Document.GetListOfPagesWithRegions();
                MessageBox.Show(Message, TrLibrary.AppName, MessageBoxButton.OK);
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }



        public MessageBoxResult AskUser(string Question)
        {
            MessageBoxResult Result = MessageBox.Show(Question, TrLibrary.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return Result;
        }

        public void TellUser(string Information)
        {
            MessageBox.Show(Information, TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                dlgShowTags ShowTags = new dlgShowTags("ALL Tags in Document:");
                ShowTags.Owner = this;
                ShowTags.lstTags.ItemsSource = Current.Document.GetTextualTags();
                ShowTags.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_ShowExpandedText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowTags ShowTags = new dlgShowTags("Expanded text:");
                ShowTags.Owner = this;
                ShowTags.lstTags.ItemsSource = Current.Document.GetExpandedText(true, false);
                ShowTags.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_FindAndReplaceText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgFindReplace FindReplace = new dlgFindReplace(Current.Document, Client);
                FindReplace.Owner = this;
                FindReplace.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");


        }

        private void MenuItem_ShowParagraphs_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowTags ShowTags = new dlgShowTags("Paragraphs:");
                ShowTags.Owner = this;
                ShowTags.lstTags.ItemsSource = Current.Document.GetExpandedText(true, false);
                ShowTags.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void LstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstPages.SelectedItem != null)
            {
                Current.Page = (lstPages.SelectedItem as TrPage);
                dlgShowParagraphs ShowParagraphs = new dlgShowParagraphs(Current.Page);
                ShowParagraphs.Owner = this;
                ShowParagraphs.ShowDialog();
            }

        }

        private void LstSecondaryPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lstSecondaryPages.SelectedItem != null)
            {
                Secondary.Page = (lstSecondaryPages.SelectedItem as TrPage);
                dlgShowParagraphs ShowParagraphs = new dlgShowParagraphs(Secondary.Page);
                ShowParagraphs.Owner = this;
                ShowParagraphs.ShowDialog();
            }

        }



        private void MenuItem_ShowAndExportPages_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowAndExportPages ShowExportPages = new dlgShowAndExportPages(Current.Document, Client);
                ShowExportPages.Owner = this;
                ShowExportPages.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }

        private void MenuItem_ExportAsPlainText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {   // TrLibrary.ExportFolder + 
                string FileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                    + "PlainText_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";
                using (StreamWriter TextFile = new StreamWriter(FileName, true))
                {
                    TextFile.WriteLine("Plain text from " + Current.Collection.Name + " - " + Current.Document.Title);

                    List<string> Lines = new List<string>();
                    foreach (TrPage P in Current.Document.Pages)
                    {
                        TextFile.WriteLine("------------------------------------------------------------------------------------");
                        TextFile.WriteLine("Page nr. " + P.PageNr.ToString());

                        Lines = P.GetExpandedText(true, false);
                        foreach (string s in Lines)
                            TextFile.WriteLine(s);
                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }

        private async void MenuItem_LoadAllDocsInCurrentCollection_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                foreach (TrDocument Doc in Current.Collection.Documents)
                {
                    // Current.Document = Doc;

                    dlgProgressLoadPages Progress = new dlgProgressLoadPages(Doc.NrOfPages);
                    Progress.Owner = this;
                    Progress.DataContext = Doc;
                    Progress.Show();

                    // fylder box op
                    // lstPages.ItemsSource = Current.Document.Pages;

                    // henter nyeste transcript for hver side
                    if (TrLibrary.OfflineMode)
                    {
                        Doc.OpenPages();
                    }
                    else
                    {
                        foreach (TrPage Page in Doc.Pages)
                        {
                            Task<bool> Loaded = Page.Transcripts[0].LoadTranscript(Client);
                            bool OK = await Loaded;
                        }

                    }


                    Progress.Hide();
                    Mouse.OverrideCursor = null;
                }



            }
            else
                TellUser("You have to choose a collection !");

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
                dlgMoveRegions MoveRegions = new dlgMoveRegions(Current.Document, Client);
                MoveRegions.Owner = this;
                MoveRegions.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }


        private void BtnCopyDocument_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null && Secondary.Collection != null && Secondary.Document != null)
            {
                string Question = $"Copy content from \n{Secondary.Collection.Name} / {Secondary.Document.Title} \nto\n{Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    CopyFromSecondaryDocument(Secondary.Document, Current.Document);
                }
            }
            else
                TellUser("You have to choose a collection AND a document in both primary and secondary!");

        }

        private void BtnCopyCollection_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Secondary.Collection != null)
            {
                string Question = $"Copy content from ALL documents in \n{Secondary.Collection.Name} \nto\n{Current.Collection.Name}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrDocument CurrentDoc in Current.Collection.Documents)
                        CurrentDoc.OpenPages();
                    foreach (TrDocument SecondaryDoc in Secondary.Collection.Documents)
                        SecondaryDoc.OpenPages();


                    CopyFromSecondaryCollection(Secondary.Collection, Current.Collection);
                }
            }
            else
                TellUser("You have to choose a collection in both primary and secondary!");
        }


        public void CopyFromSecondaryDocument(TrDocument SourceDocument, TrDocument DestinationDocument)
        {

            Debug.WriteLine($"*** CopyFromSecondaryDocument ***");
            foreach (TrPage SourcePage in SourceDocument.Pages)
            {
                string SourceFileName = SourcePage.ImageFileName;

                // Debug.WriteLine($"SourceFileName: {SourceFileName}");

                foreach (TrPage DestinationPage in DestinationDocument.Pages)
                {
                    string DestinationFileName = DestinationPage.ImageFileName;
                    // Debug.WriteLine($"DestinationFileName: {DestinationFileName}");

                    if (SourceFileName == DestinationFileName)
                    {
                        Debug.WriteLine($"Filename match!!! (page nr. {DestinationPage.PageNr}): {DestinationPage.ImageFileName}");
                        TrRegions SourceRegions = SourcePage.Transcripts[0].Regions;

                        foreach (TrRegion SourceRegion in SourceRegions)
                        {
                            // Debug.WriteLine($"Sourceregion # {SourceRegion.Number}");
                            DestinationPage.AppendRegion(SourceRegion);
                        }
                    }
                }
            }

        }

        public void CopyFromSecondaryCollection(TrCollection SourceColl, TrCollection DestinationColl)
        {
            Debug.WriteLine($"*** CopyFromSecondaryCollection ***");
            foreach (TrDocument SourceDocument in SourceColl.Documents)
            {
                string SourceDocumentName = SourceDocument.Title;
                // Debug.WriteLine($"SourceDocumentName: {SourceDocumentName}");
                {
                    foreach (TrDocument DestinationDocument in DestinationColl.Documents)
                    {
                        string DestinationDocumentName = DestinationDocument.Title;
                        // Debug.WriteLine($"DestinationDocumentName: {DestinationDocumentName}");

                        if (SourceDocumentName == DestinationDocumentName)
                        {
                            Debug.WriteLine($"Document match!!! Title: {DestinationDocumentName}");

                            CopyFromSecondaryDocument(SourceDocument, DestinationDocument);

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
                foreach (TrDocument Doc in Current.Collection.Documents)
                    Doc.Save();
            }

        }

        private void MenuItem_ExpandAllText_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                foreach (TrPage P in Current.Document.Pages)
                    P.KOBACC_ExpandText();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_AutoTagging_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                foreach (TrPage P in Current.Document.Pages)
                    P.Elfelt_AutoTag();
                // P.KOBACC_AutoTag();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void AddRegionalTagsOnEmpty_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Add <_NoTag> tag to all non-tagged regions in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage TP in Current.Document.Pages)
                    {
                        TP.Transcripts[0].SetRegionalTagsOnNonTaggedRegions("_NoTag");
                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_RenumberRegionsHorizontally_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Renumber all regions horizontally in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage TP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        TP.RenumberRegionsHorizontally();
                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_RenumberRegionsVertically_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Renumber all regions vertically in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage TP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        TP.RenumberRegionsVertically();
                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_GetAllAccNos_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null) // && Current.Document != null)
            {
                string Question = $"Get (export) all ACC-Nos in {Current.Collection.Name} ?"; // / {Current.Document.Title}
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    XDocument xAccessions = Current.Collection.KOBACC_ExportAccessions(); // Current.Document.KOBACC_ExportAccessions();
                    // TrLibrary.ExportFolder + 
                    string FileName = @"AccNos\" + "Accessions_" + Current.Collection.Name + ".xml";

                    xAccessions.Save(FileName);

                }
            }
            else
                TellUser("You have to choose a collection!"); // AND a document

        }

        private void MenuItem_SimplifyBoundingBoxes_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgSimplifyBoundingBoxes SimplifyBoundingBoxes = new dlgSimplifyBoundingBoxes(Current.Document, Client);
                SimplifyBoundingBoxes.Owner = this;
                SimplifyBoundingBoxes.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }


        private void MenuItem_RenumberLinesHorizontally_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Renumber all lines horizontally in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage TP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        foreach (TrRegion_Text TR in TP.Transcripts[0].Regions)
                        {
                            if (TR.GetType() == typeof(TrRegion_Text))
                                (TR as TrRegion_Text).TextLines.ReNumberHorizontally();
                        }

                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_RenumberLinesVertically_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Renumber all lines vertically in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage TP in Current.Document.Pages)
                    {
                        // Debug.WriteLine($"Page nr. {TP.PageNr}");
                        foreach (TrRegion TR in TP.Transcripts[0].Regions)
                        {
                            if (TR.GetType() == typeof(TrRegion_Text))
                                (TR as TrRegion_Text).TextLines.ReNumberVertically();
                        }

                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_RenumberLinesLogically_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Renumber all lines logically in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    dlgChoosePageRange ChoosePageRange = new dlgChoosePageRange(Current.Document);
                    ChoosePageRange.Owner = this;
                    ChoosePageRange.ShowDialog();

                    if (ChoosePageRange.DialogResult == true)
                    {
                        int StartPage = ChoosePageRange.StartPage;
                        int EndPage = ChoosePageRange.EndPage;
                        int LinePixelLimit = 40;    // 60 er bedst til Elfelt - måske til håndskrift generelt. Og mindre til maskinskrift?
                        foreach (TrPage TP in Current.Document.Pages)
                        {
                            if (TP.PageNr >= StartPage && TP.PageNr <= EndPage)
                            {
                                Debug.WriteLine($"Page nr. {TP.PageNr}");
                                foreach (TrRegion TR in TP.Transcripts[0].Regions)
                                {
                                    if (TR.GetType() == typeof(TrRegion_Text))
                                        (TR as TrRegion_Text).TextLines.ReNumberLogically(LinePixelLimit);
                                }
                            }
                        }

                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_ShowImages_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                TrPage TestPage = Current.Document.Pages[1];

                dlgShowPage ShowPage = new dlgShowPage(TestPage, Client);
                ShowPage.Owner = this;
                ShowPage.ShowDialog();

            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_ShowLemmas_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowLemmas ShowLemmas = new dlgShowLemmas(Current.Document);
                ShowLemmas.Owner = this;
                ShowLemmas.ShowDialog();

            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_DeleteLines_Click(object sender, RoutedEventArgs e)
        {
            string ChosenStructuralTag;
            deleteAction DeleteAction;
            if (Current.Collection != null && Current.Document != null)
            {
                dlgDeleteLines DeleteLines = new dlgDeleteLines();
                DeleteLines.Owner = this;
                DeleteLines.txtStructuralTag.ItemsSource = Current.Document.GetStructuralTags();
                DeleteLines.ShowDialog();
                if (DeleteLines.DialogResult == true)
                {
                    ChosenStructuralTag = DeleteLines.TagName;
                    DeleteAction = DeleteLines.DeleteAction;

                    if (DeleteAction == deleteAction.preserve)
                    {
                        string Question = $"Delete Other Lines Than \"{ChosenStructuralTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult Result = AskUser(Question);

                        if (Result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Preserve chosen to be: {ChosenStructuralTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                            Current.Document.DeleteLinesOtherThan(ChosenStructuralTag);

                            Mouse.OverrideCursor = null;
                        }

                    }
                    else if (DeleteAction == deleteAction.delete)
                    {
                        string Question = $"Delete Lines With Tag \"{ChosenStructuralTag}\" in {Current.Collection.Name} / {Current.Document.Title}?";
                        MessageBoxResult Result = AskUser(Question);

                        if (Result == MessageBoxResult.Yes)
                        {
                            //Debug.WriteLine($"Deleten chosen to be: {ChosenStructuralTag}");
                            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                            Current.Document.DeleteLinesWithTag(ChosenStructuralTag);

                            Mouse.OverrideCursor = null;
                        }
                    }

                    // nu kan nogle regioner være bleet tomme - derfor kalder vi DeleteEmptyRegions:
                    Current.Document.DeleteEmptyRegions();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_DeleteEmptyRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Delete Empty Regions in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    Current.Document.DeleteEmptyRegions();
                }

            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_CheckTableSituation_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                foreach (TrPage TP in Current.Document.Pages)
                {
                    Debug.Print($"Page # {TP.PageNr} : Current tables? {TP.HasTables.ToString()} - Former tables? {TP.HasFormerTables.ToString()}");
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");


        }

        private void MenuItem_ConvertTablesToTextRegions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                if (Current.Document.HasTables)
                {
                    string Question = $"Convert tables to text regions in {Current.Collection.Name} / {Current.Document.Title}?";
                    MessageBoxResult Result = AskUser(Question);
                    if (Result == MessageBoxResult.Yes)
                    {
                        dlgChoosePageRange ChoosePageRange = new dlgChoosePageRange(Current.Document);
                        ChoosePageRange.Owner = this;
                        ChoosePageRange.ShowDialog();

                        if (ChoosePageRange.DialogResult == true)
                            Current.Document.ConvertTablesToRegions(ChoosePageRange.StartPage, ChoosePageRange.EndPage);
                    }
                }
                else
                {
                    TellUser("The current document has no tables!");
                }

            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_CopyForgottenTables_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                if (Current.Document.HasFormerTables)
                {
                    string Question = $"Copy tables from former transcripts in {Current.Collection.Name} / {Current.Document.Title}?";
                    MessageBoxResult Result = AskUser(Question);
                    if (Result == MessageBoxResult.Yes)
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
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_MergeAllToTopLevel_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Merge All Regions to Top Level Region in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    dlgChoosePageRange ChoosePageRange = new dlgChoosePageRange(Current.Document);
                    ChoosePageRange.Owner = this;
                    ChoosePageRange.ShowDialog();

                    if (ChoosePageRange.DialogResult == true)
                        Current.Document.MergeAllRegionsToTopLevelRegion(ChoosePageRange.StartPage, ChoosePageRange.EndPage);
                }

            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_AutoAddAbbrevTags_Repetitions_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag with <abbrev> at Repetitions in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage P in Current.Document.Pages)
                        P.AutoAbbrevTagRepetitions();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }

        private void MenuItem_AutoTagRomanNumerals_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag roman numerals in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage P in Current.Document.Pages)
                        P.AutoTagRomanNumerals();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }



        private void MenuItem_ExportPseudoTables_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                int MinimumRecordNumber = 0;

                dlgChooseMinMaxNumbers Choose = new dlgChooseMinMaxNumbers();
                Choose.Owner = this;
                Choose.ShowDialog();

                if (Choose.DialogResult == true)
                {
                    if (Choose.Minimum != 0)
                        MinimumRecordNumber = Choose.Minimum;
                    // TrLibrary.ExportFolder + 
                    string FileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                        + "PseudoTableRecords_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".csv";
                    using (StreamWriter TextFile = new StreamWriter(FileName, true, Encoding.UTF8))
                    {
                        Debug.Print($"Exporting pseudo table text from {Current.Collection.Name} - {Current.Document.Title} - Minimum = {MinimumRecordNumber}");

                        List<TrRecord> PageRecords = Current.Document.GetPseudoTableText(MinimumRecordNumber);

                        Debug.Print($"Result: {PageRecords.Count} records in this doc! Writing to file.");

                        foreach (TrRecord PageRec in PageRecords)
                            TextFile.WriteLine(PageRec.ToString());


                    }

                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_AutoAddDateTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag with <date> in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    int Left = 0;
                    int Right = 30;
                    Current.Document.AutoDateTag(Left, Right);
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_TagEmptyTextLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Tag empty textlines in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    Current.Document.TagEmptyTextLines();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_MarkEmptyAbbrevTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Mark (tag) empty abbrev-tags in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    Current.Document.TagEmptyAbbrevTags();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_AutoAddRecordTags_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag with structural tag <RecordName> in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    double Left = 70;
                    double Right = 100;
                    Current.Document.AutoRecordTag(Left, Right);
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_ElfeltRecordCheck_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Check lines with structural tag <RecordName> in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    // TrLibrary.ExportFolder +
                    string FileName = Current.Collection.Name + "_" + Current.Document.Title + "_"
                        + "ElfeltRecordCheck_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".csv";
                    using (StreamWriter TextFile = new StreamWriter(FileName, true, Encoding.UTF8))
                    {
                        // TextFile.WriteLine("Pseudo table text from " + Current.Collection.Name + " - " + Current.Document.Title);

                        List<string> Records = Current.Document.CheckElfeltRecordNumbers();

                        foreach (string Record in Records)
                            TextFile.WriteLine(Record);
                    }
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }


        private void MenuItem_WrapSuperAndSubscriptWithSpaces_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Wrap super- and subscript with spaces in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    Current.Document.WrapSuperAndSubscriptWithSpaces();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");
        }


        private void MenuItem_CurrentTestModule_Click(object sender, RoutedEventArgs e)
        {
            double SumHpos = 0;
            double SumHendPos = 0;
            double minHpos = 100;
            double maxHpos = 0;
            double minHendPos = 100;
            double maxHendPos = 0;
            int LineCount = 0;

            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Check what-ever :) ... in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    Regex Numbers = new Regex(@"\d{4,}\p{L}?");
                    Regex YearOnly = new Regex(@"^\d{4}(?!(\-|\d))|^\d{4}$");

                    foreach (TrPage P in Current.Document.Pages)
                    {
                        //if (P.PageNr >= 3 && P.PageNr <= 10)
                        {
                            Debug.Print($"Page# {P.PageNr} ----------------------------------------------------");
                            //bool RegionOK;
                            //int i = 0;
                            foreach (TrRegion TR in P.Transcripts[0].Regions)
                                if (TR.GetType() == typeof(TrRegion_Text))
                                {
                                    foreach (TrTextLine TL in (TR as TrRegion_Text).TextLines)
                                    {
                                        bool Found = false;
                                        //MatchCollection NumberMatches = Numbers.Matches(TL.ExpandedText);
                                        MatchCollection YearOnlyMatches = YearOnly.Matches(TL.ExpandedText);
                                        if (YearOnlyMatches.Count > 0)
                                        //if (NumberMatches.Count > 0)
                                        {
                                            int i = 0;
                                            int DigitValue = 0;
                                            foreach (Match M in YearOnlyMatches)
                                            //foreach (Match M in NumberMatches)
                                            {
                                                string Content = clsLanguageLibrary.StripPunctuation(M.Value);
                                                if (clsLanguageLibrary.IsNumeric(Content))
                                                {
                                                    DigitValue = Convert.ToInt32(Content);

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
                                                if (DigitValue < 2000)
                                                {
                                                    i++;
                                                    Debug.WriteLine($"Line# {TL.Number}: Match# {i}: found {Content}");
                                                    Found = true;
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
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_TagLinesByRegex_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgFilterLinesByRegex FilterLines = new dlgFilterLinesByRegex(Current.Document, Client);
                FilterLines.Owner = this;
                FilterLines.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_FilterLines_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgFilterLines FilterLines = new dlgFilterLines(Current.Document);
                FilterLines.Owner = this;
                FilterLines.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }


        private void MenuItem_AutoAddAbbrevTags_NumericIntervals_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag with <abbrev> at numeric intervals in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    string TagName = "RecordName";
                    // vælg hvilket struct-tag, der skal bestemme det
                    foreach (TrPage P in Current.Document.Pages)
                        P.AutoAbbrevTagNumericIntervals(TagName);
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_AutoAddAbbrevTags_PlaceNames_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag with <abbrev> in place names in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage P in Current.Document.Pages)
                        P.AutoAbbrevTagPlaceNames();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }

        private void MenuItem_AutoTagFloorNumbers_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                string Question = $"Auto-Tag floor numbers with superscript in {Current.Collection.Name} / {Current.Document.Title}?";
                MessageBoxResult Result = AskUser(Question);
                if (Result == MessageBoxResult.Yes)
                {
                    foreach (TrPage P in Current.Document.Pages)
                        P.AutoTagFloorNumberSuperScript();
                }
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }


        // Histogram
        private void MenuItem_ShowHistogram_Click(object sender, RoutedEventArgs e)
        {
            if (Current.Collection != null && Current.Document != null)
            {
                dlgShowHistogram Histogram = new dlgShowHistogram(Current.Document);
                Histogram.Owner = this;
                Histogram.ShowDialog();
            }
            else
                TellUser("You have to choose a collection AND a document!");

        }
    }


}
