// <copyright file="EditBaseLines.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Windows;
    using TrClient.Core;
    using TrClient.Helpers;

    /// <summary>
    /// Interaction logic for EditBaseLines.xaml.
    /// </summary>
    public partial class EditBaseLines : Window
    {
        public TrDialogTransferSettings DialogSettings;
        private TrDocument currentDocument;
        private HttpClient currentClient;
        private List<string> listOfPages;
        private List<string> listOfRegions;
        private List<string> listOfPixelUnits = new List<string>();

        public EditBaseLines(TrDocument document, HttpClient client, TrDialogTransferSettings settings)
        {
            InitializeComponent();
            currentDocument = document;
            currentClient = client;
            DialogSettings = settings;
            Debug.WriteLine($"PRE: Short: {DialogSettings.ShortLimit}, Left: {DialogSettings.LeftAmount}, Right: {DialogSettings.RightAmount}");

            listOfPages = currentDocument.GetListOfPages();
            cmbPagesFrom.ItemsSource = listOfPages;
            cmbPagesTo.ItemsSource = listOfPages;

            listOfRegions = currentDocument.GetListOfPossibleRegions();
            cmbRegionsFrom.ItemsSource = listOfRegions;
            cmbRegionsTo.ItemsSource = listOfRegions;

            for (int i = 0; i <= 10; i++)
            {
                int p = i * 10;
                listOfPixelUnits.Add(p.ToString());
            }

            cmbLowerLimit.ItemsSource = listOfPixelUnits;
            cmbLeftExtension.ItemsSource = listOfPixelUnits;
            cmbRightExtension.ItemsSource = listOfPixelUnits;

            chkDelete.IsChecked = settings.DeleteShortBaseLines;
            cmbLowerLimit.Text = settings.ShortLimit.ToString();
            if (!settings.DeleteShortBaseLines)
            {
                cmbLowerLimit.IsEnabled = false;
            }

            chkLeft.IsChecked = settings.ExtendLeft;
            cmbLeftExtension.Text = settings.LeftAmount.ToString();
            if (!settings.ExtendLeft)
            {
                cmbLeftExtension.IsEnabled = false;
            }

            chkRight.IsChecked = settings.ExtendRight;
            cmbRightExtension.Text = settings.RightAmount.ToString();
            if (!settings.ExtendRight)
            {
                cmbRightExtension.IsEnabled = false;
            }

            rdPagesAll.IsChecked = settings.AllPages;
            cmbPagesFrom.SelectedItem = listOfPages.First();
            cmbPagesTo.SelectedItem = listOfPages.Last();
            if (settings.AllPages)
            {
                cmbPagesFrom.IsEnabled = false;
                cmbPagesTo.IsEnabled = false;
            }

            rdRegionsAll.IsChecked = settings.AllRegions;
            cmbRegionsFrom.SelectedItem = listOfRegions.First();
            cmbRegionsTo.SelectedItem = listOfRegions.Last();
            if (settings.AllRegions)
            {
                cmbRegionsFrom.IsEnabled = false;
                cmbRegionsTo.IsEnabled = false;
            }

            lstPages.ItemsSource = currentDocument.Pages;
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            bool pagesOK;
            bool regionsOK;

            DialogSettings.ShortLimit = GetNumber(cmbLowerLimit.SelectedItem.ToString());
            DialogSettings.LeftAmount = GetNumber(cmbLeftExtension.SelectedItem.ToString());
            DialogSettings.RightAmount = GetNumber(cmbRightExtension.SelectedItem.ToString());
            Debug.WriteLine($"POST: Short: {DialogSettings.ShortLimit}, Left: {DialogSettings.LeftAmount}, Right: {DialogSettings.RightAmount}");

            if (DialogSettings.AllPages)
            {
                pagesOK = true;
            }
            else
            {
                if (cmbPagesFrom.SelectedItem != null && cmbPagesTo.SelectedItem != null)
                {
                    DialogSettings.PagesFrom = GetNumber(cmbPagesFrom.SelectedItem.ToString());
                    DialogSettings.PagesTo = GetNumber(cmbPagesTo.SelectedItem.ToString());
                    pagesOK = true;
                }
                else
                {
                    pagesOK = false;
                }
            }

            if (DialogSettings.AllRegions)
            {
                regionsOK = true;
            }
            else
            {
                if (cmbRegionsFrom.SelectedItem != null && cmbRegionsTo.SelectedItem != null)
                {
                    DialogSettings.RegionsFrom = GetNumber(cmbRegionsFrom.SelectedItem.ToString());
                    DialogSettings.RegionsTo = GetNumber(cmbRegionsTo.SelectedItem.ToString());
                    regionsOK = true;
                }
                else
                {
                    regionsOK = false;
                }
            }

            if (pagesOK && regionsOK)
            {
                if (DialogSettings.DeleteShortBaseLines)
                {
                    Debug.WriteLine($"Påbegynder Delete Short Baselines");
                    currentDocument.DeleteShortBaseLines(DialogSettings);
                }

                if (DialogSettings.ExtendLeft || DialogSettings.ExtendRight)
                {
                    Debug.WriteLine($"Påbegynder Extend Baselines: Left: {DialogSettings.ExtendLeft.ToString()}, Right: {DialogSettings.ExtendRight.ToString()} ");
                    currentDocument.ExtendBaseLines(DialogSettings);
                }
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            currentDocument.Upload(currentClient);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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

        private int GetNumber(string selected)
        {
            string temp = selected;
            temp = temp.Replace("(", string.Empty);
            temp = temp.Replace(")", string.Empty);
            return Int32.Parse(temp);
        }
    }
}
