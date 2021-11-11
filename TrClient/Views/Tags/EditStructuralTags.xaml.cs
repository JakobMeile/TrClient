// <copyright file="EditStructuralTags.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for EditStructuralTags.xaml.
    /// </summary>
    public partial class EditStructuralTags : Window
    {
        private TrDocument currentDocument;
        private TrTextLines lines = new TrTextLines();
        private HttpClient currentClient;

        private List<string> listOfRegions;
        private List<string> listOfLines;
        private List<string> tempList = new List<string>();

        private int regionFrom;
        private int regionTo;
        private int lineFrom;
        private int lineTo;

        private string tagName;
        private bool overWrite = false;

        public EditStructuralTags(TrDocument document, HttpClient client)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;

            listOfRegions = currentDocument.GetListOfPossibleRegions();

            cmbRegionFrom.ItemsSource = listOfRegions;
            cmbRegionFrom.SelectedItem = listOfRegions.First();
            cmbRegionTo.ItemsSource = listOfRegions;
            cmbRegionTo.SelectedItem = listOfRegions.Last();

            regionFrom = GetNumber(listOfRegions.First().ToString()); // GetNumber(cmbRegionFrom.SelectedItem.ToString());
            regionTo = GetNumber(listOfRegions.Last().ToString()); // GetNumber(cmbRegionTo.SelectedItem.ToString());

            GetListOfLines();
        }

        private void GetListOfLines()
        {
            if (cmbRegionFrom.SelectedItem != null && cmbRegionTo.SelectedItem != null)
            {
                for (int i = regionFrom; i <= regionTo; i++)
                {
                    tempList.AddRange(currentDocument.GetListOfPossibleLinesInRegion(i));
                }

                listOfLines = tempList.Distinct().ToList();

                cmbLineFrom.ItemsSource = listOfLines;
                cmbLineFrom.SelectedItem = listOfLines.First();
                cmbLineTo.ItemsSource = listOfLines;
                cmbLineTo.SelectedItem = listOfLines.Last();
            }
        }

        private void CmbRegionFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            regionFrom = GetNumber(cmbRegionFrom.SelectedItem.ToString());

            if (cmbRegionTo.SelectedItem != null)
            {
                regionTo = GetNumber(cmbRegionTo.SelectedItem.ToString());

                if (regionTo < regionFrom)
                {
                    regionTo = regionFrom;
                    cmbRegionTo.SelectedItem = regionTo.ToString();
                }
            }

            GetListOfLines();
            GetLines();
        }

        private void CmbRegionTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            regionTo = GetNumber(cmbRegionTo.SelectedItem.ToString());

            if (cmbRegionFrom.SelectedItem != null)
            {
                regionFrom = GetNumber(cmbRegionFrom.SelectedItem.ToString());

                if (regionTo < regionFrom)
                {
                    regionFrom = regionTo;
                    cmbRegionFrom.SelectedItem = regionFrom.ToString();
                }
            }

            GetListOfLines();
            GetLines();
        }

        private void CmbLineFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lineFrom = GetNumber(cmbLineFrom.SelectedItem.ToString());

            if (cmbLineTo.SelectedItem != null)
            {
                lineTo = GetNumber(cmbLineTo.SelectedItem.ToString());

                if (lineTo < lineFrom)
                {
                    lineTo = lineFrom;
                    cmbLineTo.SelectedItem = lineTo.ToString();
                }
            }

            GetLines();
        }

        private void CmbLineTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lineTo = GetNumber(cmbLineTo.SelectedItem.ToString());

            if (cmbLineFrom.SelectedItem != null)
            {
                lineFrom = GetNumber(cmbLineFrom.SelectedItem.ToString());

                if (lineTo < lineFrom)
                {
                    lineFrom = lineTo;
                    cmbLineFrom.SelectedItem = lineFrom.ToString();
                }
            }

            GetLines();
        }

        private void GetLines()
        {
            lines.Clear();
            lstLines.ItemsSource = null;
            txtTag.Text = string.Empty;

            if (cmbLineFrom.SelectedItem != null && cmbLineTo.SelectedItem != null)
            {
                TrTextRegion textRegion;
                for (int r = regionFrom; r <= regionTo; r++)
                {
                    for (int l = lineFrom; l <= lineTo; l++)
                    {
                        TrTextLines tempLines = currentDocument.GetLinesWithNumber(r, l);
                        foreach (TrTextLine textLine in tempLines)
                        {
                            textRegion = textLine.ParentRegion;
                            lines.Add(textLine);
                            textLine.ParentRegion = textRegion;
                        }
                    }
                }

                lstLines.ItemsSource = lines;
            }
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

        private int GetNumber(string selected)
        {
            string temp = selected;
            temp = temp.Replace("(", string.Empty);
            temp = temp.Replace(")", string.Empty);
            return Int32.Parse(temp);
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
    }
}
