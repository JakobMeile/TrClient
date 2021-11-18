// <copyright file="ProgressUploadPages.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;
    
    /// <summary>
    /// Interaction logic for ProgressUploadPages.xaml
    /// </summary>
    public partial class ProgressUploadPages : Window
    {
        public ProgressUploadPages(int maximum)
        {
            InitializeComponent();

            barUploadedPages.Minimum = 0;
            barUploadedPages.Maximum = maximum;

        }
    }
}
