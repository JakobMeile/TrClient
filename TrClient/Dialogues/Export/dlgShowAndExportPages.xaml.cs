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
using System.IO;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgShowAndExportPages.xaml
    /// </summary>
    public partial class dlgShowAndExportPages : Window
    {
        private clsTrDocument CurrentDocument;
        private HttpClient CurrentClient;
        private clsTrPage CurrentPage;

        public int CurrentPageNr { get; set; }
        public int MaxPageNr { get; set; }

        public dlgShowAndExportPages(clsTrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;
            CurrentPage = Document.Pages[0];

            LoadPage();

        }

        private void LoadPage()
        {
            CurrentPageNr = CurrentPage.PageNr;
            txtCurrentPage.Text = CurrentPageNr.ToString();

            MaxPageNr = CurrentDocument.Pages.Count;
            txtPageCount.Text = MaxPageNr.ToString();

            lstLines.ItemsSource = CurrentPage.GetLines();
            lstParagraphs.ItemsSource = CurrentPage.GetParagraphs();

        }

        private void BtnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            LoadPreviousPage();
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            LoadNextPage();

        }

        private void LoadNextPage()
        {
            if (CurrentPageNr < MaxPageNr)
            {
                int NewPageIndex = CurrentPageNr;
                CurrentPage = CurrentDocument.Pages[NewPageIndex];

            }
            LoadPage();

        }

        private void LoadPreviousPage()
        {
            if (CurrentPageNr > 1)
            {
                int NewPageIndex = CurrentPageNr - 2;
                CurrentPage = CurrentDocument.Pages[NewPageIndex];

            }
            LoadPage();

        }

        //private void BtnFilterLines_Click(object sender, RoutedEventArgs e)
        //{

        //}

        private void BtnFilterParagraphs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveParagraphs_Click(object sender, RoutedEventArgs e)
        {   // clsTrLibrary.ExportFolder + 
            string FileName = CurrentDocument.ParentCollection.Name + "_" + CurrentDocument.Title + "_" 
                + "Paragraphs_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";
            using (StreamWriter ParagraphsFile = new StreamWriter(FileName, true))
            {
                ParagraphsFile.WriteLine("Paragraphs from " + CurrentDocument.ParentCollection.Name + " / " + CurrentDocument.Title);
                foreach (clsTrPage P in CurrentDocument.Pages)
                {
                    ParagraphsFile.WriteLine("------------------------------------------------------------------------------------");
                    ParagraphsFile.WriteLine("Page nr. " + P.PageNr.ToString());
                    ParagraphsFile.WriteLine(P.GetParagraphs().ToString());
                }
            }
        }

        //private void BtnExportAsExcelSheet_Click(object sender, RoutedEventArgs e)
        //{   // clsTrLibrary.ExportFolder + 
        //    string FileName = CurrentDocument.ParentCollection.Name + "_" + CurrentDocument.Title + "_"
        //        + "Paragraphs_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".xlsx";

        //    clsTrExcelSheet Sheet = new clsTrExcelSheet();

        //    List<string> Headers = CurrentDocument.GetStructuralTags();
        //    // List<string> Headers = CurrentPage.GetParagraphs().GetNames(); // CurrentDocument.GetStructuralTags();
        //    Sheet.AddHeaders(Headers);


        //    foreach (clsTrPage Page in CurrentDocument.Pages)
        //    {
        //        Sheet.AddRecord(Page.GetParagraphs());
        //    }

        //}

        private void BtnSaveMarcRecords_Click(object sender, RoutedEventArgs e)
        {

        }



        private void LstLines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListBoxItem))
            {
                clsTrTextLine TL = ((ListBoxItem)sender).Content as clsTrTextLine;
                if (TL != null)
                {
                    dlgEditTextLine dlgEdit = new dlgEditTextLine(TL);
                    dlgEdit.Owner = this;
                    dlgEdit.ShowDialog();
                    if (dlgEdit.DialogResult == true)
                    {
                        TL = dlgEdit.CurrentLine;
                        lstParagraphs.ItemsSource = CurrentPage.GetParagraphs();
                    }
                }
            }
        }

        private void DialogShowAndExportPages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                LoadPreviousPage();
            }
            if (e.Key == Key.Right)
            {
                LoadNextPage();
            }
        }
    }
}
