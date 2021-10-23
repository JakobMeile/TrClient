using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;
using TrClient.Core;
using TrClient.Dialog;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Dialog
{

    /// <summary>
    /// Interaction logic for dlgRepairBaseLines.xaml
    /// </summary>
    public partial class dlgRepairBaseLines : Window
    {
        private TrDocument CurrentDocument;
        private TrTextLines Lines = new TrTextLines();
        private HttpClient CurrentClient;

        public TrBaseLineFilter Filter = new TrBaseLineFilter();

        public dlgRepairBaseLines(TrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;
            DataContext = this.Filter;
            GetLines();

        }


        private void GetLines()
        {
            //Debug.WriteLine($"Left: {FilterWindow.LeftBorder}, Right: {FilterWindow.RightBorder}, " +
            //    $"Top: {FilterWindow.TopBorder}, Bottom: {FilterWindow.BottomBorder}, Inside: {FilterWindow.Inside}");
            Lines.Clear();
            lstLines.ItemsSource = null;

            TrRegion_Text TR;
            TrTextLines TempLines = CurrentDocument.GetLinesWithBaseLineProblems(); // CurrentDocument.GetLines_BaseLineFiltered(Filter);

            foreach (TrTextLine TL in TempLines)
            {
                TR = TL.ParentRegion;
                Lines.Add(TL);
                TL.ParentRegion = TR;
            }
            lstLines.ItemsSource = Lines;
            Debug.WriteLine($"Line count: {Lines.Count}");

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
            CurrentDocument.Upload(CurrentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        private void LstLines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() == typeof(ListBoxItem))
            {
                TrTextLine TL = ((ListBoxItem)sender).Content as TrTextLine;
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

        private void BtnLimitAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TrTextLine TL in Lines)
            {
                if (!TL.IsCoordinatesPositive)
                    TL.LimitCoordsToPage();
            }
            GetLines();
        }

        private void BtnFlattenAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TrTextLine TL in Lines)
            {
                if (!TL.IsBaseLineStraight)
                    TL.FlattenBaseLine();
            }
            GetLines();

        }
    }


}
