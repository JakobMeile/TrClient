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
using TrClient.Core;
using TrClient.Dialog;
using TrClient.Extensions;
using TrClient.Helpers;
using TrClient.Libraries;
using TrClient.Settings;
using TrClient.Tags;

namespace TrClient.Dialog
{
    /// <summary>
    /// Interaction logic for dlgShowPage.xaml
    /// </summary>
    public partial class dlgShowPage : Window
    {
        private TrPage CurrentPage;
        private HttpClient CurrentClient;

        public dlgShowPage(TrPage Page, HttpClient Client)
        {
            InitializeComponent();
            CurrentPage = Page;
            CurrentClient = Client;
            DataContext = this.CurrentPage;
        }
    }
}
