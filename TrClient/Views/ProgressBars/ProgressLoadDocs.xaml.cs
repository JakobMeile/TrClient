// <copyright file="ProgressLoadDocs.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ProgressLoadDocs.xaml.
    /// </summary>
    public partial class ProgressLoadDocs : Window
    {
        public ProgressLoadDocs(int maximum)
        {
            InitializeComponent();

            barLoadedDocuments.Minimum = 0;
            barLoadedDocuments.Maximum = maximum;
        }
    }
}
