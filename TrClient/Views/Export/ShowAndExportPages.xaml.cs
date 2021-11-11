// <copyright file="ShowAndExportPages.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for ShowAndExportPages.xaml.
    /// </summary>
    public partial class ShowAndExportPages : Window
    {
        private TrDocument currentDocument;
        private HttpClient currentClient;
        private TrPage currentPage;

        public int CurrentPageNr { get; set; }

        public int MaxPageNr { get; set; }

        public ShowAndExportPages(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            currentPage = document.Pages[0];

            LoadPage();
        }

        private void LoadPage()
        {
            CurrentPageNr = currentPage.PageNr;
            txtCurrentPage.Text = CurrentPageNr.ToString();

            MaxPageNr = currentDocument.Pages.Count;
            txtPageCount.Text = MaxPageNr.ToString();

            lstLines.ItemsSource = currentPage.GetLines();
            lstParagraphs.ItemsSource = currentPage.GetParagraphs();
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
                int newPageIndex = CurrentPageNr;
                currentPage = currentDocument.Pages[newPageIndex];
            }

            LoadPage();
        }

        private void LoadPreviousPage()
        {
            if (CurrentPageNr > 1)
            {
                int newPageIndex = CurrentPageNr - 2;
                currentPage = currentDocument.Pages[newPageIndex];
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
        {   // TrLibrary.ExportFolder +
            string fileName = currentDocument.ParentCollection.Name + "_" + currentDocument.Title + "_"
                + "Paragraphs_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".txt";
            using (StreamWriter paragraphsFile = new StreamWriter(fileName, true))
            {
                paragraphsFile.WriteLine("Paragraphs from " + currentDocument.ParentCollection.Name + " / " + currentDocument.Title);
                foreach (TrPage p in currentDocument.Pages)
                {
                    paragraphsFile.WriteLine("------------------------------------------------------------------------------------");
                    paragraphsFile.WriteLine("Page nr. " + p.PageNr.ToString());
                    paragraphsFile.WriteLine(p.GetParagraphs().ToString());
                }
            }
        }

        //private void BtnExportAsExcelSheet_Click(object sender, RoutedEventArgs e)
        //{   // TrLibrary.ExportFolder +
        //    string FileName = CurrentDocument.ParentCollection.Name + "_" + CurrentDocument.Title + "_"
        //        + "Paragraphs_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm") + ".xlsx";

        //    clsTrExcelSheet Sheet = new clsTrExcelSheet();

        //    List<string> Headers = CurrentDocument.GetStructuralTags();
        //    // List<string> Headers = CurrentPage.GetParagraphs().GetNames(); // CurrentDocument.GetStructuralTags();
        //    Sheet.AddHeaders(Headers);

        //    foreach (TrPage Page in CurrentDocument.Pages)
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
                TrTextLine textLine = ((ListBoxItem)sender).Content as TrTextLine;
                if (textLine != null)
                {
                    EditTextLine DlgEdit = new EditTextLine(textLine);
                    DlgEdit.Owner = this;
                    DlgEdit.ShowDialog();
                    if (DlgEdit.DialogResult == true)
                    {
                        textLine = DlgEdit.CurrentLine;
                        lstParagraphs.ItemsSource = currentPage.GetParagraphs();
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
