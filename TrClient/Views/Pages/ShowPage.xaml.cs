// <copyright file="ShowPage.xaml.cs" company="Kyrillos">
// Copyright (c) Jakob K. Meile 2021.
// </copyright>

namespace TrClient.Views
{
    using System.Net.Http;
    using System.Windows;
    using TrClient.Core;

    /// <summary>
    /// Interaction logic for ShowPage.xaml.
    /// </summary>
    public partial class ShowPage : Window
    {
        private TrPage currentPage;
        private HttpClient currentClient;

        public ShowPage(TrPage page, HttpClient client)
        {
            InitializeComponent();
            currentPage = page;
            currentClient = client;
            DataContext = currentPage;
        }
    }
}
