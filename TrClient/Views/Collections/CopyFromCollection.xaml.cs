// <copyright file="CopyFromCollection.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for CopyFromCollection.xaml.
    /// </summary>
    public partial class CopyFromCollection : Window
    {
        private TrDocument currentDocument;
        private TrCollection sourceCollection;
        private HttpClient currentClient;

        public CopyFromCollection(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            lstSourceCollections.ItemsSource = document.ParentCollection.ParentContainer;
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
            DialogResult = true;
        }

        public async void LoadSourceCollection()
        {
            ProgressLoadDocs progress = new ProgressLoadDocs(sourceCollection.NrOfDocs);
            progress.Owner = this;
            progress.DataContext = sourceCollection;
            progress.Show();

            Task<bool> loaded = sourceCollection.LoadDocuments(currentClient);
            bool oK = await loaded;

            // henter sider
            foreach (TrDocument doc in sourceCollection.Documents)
            {
                try
                {
                    Task<bool> pagesLoaded = doc.LoadPages(currentClient);
                    bool pagesOK = await pagesLoaded;
                }
                catch (System.Threading.Tasks.TaskCanceledException eDocLoaded)
                {
                    Debug.WriteLine($"Exception message: {eDocLoaded.Message}");
                }
            }

            progress.Hide();

            Mouse.OverrideCursor = null;
        }
    }
}
