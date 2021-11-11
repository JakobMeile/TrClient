// <copyright file="ProgressLoadPages.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ProgressLoadPages.xaml.
    /// </summary>
    public partial class ProgressLoadPages : Window
    {
        public ProgressLoadPages(int maximum)
        {
            InitializeComponent();

            barLoadedPages.Minimum = 0;
            barLoadedPages.Maximum = maximum;
        }
    }
}
