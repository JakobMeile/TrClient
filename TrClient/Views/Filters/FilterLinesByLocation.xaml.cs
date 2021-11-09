// <copyright file="FilterLinesByLocation.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Windows;
    using TranskribusClient.Core;
    using TranskribusClient.Helpers;

    /// <summary>
    /// Interaction logic for FilterLinesByLocation.xaml.
    /// </summary>
    public partial class FilterLinesByLocation : Window
    {
        private TrDocument currentDocument;
        private TrTextLines lines = new TrTextLines();
        private HttpClient currentClient;

        // public TrPercentualWindow FilterSettings = new TrPercentualWindow();
        public TrLineFilterSettings FilterSettings = new TrLineFilterSettings();

        private string tagName;
        private bool overWrite = false;

        public FilterLinesByLocation(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            DataContext = FilterSettings;
            GetLines();

            // FilterSettings = new TrPercentualWindow(0, 0, 0, 0);
        }

        private void RdInside_Checked(object sender, RoutedEventArgs e)
        {
            FilterSettings.Inside = true;
        }

        private void RdOutside_Checked(object sender, RoutedEventArgs e)
        {
            FilterSettings.Inside = false;
        }

        private void SldTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowHeigth = FilterSettings.BottomBorder - FilterSettings.TopBorder;
        }

        private void SldBottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowHeigth = FilterSettings.BottomBorder - FilterSettings.TopBorder;
        }

        private void SldLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowWidth = FilterSettings.RightBorder - FilterSettings.LeftBorder;
        }

        private void SldRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            FilterSettings.WindowWidth = FilterSettings.RightBorder - FilterSettings.LeftBorder;
        }

        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterSettings.LeftBorder}, Right: {FilterSettings.RightBorder}, " +
            //    $"Top: {FilterSettings.TopBorder}, Bottom: {FilterSettings.BottomBorder}, Inside: {FilterSettings.Inside}");
            lines.Clear();
            lstLines.ItemsSource = null;

            TrTextRegion textRegion;
            TrTextLines tempLines = currentDocument.GetLines_WindowFiltered(FilterSettings);

            foreach (TrTextLine textLine in tempLines)
            {
                textRegion = textLine.ParentRegion;
                lines.Add(textLine);
                textLine.ParentRegion = textRegion;
            }

            lstLines.ItemsSource = lines;
            Debug.WriteLine($"Line count: {lines.Count}");
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            GetLines();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            FilterSettings.Reset();
            GetLines();
        }

        private void ChkOverWrite_Checked(object sender, RoutedEventArgs e)
        {
            overWrite = true;
        }

        private void ChkOverWrite_Unchecked(object sender, RoutedEventArgs e)
        {
            overWrite = false;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Region = GetNumber(cmbRegion.Text);
            //Line = GetNumber(cmbLine.Text);
            tagName = txtTag.Text;

            if (tagName != string.Empty)
            {
                foreach (object o in lstLines.SelectedItems)
                {
                    (o as TrTextLine).AddStructuralTag(tagName, overWrite);
                }
            }

            txtTag.Text = string.Empty;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lstLines.SelectedItems)
            {
                (o as TrTextLine).DeleteStructuralTag();
            }
        }

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            RenameTags DlgRename = new RenameTags(currentDocument);
            DlgRename.Owner = this;
            DlgRename.ShowDialog();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            currentDocument.Upload(currentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lstLines.Items)
            {
                lstLines.SelectedItems.Add(o);
            }
        }

        private void BtnNone_Click(object sender, RoutedEventArgs e)
        {
            lstLines.SelectedItems.Clear();
        }
    }
}
