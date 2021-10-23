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
    /// Interaction logic for dlgEditBaseLines.xaml
    /// </summary>
    public partial class dlgEditBaseLines : Window
    {
        public TrDialogTransferSettings DialogSettings;
        private TrDocument CurrentDocument;
        private HttpClient CurrentClient;
        private List<string> ListOfPages;
        private List<string> ListOfRegions;
        private List<string> ListOfPixelUnits = new List<string>();

        public dlgEditBaseLines(TrDocument Document, HttpClient Client, TrDialogTransferSettings Settings)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;
            DialogSettings = Settings;
            Debug.WriteLine($"PRE: Short: {DialogSettings.ShortLimit}, Left: {DialogSettings.LeftAmount}, Right: {DialogSettings.RightAmount}");

            ListOfPages = CurrentDocument.GetListOfPages();
            cmbPagesFrom.ItemsSource = ListOfPages;
            cmbPagesTo.ItemsSource = ListOfPages;

            ListOfRegions = CurrentDocument.GetListOfPossibleRegions();
            cmbRegionsFrom.ItemsSource = ListOfRegions;
            cmbRegionsTo.ItemsSource = ListOfRegions;

            for (int i = 0; i <= 10; i++)
            {
                int p = i * 10;
                ListOfPixelUnits.Add(p.ToString());
            }
            cmbLowerLimit.ItemsSource = ListOfPixelUnits;
            cmbLeftExtension.ItemsSource = ListOfPixelUnits;
            cmbRightExtension.ItemsSource = ListOfPixelUnits;

            chkDelete.IsChecked = Settings.DeleteShortBaseLines;
            cmbLowerLimit.Text = Settings.ShortLimit.ToString();
            if (!Settings.DeleteShortBaseLines)
                cmbLowerLimit.IsEnabled = false;

            chkLeft.IsChecked = Settings.ExtendLeft;
            cmbLeftExtension.Text = Settings.LeftAmount.ToString();
            if (!Settings.ExtendLeft)
                cmbLeftExtension.IsEnabled = false;

            chkRight.IsChecked = Settings.ExtendRight;
            cmbRightExtension.Text = Settings.RightAmount.ToString();
            if (!Settings.ExtendRight)
                cmbRightExtension.IsEnabled = false;

            rdPagesAll.IsChecked = Settings.AllPages;
            cmbPagesFrom.SelectedItem = ListOfPages.First();
            cmbPagesTo.SelectedItem = ListOfPages.Last();
            if (Settings.AllPages)
            {
                cmbPagesFrom.IsEnabled = false;
                cmbPagesTo.IsEnabled = false;
            }

            rdRegionsAll.IsChecked = Settings.AllRegions;
            cmbRegionsFrom.SelectedItem = ListOfRegions.First();
            cmbRegionsTo.SelectedItem = ListOfRegions.Last();
            if (Settings.AllRegions)
            {
                cmbRegionsFrom.IsEnabled = false;
                cmbRegionsTo.IsEnabled = false;
            }

            lstPages.ItemsSource = CurrentDocument.Pages;
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            bool PagesOK;
            bool RegionsOK;

            DialogSettings.ShortLimit = GetNumber(cmbLowerLimit.SelectedItem.ToString());
            DialogSettings.LeftAmount = GetNumber(cmbLeftExtension.SelectedItem.ToString());
            DialogSettings.RightAmount = GetNumber(cmbRightExtension.SelectedItem.ToString());
            Debug.WriteLine($"POST: Short: {DialogSettings.ShortLimit}, Left: {DialogSettings.LeftAmount}, Right: {DialogSettings.RightAmount}");
            
            if (DialogSettings.AllPages)
            {
                PagesOK = true;
            }
            else
            {
                if (cmbPagesFrom.SelectedItem != null && cmbPagesTo.SelectedItem != null)
                {
                    DialogSettings.PagesFrom = GetNumber(cmbPagesFrom.SelectedItem.ToString());
                    DialogSettings.PagesTo = GetNumber(cmbPagesTo.SelectedItem.ToString());
                    PagesOK = true;
                }
                else
                    PagesOK = false;
            }


            if (DialogSettings.AllRegions)
            {
                RegionsOK = true;
            }
            else
            {
                if (cmbRegionsFrom.SelectedItem != null && cmbRegionsTo.SelectedItem != null)
                {
                    DialogSettings.RegionsFrom = GetNumber(cmbRegionsFrom.SelectedItem.ToString());
                    DialogSettings.RegionsTo = GetNumber(cmbRegionsTo.SelectedItem.ToString());
                    RegionsOK = true;
                }
                else
                    RegionsOK = false;
            }

            if (PagesOK && RegionsOK)
            {
                if (DialogSettings.DeleteShortBaseLines)
                {
                    Debug.WriteLine($"Påbegynder Delete Short Baselines");
                    CurrentDocument.DeleteShortBaseLines(DialogSettings);
                }

                if (DialogSettings.ExtendLeft || DialogSettings.ExtendRight)
                {
                    Debug.WriteLine($"Påbegynder Extend Baselines: Left: {DialogSettings.ExtendLeft.ToString()}, Right: {DialogSettings.ExtendRight.ToString()} ");
                    CurrentDocument.ExtendBaseLines(DialogSettings);
                }

            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            CurrentDocument.Upload(CurrentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ChkDelete_Checked(object sender, RoutedEventArgs e)
        {
            DialogSettings.DeleteShortBaseLines = true;
            cmbLowerLimit.IsEnabled = true;
        }

        private void ChkDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            DialogSettings.DeleteShortBaseLines = false;
            cmbLowerLimit.IsEnabled = false;
        }

        private void ChkLeft_Checked(object sender, RoutedEventArgs e)
        {
            DialogSettings.ExtendLeft = true;
            cmbLeftExtension.IsEnabled = true;
        }

        private void ChkLeft_Unchecked(object sender, RoutedEventArgs e)
        {
            DialogSettings.ExtendLeft = false;
            cmbLeftExtension.IsEnabled = false;
        }

        private void ChkRight_Checked(object sender, RoutedEventArgs e)
        {
            DialogSettings.ExtendRight = true;
            cmbRightExtension.IsEnabled = true;
        }

        private void ChkRight_Unchecked(object sender, RoutedEventArgs e)
        {
            DialogSettings.ExtendRight = false;
            cmbRightExtension.IsEnabled = false;
        }

        private void RdPagesAll_Checked(object sender, RoutedEventArgs e)
        {
            DialogSettings.AllPages = true;
            cmbPagesFrom.IsEnabled = false;
            cmbPagesTo.IsEnabled = false;
        }

        private void RdPagesAll_Unchecked(object sender, RoutedEventArgs e)
        {
            DialogSettings.AllPages = false;
            cmbPagesFrom.IsEnabled = true;
            cmbPagesTo.IsEnabled = true;
        }

        private void RdRegionsAll_Checked(object sender, RoutedEventArgs e)
        {
            DialogSettings.AllRegions = true;
            cmbRegionsFrom.IsEnabled = false;
            cmbRegionsTo.IsEnabled = false;
        }

        private void RdRegionsAll_Unchecked(object sender, RoutedEventArgs e)
        {
            DialogSettings.AllRegions = false;
            cmbRegionsFrom.IsEnabled = true;
            cmbRegionsTo.IsEnabled = true;
        }

        private int GetNumber(string Selected)
        {
            string temp = Selected;
            temp = temp.Replace("(", "");
            temp = temp.Replace(")", "");
            return Int32.Parse(temp);
        }

    }
}
