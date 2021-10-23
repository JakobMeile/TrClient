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
    /// Interaction logic for dlgEditLemma.xaml
    /// </summary>
    public partial class dlgEditLemma : Window
    {
        public TrLemma CurrentLemma;
        private string OldText;

        public dlgEditLemma(TrLemma Lemma)
        {
            InitializeComponent();


            CurrentLemma = Lemma;
            OldText = CurrentLemma.Content;

            DataContext = CurrentLemma;

            Loaded += Window_Loaded;

            //lblExpandedText.Content = CurrentLemma.GetExpandedText(true, false);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtRawText.Focusable = true;
            txtRawText.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != OldText)
            {
                CurrentLemma.Content = txtRawText.Text;
                CurrentLemma.HasChanged = true;
            }
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (txtRawText.Text != OldText)
            {
                CurrentLemma.Content = OldText;
                CurrentLemma.HasChanged = false;
            }
            this.DialogResult = false;
        }

        //private void TxtRawText_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (txtRawText.Text != OldText)
        //    {
        //        CurrentLemma.Content = txtRawText.Text;
        //        CurrentLemma.HasChanged = true;
        //    }
        //}

    }
}
