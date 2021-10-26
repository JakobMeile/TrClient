// <copyright file="DeleteLines.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;
    using static TrClient.MainWindow;

    /// <summary>
    /// Interaction logic for DeleteLines.xaml.
    /// </summary>
    public partial class DeleteLines : Window
    {
        public string TagName;
        public DeleteAction DeleteAction;

        public DeleteLines()
        {
            InitializeComponent();
            rdPreserve.IsChecked = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            TagName = txtStructuralTag.Text;

            if (rdPreserve.IsChecked == true)
            {
                DeleteAction = DeleteAction.Preserve;
            }
            else
            {
                DeleteAction = DeleteAction.Delete;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
