// <copyright file="MoveRegions.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Windows;
    using TranskribusClient.Core;

    /// <summary>
    /// Interaction logic for MoveRegions.xaml.
    /// </summary>
    public partial class MoveRegions : Window
    {
        private TrDocument currentDocument;
        private HttpClient currentClient;
        private List<string> listOfPages;

        private int page;

        public int DeltaH { get; set; }

        public int DeltaV { get; set; }

        public MoveRegions(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            DeltaH = 0;
            DeltaV = 0;
            DataContext = this;
            listOfPages = currentDocument.GetListOfPages();
            cmbPages.ItemsSource = listOfPages;
            cmbPages.SelectedIndex = 0;
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            if (!currentDocument.HasChanged)
            {
                page = Int32.Parse(cmbPages.SelectedItem.ToString());
                Debug.WriteLine($"Page: {page}, DeltaH: {DeltaH}, DeltaV: {DeltaV}");
                currentDocument.Move(page, DeltaH, DeltaV);
                DeltaH = 0;
                DeltaV = 0;
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            currentDocument.Upload(currentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
