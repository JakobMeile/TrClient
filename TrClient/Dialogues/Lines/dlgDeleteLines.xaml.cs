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

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgDeleteLines.xaml
    /// </summary>
    public partial class dlgDeleteLines : Window
    {
        public string TagName;
        public deleteAction DeleteAction;

        public dlgDeleteLines()
        {
            InitializeComponent();
            rdPreserve.IsChecked = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            TagName = txtStructuralTag.Text;

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
