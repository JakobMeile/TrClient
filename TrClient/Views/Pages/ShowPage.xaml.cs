// <copyright file="ShowPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
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
