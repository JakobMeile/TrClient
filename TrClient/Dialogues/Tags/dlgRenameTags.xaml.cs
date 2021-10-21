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

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgRenameTags.xaml
    /// </summary>
    public partial class dlgRenameTags : Window
    {
        private clsTrDocument CurrentDocument;
        private string OldName;
        private string NewName;

        public dlgRenameTags(clsTrDocument Document)
        {
            InitializeComponent();
            CurrentDocument = Document;
            cmbTags.ItemsSource = CurrentDocument.GetStructuralTags();
        }

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTags.SelectedItem != null && txtNewValue.Text != "")
            {
                OldName = cmbTags.SelectedItem.ToString();
                NewName = txtNewValue.Text;
                CurrentDocument.RenameStructuralTags(OldName, NewName);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
