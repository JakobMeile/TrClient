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
using System.Net.Http;
using System.Windows.Shapes;
using System.Diagnostics;
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
    /// Interaction logic for dlgMoveRegions.xaml
    /// </summary>
    public partial class dlgMoveRegions : Window
    {
        private TrDocument CurrentDocument;
        private HttpClient CurrentClient;
        private List<string> ListOfPages;

        private int Page;
        public int DeltaH { get; set; }
        public int DeltaV { get; set; }

        public dlgMoveRegions(TrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;
            DeltaH = 0;
            DeltaV = 0;
            DataContext = this;
            ListOfPages = CurrentDocument.GetListOfPages();
            cmbPages.ItemsSource = ListOfPages;
            cmbPages.SelectedIndex = 0;
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentDocument.HasChanged)
            {
                Page = Int32.Parse(cmbPages.SelectedItem.ToString());
                Debug.WriteLine($"Page: {Page}, DeltaH: {DeltaH}, DeltaV: {DeltaV}");
                CurrentDocument.Move(Page, DeltaH, DeltaV);
                DeltaH = 0;
                DeltaV = 0;
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            CurrentDocument.Upload(CurrentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
