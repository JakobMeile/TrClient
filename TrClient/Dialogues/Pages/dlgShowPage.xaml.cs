using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Diagnostics;

namespace TrClient
{
    /// <summary>
    /// Interaction logic for dlgShowPage.xaml
    /// </summary>
    public partial class dlgShowPage : Window
    {
        private clsTrPage CurrentPage;
        private HttpClient CurrentClient;

        public dlgShowPage(clsTrPage Page, HttpClient Client)
        {
            InitializeComponent();
            CurrentPage = Page;
            CurrentClient = Client;
            DataContext = this.CurrentPage;
        }
    }
}
