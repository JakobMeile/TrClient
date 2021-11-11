// <copyright file="ShowHistogram.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using TrClient.Core;
    using TrClient.Helpers;
    using TrClient.Libraries;

    /// <summary>
    /// Interaction logic for ShowHistogram.xaml.
    /// </summary>
    public partial class ShowHistogram : Window
    {
        private TrDocument currentDocument;
        private int bucketSize;
        private HistogramType type;
        private List<string> listOfBucketSizes = new List<string>();

        public ShowHistogram(TrDocument document)
        {
            InitializeComponent();
            currentDocument = document;

            // fills up cmbHistogramType and sets initial type
            cmbHistogramType.ItemsSource = Enum.GetValues(typeof(HistogramType));
            cmbHistogramType.SelectedItem = cmbHistogramType.Items[0];

            // makes a list of bucket sizes
            for (int i = 1; i <= 10; i++)
            {
                int p = i * 10;
                listOfBucketSizes.Add(p.ToString());
            }

            // and fills this in cmbBucketSize; sets initial size
            cmbBucketSize.ItemsSource = listOfBucketSizes;
            cmbBucketSize.SelectedItem = listOfBucketSizes.First();
        }

        private void cmbHistogramType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawHistogram();
        }

        private void cmbBucketSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawHistogram();
        }

        private void DrawHistogram()
        {
            if ((cmbHistogramType.SelectedItem != null) && (cmbBucketSize.SelectedItem != null))
            {
                if (currentDocument.HasLines)
                {
                    type = (HistogramType)cmbHistogramType.SelectedItem;
                    bucketSize = Convert.ToInt32(cmbBucketSize.SelectedItem);
                    Debug.Print($"ShowHistogram: type = {type.ToString()}, bucketSize = {bucketSize}");

                    if (bucketSize != 0)
                    {
                        Histogram histogram = new Histogram(currentDocument, type, bucketSize);
                        lstRanges.ItemsSource = histogram.Result;
                    }
                }
                else
                {
                    MessageBox.Show("Document has no lines!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        //private void BtnDrawHistogram_Click(object sender, RoutedEventArgs e)
        //{
        //}

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
