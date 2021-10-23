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
using System.Diagnostics;
using TrClient;
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
    /// Interaction logic for dlgChoosePageRange.xaml
    /// </summary>
    public partial class dlgChoosePageRange : Window
    {
        private TrDocument CurrentDocument;
        private List<string> ListOfPages;

        public int StartPage = 0;
        public int EndPage = 0;

        public dlgChoosePageRange(TrDocument Document)
        {
            InitializeComponent();
            CurrentDocument = Document;

            ListOfPages = CurrentDocument.GetListOfPages();
            cmbPagesFrom.ItemsSource = ListOfPages;
            cmbPagesTo.ItemsSource = ListOfPages;

            rdPagesAll.IsChecked = true;
            cmbPagesFrom.SelectedItem = ListOfPages.First();
            cmbPagesTo.SelectedItem = ListOfPages.Last();
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
                StartPage = GetNumber(ListOfPages.First());
                EndPage = GetNumber(ListOfPages.Last());
            }
            else
            {
                StartPage = GetNumber(cmbPagesFrom.SelectedItem.ToString());
                EndPage = GetNumber(cmbPagesTo.SelectedItem.ToString());
            }

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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
