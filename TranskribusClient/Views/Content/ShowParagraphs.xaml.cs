// <copyright file="ShowParagraphs.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TranskribusClient.Views
{
    using System.Windows;
    using TranskribusClient.Core;

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
