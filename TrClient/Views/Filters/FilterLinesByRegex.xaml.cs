// <copyright file="FilterLinesByRegex.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Windows;
    using TrClient.Core;
    using TrClient.Libraries;

    /// <summary>
    /// Interaction logic for FilterLinesByRegex.xaml.
    /// </summary>
    public partial class FilterLinesByRegex : Window
    {
        private TrDocument currentDocument;
        private TrTextLines lines = new TrTextLines();
        private HttpClient currentClient;

        private string regexPattern = string.Empty;

        private string tagName;
        private bool overWrite = false;

        public FilterLinesByRegex(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            txtPattern.Text = string.Empty;

            //DataContext = this.FilterWindow;
            //GetLines();
        }

        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterWindow.LeftBorder}, Right: {FilterWindow.RightBorder}, " +
            //    $"Top: {FilterWindow.TopBorder}, Bottom: {FilterWindow.BottomBorder}, Inside: {FilterWindow.Inside}");
            if (txtPattern.Text != string.Empty)
            {
                if (TrLibrary.VerifyRegex(regexPattern))
                {
                    lines.Clear();
                    lstLines.ItemsSource = null;

                    // Regex MatchPattern = new Regex(RegexPattern);
                    TrTextRegion textRegion;
                    TrTextLines tempLines = currentDocument.GetLines_RegexFiltered(regexPattern);

                    foreach (TrTextLine textLine in tempLines)
                    {
                        textRegion = textLine.ParentRegion;
                        lines.Add(textLine);
                        textLine.ParentRegion = textRegion;
                    }

                    lstLines.ItemsSource = lines;
                    Debug.WriteLine($"Line count: {lines.Count}");
                }
                else
                {
                    Debug.Print("Wrong regex!");
                }
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            regexPattern = string.Format(@"({0})", txtPattern.Text.Trim());

            //string escape = Regex.Escape(RegexPattern);
            //string unEsc = ""; // Regex.Unescape(RegexPattern);
            Debug.Print($"Regex Pattern = _{regexPattern}_"); // , esc: _{escape}_, unesc: _{unEsc}_
            GetLines();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            //FilterWindow.Reset();
            regexPattern = string.Empty;
            lines.Clear();
            lstLines.ItemsSource = null;

            //GetLines();
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
