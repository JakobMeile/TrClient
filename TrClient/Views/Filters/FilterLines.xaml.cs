// <copyright file="FilterLines.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TrClient.Core;
    using TrClient.Helpers;
    using TrClient.Libraries;

    /// <summary>
    /// Interaction logic for FilterLines.xaml.
    /// </summary>
    public partial class FilterLines : Window
    {
        //GridViewColumnHeader _lastHeaderClicked = null;
        //ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private TrDocument currentDocument;
        private List<string> listOfPages;
        private List<string> listOfTags;

        private TrTextLines lines = new TrTextLines();

        public TrLineFilterSettings FilterSettings = new TrLineFilterSettings();

        private string tagName = string.Empty;
        private bool overWrite = false;

        // ------------------------------------------------------------------------------------------------
        public FilterLines(TrDocument document)
        {
            InitializeComponent();
            currentDocument = document;

            DataContext = FilterSettings;

            listOfPages = currentDocument.GetListOfPages();
            cmbPagesFrom.ItemsSource = listOfPages;
            cmbPagesTo.ItemsSource = listOfPages;

            txtLinesTotal.Text = currentDocument.NumberOfLines.ToString("#,##0");

            Reset();

            Debug.Print($"First page: {listOfPages.First()} - Last page: {listOfPages.Last()}");
        }

        private void ChkPages_Checked(object sender, RoutedEventArgs e)
        {
            lblFrom.IsEnabled = true;
            cmbPagesFrom.IsEnabled = true;
            lblTo.IsEnabled = true;
            cmbPagesTo.IsEnabled = true;
        }

        private void ChkPages_Unchecked(object sender, RoutedEventArgs e)
        {
            lblFrom.IsEnabled = false;
            cmbPagesFrom.IsEnabled = false;
            lblTo.IsEnabled = false;
            cmbPagesTo.IsEnabled = false;
        }

        private void CmbTagName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSettings.StructuralTag = cmbTagName.SelectedItem.ToString().Trim();
        }

        private void CmbPagesFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSettings.StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());

            if (cmbPagesTo.SelectedItem != null)
            {
                FilterSettings.EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());

                if (FilterSettings.EndPage < FilterSettings.StartPage)
                {
                    FilterSettings.EndPage = FilterSettings.StartPage;
                    cmbPagesTo.SelectedItem = FilterSettings.EndPage.ToString();
                }
            }
        }

        private void CmbPagesTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSettings.EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());

            if (cmbPagesFrom.SelectedItem != null)
            {
                FilterSettings.StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());

                if (FilterSettings.EndPage < FilterSettings.StartPage)
                {
                    FilterSettings.StartPage = FilterSettings.EndPage;
                    cmbPagesFrom.SelectedItem = FilterSettings.StartPage.ToString();
                }
            }
        }

        private int GetNumber(string selected)
        {
            string temp = selected;
            temp = temp.Replace("(", string.Empty);
            temp = temp.Replace(")", string.Empty);
            return Int32.Parse(temp);
        }

        private void ChkRegEx_Checked(object sender, RoutedEventArgs e)
        {
            lblPattern.IsEnabled = true;
            txtPattern.IsEnabled = true;
        }

        private void ChkRegEx_Unchecked(object sender, RoutedEventArgs e)
        {
            lblPattern.IsEnabled = false;
            txtPattern.IsEnabled = false;
        }

        private void ChkTag_Checked(object sender, RoutedEventArgs e)
        {
            lblTagName.IsEnabled = true;
            cmbTagName.IsEnabled = true;
        }

        private void ChkTag_Unchecked(object sender, RoutedEventArgs e)
        {
            lblTagName.IsEnabled = false;
            cmbTagName.IsEnabled = false;
        }

        private void ChkTextSize_Checked(object sender, RoutedEventArgs e)
        {
            lblLowerLimit.IsEnabled = true;
            txtLowerLimit.IsEnabled = true;
            lblUpperLimit.IsEnabled = true;
            txtUpperLimit.IsEnabled = true;
        }

        private void ChkTextSize_Unchecked(object sender, RoutedEventArgs e)
        {
            lblLowerLimit.IsEnabled = false;
            txtLowerLimit.IsEnabled = false;
            lblUpperLimit.IsEnabled = false;
            txtUpperLimit.IsEnabled = false;
        }

        private void TxtLowerLimit_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //if (txtLowerLimit.Text != null)
            //{
            //    if (txtLowerLimit.Text != "")
            //    {
            //        FilterSettings.LowerLimitTextSizeFactor = Convert.ToInt32(txtLowerLimit.Text);
            //    }
            //}
        }

        private void TxtUpperLimit_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //if (txtUpperLimit.Text != null)
            //{
            //    if (txtUpperLimit.Text != "")
            //    {
            //        FilterSettings.UpperLimitTextSizeFactor = Convert.ToInt32(txtUpperLimit.Text);
            //    }
            //}
        }

        private void TxtLengthLowerLimit_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void TxtLengthUpperLimit_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }

        private void ChkTextLength_Checked(object sender, RoutedEventArgs e)
        {
            lblLengthLowerLimit.IsEnabled = true;
            txtLengthLowerLimit.IsEnabled = true;
            lblLengthUpperLimit.IsEnabled = true;
            txtLengthUpperLimit.IsEnabled = true;
        }

        private void ChkTextLength_Unchecked(object sender, RoutedEventArgs e)
        {
            lblLengthLowerLimit.IsEnabled = false;
            txtLengthLowerLimit.IsEnabled = false;
            lblLengthUpperLimit.IsEnabled = false;
            txtLengthUpperLimit.IsEnabled = false;
        }

        private void ChkPosition_Checked(object sender, RoutedEventArgs e)
        {
            lblMode.IsEnabled = true;
            rdInside.IsEnabled = true;
            rdOutside.IsEnabled = true;

            lblTop.IsEnabled = true;
            txtTop.IsEnabled = true;
            lblTopPercent.IsEnabled = true;
            sldTop.IsEnabled = true;

            lblBottom.IsEnabled = true;
            txtBottom.IsEnabled = true;
            lblBottomPercent.IsEnabled = true;
            sldBottom.IsEnabled = true;

            lblLeft.IsEnabled = true;
            txtLeft.IsEnabled = true;
            lblLeftPercent.IsEnabled = true;
            sldLeft.IsEnabled = true;

            lblRight.IsEnabled = true;
            txtRight.IsEnabled = true;
            lblRightPercent.IsEnabled = true;
            sldRight.IsEnabled = true;

            //lblWidth.IsEnabled = true;
            //txtWidth.IsEnabled = true;
            //lblWidthPercent.IsEnabled = true;

            //lblHeigth.IsEnabled = true;
            //txtHeigth.IsEnabled = true;
            //lblHeigthPercent.IsEnabled = true;
            rectBack.IsEnabled = true;
            rectFront.IsEnabled = true;

            lnTop.IsEnabled = true;
            lnBottom.IsEnabled = true;
            lnLeft.IsEnabled = true;
            lnRight.IsEnabled = true;
        }

        private void ChkPosition_Unchecked(object sender, RoutedEventArgs e)
        {
            lblMode.IsEnabled = false;
            rdInside.IsEnabled = false;
            rdOutside.IsEnabled = false;

            lblTop.IsEnabled = false;
            txtTop.IsEnabled = false;
            lblTopPercent.IsEnabled = false;
            sldTop.IsEnabled = false;

            lblBottom.IsEnabled = false;
            txtBottom.IsEnabled = false;
            lblBottomPercent.IsEnabled = false;
            sldBottom.IsEnabled = false;

            lblLeft.IsEnabled = false;
            txtLeft.IsEnabled = false;
            lblLeftPercent.IsEnabled = false;
            sldLeft.IsEnabled = false;

            lblRight.IsEnabled = false;
            txtRight.IsEnabled = false;
            lblRightPercent.IsEnabled = false;
            sldRight.IsEnabled = false;

            //lblWidth.IsEnabled = false;
            //txtWidth.IsEnabled = false;
            //lblWidthPercent.IsEnabled = false;

            //lblHeigth.IsEnabled = false;
            //txtHeigth.IsEnabled = false;
            //lblHeigthPercent.IsEnabled = false;
            rectBack.IsEnabled = false;
            rectFront.IsEnabled = false;

            lnTop.IsEnabled = false;
            lnBottom.IsEnabled = false;
            lnLeft.IsEnabled = false;
            lnRight.IsEnabled = false;
        }

        private void RdInside_Checked(object sender, RoutedEventArgs e)
        {
            FilterSettings.Inside = true;
        }

        private void RdOutside_Checked(object sender, RoutedEventArgs e)
        {
            FilterSettings.Inside = false;
        }

        private void ChkIncludeEnding_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void ChkIncludeEnding_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void ChkExcludeOddsizedPages_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void ChkExcludeOddsizedPages_Unchecked(object sender, RoutedEventArgs e)
        {
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

        //private void lvLinesColumnHeader_Click(object sender, RoutedEventArgs e)
        //{
        //    var headerClicked = e.OriginalSource as GridViewColumnHeader;
        //    ListSortDirection direction;

        //    if (headerClicked != null)
        //    {
        //        if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
        //        {
        //            if (headerClicked != _lastHeaderClicked)
        //            {
        //                direction = ListSortDirection.Ascending;
        //            }
        //            else
        //            {
        //                if (_lastDirection == ListSortDirection.Ascending)
        //                {
        //                    direction = ListSortDirection.Descending;
        //                }
        //                else
        //                {
        //                    direction = ListSortDirection.Ascending;
        //                }
        //            }

        //            var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
        //            var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

        //            Sort(sortBy, direction);

        //            if (direction == ListSortDirection.Ascending)
        //            {
        //                headerClicked.Column.HeaderTemplate = Application.Current.FindResource("ArrowUp") as DataTemplate;
        //            }
        //            else
        //            {
        //                headerClicked.Column.HeaderTemplate = Application.Current.FindResource("ArrowDown") as DataTemplate;
        //            }

        //            // Remove arrow from previously sorted header
        //            if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
        //            {
        //                _lastHeaderClicked.Column.HeaderTemplate = null;
        //            }

        //            _lastHeaderClicked = headerClicked;
        //            _lastDirection = direction;
        //        }
        //    }

        //}

        //private void Sort(string sortBy, ListSortDirection direction)
        //{
        //    ICollectionView dataView =
        //      CollectionViewSource.GetDefaultView(lvLines.ItemsSource);

        //    dataView.SortDescriptions.Clear();
        //    SortDescription sd = new SortDescription(sortBy, direction);
        //    dataView.SortDescriptions.Add(sd);
        //    dataView.Refresh();
        //}
        private void LvLines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListViewItem))
            {
                TrTextLine textLine = ((ListViewItem)sender).Content as TrTextLine;
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

        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterWindow.LeftBorder}, Right: {FilterWindow.RightBorder}, " +
            //    $"Top: {FilterWindow.TopBorder}, Bottom: {FilterWindow.BottomBorder}, Inside: {FilterWindow.Inside}");
            bool doRunFilter = true;

            if (FilterSettings.FilterByRegEx)
            {
                if (txtPattern.Text != string.Empty)
                {
                    string tempPattern = string.Format(@"({0})", txtPattern.Text.Trim());
                    Debug.Print($"txtPattern: {tempPattern}");
                    if (!TrLibrary.VerifyRegex(tempPattern))
                    {
                        doRunFilter = false;
                        MessageBox.Show($"Illegal reg-ex! {tempPattern}", TrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        FilterSettings.RegExPattern = tempPattern;
                        Debug.Print($"Filter by reg-ex: pattern = {FilterSettings.RegExPattern}");
                    }
                }
            }

            if (doRunFilter)
            {
                if (FilterSettings.FilterByPageNumber)
                {
                    FilterSettings.StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());
                    FilterSettings.EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());
                    Debug.Print($"Filter by pagenumber: pages = {FilterSettings.StartPage}-{FilterSettings.EndPage}");
                }

                if (FilterSettings.FilterByStructuralTag)
                {
                    if (cmbTagName.SelectedItem != null)
                    {
                        FilterSettings.StructuralTag = cmbTagName.SelectedItem.ToString().Trim();

                        //FilterSettings.StructuralTag = txtTagName.Text.Trim();
                        Debug.Print($"Filter by structural tag: tagname = {FilterSettings.StructuralTag}");
                    }
                    else
                    {
                        Debug.Print($"Filter by structural tag impossible: tagname = null");
                    }
                }

                if (FilterSettings.FilterByTextSizeFactor)
                {
                    FilterSettings.LowerLimitTextSizeFactor = 0;
                    FilterSettings.UpperLimitTextSizeFactor = 0;

                    if (txtLowerLimit.Text != null)
                    {
                        if (txtLowerLimit.Text != string.Empty)
                        {
                            FilterSettings.LowerLimitTextSizeFactor = Convert.ToInt32(txtLowerLimit.Text);
                        }
                    }

                    if (txtUpperLimit.Text != null)
                    {
                        if (txtUpperLimit.Text != string.Empty)
                        {
                            FilterSettings.UpperLimitTextSizeFactor = Convert.ToInt32(txtUpperLimit.Text);
                        }
                    }

                    Debug.Print($"Filter by text size: lower limit = {FilterSettings.LowerLimitTextSizeFactor}, upper limit = {FilterSettings.UpperLimitTextSizeFactor}");
                }

                if (FilterSettings.FilterByTextLength)
                {
                    FilterSettings.LowerLimitTextLength = 0;
                    FilterSettings.UpperLimitTextLength = 0;

                    if (txtLengthLowerLimit.Text != null)
                    {
                        if (txtLengthLowerLimit.Text != string.Empty)
                        {
                            FilterSettings.LowerLimitTextLength = Convert.ToInt32(txtLengthLowerLimit.Text);
                        }
                    }

                    if (txtLengthUpperLimit.Text != null)
                    {
                        if (txtLengthUpperLimit.Text != string.Empty)
                        {
                            FilterSettings.UpperLimitTextLength = Convert.ToInt32(txtLengthUpperLimit.Text);
                        }
                    }

                    Debug.Print($"Filter by text length: lower limit = {FilterSettings.LowerLimitTextLength}, upper limit = {FilterSettings.UpperLimitTextLength}");
                }

                if (FilterSettings.FilterByPosition)
                {
                    Debug.Print($"Filter by position: borders = Top {FilterSettings.TopBorder}, Bottom {FilterSettings.BottomBorder}, " +
                        $"Left {FilterSettings.LeftBorder}, Right {FilterSettings.RightBorder}");
                }

                lines.Clear();
                lstLines.ItemsSource = null;

                TrTextRegion textRegion;

                TrTextLines tempLines = currentDocument.GetFilteredLines(FilterSettings);

                foreach (TrTextLine textLine in tempLines)
                {
                    textRegion = textLine.ParentRegion;
                    lines.Add(textLine);
                    textLine.ParentRegion = textRegion;
                }

                lstLines.ItemsSource = lines;

                txtLinesFilterResult.Text = lines.Count.ToString("#,##0");

                Debug.WriteLine($"Line count: {lines.Count}");
            }
        }

        private void Reset()
        {
            FilterSettings.Reset();

            overWrite = false;
            tagName = string.Empty;

            chkOverWrite.IsChecked = false;

            listOfTags = currentDocument.GetStructuralTags();
            cmbTagName.ItemsSource = listOfTags;

            chkPages.IsChecked = false;

            lblFrom.IsEnabled = false;
            cmbPagesFrom.IsEnabled = false;
            cmbPagesFrom.SelectedItem = listOfPages.First();

            lblTo.IsEnabled = false;
            cmbPagesTo.IsEnabled = false;
            cmbPagesTo.SelectedItem = listOfPages.Last();

            chkRegEx.IsChecked = false;

            lblPattern.IsEnabled = false;
            txtPattern.IsEnabled = false;
            txtPattern.Text = string.Empty;

            chkTag.IsChecked = false;

            lblTagName.IsEnabled = false;
            cmbTagName.IsEnabled = false;
            txtTag.Text = string.Empty;

            chkTextSize.IsChecked = false;

            lblLowerLimit.IsEnabled = false;
            txtLowerLimit.IsEnabled = false;
            lblUpperLimit.IsEnabled = false;
            txtUpperLimit.IsEnabled = false;

            txtLowerLimit.Text = string.Empty;
            txtUpperLimit.Text = string.Empty;

            lblLengthLowerLimit.IsEnabled = false;
            txtLengthLowerLimit.IsEnabled = false;
            lblLengthUpperLimit.IsEnabled = false;
            txtLengthUpperLimit.IsEnabled = false;

            txtLengthLowerLimit.Text = string.Empty;
            txtLengthUpperLimit.Text = string.Empty;

            chkPosition.IsChecked = false;

            lblMode.IsEnabled = false;
            rdInside.IsEnabled = false;
            rdOutside.IsEnabled = false;

            lblTop.IsEnabled = false;
            txtTop.IsEnabled = false;
            lblTopPercent.IsEnabled = false;
            sldTop.IsEnabled = false;

            lblBottom.IsEnabled = false;
            txtBottom.IsEnabled = false;
            lblBottomPercent.IsEnabled = false;
            sldBottom.IsEnabled = false;

            lblLeft.IsEnabled = false;
            txtLeft.IsEnabled = false;
            lblLeftPercent.IsEnabled = false;
            sldLeft.IsEnabled = false;

            lblRight.IsEnabled = false;
            txtRight.IsEnabled = false;
            lblRightPercent.IsEnabled = false;
            sldRight.IsEnabled = false;

            //lblWidth.IsEnabled = false;
            //txtWidth.IsEnabled = false;
            //lblWidthPercent.IsEnabled = false;

            //lblHeigth.IsEnabled = false;
            //txtHeigth.IsEnabled = false;
            //lblHeigthPercent.IsEnabled = false;
            rectBack.IsEnabled = false;
            rectFront.IsEnabled = false;

            lnTop.IsEnabled = false;
            lnBottom.IsEnabled = false;
            lnLeft.IsEnabled = false;
            lnRight.IsEnabled = false;

            //lstLines.ItemsSource = CurrentDocument.GetFilteredLines(FilterSettings);
            GetLines();
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            GetLines();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void ChkOverWrite_Checked(object sender, RoutedEventArgs e)
        {
            overWrite = true;
        }

        private void ChkOverWrite_Unchecked(object sender, RoutedEventArgs e)
        {
            overWrite = false;
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
