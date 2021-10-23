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
using static TrClient.MainWindow;
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
    /// Interaction logic for dlgDeleteRegions.xaml
    /// </summary>
    public partial class dlgDeleteRegions : Window
    {

        public string TagName;
        public deleteAction DeleteAction;

        public dlgDeleteRegions()
        {
            InitializeComponent();
            rdPreserve.IsChecked = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            TagName = txtRegionalTag.Text;

            if (rdPreserve.IsChecked == true)
                DeleteAction = deleteAction.preserve;
            else
                DeleteAction = deleteAction.delete;

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
