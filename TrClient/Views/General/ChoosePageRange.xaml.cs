// <copyright file="ChoosePageRange.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for ChoosePageRange.xaml.
    /// </summary>
    public partial class ChoosePageRange : Window
    {
        private TrDocument currentDocument;
        private List<string> listOfPages;

        public int StartPage = 0;
        public int EndPage = 0;

        public ChoosePageRange(TrDocument document)
        {
            InitializeComponent();
            currentDocument = document;

            listOfPages = currentDocument.GetListOfPages();
            cmbPagesFrom.ItemsSource = listOfPages;
            cmbPagesTo.ItemsSource = listOfPages;

            rdPagesAll.IsChecked = true;
            cmbPagesFrom.SelectedItem = listOfPages.First();
            cmbPagesTo.SelectedItem = listOfPages.Last();
            cmbPagesFrom.IsEnabled = false;
            cmbPagesTo.IsEnabled = false;
        }

        private void RdPagesAll_Checked(object sender, RoutedEventArgs e)
        {
            cmbPagesFrom.IsEnabled = false;
            cmbPagesTo.IsEnabled = false;
        }

        private void RdPagesAll_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbPagesFrom.IsEnabled = true;
            cmbPagesTo.IsEnabled = true;
        }

        private void CmbPagesFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());

            if (cmbPagesTo.SelectedItem != null)
            {
                EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());

                if (EndPage < StartPage)
                {
                    EndPage = StartPage;
                    cmbPagesTo.SelectedItem = EndPage.ToString();
                }
            }
        }

        private void CmbPagesTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());

            if (cmbPagesFrom.SelectedItem != null)
            {
                StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());

                if (EndPage < StartPage)
                {
                    StartPage = EndPage;
                    cmbPagesFrom.SelectedItem = StartPage.ToString();
                }
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (rdPagesAll.IsChecked == true)
            {
                StartPage = GetNumber(listOfPages.First());
                EndPage = GetNumber(listOfPages.Last());
            }
            else
            {
                StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());
                EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
