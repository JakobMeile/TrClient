// <copyright file="RepairBaseLines.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TranskribusClient.Core;
    using TranskribusClient.Helpers;

    /// <summary>
    /// Interaction logic for RepairBaseLines.xaml.
    /// </summary>
    public partial class RepairBaseLines : Window
    {
        private TrDocument currentDocument;
        private TrTextLines lines = new TrTextLines();
        private HttpClient currentClient;

        public TrBaseLineFilter Filter = new TrBaseLineFilter();

        public RepairBaseLines(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            DataContext = Filter;
            GetLines();
        }

        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterWindow.LeftBorder}, Right: {FilterWindow.RightBorder}, " +
            //    $"Top: {FilterWindow.TopBorder}, Bottom: {FilterWindow.BottomBorder}, Inside: {FilterWindow.Inside}");
            lines.Clear();
            lstLines.ItemsSource = null;

            TrTextRegion textRegion;
            TrTextLines tempLines = currentDocument.GetLinesWithBaseLineProblems(); // CurrentDocument.GetLines_BaseLineFiltered(Filter);

            foreach (TrTextLine textLine in tempLines)
            {
                textRegion = textLine.ParentRegion;
                lines.Add(textLine);
                textLine.ParentRegion = textRegion;
            }

            lstLines.ItemsSource = lines;
            Debug.WriteLine($"Line count: {lines.Count}");
        }

        private void ChkCoordinatesPositive_Checked(object sender, RoutedEventArgs e)
        {
            Filter.CoordinatesPositive = true;
        }

        private void ChkBaseLineStraight_Checked(object sender, RoutedEventArgs e)
        {
            Filter.BaseLineStraight = true;
        }

        private void ChkBaseLineDirectionOK_Checked(object sender, RoutedEventArgs e)
        {
            Filter.BaseLineDirectionOK = true;
        }

        private void ChkCoordinatesPositive_Unchecked(object sender, RoutedEventArgs e)
        {
            Filter.CoordinatesPositive = false;
        }

        private void ChkBaseLineStraight_Unchecked(object sender, RoutedEventArgs e)
        {
            Filter.BaseLineStraight = false;
        }

        private void ChkBaseLineDirectionOK_Unchecked(object sender, RoutedEventArgs e)
        {
            Filter.BaseLineDirectionOK = false;
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            GetLines();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Filter.Reset();
            GetLines();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            currentDocument.Upload(currentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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
                    }
                }
            }
        }

        private void BtnLimitAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TrTextLine textLine in lines)
            {
                if (!textLine.IsCoordinatesPositive)
                {
                    textLine.LimitCoordsToPage();
                }
            }

            GetLines();
        }

        private void BtnFlattenAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TrTextLine textLine in lines)
            {
                if (!textLine.IsBaseLineStraight)
                {
                    textLine.FlattenBaseLine();
                }
            }

            GetLines();
        }
    }
}
