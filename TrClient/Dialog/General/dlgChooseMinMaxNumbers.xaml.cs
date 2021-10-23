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
    /// Interaction logic for dlgChooseMinMaxNumbers.xaml
    /// </summary>
    public partial class dlgChooseMinMaxNumbers : Window
    {
        public int Minimum = 0;
        public int Maximum = 0;

        public dlgChooseMinMaxNumbers()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtMinimum.Text != null)
            {
                if (txtMinimum.Text != "")
                {
                    Minimum = Convert.ToInt32(txtMinimum.Text);
                }
            }

            if (txtMaximum.Text != null)
            {
                if (txtMaximum.Text != "")
                {
                    Maximum = Convert.ToInt32(txtMaximum.Text);
                }
            }

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
