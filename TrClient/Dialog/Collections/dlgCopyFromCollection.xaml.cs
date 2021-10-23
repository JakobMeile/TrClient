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
using TrClient.Core;
using TrClient.Dialog;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Dialog
{
    /// <summary>
    /// Interaction logic for dlgCopyFromCollection.xaml
    /// </summary>
    public partial class dlgCopyFromCollection : Window
    {
        private TrDocument CurrentDocument;
        private TrCollection SourceCollection;
        private HttpClient CurrentClient;

        public dlgCopyFromCollection(TrDocument Document, HttpClient Client)
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
            //    SourceCollection = (lstSourceCollections.SelectedItem as TrCollection);
            //    LoadSourceCollection();
            //    MessageBox.Show("Klar til kopiering!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);

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
            foreach (TrDocument Doc in SourceCollection.Documents)
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
