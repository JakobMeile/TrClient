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
    /// Interaction logic for dlgSimplifyBoundingBoxes.xaml
    /// </summary>
    public partial class dlgSimplifyBoundingBoxes : Window
    {
        private TrDocument CurrentDocument;
        private HttpClient CurrentClient;

        private List<string> ListOfPixelUnits = new List<string>();

        private bool UseMaximumHeight = true;
        private int MaximumHeight = 100;
        private bool UseMinimumHeight = true;
        private int MinimumHeight = 70;

        public dlgSimplifyBoundingBoxes(TrDocument Document, HttpClient Client)
        {
            InitializeComponent();
            CurrentDocument = Document;
            CurrentClient = Client;

            for (int i = 0; i <= 20; i++)
            {
                int p = i * 10;
                ListOfPixelUnits.Add(p.ToString());
            }
            cmbMinHeight.ItemsSource = ListOfPixelUnits;
            cmbMinHeight.SelectedValue = MinimumHeight.ToString();
            cmbMinHeight.IsEnabled = true;
            chkMinimum.IsChecked = UseMinimumHeight;

            cmbMaxHeight.ItemsSource = ListOfPixelUnits;
            cmbMaxHeight.SelectedValue = MaximumHeight.ToString();
            cmbMaxHeight.IsEnabled = true;
            chkMaximum.IsChecked = UseMaximumHeight;

        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (UseMinimumHeight || UseMaximumHeight)
            {
                if (cmbMinHeight.SelectedItem != null && cmbMaxHeight.SelectedItem != null)
                {
                    MinimumHeight = Int32.Parse(cmbMinHeight.SelectedItem.ToString());
                    MaximumHeight = Int32.Parse(cmbMaxHeight.SelectedItem.ToString());
                    CurrentDocument.SimplifyBoundingBoxes(MinimumHeight, MaximumHeight);
                }
            }
            else
                CurrentDocument.SimplifyBoundingBoxes();

            this.DialogResult = true;
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ChkMinimum_Checked(object sender, RoutedEventArgs e)
        {
            UseMinimumHeight = true;
            cmbMinHeight.IsEnabled = true;
        }

        private void ChkMinimum_Unchecked(object sender, RoutedEventArgs e)
        {
            UseMinimumHeight = false;
            cmbMinHeight.IsEnabled = false;
        }

        private void ChkMaximum_Checked(object sender, RoutedEventArgs e)
        {
            UseMaximumHeight = true;
            cmbMaxHeight.IsEnabled = true;
        }

        private void ChkMaximum_Unchecked(object sender, RoutedEventArgs e)
        {
            UseMaximumHeight = false;
            cmbMaxHeight.IsEnabled = false;
        }
    }
}
