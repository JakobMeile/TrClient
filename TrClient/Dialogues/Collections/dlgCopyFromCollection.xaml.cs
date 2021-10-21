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
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;
using TrClient;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgCopyFromCollection.xaml
    /// </summary>
    public partial class dlgCopyFromCollection : Window
    {
        private clsTrDocument CurrentDocument;
        private clsTrCollection SourceCollection;
        private HttpClient CurrentClient;

        public dlgCopyFromCollection(clsTrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument= Document;
            CurrentClient = Client;
            lstSourceCollections.ItemsSource = Document.ParentCollection.ParentContainer;

        }


        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            //if (lstSourceCollections.SelectedItem != null)
            //{
            //    // LoadCurrentCollection();
            //    SourceCollection = (lstSourceCollections.SelectedItem as clsTrCollection);
            //    LoadSourceCollection();
            //    MessageBox.Show("Klar til kopiering!", clsTrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);

            //    CurrentDocument.CopyFromOtherCollection(SourceCollection, CurrentClient);
            //    this.Hide();
            //}

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }




        public async void LoadSourceCollection()
        {
            dlgProgressLoadDocs Progress = new dlgProgressLoadDocs(SourceCollection.NrOfDocs);
            Progress.Owner = this;
            Progress.DataContext = SourceCollection;
            Progress.Show();

            Task<bool> Loaded = SourceCollection.LoadDocuments(CurrentClient);
            bool OK = await Loaded;

            // henter sider
            foreach (clsTrDocument Doc in SourceCollection.Documents)
            {
                try
                {
                    Task<bool> PagesLoaded = Doc.LoadPages(CurrentClient);
                    bool PagesOK = await PagesLoaded;
                }
                catch (System.Threading.Tasks.TaskCanceledException eDocLoaded)
                {
                    Debug.WriteLine($"Exception message: {eDocLoaded.Message}");
                }
            }

            Progress.Hide();

            Mouse.OverrideCursor = null;

        }


    }
}
