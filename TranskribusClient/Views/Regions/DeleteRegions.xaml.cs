// <copyright file="DeleteRegions.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.Windows;
    using static TranskribusClient.MainWindow;

    /// <summary>
    /// Interaction logic for DeleteRegions.xaml.
    /// </summary>
    public partial class DeleteRegions : Window
    {
        public string TagName;
        public DeleteAction DeleteAction;

        public DeleteRegions()
        {
            InitializeComponent();
            rdPreserve.IsChecked = true;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            TagName = txtRegionalTag.Text;

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
