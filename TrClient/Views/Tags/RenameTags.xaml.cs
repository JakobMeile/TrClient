// <copyright file="RenameTags.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.Windows;
    using TranskribusClient.Core;

    /// <summary>
    /// Interaction logic for RenameTags.xaml.
    /// </summary>
    public partial class RenameTags : Window
    {
        private TrDocument currentDocument;
        private string oldName;
        private string newName;

        public RenameTags(TrDocument document)
        {
            InitializeComponent();
            currentDocument = document;
            cmbTags.ItemsSource = currentDocument.GetStructuralTags();
        }

        private void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTags.SelectedItem != null && txtNewValue.Text != string.Empty)
            {
                oldName = cmbTags.SelectedItem.ToString();
                newName = txtNewValue.Text;
                currentDocument.RenameStructuralTags(oldName, newName);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
