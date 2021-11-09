// <copyright file="ShowTags.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
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
