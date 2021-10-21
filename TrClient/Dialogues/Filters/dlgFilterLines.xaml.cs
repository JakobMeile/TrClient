using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgFilterLines.xaml
    /// </summary>
    public partial class dlgFilterLines : Window
    {
        //GridViewColumnHeader _lastHeaderClicked = null;
        //ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private clsTrDocument CurrentDocument;
        private List<string> ListOfPages;
        private List<string> ListOfTags;

        private clsTrTextLines Lines = new clsTrTextLines();

        public clsTrLineFilterSettings FilterSettings = new clsTrLineFilterSettings();

        private string TagName = "";
        private bool OverWrite = false;

        // ------------------------------------------------------------------------------------------------

        public dlgFilterLines(clsTrDocument Document)
        {
            InitializeComponent();
            CurrentDocument = Document;

            DataContext = this.FilterSettings;

            ListOfPages = CurrentDocument.GetListOfPages();
            cmbPagesFrom.ItemsSource = ListOfPages;
            cmbPagesTo.ItemsSource = ListOfPages;

            txtLinesTotal.Text = CurrentDocument.NumberOfLines.ToString("#,##0");

            Reset();

            Debug.Print($"First page: {ListOfPages.First()} - Last page: {ListOfPages.Last()}");

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

        private int GetNumber(string Selected)
        {
            string temp = Selected;
            temp = temp.Replace("(", "");
            temp = temp.Replace(")", "");
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
                clsTrTextLine TL = ((ListViewItem)sender).Content as clsTrTextLine;
                if (TL != null)
                {

                    dlgEditTextLine dlgEdit = new dlgEditTextLine(TL);
                    dlgEdit.Owner = this;
                    dlgEdit.ShowDialog();
                    if (dlgEdit.DialogResult == true)
                        TL = dlgEdit.CurrentLine;

                }
            }
        }

        private void LstLines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListBoxItem))
            {
                clsTrTextLine TL = ((ListBoxItem)sender).Content as clsTrTextLine;
                if (TL != null)
                {

                    dlgEditTextLine dlgEdit = new dlgEditTextLine(TL);
                    dlgEdit.Owner = this;
                    dlgEdit.ShowDialog();
                    if (dlgEdit.DialogResult == true)
                        TL = dlgEdit.CurrentLine;

                }
            }
        }




        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterWindow.LeftBorder}, Right: {FilterWindow.RightBorder}, " +
            //    $"Top: {FilterWindow.TopBorder}, Bottom: {FilterWindow.BottomBorder}, Inside: {FilterWindow.Inside}");

            bool DoRunFilter = true;

            if (FilterSettings.FilterByRegEx)
            {
                if (txtPattern.Text != "")
                {
                    string TempPattern = string.Format(@"({0})", txtPattern.Text.Trim());
                    Debug.Print($"txtPattern: {TempPattern}");
                    if (!clsTrLibrary.VerifyRegex(TempPattern))
                    {
                        DoRunFilter = false;
                        MessageBox.Show($"Illegal reg-ex! {TempPattern}", clsTrLibrary.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        FilterSettings.RegExPattern = TempPattern;
                        Debug.Print($"Filter by reg-ex: pattern = {FilterSettings.RegExPattern}");
                    }
                }
            }

            if (DoRunFilter)
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
                        if (txtLowerLimit.Text != "")
                        {
                            FilterSettings.LowerLimitTextSizeFactor = Convert.ToInt32(txtLowerLimit.Text);
                        }
                    }
                    if (txtUpperLimit.Text != null)
                    {
                        if (txtUpperLimit.Text != "")
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
                        if (txtLengthLowerLimit.Text != "")
                        {
                            FilterSettings.LowerLimitTextLength = Convert.ToInt32(txtLengthLowerLimit.Text);
                        }
                    }
                    if (txtLengthUpperLimit.Text != null)
                    {
                        if (txtLengthUpperLimit.Text != "")
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

                Lines.Clear();
                lstLines.ItemsSource = null;

                clsTrTextRegion TR;

                clsTrTextLines TempLines = CurrentDocument.GetFilteredLines(FilterSettings);

                foreach (clsTrTextLine TL in TempLines)
                {
                    TR = TL.ParentRegion;
                    Lines.Add(TL);
                    TL.ParentRegion = TR;
                }
                lstLines.ItemsSource = Lines;


                txtLinesFilterResult.Text = Lines.Count.ToString("#,##0");

                Debug.WriteLine($"Line count: {Lines.Count}");

            }

        }

        private void Reset()
        {
            FilterSettings.Reset();

            OverWrite = false;
            TagName = "";

            chkOverWrite.IsChecked = false;

            ListOfTags = CurrentDocument.GetStructuralTags();
            cmbTagName.ItemsSource = ListOfTags;


            chkPages.IsChecked = false;

            lblFrom.IsEnabled = false;
            cmbPagesFrom.IsEnabled = false;
            cmbPagesFrom.SelectedItem = ListOfPages.First();

            lblTo.IsEnabled = false;
            cmbPagesTo.IsEnabled = false;
            cmbPagesTo.SelectedItem = ListOfPages.Last();

            chkRegEx.IsChecked = false;

            lblPattern.IsEnabled = false;
            txtPattern.IsEnabled = false;
            txtPattern.Text = "";

            chkTag.IsChecked = false;

            lblTagName.IsEnabled = false;
            cmbTagName.IsEnabled = false;
            txtTag.Text = "";

            chkTextSize.IsChecked = false;

            lblLowerLimit.IsEnabled = false;
            txtLowerLimit.IsEnabled = false;
            lblUpperLimit.IsEnabled = false;
            txtUpperLimit.IsEnabled = false;

            txtLowerLimit.Text = "";
            txtUpperLimit.Text = "";

            lblLengthLowerLimit.IsEnabled = false;
            txtLengthLowerLimit.IsEnabled = false;
            lblLengthUpperLimit.IsEnabled = false;
            txtLengthUpperLimit.IsEnabled = false;

            txtLengthLowerLimit.Text = "";
            txtLengthUpperLimit.Text = "";

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
            OverWrite = true;
        }

        private void ChkOverWrite_Unchecked(object sender, RoutedEventArgs e)
        {
            OverWrite = false;
        }

        private void BtnAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lstLines.Items)
                lstLines.SelectedItems.Add(o);
        }

        private void BtnNone_Click(object sender, RoutedEventArgs e)
        {
            lstLines.SelectedItems.Clear();
        }



        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Region = GetNumber(cmbRegion.Text);
            //Line = GetNumber(cmbLine.Text);
            TagName = txtTag.Text;

            if (TagName != "")
            {
                foreach (object o in lstLines.SelectedItems)
                    (o as clsTrTextLine).AddStructuralTag(TagName, OverWrite);
            }
            txtTag.Text = "";
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            foreach (object o in lstLines.SelectedItems)
                (o as clsTrTextLine).DeleteStructuralTag();
        }



        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
