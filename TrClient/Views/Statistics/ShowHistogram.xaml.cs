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

        public Histogram CurrentHistogram;

        public ShowHistogram(TrDocument document)
        {
            InitializeComponent();
            currentDocument = document;

            // fills up cmbHistogramType 
            cmbHistogramType.ItemsSource = Enum.GetValues(typeof(HistogramType));

            // sets initial value for cmbHistogramType - which in turn calls this' selectionChanged - which calls sldBucketSize Changed - and hence the histogram is shown
            cmbHistogramType.SelectedItem = cmbHistogramType.Items[0];
            DrawHistogram();
        }

        private void CmbHistogramType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            type = (HistogramType)cmbHistogramType.SelectedItem;
            switch (type)
            {
                case HistogramType.LineLength:
                    sldBucketSize.Value = 10;
                    break;

                case HistogramType.LineWidth:
                    sldBucketSize.Value = 100;
                    break;

                case HistogramType.LineHpos:
                    sldBucketSize.Value = 200;
                    break;

                case HistogramType.LineVpos:
                    sldBucketSize.Value = 300;
                    break;

                default:
                    break;
            }
        }

        private void SldBucketSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DrawHistogram();
        }

        private void DrawHistogram()
        {
            if (cmbHistogramType.SelectedItem != null) 
            {
                if (currentDocument.HasLines)
                {
                    type = (HistogramType)cmbHistogramType.SelectedItem;
                    bucketSize = Convert.ToInt32(sldBucketSize.Value);
#if DEBUG
                    Debug.Print($"ShowHistogram: type = {type.ToString()}, bucketSize = {bucketSize}");
#endif
                    if (bucketSize != 0)
                    {
                        CurrentHistogram = new Histogram(currentDocument, type, bucketSize);
                        lstRanges.ItemsSource = CurrentHistogram.Result;

                        this.DataContext = CurrentHistogram;
                    }
                }
                else
                {
                    MessageBox.Show("Document has no lines!", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
                
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
