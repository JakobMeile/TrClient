// <copyright file="ShowTags.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ShowTags.xaml.
    /// </summary>
    public partial class ShowTags : Window
    {
        public ShowTags(string caption)
        {
            InitializeComponent();

            txtCaption.Text = caption;
        }
    }
}
