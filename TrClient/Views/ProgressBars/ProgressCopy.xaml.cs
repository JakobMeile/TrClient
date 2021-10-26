// <copyright file="ProgressCopy.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
