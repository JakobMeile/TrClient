// <copyright file="ShowParagraphs.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TrClient.Views
{
    using System.Windows;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for ShowParagraphs.xaml.
    /// </summary>
    public partial class ShowParagraphs : Window
    {
        public ShowParagraphs(TrPage page)
        {
            InitializeComponent();

            lstParagraphs.ItemsSource = page.GetParagraphs();
        }
    }
}
