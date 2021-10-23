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
using TrClient;
using TrClient.Core;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;
using TrClient.Dialog;

namespace TrClient.Dialog
{
    /// <summary>
    /// Interaction logic for dlgShowHistogram.xaml
    /// </summary>
    public partial class dlgShowHistogram : Window
    {
        private TrDocument CurrentDocument;
        private int BucketSize;
        private HistogramType Type;
        private List<string> ListOfBucketSizes = new List<string>();

        public dlgShowHistogram(TrDocument Document)
        {
            InitializeComponent();
            CurrentDocument = Document;
            cmbHistogramType.ItemsSource = Enum.GetValues(typeof(HistogramType));

            for (int i = 1; i <= 10; i++)
            {
                int p = i * 10;
                ListOfBucketSizes.Add(p.ToString());
            }
            cmbBucketSize.ItemsSource = ListOfBucketSizes;
        }

        private void BtnDrawHistogram_Click(object sender, RoutedEventArgs e)
        {
            if ((cmbHistogramType.SelectedItem != null) && (cmbBucketSize.SelectedItem != null))
            {
                Type = (HistogramType)cmbHistogramType.SelectedItem;
                BucketSize = Convert.ToInt32(cmbBucketSize.SelectedItem);

                Histogram Histogram = new Histogram(CurrentDocument, Type, BucketSize);
                lstRanges.ItemsSource = Histogram.Result;
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


    }
}
