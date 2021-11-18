// <copyright file="ProgressLoadPages.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ProgressLoadImages.xaml
    /// </summary>
    public partial class ProgressLoadImages : Window
    {
        public ProgressLoadImages(int maximum)
        {
            InitializeComponent();

            barLoadedImages.Minimum = 0;
            barLoadedImages.Maximum = maximum;

        }
    }
}
