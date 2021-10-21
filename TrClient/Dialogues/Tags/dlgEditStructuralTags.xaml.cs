using System;
using System.Collections.Generic;
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


namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgEditStructuralTags.xaml
    /// </summary>
    public partial class dlgEditStructuralTags : Window
    {
        private clsTrDocument CurrentDocument;
        private clsTrTextLines Lines = new clsTrTextLines();
        private HttpClient CurrentClient;

        private List<string> ListOfRegions;
        private List<string> ListOfLines;
        private List<string> TempList = new List<string>();

        private int RegionFrom;
        private int RegionTo;
        private int LineFrom;
        private int LineTo;

        private string TagName;
        private bool OverWrite = false;

        public dlgEditStructuralTags(clsTrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;

            ListOfRegions = CurrentDocument.GetListOfPossibleRegions();

            cmbRegionFrom.ItemsSource = ListOfRegions;
            cmbRegionFrom.SelectedItem = ListOfRegions.First();
            cmbRegionTo.ItemsSource = ListOfRegions;
            cmbRegionTo.SelectedItem = ListOfRegions.Last();

            RegionFrom = GetNumber(ListOfRegions.First().ToString()); // GetNumber(cmbRegionFrom.SelectedItem.ToString());
            RegionTo = GetNumber(ListOfRegions.Last().ToString()); // GetNumber(cmbRegionTo.SelectedItem.ToString());

            GetListOfLines();

        }

        private void GetListOfLines()
        {
            if (cmbRegionFrom.SelectedItem != null && cmbRegionTo.SelectedItem != null)
            {
                for (int i = RegionFrom; i <= RegionTo; i++)
                    TempList.AddRange(CurrentDocument.GetListOfPossibleLinesInRegion(i));
                ListOfLines = TempList.Distinct().ToList();

                cmbLineFrom.ItemsSource = ListOfLines;
                cmbLineFrom.SelectedItem = ListOfLines.First();
                cmbLineTo.ItemsSource = ListOfLines;
                cmbLineTo.SelectedItem = ListOfLines.Last();
            }
        }



        private void CmbRegionFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RegionFrom = GetNumber(cmbRegionFrom.SelectedItem.ToString());

            if (cmbRegionTo.SelectedItem != null)
            {
                RegionTo = GetNumber(cmbRegionTo.SelectedItem.ToString());

                if (RegionTo < RegionFrom)
                {
                    RegionTo = RegionFrom;
                    cmbRegionTo.SelectedItem = RegionTo.ToString();
                }
            }

            GetListOfLines();
            GetLines();
        }

        private void CmbRegionTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RegionTo = GetNumber(cmbRegionTo.SelectedItem.ToString());

            if (cmbRegionFrom.SelectedItem != null)
            {
                RegionFrom = GetNumber(cmbRegionFrom.SelectedItem.ToString());

                if (RegionTo < RegionFrom)
                {
                    RegionFrom  = RegionTo;
                    cmbRegionFrom.SelectedItem = RegionFrom.ToString();
                }
            }

            GetListOfLines();
            GetLines();
        }


        private void CmbLineFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LineFrom = GetNumber(cmbLineFrom.SelectedItem.ToString());

            if (cmbLineTo.SelectedItem != null)
            {
                LineTo = GetNumber(cmbLineTo.SelectedItem.ToString());

                if (LineTo < LineFrom)
                {
                    LineTo = LineFrom;
                    cmbLineTo.SelectedItem = LineTo.ToString();
                }
            }

            GetLines();
        }

        private void CmbLineTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LineTo = GetNumber(cmbLineTo.SelectedItem.ToString());

            if (cmbLineFrom.SelectedItem != null)
            {
                LineFrom = GetNumber(cmbLineFrom.SelectedItem.ToString());

                if (LineTo < LineFrom)
                {
                    LineFrom = LineTo;
                    cmbLineFrom.SelectedItem = LineFrom.ToString();
                }
            }

            GetLines();
        }

        private void GetLines()
        {
            Lines.Clear();
            lstLines.ItemsSource = null;
            txtTag.Text = "";

            if (cmbLineFrom.SelectedItem != null && cmbLineTo.SelectedItem != null)
            {

                clsTrTextRegion TR;
                for (int r = RegionFrom; r <= RegionTo; r++)
                    for (int l = LineFrom; l <= LineTo; l++)
                    {
                        clsTrTextLines TempLines = CurrentDocument.GetLinesWithNumber(r, l);
                        foreach (clsTrTextLine TL in TempLines)
                        {
                            TR = TL.ParentRegion;
                            Lines.Add(TL);
                            TL.ParentRegion = TR;
                        }
                            
                    }
                lstLines.ItemsSource = Lines;
            }

        }

        private void ChkOverWrite_Checked(object sender, RoutedEventArgs e)
        {
            OverWrite = true;
        }

        private void ChkOverWrite_Unchecked(object sender, RoutedEventArgs e)
        {
            OverWrite = false;
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

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            dlgRenameTags dlgRename = new dlgRenameTags(CurrentDocument);
            dlgRename.Owner = this;
            dlgRename.ShowDialog();
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            CurrentDocument.Upload(CurrentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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

        private int GetNumber(string Selected)
        {
            string temp = Selected;
            temp = temp.Replace("(", "");
            temp = temp.Replace(")", "");
            return Int32.Parse(temp);
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


    }
}
