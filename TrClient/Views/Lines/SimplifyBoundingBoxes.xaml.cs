// <copyright file="SimplifyBoundingBoxes.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Windows;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for SimplifyBoundingBoxes.xaml.
    /// </summary>
    public partial class SimplifyBoundingBoxes : Window
    {
        private TrDocument currentDocument;
        private HttpClient currentClient;

        private List<string> listOfPixelUnits = new List<string>();

        private bool useMaximumHeight = true;
        private int maximumHeight = 100;
        private bool useMinimumHeight = true;
        private int minimumHeight = 70;

        public SimplifyBoundingBoxes(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;

            for (int i = 0; i <= 20; i++)
            {
                int p = i * 10;
                listOfPixelUnits.Add(p.ToString());
            }

            cmbMinHeight.ItemsSource = listOfPixelUnits;
            cmbMinHeight.SelectedValue = minimumHeight.ToString();
            cmbMinHeight.IsEnabled = true;
            chkMinimum.IsChecked = useMinimumHeight;

            cmbMaxHeight.ItemsSource = listOfPixelUnits;
            cmbMaxHeight.SelectedValue = maximumHeight.ToString();
            cmbMaxHeight.IsEnabled = true;
            chkMaximum.IsChecked = useMaximumHeight;
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (useMinimumHeight || useMaximumHeight)
            {
                if (cmbMinHeight.SelectedItem != null && cmbMaxHeight.SelectedItem != null)
                {
                    minimumHeight = Int32.Parse(cmbMinHeight.SelectedItem.ToString());
                    maximumHeight = Int32.Parse(cmbMaxHeight.SelectedItem.ToString());
                    currentDocument.SimplifyBoundingBoxes(minimumHeight, maximumHeight);
                }
            }
            else
            {
                currentDocument.SimplifyBoundingBoxes();
            }

            DialogResult = true;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ChkMinimum_Checked(object sender, RoutedEventArgs e)
        {
            useMinimumHeight = true;
            cmbMinHeight.IsEnabled = true;
        }

        private void ChkMinimum_Unchecked(object sender, RoutedEventArgs e)
        {
            useMinimumHeight = false;
            cmbMinHeight.IsEnabled = false;
        }

        private void ChkMaximum_Checked(object sender, RoutedEventArgs e)
        {
            useMaximumHeight = true;
            cmbMaxHeight.IsEnabled = true;
        }

        private void ChkMaximum_Unchecked(object sender, RoutedEventArgs e)
        {
            useMaximumHeight = false;
            cmbMaxHeight.IsEnabled = false;
        }
    }
}
