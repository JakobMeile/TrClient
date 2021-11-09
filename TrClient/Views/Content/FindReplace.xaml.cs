// <copyright file="FindReplace.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.Net.Http;
    using System.Windows;
    using TranskribusClient.Core;

    /// <summary>
    /// Interaction logic for FindReplace.xaml.
    /// </summary>
    public partial class FindReplace : Window
    {
        private TrDocument currentDocument;
        private HttpClient currentClient;
        private TrTextLines foundLines = new TrTextLines();

        private string textToFind;
        private string textToReplaceWith;

        public FindReplace(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;

            currentClient = client;
            txtFindText.Focus();
            btnFind.IsDefault = true;
            txtReplaceText.IsEnabled = false;
            btnReplace.IsEnabled = false;
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            textToFind = txtFindText.Text;

            // txtReplaceText.Clear();
            if (textToFind != null)
            {
                if (textToFind != string.Empty)
                {
                    foundLines = currentDocument.FindText(textToFind);

                    lstLines.ItemsSource = foundLines;
                    if (foundLines.Count > 0)
                    {
                        lblCount.Content = foundLines.Count.ToString();
                        btnFind.IsDefault = false;
                        txtReplaceText.IsEnabled = true;
                        txtReplaceText.Focus();
                        btnReplace.IsEnabled = true;
                        btnReplace.IsDefault = true;
                    }
                    else
                    {
                        lblCount.Content = "(none)";
                    }
                }
            }
        }

        private void BtnReplace_Click(object sender, RoutedEventArgs e)
        {
            textToReplaceWith = txtReplaceText.Text;
            if (textToReplaceWith != null)
            {
                if (textToReplaceWith != string.Empty)
                {
                    foreach (TrTextLine textLine in foundLines)
                    {
                        textLine.Replace(textToFind, textToReplaceWith);
                    }
                }
            }

            txtFindText.Clear();
            txtReplaceText.Clear();
        }
    }
}
