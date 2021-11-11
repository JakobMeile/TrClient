// <copyright file="ProgressCopy.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ProgressCopy.xaml.
    /// </summary>
    public partial class ProgressCopy : Window
    {
        public ProgressCopy(int maximum)
        {
            InitializeComponent();
            barLoadedDocuments.Minimum = 0;
            barLoadedDocuments.Maximum = maximum;
        }
    }
}
